using Kumi.Game.Charts;
using Kumi.Game.Database;
using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Online.API;
using Kumi.Game.Online.API.Requests;
using Kumi.Game.Overlays.Settings.Components;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Threading;
using osuTK;

namespace Kumi.Game.Screens.Edit.Popup;

public partial class UploadPopup : EditorPopup
{
    protected override bool CanBeExited => !uploadInProgress;

    [Resolved]
    private IAPIConnectionProvider api { get; set; } = null!;

    [Resolved]
    private ChartManager chartManager { get; set; } = null!;

    [Resolved]
    private RealmAccess realm { get; set; } = null!;

    [Resolved]
    private Editor editor { get; set; } = null!;

    [Resolved]
    private EditorChart editorChart { get; set; } = null!;

    public UploadPopup()
    {
        Size = new Vector2(400, 200);
    }

    private bool uploadInProgress;

    private KumiTextBox descriptionBox = null!;
    private readonly BindableBool isWip = new BindableBool(true);

    private KumiProgressButton uploadButton = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        Child = new Container
        {
            RelativeSizeAxes = Axes.Both,
            Masking = true,
            CornerRadius = 5,
            Children = new Drawable[]
            {
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Colour = Colours.Gray(0.05f)
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(8),
                    Children = new Drawable[]
                    {
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Direction = FillDirection.Vertical,
                            Spacing = new Vector2(0, 4),
                            Children = new Drawable[]
                            {
                                new SpriteText
                                {
                                    Text = "Upload",
                                    Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold),
                                    Colour = Colours.GRAY_C,
                                },
                                new SpriteText
                                {
                                    Text = "Upload your map to the Kumi database.",
                                    Font = KumiFonts.GetFont(size: 12),
                                    Colour = Colours.GRAY_6,
                                },
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(0, 4),
                                    Margin = new MarginPadding { Top = 12 },
                                    Children = new Drawable[]
                                    {
                                        descriptionBox = new KumiTextBox
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 24,
                                            PlaceholderText = "Description",
                                        },
                                        new CheckboxSettingItem
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Label = "WIP",
                                            Description = "Is this map a work in progress?",
                                            Current = { BindTarget = isWip }
                                        }
                                    }
                                },
                            }
                        },
                        new FillFlowContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            Anchor = Anchor.BottomCentre,
                            Origin = Anchor.BottomCentre,
                            Direction = FillDirection.Horizontal,
                            Spacing = new Vector2(8, 0),
                            Children = new Drawable[]
                            {
                                new KumiButton
                                {
                                    RelativeSizeAxes = Axes.None,
                                    Width = 100,
                                    Text = "Cancel",
                                    Action = Hide,
                                    Anchor = Anchor.BottomCentre,
                                    Origin = Anchor.BottomCentre
                                },
                                uploadButton = new KumiProgressButton
                                {
                                    RelativeSizeAxes = Axes.None,
                                    Width = 100,
                                    Height = 25,
                                    Label = "Upload",
                                    Icon = FontAwesome.Solid.Upload,
                                    IconScale = new Vector2(0.6f),
                                    Important = true,
                                    Anchor = Anchor.BottomCentre,
                                    Origin = Anchor.BottomCentre,
                                    Action = () =>
                                    {
                                        if (uploadInProgress)
                                            return;

                                        editor.Save();

                                        startUpload();
                                        waitForUpload();
                                    },
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private Task? uploadTask;

    private void startUpload()
    {
        uploadInProgress = true;
        descriptionBox.Current.Disabled = true;
        isWip.Disabled = true;
        uploadButton.State = ButtonState.Loading;

        uploadTask = Task.Factory.StartNew(upload, TaskCreationOptions.LongRunning);
    }

    private ScheduledDelegate? waitForUploadDelegate;

    private void waitForUpload()
    {
        Scheduler.Add(waitForUploadDelegate = new ScheduledDelegate(() =>
        {
            if (uploadTask is not { IsCompleted: true })
                return;

            uploadTask = null;
            waitForUploadDelegate?.Cancel();
            waitForUploadDelegate = null;

            finishUpload();
        }, 0, 10));
    }

    private void upload()
    {
        using var stream = new MemoryStream();
        chartManager.ExportModelToStream(editorChart.ChartInfo.ChartSet!, stream);

        stream.Seek(0, SeekOrigin.Begin);

        var request = new UploadChartSetRequest
        {
            ChartSetStream = stream,
            IsWip = isWip.Value,
        };

        request.UploadProgress += (value, length) =>
        {
            var progress = (float) value / length;
            uploadButton.Progress = progress;
        };

        request.Success += () => Schedule(() =>
        {
            var data = request.Response;
            var uploadedCharts = data.GetUploadedCharts();
            var uploadedSet = data.GetUploadedSet();

            realm.Write(r =>
            {
                var set = r.Find<ChartSetInfo>(editorChart.ChartInfo.ChartSet!.ID);
                if (set is null)
                    return;

                set.OnlineID = uploadedSet.Id;
                foreach (var chartInfo in set.Charts)
                {
                    var uploadedChart = uploadedCharts.FirstOrDefault(c => c.OriginalHash == chartInfo.Hash);
                    if (uploadedChart is null)
                        continue;

                    chartInfo.OnlineID = uploadedChart.Chart.Id;
                }
            });
        });
        
        api.Perform(request);
    }

    private void finishUpload()
    {
        uploadInProgress = false;
        descriptionBox.Current.Disabled = false;
        isWip.Disabled = false;
        uploadButton.State = ButtonState.Idle;
    }
}
