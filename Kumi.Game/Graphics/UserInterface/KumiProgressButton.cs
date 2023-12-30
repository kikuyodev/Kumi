using osu.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Localisation;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Graphics.UserInterface;

public partial class KumiProgressButton : KumiIconButton, IStateful<ButtonState>
{
    public LocalisableString Label
    {
        get => labelText.Text;
        set => labelText.Text = value;
    }

    public float Progress
    {
        get => progress.Width;
        set => Schedule(() => progress.ResizeWidthTo(value, 200, Easing.OutQuint));
    }
    
    public new Vector2 IconScale
    {
        get => base.IconScale;
        set
        {
            base.IconScale = value;
            loadingIcon.Scale = value;
        }
    }

    private ButtonState state;

    public ButtonState State
    {
        get => state;
        set
        {
            if (state == value)
                return;

            state = value;
            StateChanged?.Invoke(state);
        }
    }

    public event Action<ButtonState>? StateChanged;

    private SpriteIcon loadingIcon = null!;
    private SpriteText labelText = null!;

    private Circle progress = null!;

    protected override Drawable[] CreateContent()
        => new Drawable[]
        {
            Background = new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = BackgroundColour
            },
            Container = new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new FillFlowContainer
                    {
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Direction = FillDirection.Horizontal,
                        Spacing = new Vector2(4),
                        Children = new Drawable[]
                        {
                            new Container
                            {
                                Size = new Vector2(20),
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Children = new Drawable[]
                                {
                                    SpriteIcon = new SpriteIcon
                                    {
                                        Icon = Icon,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Size = new Vector2(20)
                                    },
                                    loadingIcon = new SpriteIcon
                                    {
                                        Icon = FontAwesome.Solid.CircleNotch,
                                        Anchor = Anchor.Centre,
                                        Origin = Anchor.Centre,
                                        Size = new Vector2(20),
                                        Alpha = 0,
                                        AlwaysPresent = true
                                    }
                                }
                            },
                            labelText = new SpriteText
                            {
                                Anchor = Anchor.Centre,
                                Origin = Anchor.Centre,
                                Font = KumiFonts.GetFont(weight: FontWeight.Medium, size: 14)
                            }
                        }
                    },
                    new Container
                    {
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        RelativeSizeAxes = Axes.X,
                        AutoSizeAxes = Axes.Y,
                        Padding = new MarginPadding(2),
                        Child = progress = new Circle
                        {
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            RelativeSizeAxes = Axes.X,
                            Height = 2,
                            Width = 0,
                            Colour = Color4.White,
                            Alpha = 0
                        }
                    }
                }
            }
        };

    protected override void LoadComplete()
    {
        base.LoadComplete();

        loadingIcon.Spin(1000, RotationDirection.Clockwise).Loop();

        StateChanged += s => Schedule(() =>
        {
            switch (s)
            {
                case ButtonState.Idle:
                    SpriteIcon.FadeIn(200, Easing.OutQuint);
                    loadingIcon.FadeOut(200, Easing.OutQuint);
                    progress.FadeOut(200, Easing.OutQuint);
                    Enabled.Value = true;
                    break;

                case ButtonState.Loading:
                    SpriteIcon.FadeOut(200, Easing.OutQuint);
                    loadingIcon.FadeIn(200, Easing.OutQuint);
                    progress.FadeIn(200, Easing.OutQuint);
                    Enabled.Value = false;
                    break;

                case ButtonState.Success:
                    SpriteIcon.FadeIn(200, Easing.OutQuint);
                    loadingIcon.FadeOut(200, Easing.OutQuint);
                    progress.FadeOut(200, Easing.OutQuint);
                    Enabled.Value = true;
                    break;
            }
        });
    }
}

public enum ButtonState
{
    Idle,
    Loading,
    Success,
}
