using Kumi.Game.Input;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Gameplay.UI;

public partial class DrawableDrum : Container
{
    public DrawableDrum()
    {
        RelativeSizeAxes = Axes.Both;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        const float split = 0.53f;

        Children = new Drawable[]
        {
            new CircularContainer
            {
                BorderColour = Color4.White.Opacity(0.1f),
                BorderThickness = 2,
                Masking = true,
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    // To make the border visible.
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Alpha = 0,
                        AlwaysPresent = true
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Size = new Vector2(0.85f),
                Children = new Drawable[]
                {
                    new DrawableHalfDrum(false)
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.CentreRight,
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.5f,
                        RelativePositionAxes = Axes.X,
                        X = split / 2,
                    },
                    new DrawableHalfDrum(true)
                    {
                        Anchor = Anchor.Centre,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.Both,
                        Width = 0.5f,
                        RelativePositionAxes = Axes.X,
                        X = -split / 2,
                    }
                }
            }
        };
    }

    private partial class DrawableHalfDrum : Container, IKeyBindingHandler<GameplayAction>
    {
        private readonly GameplayAction rimAction;
        private readonly GameplayAction centreAction;

        private readonly Sprite rim;
        private readonly Sprite rimHit;
        private readonly Sprite centre;
        private readonly Sprite centreHit;

        public DrawableHalfDrum(bool flipped)
        {
            Scale = new Vector2(flipped ? -1 : 1, 1);

            if (!flipped)
            {
                rimAction = GameplayAction.RightRim;
                centreAction = GameplayAction.RightCentre;
            }
            else
            {
                rimAction = GameplayAction.LeftRim;
                centreAction = GameplayAction.LeftCentre;
            }

            Children = new Drawable[]
            {
                rim = new Sprite
                {
                    Anchor = flipped ? Anchor.CentreLeft : Anchor.CentreRight,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    Alpha = 0.1f
                },
                rimHit = new Sprite
                {
                    Anchor = flipped ? Anchor.CentreLeft : Anchor.CentreRight,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.X,
                    Scale = new Vector2(2.2f, 1.2f),
                    X = -0.39f,
                    Alpha = 0f
                },
                centre = new Sprite
                {
                    Anchor = flipped ? Anchor.CentreLeft : Anchor.CentreRight,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.X,
                    Alpha = 0.1f,
                    Scale = new Vector2(0.7f),
                    X = -0.125f,
                },
                centreHit = new Sprite
                {
                    Anchor = flipped ? Anchor.CentreLeft : Anchor.CentreRight,
                    Origin = Anchor.Centre,
                    RelativeSizeAxes = Axes.Both,
                    RelativePositionAxes = Axes.X,
                    Scale = new Vector2(0.7f),
                    Size = new Vector2(2.27f, 1.27f),
                    X = -0.37f,
                    Alpha = 0f
                }
            };
        }

        [BackgroundDependencyLoader]
        private void load(TextureStore store)
        {
            rim.Texture = store.Get("Gameplay/Drum/rim");
            rimHit.Texture = store.Get("Gameplay/Drum/rim-hit");
            centre.Texture = store.Get("Gameplay/Drum/centre");
            centreHit.Texture = store.Get("Gameplay/Drum/centre-hit");
        }

        public bool OnPressed(KeyBindingPressEvent<GameplayAction> e)
        {
            Drawable? target = null;

            if (e.Action == centreAction)
                target = centreHit;
            else if (e.Action == rimAction)
                target = rimHit;

            if (target != null)
            {
                target.Animate(
                    t => t.FadeTo(1f, 50, Easing.OutQuint)
                ).Then(
                    t => t.FadeOut(800, Easing.OutQuint)
                );
            }

            // Don't consume the event.
            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<GameplayAction> e)
        {
        }
    }
}
