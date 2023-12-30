using Kumi.Game.Audio;
using Kumi.Game.Graphics;
using Kumi.Game.Online.API.Charts;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osuTK;

namespace Kumi.Game.Overlays.Listing.Info;

public partial class PreviewButton : ClickableContainer
{
    private const float button_size = 32;

    [Resolved]
    private TrackPreviewManager previewManager { get; set; } = null!;

    [Resolved]
    private Bindable<APIChartSet> selectedChartSet { get; set; } = null!;

    public PreviewButton()
    {
        Size = new Vector2(button_size);
    }

    private TrackPreview? preview;

    private CircularProgress progress = null!;
    private SpriteIcon icon = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new CircularContainer
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colours.Gray(0.1f)
                },
                progress = new CircularProgress
                {
                    RelativeSizeAxes = Axes.Both,
                    Scale = new Vector2(0.8f),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Current = { Value = 0f },
                    InnerRadius = 0.15f,
                    RoundedCaps = true,
                    Colour = Colours.CYAN_ACCENT_LIGHT,
                    Alpha = 0
                },
                icon = new SpriteIcon
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Size = new Vector2(8),
                    Icon = FontAwesome.Solid.Play,
                    Colour = Colours.GRAY_C
                }
            }
        };

        Action = togglePreview;

        selectedChartSet.BindValueChanged(_ =>
        {
            preview?.Stop();
            preview = null;
            
            progress.Alpha = 0;
            icon.Icon = FontAwesome.Solid.Play;
        }, true);
    }

    protected override void Update()
    {
        base.Update();

        if (preview is not { IsLoaded: true })
            return;

        progress.Alpha = preview.IsRunning ? 1 : 0;
        progress.Current.Value = preview.CurrentTime / preview.Length;
        icon.Icon = preview.IsRunning ? FontAwesome.Solid.Stop : FontAwesome.Solid.Play;
    }

    private void togglePreview()
    {
        if (preview?.IsRunning ?? false)
        {
            preview?.Stop();
            return;
        }

        if (preview == null)
        {
            preview = previewManager.Get(selectedChartSet.Value);
            LoadComponentAsync(preview, p =>
            {
                Add(p);
                togglePreview();
            });

            return;
        }

        // Stop any previews that were playing before this one
        previewManager.StopPlaying();

        preview?.Start();
    }
}
