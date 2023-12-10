using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Containers;
using Kumi.Game.Graphics.UserInterface;
using Kumi.Game.Online.API;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;
using osu.Framework.Threading;
using osuTK;

namespace Kumi.Game.Overlays;

public partial class AccountRegistrationOverlay : KumiFocusedOverlayContainer
{
    private FillFlowContainer content = null!;
    private TextFlowContainer kumiText = null!;
    private TextFlowContainer legalitiesText = null!;

    private LabeledTextBox usernameTextBox = null!;
    private LabeledTextBox emailTextBox = null!;
    private LabeledTextBox passwordTextBox = null!;

    private LabeledTextBox[] textBoxes = null!;

    [Resolved]
    private IAPIConnectionProvider api { get; set; } = null!;

    public AccountRegistrationOverlay()
    {
        Width = 600;
        AutoSizeAxes = Axes.Y;
        Anchor = Anchor.Centre;
        Origin = Anchor.Centre;
    }

    private readonly IBindable<APIState> apiState = new Bindable<APIState>();

    [BackgroundDependencyLoader]
    private void load(LargeTextureStore textures)
    {
        apiState.BindTo(api.State);
        apiState.BindValueChanged(apiStateChanged, true);

        Masking = true;
        CornerRadius = 5;

        Children = new Drawable[]
        {
            new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    new BufferedContainer(cachedFrameBuffer: true)
                    {
                        RelativeSizeAxes = Axes.Both,
                        Child = new Sprite
                        {
                            RelativeSizeAxes = Axes.Both,
                            Anchor = Anchor.Centre,
                            Origin = Anchor.Centre,
                            FillMode = FillMode.Fill,
                            Texture = textures.Get("https://pbs.twimg.com/media/GA9eCZLbEAAY3u_?format=jpg&name=orig")
                        }
                    },
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = ColourInfo.GradientHorizontal(Colours.Gray(0.05f).Opacity(1f), Colours.Gray(0.05f).Opacity(0.9f))
                    }
                }
            },
            new Container
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Padding = new MarginPadding
                {
                    Horizontal = 8 + 2 + 4,
                    Vertical = 8 + 4
                },
                AutoSizeEasing = Easing.OutQuint,
                AutoSizeDuration = 200,
                Child = content = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Direction = FillDirection.Vertical,
                    Spacing = new Vector2(0, 16),
                    Children = new Drawable[]
                    {
                        new SpriteText
                        {
                            Text = "REGISTER",
                            Font = KumiFonts.GetFont(FontFamily.Montserrat, FontWeight.SemiBold)
                        },
                        new GridContainer
                        {
                            RelativeSizeAxes = Axes.X,
                            AutoSizeAxes = Axes.Y,
                            ColumnDimensions = new[]
                            {
                                new Dimension(),
                                new Dimension()
                            },
                            RowDimensions = new[]
                            {
                                new Dimension(GridSizeMode.AutoSize)
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new FillFlowContainer
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Direction = FillDirection.Vertical,
                                        Spacing = new Vector2(0, 8),
                                        Children = new Drawable[]
                                        {
                                            usernameTextBox = new LabeledTextBox
                                            {
                                                PlaceholderText = "Username",
                                                Label = "The name everyone will see you as. Please make sure the name is appropriate.",
                                                TabbableContentContainer = this
                                            },
                                            emailTextBox = new LabeledTextBox
                                            {
                                                PlaceholderText = "Email",
                                                Label = "Used for notifications, account verification, and in case you forget your password.",
                                                TabbableContentContainer = this
                                            },
                                            passwordTextBox = new LabeledTextBox(true)
                                            {
                                                PlaceholderText = "Password",
                                                Label = "A secure and long passphrase that only you will remember. Must be at least 8 characters long.",
                                                TabbableContentContainer = this
                                            },
                                            new KumiButton
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                Height = 30,
                                                BackgroundColour = Colours.ORANGE_ACCENT,
                                                Text = "Register",
                                                Important = true,
                                                Action = performRegistration
                                            }
                                        }
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding { Left = 8 },
                                        Children = new Drawable[]
                                        {
                                            kumiText = new TextFlowContainer(c => c.Font = KumiFonts.GetFont(size: 12))
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                            },
                                            legalitiesText = new TextFlowContainer(c => c.Font = KumiFonts.GetFont(size: 12))
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                Anchor = Anchor.BottomLeft,
                                                Origin = Anchor.BottomLeft,
                                                // https://github.com/ppy/osu-framework/issues/5084
                                                // TextAnchor = Anchor.BottomLeft,
                                            },
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
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(4),
                Children = new Drawable[]
                {
                    new Circle
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 2,
                        Anchor = Anchor.TopCentre,
                        Origin = Anchor.TopCentre,
                        Colour = Colours.ORANGE_ACCENT
                    },
                    new Circle
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 2,
                        Anchor = Anchor.BottomCentre,
                        Origin = Anchor.BottomCentre,
                        Colour = Colours.ORANGE_ACCENT
                    }
                }
            }
        };

        textBoxes = new[] { usernameTextBox, emailTextBox, passwordTextBox };

        kumiText.AddText("Welcome to ");
        kumiText.AddText("Kumi!", c => c.Font = KumiFonts.GetFont(weight: FontWeight.SemiBold, size: 12));
        kumiText.AddText("A faithful recreation to Taiko no Tatsujin.");

        legalitiesText.AddParagraph("Kumi is not affiliated with Bandai Namco Entertainment Inc. or any of its subsidiaries. All rights reserved to their respective owners.");
        legalitiesText.AddParagraph("");
        legalitiesText.AddText("By creating an account on Kumi, you agree to our ");
        legalitiesText.AddText("Terms of Service", c => c.Colour = Colours.CYAN_ACCENT_LIGHT);
        legalitiesText.AddText(" and ");
        legalitiesText.AddText("Privacy Policy", c => c.Colour = Colours.CYAN_ACCENT_LIGHT);
        legalitiesText.AddText(".");
    }

    protected override void PopIn()
    {
        content.FadeIn();
        this.FadeIn(200, Easing.OutQuint);

        scheduledHide?.Cancel();
        scheduledHide = null;
    }

    protected override void PopOut()
    {
        content.FadeOut();
        this.FadeOut(200);
    }

    private void performRegistration()
    {
        if (focusNextEmptyTextBox())
            return;

        Task.Run(() =>
        {
            bool success;

            try
            {
                success = api.Register(usernameTextBox.Current.Value, emailTextBox.Current.Value, passwordTextBox.Current.Value);
            }
            catch (Exception)
            {
                success = false;
            }

            Schedule(() =>
            {
                if (success)
                    api.Login(usernameTextBox.Current.Value, passwordTextBox.Current.Value);
            });
        });
    }

    private bool focusNextEmptyTextBox()
    {
        var next = nextEmptyTextBox();

        if (next != null)
        {
            Schedule(() => GetContainingInputManager().ChangeFocus(next.TextBox));
            return true;
        }

        return false;
    }

    private LabeledTextBox? nextEmptyTextBox()
        => textBoxes.FirstOrDefault(t => string.IsNullOrEmpty(t.Current.Value));

    private ScheduledDelegate? scheduledHide;

    private void apiStateChanged(ValueChangedEvent<APIState> state)
    {
        switch (state.NewValue)
        {
            case APIState.Failed:
            case APIState.Offline:
            case APIState.Connecting:
                break;

            case APIState.Online:
                scheduledHide?.Cancel();
                scheduledHide = Schedule(PopOut);
                break;
        }
    }

    private partial class LabeledTextBox : FillFlowContainer, IHasCurrentValue<string>
    {
        public Bindable<string> Current { get; set; } = new Bindable<string>();

        public LocalisableString Label
        {
            set => textFlow.Text = value;
        }

        public LocalisableString PlaceholderText
        {
            set => textBox.PlaceholderText = value;
        }

        public Container TabbableContentContainer
        {
            set => textBox.TabbableContentContainer = value;
        }

        private readonly KumiTextBox textBox;
        private readonly TextFlowContainer textFlow;

        public KumiTextBox TextBox => textBox;

        public LabeledTextBox(bool secret = false)
        {
            AutoSizeAxes = Axes.Y;
            RelativeSizeAxes = Axes.X;
            Direction = FillDirection.Vertical;
            Spacing = new Vector2(0, 4);

            Children = new Drawable[]
            {
                textBox = createTextBox(secret),
                textFlow = new TextFlowContainer(c => c.Font = KumiFonts.GetFont(size: 12))
                {
                    RelativeSizeAxes = Axes.X,
                    AutoSizeAxes = Axes.Y,
                    Colour = Colours.GRAY_C
                }
            };
        }

        private KumiTextBox createTextBox(bool secret)
        {
            var input = secret
                              ? new KumiPasswordTextBox
                              {
                                  RelativeSizeAxes = Axes.X,
                                  Height = 24,
                              }
                              : new KumiTextBox
                              {
                                  RelativeSizeAxes = Axes.X,
                                  Height = 24,
                              };
            
            Current.BindTo(input.Current);

            return input;
        }
    }
}
