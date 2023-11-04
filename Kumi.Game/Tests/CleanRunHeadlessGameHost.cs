using System.Runtime.CompilerServices;
using osu.Framework;
using osu.Framework.Testing;

namespace Kumi.Game.Tests;

public class CleanRunHeadlessGameHost : TestRunHeadlessGameHost
{
    private readonly bool bypassCleanupOnSetup;

    public CleanRunHeadlessGameHost(bool bindIPC = false, bool realtime = true, bool bypassCleanupOnSetup = false, bool bypassCleanupOnDispose = false,
            [CallerMemberName] string callingMethodName = @"")
        : base($"{callingMethodName}-{Guid.NewGuid()}", new HostOptions
        {
            BindIPC = bindIPC
        }, bypassCleanup: bypassCleanupOnDispose, realtime: realtime)
    {
        this.bypassCleanupOnSetup = bypassCleanupOnSetup;
    }

    protected override void SetupForRun()
    {
        if (!bypassCleanupOnSetup)
        {
            try
            {
                Storage.DeleteDirectory(string.Empty);
            }
            catch
            {
            }
        }
        
        base.SetupForRun();
    }
}
