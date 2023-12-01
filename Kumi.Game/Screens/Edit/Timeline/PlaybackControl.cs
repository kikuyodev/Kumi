using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Input;

namespace Kumi.Game.Screens.Edit.Timeline;

public partial class PlaybackControl : Container
{
    private KumiIconButton playButton = null!;

    [Resolved]
    private EditorClock clock { get; set; } = null!;

    private readonly IBindable<Track> track = new Bindable<Track>();

    private readonly BindableNumber<double> tempo = new BindableNumber<double>(1);

    [BackgroundDependencyLoader]
    private void load()
    {
        Children = new Drawable[]
        {
            playButton = new KumiIconButton
            {
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft,
                Icon = FontAwesome.Solid.PlayCircle,
                IconScale = new(0.5f),
                BackgroundColour = Color4Extensions.FromHex("0D0D0D"),
                IconColour = Color4Extensions.FromHex("CCCCCC"),
                Size = new Vector2(20),
                Action = togglePause
            },
            new Container
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding { Left = 20 },
                Child = new PlaybackTabControl { Current = tempo }
            }
        };

        track.BindTo(clock.Track);
        track.BindValueChanged(t => t.NewValue?.AddAdjustment(AdjustableProperty.Tempo, tempo), true);
    }

    protected override void Dispose(bool isDisposing)
    {
        track.Value?.RemoveAdjustment(AdjustableProperty.Tempo, tempo);

        base.Dispose(isDisposing);
    }

    protected override bool OnKeyDown(KeyDownEvent e)
    {
        if (e.Repeat)
            return false;

        switch (e.Key)
        {
            case Key.Space:
                togglePause();
                return true;
        }

        return base.OnKeyDown(e);
    }

    private void togglePause()
    {
        if (clock.IsRunning)
            clock.Stop();
        else
            clock.Start();
    }

    protected override void Update()
    {
        base.Update();
        playButton.Icon = clock.IsRunning ? FontAwesome.Regular.PauseCircle : FontAwesome.Solid.PlayCircle;
    }

    private partial class PlaybackTabControl : TabControl<double>
    {
        private static readonly double[] tempo_values = { 0.25, 0.5, 0.75, 1 };

        protected override TabItem<double> CreateTabItem(double value) => new PlaybackTabItem(value);
        protected override Dropdown<double> CreateDropdown() => null!;

        protected override TabFillFlowContainer CreateTabFlow() => new TabFillFlowContainer()
        {
            Direction = FillDirection.Horizontal,
            RelativeSizeAxes = Axes.Both,
            Depth = -1
        };

        private readonly Circle strip;

        public PlaybackTabControl()
        {
            RelativeSizeAxes = Axes.Both;
            TabContainer.Spacing = Vector2.Zero;

            tempo_values.ForEach(AddItem);

            Current.Value = tempo_values.Last();
            Current.BindValueChanged(_ => onNewValue());

            AddInternal(strip = new Circle
            {
                Anchor = Anchor.BottomLeft,
                Origin = Anchor.BottomLeft,
                Size = new Vector2(0, 3),
                Colour = Color4Extensions.FromHex("3377FF"),
                Alpha = 0,
                Y = 5,
                EdgeEffect = new EdgeEffectParameters
                {
                    Roundness = 3,
                    Type = EdgeEffectType.Glow,
                    Colour = Color4Extensions.FromHex("3377FF").Opacity(0.15f),
                    Radius = 4
                }
            });
        }
        protected override void LoadComplete()
        {
            base.LoadComplete();
            Scheduler.AddDelayed(onNewValue, 25);
        }

        private void onNewValue()
        {
            var drawable = TabContainer.Children.FirstOrDefault(c => c.Value == Current.Value);

            if (drawable == null)
                return;

            strip.MoveToX(drawable.DrawPosition.X, 200, Easing.OutQuint);
            strip.ResizeWidthTo(drawable.DrawSize.X, 200, Easing.OutQuint);
            strip.FadeIn(200, Easing.OutQuint);
        }

        private partial class PlaybackTabItem : TabItem<double>
        {
            private const float fade_duration = 200;

            private readonly SpriteText text;
            private readonly SpriteText textBold;

            public PlaybackTabItem(double value)
                : base(value)
            {
                RelativeSizeAxes = Axes.Both;

                Width = 1f / tempo_values.Length;

                Children = new Drawable[]
                {
                    text = new SpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = $"{value:0%}",
                        Font = KumiFonts.GetFont(size: 10),
                        Colour = Color4Extensions.FromHex("666666")
                    },
                    textBold = new SpriteText
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Text = $"{value:0%}",
                        Font = KumiFonts.GetFont(weight: FontWeight.SemiBold, size: 10),
                        Colour = Color4Extensions.FromHex("CCCCCC"),
                        Alpha = 0
                    },
                };
            }

            protected override bool OnHover(HoverEvent e)
            {
                updateState();
                return base.OnHover(e);
            }

            protected override void OnHoverLost(HoverLostEvent e) => updateState();
            protected override void OnActivated() => updateState();
            protected override void OnDeactivated() => updateState();

            private void updateState()
            {
                text.FadeColour(Active.Value || IsHovered ? Color4Extensions.FromHex("CCCCCC") : Color4Extensions.FromHex("666666"), fade_duration, Easing.OutQuint);
                text.FadeTo(Active.Value ? 0 : 1, fade_duration, Easing.OutQuint);
                textBold.FadeTo(Active.Value ? 1 : 0, fade_duration, Easing.OutQuint);
            }
        }
    }
}
