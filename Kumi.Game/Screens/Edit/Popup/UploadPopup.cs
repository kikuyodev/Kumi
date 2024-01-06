using Kumi.Game.Charts;
using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Online.API.Requests;
using Kumi.Game.Overlays.Settings.Components;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osuTK;

namespace Kumi.Game.Screens.Edit.Popup;

public partial class UploadPopup : EditorPopup
{
    protected override bool CanBeExited => !uploadTransmit.TransmissionInProgress;

    [Resolved]
    private Editor editor { get; set; } = null!;

    [Resolved]
    private EditorChart editorChart { get; set; } = null!;

    [Resolved]
    private ChartUploadTransmit uploadTransmit { get; set; } = null!;

    public UploadPopup()
    {
        Size = new Vector2(400, 200);
    }

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
                                        if (uploadTransmit.TransmissionInProgress)
                                            return;

                                        editor.Save();

                                        tryUpload();
                                    },
                                }
                            }
                        }
                    }
                }
            }
        };
    }

    private void tryUpload()
    {
        uploadTransmit.ModifyRequest += modifyRequest;
        uploadTransmit.TransmitStarted += onTransmissionStarted;
        uploadTransmit.TransmitCompleted += onTransmissionFinished;
        
        uploadTransmit.StartTransmit(editorChart.ChartInfo.ChartSet!);
        uploadTransmit.WaitForTransmit(editorChart.ChartInfo.ChartSet!);
    }

    private void modifyRequest(UploadChartSetRequest req)
    {
        req.IsWip = isWip.Value;
        
        req.UploadProgress += (value, length) =>
        {
            var progress = (float) value / length;
            uploadButton.Progress = progress;
        };
    }

    private void onTransmissionStarted()
    {
        descriptionBox.Current.Disabled = true;
        isWip.Disabled = true;
        uploadButton.State = ButtonState.Loading;
    }

    private void onTransmissionFinished(ChartSetInfo? model)
    {
        descriptionBox.Current.Disabled = false;
        isWip.Disabled = false;
        uploadButton.State = ButtonState.Idle;
    }

    protected override void Dispose(bool isDisposing)
    {
        base.Dispose(isDisposing);
        
        uploadTransmit.ModifyRequest -= modifyRequest;
        uploadTransmit.TransmitStarted -= onTransmissionStarted;
        uploadTransmit.TransmitCompleted -= onTransmissionFinished;
    }
}
