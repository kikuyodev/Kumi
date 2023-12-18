using Kumi.Game.Charts;
using Kumi.Game.Charts.Objects;
using Kumi.Game.Graphics;
using Kumi.Game.Screens.Edit.Blueprints;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Primitives;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osuTK;

namespace Kumi.Game.Screens.Edit.Timeline;

public partial class TimelineNoteBlueprint : SelectionBlueprint<Note>
{
    private readonly Bindable<double> startTime;
    
    private readonly SpanCircle spanCircle;
    private readonly Border border;

    public TimelineNoteBlueprint(Note item)
        : base(item)
    {
        Anchor = Anchor.CentreLeft;
        Origin = Anchor.CentreLeft;

        startTime = item.StartTimeBindable.GetBoundCopy();
        startTime.BindValueChanged(time => X = (float) time.NewValue, true);

        RelativePositionAxes = Axes.X;

        RelativeSizeAxes = Axes.X;
        Height = 32;
        
        AddRangeInternal(new Drawable[]
        {
            spanCircle = new SpanCircle
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            },
            border = new Border
            {
                RelativeSizeAxes = Axes.Both,
                Anchor = Anchor.CentreLeft,
                Origin = Anchor.CentreLeft
            }
        });
        
        updateColour();
    }

    protected override void OnSelected()
    {
        updateColour();
    }

    protected override void OnDeselected()
    {
        updateColour();
    }

    private void updateColour()
    {
        var colour = Item.Type.Value switch
        {
            NoteType.Don => Color4Extensions.FromHex("F96226"),
            NoteType.Kat => Color4Extensions.FromHex("68C0C1"),
            _ => Color4Extensions.FromHex("F9A826")
        };
        
        if (IsSelected)
            border.Show();
        else
            border.Hide();

        if (Item is IHasEndTime endTime && endTime.EndTime > Item.StartTime)
            spanCircle.Colour = ColourInfo.GradientHorizontal(colour, colour.Lighten(0.4f));
        else
            spanCircle.Colour = colour;
    }

    protected override void Update()
    {
        base.Update();

        var duration = (float) (Item.GetEndTime() - Item.StartTime);

        if (Width != duration)
            Width = duration;
    }

    protected override bool ShouldBeConsideredForInput(Drawable child)
        => true;

    public override bool ReceivePositionalInputAt(Vector2 screenSpacePos)
        => spanCircle.ReceivePositionalInputAt(screenSpacePos);

    public override Quad SelectionQuad
        => spanCircle.ScreenSpaceDrawQuad;

    public override Vector2 ScreenSpaceSelectionPoint
        => ScreenSpaceDrawQuad.TopLeft;

    protected override bool ComputeIsMaskedAway(RectangleF maskingBounds)
    {
        if (!base.ComputeIsMaskedAway(maskingBounds))
            return false;

        var rect = RectangleF.Union(ScreenSpaceDrawQuad.AABBFloat, spanCircle.ScreenSpaceDrawQuad.AABBFloat);
        return !Precision.AlmostIntersects(maskingBounds, rect);
    }
    
    public partial class Border : SpanCircle
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Content.Child.Alpha = 0;
            Content.Child.AlwaysPresent = true;

            Content.BorderColour = Colours.YELLOW_ACCENT_LIGHT;
        }
    }

    public partial class SpanCircle : CompositeDrawable
    {
        protected readonly Circle Content;

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos)
            => Content.ReceivePositionalInputAt(screenSpacePos);

        public override Quad ScreenSpaceDrawQuad
            => Content.ScreenSpaceDrawQuad;
        
        public new ColourInfo Colour
        {
            get => base.Colour;
            set
            {
                base.Colour = value;
                Content.BorderColour = value;
            }
        }

        public SpanCircle()
        {
            Padding = new MarginPadding { Horizontal = -32 / 2f };

            InternalChild = Content = new Circle
            {
                BorderThickness = 4,
                Masking = true,
                RelativeSizeAxes = Axes.Both
            };
        }
    }
}
