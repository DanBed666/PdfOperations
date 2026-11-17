using PdfOperations.Gui.Models;
using PdfOperations.Gui.Services;
using PdfOperations.Gui.ViewModels;
using Velopack;
using Velopack.Logging;
using Velopack.Sources;

namespace PdfOperations.Tests;

[TestClass]
[DoNotParallelize]
public sealed class UpdateDownloadTests
{
    private static UpdateInfo Update() => new(new VelopackAsset
    {
        Version = new SemanticVersion(1, 0, 2), Type = VelopackAssetType.Full
    }, false);

    [TestMethod]
    public void DownloadAndPreparationHaveDistinctStatusAndCompletionEnablesRestart()
    {
        var service = new FakeUpdateService();
        var vm = new AboutViewModel(() => false, service) { AvailableUpdate = Update() };
        var ui = new QueuedContext();
        var task = ui.Start(() => vm.DownloadUpdateCommand.ExecuteAsync(null));
        service.Report!(new(UpdateDownloadPhase.Downloading, 36));
        ui.Drain();
        Assert.AreEqual(vm.Loc.DownloadingUpdate, vm.StatusMessage);
        Assert.AreEqual(36, vm.DownloadProgress);
        Assert.Contains("36%", vm.UpdateProgressText);
        Assert.IsFalse(vm.CheckUpdatesCommand.CanExecute(null));
        Assert.IsFalse(vm.DownloadUpdateCommand.CanExecute(null));
        Assert.IsFalse(vm.RestartToUpdateCommand.CanExecute(null));

        service.Report(new(UpdateDownloadPhase.Preparing, 70));
        ui.Drain();
        Assert.IsTrue(vm.IsPreparingUpdate);
        Assert.AreEqual(vm.Loc.PreparingUpdate, vm.StatusMessage);
        service.Report(new(UpdateDownloadPhase.Preparing, 100));
        ui.Drain();
        Assert.AreEqual(99, vm.DownloadProgress);
        Assert.IsFalse(vm.RestartToUpdateCommand.CanExecute(null));

        service.Completion.SetResult();
        ui.Drain();
        Assert.IsTrue(task.IsCompletedSuccessfully);
        Assert.AreEqual(100, vm.DownloadProgress);
        Assert.IsFalse(vm.IsDownloading);
        Assert.IsTrue(vm.RestartToUpdateCommand.CanExecute(null));
        Assert.AreEqual(vm.Loc.UpdateDownloaded, vm.StatusMessage);
    }

    [TestMethod]
    public void CancellationWaitsForWorkerAndAllowsRetryWithoutStaleProgress()
    {
        var service = new FakeUpdateService();
        var vm = new AboutViewModel(() => false, service) { AvailableUpdate = Update() };
        var ui = new QueuedContext();
        var task = ui.Start(() => vm.DownloadUpdateCommand.ExecuteAsync(null));
        var oldProgress = service.Report!;
        service.Report!(new(UpdateDownloadPhase.Preparing, 70));
        ui.Drain();
        vm.CancelDownloadCommand.Execute(null);
        Assert.IsTrue(service.Token.IsCancellationRequested);
        Assert.IsTrue(vm.IsDownloading);
        Assert.IsFalse(vm.CancelDownloadCommand.CanExecute(null));
        Assert.IsFalse(vm.DownloadUpdateCommand.CanExecute(null));
        Assert.IsFalse(vm.CheckUpdatesCommand.CanExecute(null));
        Assert.AreEqual(vm.Loc.CancellingUpdate, vm.StatusMessage);
        oldProgress(new(UpdateDownloadPhase.Downloading, 80));
        ui.Drain();
        Assert.AreEqual(vm.Loc.CancellingUpdate, vm.StatusMessage);

        // Simulate the native patch step returning successfully after a cancellation request.
        service.Completion.SetResult();
        ui.Drain();
        Assert.IsTrue(task.IsCompletedSuccessfully);
        Assert.IsNull(vm.DownloadedUpdate);
        Assert.IsFalse(vm.RestartToUpdateCommand.CanExecute(null));
        Assert.IsTrue(vm.DownloadUpdateCommand.CanExecute(null));
        Assert.AreEqual(vm.Loc.UpdateDownloadCancelled, vm.StatusMessage);
        Assert.AreEqual("", vm.ErrorDetails);

        service.Completion = new();
        var retry = ui.Start(() => vm.DownloadUpdateCommand.ExecuteAsync(null));
        oldProgress(new(UpdateDownloadPhase.Preparing, 95));
        ui.Drain();
        Assert.AreEqual(0, vm.DownloadProgress);
        Assert.IsFalse(vm.IsPreparingUpdate);
        Assert.IsFalse(service.Token.IsCancellationRequested);
        service.Completion.SetResult();
        ui.Drain();
        Assert.IsTrue(retry.IsCompletedSuccessfully);
        Assert.IsNotNull(vm.DownloadedUpdate);
    }

    [TestMethod]
    public void CancelledTransferIsNotReportedAsError()
    {
        var service = new FakeUpdateService();
        var vm = new AboutViewModel(() => false, service) { AvailableUpdate = Update() };
        var ui = new QueuedContext();
        var task = ui.Start(() => vm.DownloadUpdateCommand.ExecuteAsync(null));
        vm.CancelDownloadCommand.Execute(null);
        service.Completion.SetCanceled(service.Token);
        ui.Drain();
        Assert.IsTrue(task.IsCompletedSuccessfully);
        Assert.AreEqual(StatusKind.Info, vm.StatusKind);
        Assert.IsTrue(vm.DownloadUpdateCommand.CanExecute(null));
        Assert.AreEqual("", vm.ErrorDetails);
    }

    [TestMethod]
    public void FailureUnlocksCommandsAndDoesNotEnableRestart()
    {
        var service = new FakeUpdateService();
        var vm = new AboutViewModel(() => false, service) { AvailableUpdate = Update() };
        var ui = new QueuedContext();
        var task = ui.Start(() => vm.DownloadUpdateCommand.ExecuteAsync(null));
        service.Completion.SetException(new IOException("test failure"));
        ui.Drain();
        Assert.IsTrue(task.IsCompletedSuccessfully);
        Assert.AreEqual(StatusKind.Error, vm.StatusKind);
        Assert.Contains("test failure", vm.ErrorDetails);
        Assert.IsTrue(vm.DownloadUpdateCommand.CanExecute(null));
        Assert.IsTrue(vm.CheckUpdatesCommand.CanExecute(null));
        Assert.IsFalse(vm.RestartToUpdateCommand.CanExecute(null));
    }

    [TestMethod]
    public void ActiveStatusAndPercentSupportBothLanguages()
    {
        var service = new FakeUpdateService();
        var vm = new AboutViewModel(() => false, service) { AvailableUpdate = Update() };
        var original = vm.Loc.Language;
        var ui = new QueuedContext();
        var task = ui.Start(() => vm.DownloadUpdateCommand.ExecuteAsync(null));
        try
        {
            service.Report!(new(UpdateDownloadPhase.Preparing, 70));
            ui.Drain();
            vm.Loc.Language = AppLanguage.Polish;
            vm.RefreshLocalization();
            Assert.StartsWith("Przygotowywanie", vm.StatusMessage);
            Assert.AreEqual("Postęp całej operacji: 70%", vm.UpdateProgressText);
            vm.Loc.Language = AppLanguage.English;
            vm.RefreshLocalization();
            Assert.StartsWith("Preparing", vm.StatusMessage);
            Assert.AreEqual("Overall progress: 70%", vm.UpdateProgressText);
            vm.CancelDownloadCommand.Execute(null);
            Assert.StartsWith("Cancelling", vm.StatusMessage);
        }
        finally
        {
            service.Completion.SetResult();
            ui.Drain();
            vm.Loc.Language = original;
        }
        Assert.IsTrue(task.IsCompletedSuccessfully);
    }

    [TestMethod]
    public async Task SourceReportsActualTransferAndPreparationIncludingFullFallback()
    {
        var inner = new FakeSource();
        var source = new UpdateProgressSource(inner);
        var states = new List<UpdateDownloadProgress>();
        source.Begin(states.Add);
        inner.Transfer = (_, report, _) => { report(70); return Task.CompletedTask; };
        await source.DownloadReleaseEntry(null!, new() { Type = VelopackAssetType.Delta }, "unused", source.ReportProgress);
        Assert.Contains(new UpdateDownloadProgress(UpdateDownloadPhase.Downloading, 70), states);
        Assert.AreEqual(new(UpdateDownloadPhase.Preparing, 70), states[^1]);

        states.Clear();
        inner.Transfer = (_, report, _) => { report(100); return Task.CompletedTask; };
        await source.DownloadReleaseEntry(null!, new() { Type = VelopackAssetType.Full }, "unused", source.ReportProgress);
        Assert.AreEqual(new(UpdateDownloadPhase.Downloading, 0), states[0]);
        Assert.AreEqual(new(UpdateDownloadPhase.Preparing, 99), states[^1]);
        source.End();
        int count = states.Count;
        source.ReportProgress(100);
        Assert.HasCount(count, states);
    }

    [TestMethod]
    public async Task SourceKeepsDownloadingUntilAllConcurrentTransfersFinish()
    {
        var first = new TaskCompletionSource();
        var second = new TaskCompletionSource();
        var inner = new FakeSource { Transfer = (asset, _, _) => asset.FileName == "first" ? first.Task : second.Task };
        var source = new UpdateProgressSource(inner);
        var states = new List<UpdateDownloadProgress>();
        source.Begin(states.Add);
        var a = source.DownloadReleaseEntry(null!, new() { FileName = "first" }, "unused", source.ReportProgress);
        var b = source.DownloadReleaseEntry(null!, new() { FileName = "second" }, "unused", source.ReportProgress);
        first.SetResult();
        await a;
        Assert.AreEqual(UpdateDownloadPhase.Downloading, states[^1].Phase);
        second.SetResult();
        await b;
        Assert.AreEqual(UpdateDownloadPhase.Preparing, states[^1].Phase);
        source.End();
    }

    [TestMethod]
    public async Task SourceDoesNotStartFullFallbackWhenAlreadyCancelled()
    {
        var inner = new FakeSource();
        var source = new UpdateProgressSource(inner);
        using var cancellation = new CancellationTokenSource();
        source.Begin(_ => { });
        inner.Transfer = (_, _, token) => { cancellation.Cancel(); return Task.FromCanceled(token); };
        await Assert.ThrowsAsync<TaskCanceledException>(() => source.DownloadReleaseEntry(null!,
            new() { Type = VelopackAssetType.Delta }, "unused", source.ReportProgress, cancellation.Token));
        await Assert.ThrowsAsync<OperationCanceledException>(() => source.DownloadReleaseEntry(null!,
            new() { Type = VelopackAssetType.Full }, "unused", source.ReportProgress, cancellation.Token));
        Assert.AreEqual(1, inner.Calls);
        source.End();
    }

    private sealed class FakeUpdateService : IAppUpdateService
    {
        public bool IsSupported => true;
        public TaskCompletionSource Completion = new();
        public Action<UpdateDownloadProgress>? Report;
        public CancellationToken Token;
        public Task<UpdateInfo?> CheckAsync() => Task.FromResult<UpdateInfo?>(Update());
        public Task DownloadAsync(UpdateInfo update, Action<UpdateDownloadProgress> progress, CancellationToken cancellationToken)
        {
            Report = progress;
            Token = cancellationToken;
            return Completion.Task;
        }
        public void ApplyAndRestart(UpdateInfo update) => Assert.Fail("Tests must not apply an update.");
    }

    private sealed class FakeSource : IUpdateSource
    {
        public int Calls;
        public Func<VelopackAsset, Action<int>, CancellationToken, Task> Transfer = (_, _, _) => Task.CompletedTask;
        public Task<VelopackAssetFeed> GetReleaseFeed(IVelopackLogger logger, string? appId, string channel,
            Guid? stagingId = null, VelopackAsset? latestLocalRelease = null) => throw new NotSupportedException();
        public Task DownloadReleaseEntry(IVelopackLogger logger, VelopackAsset releaseEntry, string localFile,
            Action<int> progress, CancellationToken cancelToken = default)
        {
            Calls++;
            return Transfer(releaseEntry, progress, cancelToken);
        }
    }

    private sealed class QueuedContext : SynchronizationContext
    {
        private readonly Queue<Action> pending = new();
        public override void Post(SendOrPostCallback callback, object? state) => pending.Enqueue(() => callback(state));
        public Task Start(Func<Task> action)
        {
            var previous = Current;
            SetSynchronizationContext(this);
            try { return action(); }
            finally { SetSynchronizationContext(previous); }
        }
        public void Drain()
        {
            while (pending.TryDequeue(out var callback)) callback();
        }
    }
}
