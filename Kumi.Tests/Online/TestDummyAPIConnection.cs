using Kumi.Game.Online.API;
using Kumi.Game.Tests;
using Kumi.Tests.Visual;
using NUnit.Framework;
using osu.Framework.Testing;

namespace Kumi.Tests.Online;

[HeadlessTest]
public partial class TestDummyAPIConnection : KumiTestScene
{
    [Test]
    public void TestLogin()
    {
        AddStep("login to account", () =>
        {
            // Login to the API.
            Provider.Login("username", "password");
        });
        
        var account = Provider.LocalAccount;

        AddAssert("is online", () => Provider.State.Value == APIState.Online);
        AddAssert("account is not null", () => account.Value != null);
        AddAssert("username is equal to Dummy", () => !string.IsNullOrEmpty(account.Value.Username) && account.Value.Username == "Dummy");
    }

    [TestCase(false)]
    [TestCase(true)]
    public void SimpleTestRequest(bool shouldFail)
    {
        APIRequest? request = null;
        var dummy = Provider as DummyAPIConnection;

        AddStep("assign request handler", () => dummy!.HandleRequest = _ => !shouldFail);
        AddAssert("request is null", () => request == null);
        AddStep("make request", () => dummy!.Perform(request = new TestAPIRequest()));
        AddAssert($"request {(shouldFail ? "failed" : "succeeded")}", () => request != null && request.CompletionState == (shouldFail ? APICompletionState.Failed : APICompletionState.Completed));
        AddStep("clear request", () => request = null);
        AddStep("make request async", () => dummy!.PerformAsync(request = new TestAPIRequest()));
        AddUntilStep($"request {(shouldFail ? "failed" : "succeeded")}", () => request != null && request.CompletionState == (shouldFail ? APICompletionState.Failed : APICompletionState.Completed));
    }

    [TestCase(1)]
    [TestCase(10)]
    public void QueueTestRequest(int amount)
    {
        var dummy = Provider as DummyAPIConnection;

        AddStep("pause queue", () => dummy!.PauseQueue = true);
        AddAssert("queue is empty", () => dummy!.RequestQueue.Count == 0);
        AddStep("queue requests", () =>
        {
            for (int i = 0; i < amount; i++)
                dummy!.Queue(new TestAPIRequest());
        });
        AddAssert($"queue has {amount} requests", () => dummy!.RequestQueue.Count == amount);
        AddStep("flush queue", () => dummy!.PauseQueue = false);
        AddUntilStep("wait until empty", () => dummy!.RequestQueue.Count == 0);
        AddStep("pause queue", () => dummy!.PauseQueue = true);
        AddStep("queue requests", () =>
        {
            for (int i = 0; i < amount; i++)
                dummy!.Queue(new TestAPIRequest());
        });
        AddAssert("front item has ID -1", () => ((TestAPIRequest)dummy!.RequestQueue.Peek()).Id == -1);
        AddStep("queue request at front", () => dummy!.ForceDequeue(new TestAPIRequest(1)));
        AddAssert("front item has ID 1", () => ((TestAPIRequest)dummy!.RequestQueue.Peek()).Id == 1);
        AddStep("flush queue", () => dummy!.PauseQueue = false);
        AddUntilStep("wait until empty", () => dummy!.RequestQueue.Count == 0);
    }

    private class TestAPIRequest : APIRequest
    {
        public readonly int Id;

        public TestAPIRequest(int id = -1)
        {
            Id = id;
        }

        public override string Endpoint => "/nowhere";
    }
}
