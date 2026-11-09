using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Text.RegularExpressions;

namespace PdfOperations;

internal static class SpacedGlyphOcr
{
    private sealed record Reading(string Text, double Confidence);
    private sealed record Line(string Original, List<Rectangle> Glyphs);
    private static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

    // Return null when the original text should be used verbatim.
    public static string? Refine(byte[,] gray, string tsv, string text, string directory, string tool,
        Func<int, int, bool> excluded, CancellationToken token)
    {
        var originalLines = Regex.Matches(text, @"[^\r\n]+").Select(m => m.Value)
            .GroupBy(s => s).Where(g => g.Count() == 1).Select(g => g.Key).ToHashSet();
        var candidates = new List<Line>();
        int glyphCount = 0;
        foreach (var group in Words(tsv).GroupBy(v => (v[1], v[2], v[3], v[4])))
        {
            token.ThrowIfCancellationRequested();
            string original = string.Join(" ", group.Select(v => v[11]));
            if (!originalLines.Contains(original) || original.Length > 100 ||
                !group.Any(v => double.Parse(v[10], Invariant) < 75)) continue;

            var bounds = group.Select(v => new Rectangle(int.Parse(v[6]), int.Parse(v[7]),
                int.Parse(v[8]), int.Parse(v[9]))).Aggregate(Rectangle.Union);
            if (bounds.Height is < 8 or > 160 || excluded(bounds.Top, bounds.Bottom)) continue;
            var glyphs = Segment(gray, bounds);
            if (glyphs is null || glyphCount + glyphs.Count > 96) continue;
            glyphCount += glyphs.Count;
            candidates.Add(new Line(original, glyphs));
        }
        if (candidates.Count == 0) return null;

        // A Tesseract image list handles both scales of every glyph in one process.
        var paths = new List<string>();
        foreach (var glyph in candidates.SelectMany(line => line.Glyphs))
        {
            foreach (int scale in new[] { 1, 2 })
            {
                token.ThrowIfCancellationRequested();
                string path = Path.Combine(directory, $"glyph-{paths.Count}.png");
                SaveGlyph(gray, glyph, scale, path);
                paths.Add(path);
            }
        }
        string list = Path.Combine(directory, "glyphs.txt"), output = Path.Combine(directory, "letters");
        File.WriteAllLines(list, paths);
        RunClass.Run(tool, [list, output, "-l", "pol", "--psm", "10", "-c", "tessedit_create_tsv=1"],
            OperationTimeouts.ImageToTxt, token);
        var readings = Words(File.ReadAllText(output + ".tsv")).GroupBy(v => int.Parse(v[1]))
            .Where(g => g.Count() == 1 && g.First()[11].Length == 1)
            .ToDictionary(g => g.Key, g => new Reading(g.First()[11], double.Parse(g.First()[10], Invariant)));
        var changes = new Dictionary<string, string>();
        int page = 1;
        foreach (var candidate in candidates)
        {
            var alternatives = candidate.Glyphs.Select(_ =>
            {
                var pair = new[] { readings.GetValueOrDefault(page++), readings.GetValueOrDefault(page++) };
                return pair.OfType<Reading>().OrderByDescending(v => v.Confidence).ToArray();
            }).ToArray();
            if (alternatives.Any(a => a.Length == 0)) continue;
            string? value = Choose(candidate, alternatives);
            if (value != null && value != candidate.Original) changes[candidate.Original] = value;
        }
        return changes.Count == 0 ? null : Regex.Replace(text, @"[^\r\n]+", m => changes.GetValueOrDefault(m.Value, m.Value));
    }

    private static IEnumerable<string[]> Words(string tsv) => tsv.Split('\n')
        .Select(s => s.TrimEnd('\r').Split('\t', 12))
        .Where(v => v.Length == 12 && v[0] == "5" && !string.IsNullOrWhiteSpace(v[11]));

    private static List<Rectangle>? Segment(byte[,] g, Rectangle bounds)
    {
        int pad = Math.Max(2, bounds.Height / 5);
        var region = Rectangle.Intersect(Rectangle.Inflate(bounds, pad, pad),
            new Rectangle(0, 0, g.GetLength(1), g.GetLength(0)));
        var glyphs = new List<Rectangle>();
        int start = -1, last = -1;
        for (int x = region.Left; x < region.Right + 2; x++)
        {
            bool ink = false;
            if (x < region.Right)
                for (int y = region.Top; y < region.Bottom && !ink; y++) ink = g[y, x] < 150;
            if (ink)
            {
                if (start < 0) start = x;
                last = x;
            }
            else if (start >= 0 && x - last >= 2)
            {
                glyphs.Add(Rectangle.FromLTRB(start, region.Top, last + 1, region.Bottom));
                start = -1;
            }
        }
        if (glyphs.Count is < 6 or > 48 || glyphs.Any(b => b.Width > bounds.Height * 1.3)) return null;
        int[] gaps = glyphs.Zip(glyphs.Skip(1), (a, b) => b.Left - a.Right).Order().ToArray();
        int median = gaps[gaps.Length / 2];
        if (median < Math.Max(3, bounds.Height * .23) || gaps[^1] > median * 2.3 ||
            gaps.Count(n => n >= median * .6 && n <= median * 1.6) < gaps.Length * .8) return null;
        return glyphs;
    }

    private static void SaveGlyph(byte[,] g, Rectangle box, int scale, string path)
    {
        using var raw = new Bitmap(box.Width, box.Height);
        int min = 255, max = 0;
        for (int y = box.Top; y < box.Bottom; y++)
            for (int x = box.Left; x < box.Right; x++)
            { min = Math.Min(min, g[y, x]); max = Math.Max(max, g[y, x]); }
        for (int y = 0; y < box.Height; y++)
            for (int x = 0; x < box.Width; x++)
            {
                int v = (g[box.Top + y, box.Left + x] - min) * 255 / Math.Max(1, max - min);
                raw.SetPixel(x, y, Color.FromArgb(v, v, v));
            }
        using var padded = new Bitmap(box.Width * scale + 20, box.Height * scale + 4);
        using (var graphics = Graphics.FromImage(padded))
        {
            graphics.Clear(Color.White);
            if (scale == 1) graphics.DrawImageUnscaled(raw, 10, 2);
            else
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                using var attrs = new ImageAttributes();
                attrs.SetWrapMode(WrapMode.TileFlipXY);
                graphics.DrawImage(raw, new Rectangle(10, 2, box.Width * scale, box.Height * scale),
                    0, 0, box.Width, box.Height, GraphicsUnit.Pixel, attrs);
            }
        }
        padded.Save(path, ImageFormat.Png);
    }

    private static string? Choose(Line line, Reading[][] readings)
    {
        // Align the two glyph readings with the original OCR, which may insert or omit letters.
        string original = string.Concat(line.Original.Where(c => !char.IsWhiteSpace(c)));
        int n = readings.Length, m = original.Length;
        if (Math.Abs(n - m) > Math.Max(2, n / 5)) return null;
        var cost = new int[n + 1, m + 1];
        for (int i = 0; i <= n; i++) cost[i, 0] = i;
        for (int j = 0; j <= m; j++) cost[0, j] = j;
        int Penalty(int i, int j) => readings[i].Any(r => r.Text[0] == original[j]) ? 0 : 1;
        for (int i = 1; i <= n; i++)
            for (int j = 1; j <= m; j++)
                cost[i, j] = Math.Min(cost[i - 1, j - 1] + Penalty(i - 1, j - 1), Math.Min(cost[i - 1, j], cost[i, j - 1]) + 1);
        if (cost[n, m] > Math.Max(2, n / 3)) return null;
        var aligned = new char?[n];
        for (int i = n, j = m; i > 0 || j > 0;)
        {
            if (i > 0 && j > 0 && cost[i, j] == cost[i - 1, j - 1] + Penalty(i - 1, j - 1)) aligned[--i] = original[--j];
            else if (i > 0 && cost[i, j] == cost[i - 1, j] + 1) i--;
            else j--;
        }
        var result = new System.Text.StringBuilder();
        for (int i = 0; i < n; i++)
        {
            Reading[] options = readings[i];
            char? c = aligned[i];
            if (!options.Any(r => r.Text[0] == c))
            {
                bool agree = options.Length == 2 && options[0].Text == options[1].Text;
                if ((agree && options[0].Confidence >= 40) ||
                    (options[0].Confidence >= 80 && (options.Length == 1 || options[0].Confidence - options[1].Confidence >= 15)))
                    c = options[0].Text[0];
            }
            if (c is null) return null;
            if (i > 0 && !char.IsPunctuation(c.Value)) result.Append(' ');
            // Detached separators are spaced; a trailing full stop stays next to its letter.
            else if (i > 0 && line.Glyphs[i].Left - line.Glyphs[i - 1].Right >= line.Glyphs[i].Height * .16) result.Append(' ');
            result.Append(c.Value);
        }
        return result.ToString();
    }
}
