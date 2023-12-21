using Kumi.Game.Charts.Timings;
using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Kumi.Game.Screens.Edit.Timing.Configuration;

public partial class TimeSignatureTextBox : FillFlowContainer
{
    private readonly Bindable<TimeSignature> timeSignatureBindable = new Bindable<TimeSignature>();
    private readonly Bindable<int> numeratorBindable = new Bindable<int>();
    private readonly Bindable<int> denominatorBindable = new Bindable<int>();

    public Bindable<TimeSignature> TimeSignatureBindable
    {
        get => timeSignatureBindable;
        set
        {
            timeSignatureBindable.UnbindBindings();
            timeSignatureBindable.BindTo(value);
            
            numeratorBindable.UnbindAll();
            numeratorBindable.BindValueChanged(v =>
            {
                timeSignatureBindable.Value = new TimeSignature(v.NewValue, timeSignatureBindable.Value.Denominator);
            });
            
            denominatorBindable.UnbindAll();
            denominatorBindable.BindValueChanged(v =>
            {
                timeSignatureBindable.Value = new TimeSignature(timeSignatureBindable.Value.Numerator, v.NewValue);
            });
            
            timeSignatureBindable.BindValueChanged(v =>
            {
                numeratorBindable.Value = v.NewValue.Numerator;
                denominatorBindable.Value = v.NewValue.Denominator;
            }, true);
        }
    }

    public TimeSignatureTextBox()
    {
        AutoSizeAxes = Axes.Both;
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;
        Direction = FillDirection.Horizontal;
        Spacing = new Vector2(4, 0);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            new NumberTextBox
            {
                Width = 30,
                Height = 24,
                NumberBindable = numeratorBindable,
                BackgroundColour = Colours.Gray(0.05f),
                LengthLimit = 1,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            new SpriteText
            {
                Text = "/",
                Font = KumiFonts.GetFont(size: 12),
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            new NumberTextBox
            {
                Width = 30,
                Height = 24,
                NumberBindable = denominatorBindable,
                BackgroundColour = Colours.Gray(0.05f),
                LengthLimit = 1,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            }
        });
    }

    private partial class NumberTextBox : KumiTextBox
    {
        private readonly Bindable<int> numberBindable = new Bindable<int>();

        public Bindable<int> NumberBindable
        {
            get => numberBindable;
            set
            {
                numberBindable.UnbindBindings();
                numberBindable.BindTo(value);
            }
        }
        
        public NumberTextBox()
        {
            CommitOnFocusLost = true;
            
            OnCommit += (_, isNew) =>
            {
                if (!isNew)
                    return;

                try
                {
                    if (int.TryParse(Current.Value, out int newValue) && newValue > 0)
                        numberBindable.Value = newValue;
                }
                catch
                {
                    // ignored
                }
                
                numberBindable.TriggerChange();
            };
            
            numberBindable.BindValueChanged(val =>
            {
                Current.Value = $"{val.NewValue:N0}";
            }, true);
        }
    }
}
