using Kumi.Game.Graphics;
using Kumi.Game.Graphics.UserInterface;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osuTK;

namespace Kumi.Game.Screens.Edit.Popup;

public partial class ExitWithoutSavingPopup : EditorPopup
{
    public Action? Action;

    public ExitWithoutSavingPopup()
    {
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
        Size = new Vector2(400, 200);
    }

    [BackgroundDependencyLoader]
    private void load()
    {
        TextFlowContainer textFlowContainer;

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
                    Colour = Colours.Gray(0.05f),
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Padding = new MarginPadding(8),
                    Children = new Drawable[]
                    {
                        textFlowContainer = new TextFlowContainer(t =>
                        {
                            t.Font = KumiFonts.GetFont(size: 12);
                            t.Colour = Colours.GRAY_6;
                        })
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            TextAnchor = Anchor.Centre
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
                                new KumiButton
                                {
                                    RelativeSizeAxes = Axes.None,
                                    Width = 100,
                                    Text = "Exit",
                                    Important = true,
                                    Anchor = Anchor.BottomCentre,
                                    Origin = Anchor.BottomCentre,
                                    Action = () =>
                                    {
                                        Action?.Invoke();
                                        Hide();
                                    },
                                }
                            }
                        }
                    }
                }
            }
        };

        textFlowContainer.AddParagraph("Are you sure you want to exit without saving?", c =>
        {
            c.Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold, size: 16);
            c.Colour = Colours.GRAY_C;
        });
        textFlowContainer.AddParagraph("");
        textFlowContainer.AddParagraph("All unsaved changes will be lost.");
    }
}
