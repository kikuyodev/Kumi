// TODO: Find a better place for this

using osu.Framework.Graphics;

namespace Kumi.Game.Screens.Edit;

public abstract partial class TransactionalCommitComponent : Component
{
    public event Action? TransactionBegan;
    public event Action? TransactionEnded;
    public event Action? SaveStateTriggered;

    public bool TransactionInProgress => bulkChangesStarted > 0;
    
    private int bulkChangesStarted;

    public virtual void BeginChange()
    {
        if (bulkChangesStarted++ == 0)
            TransactionBegan?.Invoke();
    }

    public void EndChange()
    {
        if (bulkChangesStarted == 0)
            throw new InvalidOperationException($"Cannot call {nameof(EndChange)} without a matching {nameof(BeginChange)} call.");

        if (--bulkChangesStarted == 0)
        {
            UpdateState();
            TransactionEnded?.Invoke();
        }
    }

    public void SaveState()
    {
        if (bulkChangesStarted > 0)
            return;
        
        SaveStateTriggered?.Invoke();
        UpdateState();
    }

    protected abstract void UpdateState();
}
