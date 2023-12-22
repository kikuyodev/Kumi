using Kumi.Game.Charts;
using Kumi.Game.Database;
using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;

namespace Kumi.Game.Screens.Select;

public partial class EditWedge : CompositeDrawable, ICanAcceptFiles
{
    public IEnumerable<string> HandledFileExtensions => new[] { ".mp3", ".ogg" };

    public Action<WorkingChart>? OnImport;

    [Resolved]
    private ChartManager manager { get; set; } = null!;

    [Resolved]
    private KumiGameBase? game { get; set; }

    public EditWedge()
    {
        RelativeSizeAxes = Axes.X;
        Height = 128;

        Padding = new MarginPadding { Top = 16 };
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        InternalChild = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            CornerRadius = 5,
            Children = new Drawable[]
            {
                new Box
                {
                    Colour = Colours.Gray(0.05f),
                    Alpha = 0.9f,
                    RelativeSizeAxes = Axes.Both
                },
                new SpriteText
                {
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Text = "Drag and drop audio files here to import them.",
                }
            }
        };
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();

        game?.RegisterFileAcceptor(this);
    }

    public Task Import(ImportTask[] tasks) => throw new NotImplementedException();

    public Task Import(params string[] paths)
    {
        var toImport = new FileInfo(paths.First());

        if (!toImport.Exists)
            return Task.CompletedTask;

        var newChart = manager.CreateNew();
        var destination = new FileInfo($"audio{toImport.Extension}");

        using (var stream = toImport.OpenRead())
            manager.AddFile(newChart.ChartSetInfo, stream, destination.Name);

        newChart.Metadata.AudioFile = destination.Name;

        OnImport?.Invoke(newChart);

        return Task.CompletedTask;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);

        game?.UnregisterFileAcceptor(this);
    }
}
