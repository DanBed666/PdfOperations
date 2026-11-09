using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.IO;
using System.Threading;

namespace PdfOperations;

// Single-column scans with dark headers, vertical separators and single-line cells.
internal static class TableOcr
{
    private sealed record Table(int Top, int Bottom, int[] Edges, List<(int A, int B)> Rows);

    public static bool TryConvert(FileJob fileJob, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();
        using Bitmap? image = OpenImage(fileJob.InputFile);
        if (image is null || (long)image.Width * image.Height > 20_000_000 ||
            image.FrameDimensionsList.Any(id => image.GetFrameCount(new FrameDimension(id)) != 1))
            return false;

        byte[,] gray = Grayscale(image, token);
        var tables = Detect(gray, token);
        if (tables.Sum(t => (long)t.Rows.Count * (t.Edges.Length - 1)) > 128)
            return false;

        string workDir = Directory.CreateTempSubdirectory("PdfOperationsOcr-").FullName;
        TemporaryDirectoryCleanup.Run(workDir, () =>
        {
            string tool = ToolPaths.ToolPathsDict[Tool.Tesseract];
            string page = Path.Combine(workDir, "page");
            RunClass.Run(tool, [fileJob.InputFile, page, "-l", "pol", "--psm", "3", "-c", "tessedit_create_tsv=1", "-c", "tessedit_create_txt=1"],
                OperationTimeouts.ImageToTxt, token);
            string cellPath = Path.Combine(workDir, "cell.png");
            string cellOutput = Path.Combine(workDir, "cell");
            string tsv = File.ReadAllText(page + ".tsv");
            string? tableText = tables.Count == 0 ? null : ReadTables(gray, tables, tsv, cell =>
            {
                cell.Save(cellPath, ImageFormat.Png);
                RunClass.Run(tool, [cellPath, cellOutput, "-l", "pol", "--psm", "7"],
                    OperationTimeouts.ImageToTxt, token);
                return File.ReadAllText(cellOutput + ".txt");
            }, token);
            string baseline = tableText ?? File.ReadAllText(page + ".txt");
            string? refined = SpacedGlyphOcr.Refine(gray, tsv, baseline, workDir, tool,
                (top, bottom) => tables.Any(t => t.Top < bottom && t.Bottom > top), token);
            token.ThrowIfCancellationRequested();
            string output = Files.PrepareTempPathWithoutExt(fileJob.TempPath) + ".txt";
            if (refined is null && tableText is null)
                File.Copy(page + ".txt", output, overwrite: true);
            else
                File.WriteAllText(output, refined ?? baseline);
        });
        token.ThrowIfCancellationRequested();
        return true;
    }

    private static Bitmap? OpenImage(string path)
    {
        // GDI+ reports unsupported decoders as ArgumentException or OutOfMemoryException.
        // The ordinary Tesseract path may still support this image format.
        try { return new Bitmap(path); }
        catch (ArgumentException) { return null; }
        catch (OutOfMemoryException) { return null; }
    }

    private static string? ReadTables(byte[,] gray, List<Table> tables, string pageTsv,
        Func<Bitmap, string> readCell, CancellationToken token)
    {
        var prose = pageTsv.Split('\n')
            .Select(line => line.TrimEnd('\r').Split('\t', 12))
            .Where(v => v.Length == 12 && v[0] == "5" && !string.IsNullOrWhiteSpace(v[11]))
            .Select(v => (Paragraph: (v[1], v[2], v[3]), Line: v[4], Y: int.Parse(v[7]) + int.Parse(v[9]) / 2, Word: v[11]))
            .ToArray();
        if (prose.Length == 0) return null;
        var parts = new List<string>();
        int cursor = 0;
        foreach (var table in tables)
        {
            AddProse(cursor, table.Top);
            var lines = new List<string>();
            for (int row = 0; row < table.Rows.Count; row++)
            {
                var (a, b) = table.Rows[row];
                var cells = new List<string>();
                for (int col = 1; col < table.Edges.Length; col++)
                {
                    token.ThrowIfCancellationRequested();
                    int x = table.Edges[col - 1] + 3, right = table.Edges[col] - 3;
                    var box = Rectangle.FromLTRB(x, a, right, b);
                    using var cell = new Bitmap(box.Width + 16, box.Height + 16);
                    PrepareCell(cell, gray, box, row == 0, token);
                    cells.Add(readCell(cell).Trim());
                }
                if (row == 0 && cells.Count(cell => cell.Length > 0) < 2)
                    return null;
                lines.Add(string.Join("\t", cells));
            }
            parts.Add(string.Join(Environment.NewLine, lines));
            cursor = table.Bottom;
        }
        AddProse(cursor, gray.GetLength(0));
        return string.Join(Environment.NewLine + Environment.NewLine, parts);

        void AddProse(int top, int bottom)
        {
            if (bottom <= top) return;
            token.ThrowIfCancellationRequested();
            foreach (var paragraph in prose.Where(word => word.Y >= top && word.Y < bottom).GroupBy(word => word.Paragraph))
                parts.Add(string.Join(Environment.NewLine, paragraph.GroupBy(word => word.Line)
                    .Select(line => string.Join(" ", line.Select(word => word.Word)))));
        }
    }

    private static List<Table> Detect(byte[,] g, CancellationToken token)
    {
        int h = g.GetLength(0), w = g.GetLength(1);
        var darkBands = Runs(0, h, y =>
        {
            token.ThrowIfCancellationRequested();
            int n = 0;
            for (int x = 0; x < w; x++) if (g[y, x] < 115) n++;
            return n > .43 * w;
        });
        var tables = new List<Table>();
        foreach (var (top, end) in darkBands)
        {
            int size = end - top;
            if (size < 12 || size > h / 5 || (tables.Count > 0 && top < tables[^1].Bottom)) continue;
            int a = top + size / 6, b = end - size / 6;
            // Full-height light gaps in the dark header, including the two outer margins.
            var gaps = Runs(0, w, x =>
            {
                token.ThrowIfCancellationRequested();
                int n = 0;
                for (int y = a; y < b; y++) if (g[y, x] > 130) n++;
                return n >= .84 * (b - a);
            });
            if (gaps.Count < 3 || gaps[0].A != 0 || gaps[^1].B != w) continue;
            int[] edges = [gaps[0].B, .. gaps.Skip(1).SkipLast(1).Select(s => (s.A + s.B - 1) / 2), gaps[^1].A];
            if (edges.Zip(edges.Skip(1)).Any(p => p.Second - p.First < Math.Max(12, size / 3))) continue;
            int last = end;
            for (int y = end; y < h && y - last <= size / 3; y++)
            {
                token.ThrowIfCancellationRequested();
                int supported = edges.Skip(1).SkipLast(1).Count(x =>
                {
                    int r = Math.Max(3, size / 10);
                    for (int i = Math.Max(3, x - r); i <= Math.Min(w - 4, x + r); i++)
                        if (g[y, i] + 20 < Math.Min(g[y, i - 3], g[y, i + 3])) return true;
                    return false;
                });
                if (supported * 2 >= edges.Length - 2) last = y + 1;
            }
            if (last - end < size / 2) continue;
            // A side column would be lost by full-width slicing: leave such pages to ordinary OCR.
            bool hasSideText = false;
            for (int y = top; y < last && !hasSideText; y++)
            {
                token.ThrowIfCancellationRequested();
                for (int x = 0; x < w; x++)
                    if ((x < edges[0] - size / 6 || x > edges[^1] + size / 6) && g[y, x] < 130)
                    { hasSideText = true; break; }
            }
            if (hasSideText) continue;
            var bands = Runs(end + size / 8, last, y =>
            {
                token.ThrowIfCancellationRequested();
                int ink = 0;
                for (int col = 1; col < edges.Length; col++)
                    for (int x = edges[col - 1] + size / 10; x < edges[col] - size / 10; x++)
                        if (g[y, x] < 135) ink++;
                return ink >= 3 && ink < .45 * (edges[^1] - edges[0]);
            });
            var merged = new List<(int A, int B)>();
            foreach (var band in bands)
            {
                if (merged.Count > 0 && band.A - merged[^1].B <= Math.Max(2, size / 12))
                    merged[^1] = (merged[^1].A, band.B);
                else merged.Add(band);
            }
            var rows = merged.Where(s => s.B - s.A >= Math.Max(4, size / 15))
                .Select(s => (Math.Max(end, s.A - size / 12), Math.Min(last, s.B + size / 12))).ToList();
            if (rows.Count == 0) continue;
            rows.Insert(0, (top + size / 8, end - size / 8));
            tables.Add(new Table(top, last, edges, rows));
        }
        return tables;
    }

    private static List<(int A, int B)> Runs(int start, int end, Func<int, bool> predicate)
    {
        var spans = new List<(int, int)>();
        int first = -1;
        for (int i = start; i <= end; i++)
        {
            if (i < end && predicate(i)) { if (first < 0) first = i; }
            else if (first >= 0) { spans.Add((first, i)); first = -1; }
        }
        return spans;
    }

    private static byte[,] Grayscale(Bitmap input, CancellationToken token)
    {
        using var rgb = new Bitmap(input.Width, input.Height, PixelFormat.Format24bppRgb);
        using (var graphics = Graphics.FromImage(rgb))
        { graphics.Clear(Color.White); graphics.DrawImageUnscaled(input, 0, 0); }
        var data = rgb.LockBits(new Rectangle(0, 0, rgb.Width, rgb.Height), ImageLockMode.ReadOnly, rgb.PixelFormat);
        try
        {
            int width = rgb.Width, height = rgb.Height;
            var g = new byte[height, width];
            var line = new byte[rgb.Width * 3];
            for (int y = 0; y < height; y++)
            {
                token.ThrowIfCancellationRequested();
                Marshal.Copy(data.Scan0 + y * data.Stride, line, 0, line.Length);
                for (int x = 0; x < width; x++)
                    g[y, x] = (byte)((29 * line[x * 3] + 150 * line[x * 3 + 1] + 77 * line[x * 3 + 2]) >> 8);
            }
            return g;
        }
        finally { rgb.UnlockBits(data); }
    }

    private static void PrepareCell(Bitmap image, byte[,] gray, Rectangle box, bool lightText, CancellationToken token)
    {
        using (var graphics = Graphics.FromImage(image)) graphics.Clear(Color.White);
        for (int y = 0; y < box.Height; y++)
        {
            token.ThrowIfCancellationRequested();
            for (int x = 0; x < box.Width; x++)
            {
                byte value = gray[box.Top + y, box.Left + x];
                bool ink = lightText ? value > 140 : value < 145;
                image.SetPixel(x + 8, y + 8, ink ? Color.Black : Color.White);
            }
        }
    }
}
