using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Gameplay.Algorithms;
using Kumi.Game.Gameplay.Drawables;
using Kumi.Game.Gameplay.UI;
using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Containers;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;
using osuTK.Graphics;

namespace Kumi.Game.Gameplay;

public partial class KumiPlayfield : ScrollingPlayfield
{
    private Container contentContainer = null!;
    
    private readonly Bindable<IScrollAlgorithm> algorithm = new Bindable<IScrollAlgorithm>(new LinearScrollAlgorithm());

    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) => contentContainer.ReceivePositionalInputAt(screenSpacePos);

    public KumiPlayfield(WorkingChart workingChart)
        : base(workingChart)
    {
        ScrollContainer.Algorithm.BindTo(algorithm);
        ScrollContainer.TimeRange = 1000;
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        var drawableDrum = new DrawableDrum
        {
            Anchor = Anchor.CentreRight,
            Origin = Anchor.CentreRight
        };

        Children = new Drawable[]
        {
            contentContainer = new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 200,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Colours.Gray(0.05f).Opacity(0.95f)
                    },
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        FillMode = FillMode.Fit,
                        Child = new ConstrictedScalingContainer
                        {
                            PreferredSize = new Vector2(144),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            RelativeSizeAxes = Axes.Both,
                            RelativePositionAxes = Axes.X,
                            Size = new Vector2(0.8f),
                            X = 0.3f,
                            Child = drawableDrum.CreateProxy()
                        }
                    },
                    new Circle
                    {
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.Y,
                        RelativePositionAxes = Axes.X,
                        X = 0.2275f,
                        Size = new Vector2(3, 0.8f),
                        Colour = Color4.White.Opacity(0.05f),
                    },
                    new ConstrictedScalingContainer
                    {
                        PreferredSize = new Vector2(114),
                        Anchor = Anchor.CentreLeft,
                        Origin = Anchor.CentreLeft,
                        RelativeSizeAxes = Axes.Y,
                        Height = 200,
                        Size = new Vector2(0.6f),
                        RelativePositionAxes = Axes.X,
                        X = 0.325f,
                        Child = new CircularContainer
                        {
                            Masking = true,
                            RelativeSizeAxes = Axes.Both,
                            BorderThickness = 2,
                            BorderColour = Color4.White.Opacity(0.05f),
                            Children = new Drawable[]
                            {
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Alpha = 0,
                                    AlwaysPresent = true
                                },
                                new CircularContainer
                                {
                                    Masking = true,
                                    RelativeSizeAxes = Axes.Both,
                                    Size = new Vector2(0.7f),
                                    Anchor = Anchor.Centre,
                                    Origin = Anchor.Centre,
                                    BorderThickness = 2,
                                    BorderColour = Color4.White.Opacity(0.1f),
                                    Children = new Drawable[]
                                    {
                                        new Box
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Colour = Colours.Gray(0.05f).Opacity(0.95f)
                                        },
                                        new Circle
                                        {
                                            RelativeSizeAxes = Axes.Both,
                                            Size = new Vector2(0.9f),
                                            Anchor = Anchor.Centre,
                                            Origin = Anchor.Centre,
                                            Colour = Color4.White.Opacity(0.05f)
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 200 * 0.6f,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                RelativePositionAxes = Axes.Both,
                Width = 1 - 0.325f,
                Child = ScrollContainer
            },
            // Added to the end of the hierarchy to receive input before any objects
            new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = 200,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                RelativePositionAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        FillMode = FillMode.Fit,
                        Child = new ConstrictedScalingContainer
                        {
                            PreferredSize = new Vector2(144),
                            Anchor = Anchor.CentreLeft,
                            Origin = Anchor.CentreLeft,
                            RelativeSizeAxes = Axes.Both,
                            RelativePositionAxes = Axes.X,
                            Size = new Vector2(0.8f),
                            X = 0.3f,
                            Child = drawableDrum
                        }
                    }
                }
            }
        };
    }

    protected override DrawableNote CreateDrawableNote(INote note)
    {
        if (note is DrumHit drumHit)
        {
            if (note.Flags.Value.HasFlagFast(NoteFlags.Big))
                return new DrawableBigDrumHit(drumHit);
            
            return new DrawableDrumHit(drumHit);
        }
        
        throw new ArgumentException("Unsupported note type", nameof(note));
    }

    public double TimeAtScreenSpacePosition(Vector2 screenSpacePosition)
        => ScrollContainer.TimeAtScreenSpacePosition(screenSpacePosition);
    
    public Vector2 ScreenSpacePositionAtTime(double time)
        => ScrollContainer.ScreenSpacePositionAtTime(time);
}
