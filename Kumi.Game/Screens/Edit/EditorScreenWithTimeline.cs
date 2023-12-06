using Kumi.Game.Screens.Edit.Timeline;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Screens.Edit;

public abstract partial class EditorScreenWithTimeline : EditorScreen
{
    public const float PADDING = 8;

    public Container TimelineContent { get; private set; } = null!;

    public Container MainContent { get; private set; } = null!;

    protected EditorScreenWithTimeline(EditorScreenMode type, bool pushContent = true)
        : base(type)
    {
        if (pushContent)
        {
            InternalChild = new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                RowDimensions = new []
                {
                    new Dimension(GridSizeMode.AutoSize),
                    new Dimension()
                },
                Content = new[]
                {
                    new[] { createTimelineContent() },
                    new[] { createMainContent() }
                }
            };    
        }
        else
        {
            InternalChild = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new[]
                {
                    CreateMainContent(),
                    createTimelineContent()
                }
            };
        }
    }

    protected abstract Drawable CreateMainContent();
    
    private Drawable createMainContent()
    {
        MainContent = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Depth = float.MaxValue
        };

        Scheduler.AddOnce(() => LoadComponentAsync(CreateMainContent(), content =>
        {
            MainContent.Add(content);
            content.FadeInFromZero(300);
        }));

        return MainContent;
    }

    private Drawable createTimelineContent()
    {
        TimelineContent = new Container
        {
            RelativeSizeAxes = Axes.X,
            AutoSizeAxes = Axes.Y,
            Padding = new MarginPadding { Top = 12 + EditorOverlay.TOP_BAR_HEIGHT + PADDING, Horizontal = 12 },
        };

        Scheduler.AddOnce(() => LoadComponentAsync(new TimelineArea(), TimelineContent.Add));

        return TimelineContent;
    }
}
