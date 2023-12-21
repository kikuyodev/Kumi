using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Bindables;

namespace Kumi.Game.Screens.Edit.Timing.Configuration;

public partial class BPMTextBox : KumiTextBox
{
    private readonly Bindable<float> bpmBindable = new Bindable<float>();

    public Bindable<float> BpmBindable
    {
        get => bpmBindable;
        set
        {
            bpmBindable.UnbindBindings();
            bpmBindable.BindTo(value);
        }
    }
        
    public BPMTextBox()
    {
        OnCommit += (_, isNew) =>
        {
            if (!isNew)
                return;

            try
            {
                if (double.TryParse(Current.Value, out double newValue) && newValue > 0)
                    bpmBindable.Value = (float)newValue;
            }
            catch
            {
                // ignored
            }
                
            bpmBindable.TriggerChange();
        };
            
        bpmBindable.BindValueChanged(val =>
        {
            Current.Value = $"{val.NewValue:N2}";
        }, true);
    }
}
