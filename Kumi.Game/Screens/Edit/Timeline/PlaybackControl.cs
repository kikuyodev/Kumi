using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.IEnumerableExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
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

    private partial class PlaybackTabControl : KumiTabControl<double>
    {
        private static readonly double[] tempo_values = { 0.25, 0.5, 0.75, 1 };

        protected override TabItem<double> CreateTabItem(double value) => new PlaybackTabItem(value);

        public PlaybackTabControl()
        {
            tempo_values.ForEach(AddItem);

            Current.Value = tempo_values.Last();
        }
        
        private partial class PlaybackTabItem : KumiTabItem
        {
            public PlaybackTabItem(double value)
                : base(value)
            {
                Width = 1f / tempo_values.Length;
            }

            protected override LocalisableString GetText() => $"{Value:0%}";
        }
    }
}
