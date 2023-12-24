using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace Kumi.Game.Overlays.Control;

public abstract partial class ProgressNotification : BasicNotification
{
    public BindableInt Current { get; set; } = new BindableInt();
    public BindableInt Target { get; set; } = new BindableInt();

    public ProgressNotification(int target, int current = 0)
    {
        Current.Set(current);
        Target.Set(target);
        
        Current.BindValueChanged(_ => updateProgress());
        Target.BindValueChanged(_ =>
        {
            _current.MaxValue = Target.Value;
            updateProgress();
        });
        
        _current = new BindableNumberWithCurrent<float>()
        {
            MinValue = 0,
            MaxValue = target
        };
    }

    public void Increment(int amount = 1) => Current.Set(Current.Value + amount);
    public void Decrement(int amount = 1) => Current.Set(Current.Value - amount);
    public void Set(int value) => Current.Set(value);

    private void updateProgress()
    {
        if (Current.Value == Target.Value)
        {
            Hide();
            return;
        }
        
        _current.Set(Current.Value);
    }

    private BindableNumberWithCurrent<float> _current;

    protected string GetProgressText() => $"{Current.Value}/{Target}";

    protected override void CreateContent()
    {
        base.CreateContent();

        Content.AddRange(new []
        {
            new ProgressBar(_current)
        });
    }

    private partial class ProgressBar : Container
    {
        private Container progressBackground;
        private Box progressBar;
        
        public BindableNumberWithCurrent<float> Current { get; set; }

        public ProgressBar(BindableNumberWithCurrent<float> current)
        {
            Current = current;
        }

        [BackgroundDependencyLoader]
        private void load()
        {
            CornerRadius = 5;
            RelativeSizeAxes = Axes.X;
            Height = 5;
            
            InternalChildren = new[]
            {
                progressBackground = new Container()
                {
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.5f,
                    Children = new []
                    {
                        progressBar = new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Width = 0,
                            Colour = Colours.BLUE_ACCENT_LIGHT
                        },
                        new Box()
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = Colours.GRAY_4,
                        }
                    }
                },
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            Current.BindValueChanged(value =>
            {
                progressBar.ResizeWidthTo(value.NewValue / Current.MaxValue, 500, Easing.OutQuint);
            });
        }
    }
}
