using Kumi.Game.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Localisation;

namespace Kumi.Game.Graphics.UserInterface;

public partial class KumiDropdown<T> : Dropdown<T>
{
    protected override DropdownHeader CreateHeader()
        => new KumiDropdownHeader();

    protected override DropdownMenu CreateMenu()
        => new KumiDropdownMenu();

    public partial class KumiDropdownHeader : DropdownHeader
    {
        private readonly SpriteText label;

        protected override LocalisableString Label
        {
            get => label.Text;
            set => label.Text = value;
        }

        public KumiDropdownHeader()
        {
            Background.Masking = true;
            Background.CornerRadius = 5;

            Foreground.Padding = new MarginPadding(4);

            BackgroundColour = Colours.Gray(0.1f);
            BackgroundColourHover = Colours.Gray(0.15f);

            Children = new Drawable[]
            {
                label = new SpriteText
                {
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Font = KumiFonts.GetFont()
                }
            };
        }

        protected override DropdownSearchBar CreateSearchBar() => new KumiDropdownSearchBar();

        public partial class KumiDropdownSearchBar : DropdownSearchBar
        {
            protected override void PopIn()
            {
                this.FadeIn(200, Easing.OutQuint);
            }

            protected override void PopOut()
            {
                this.FadeOut(200, Easing.OutQuint);
            }

            protected override TextBox CreateTextBox()
                => new KumiTextBox
                {
                    PlaceholderText = "Type to search",
                    FontSize = 16
                };
        }
    }

    public partial class KumiDropdownMenu : DropdownMenu
    {
        protected override Menu CreateSubMenu() => new BasicMenu(Direction.Vertical);

        protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction)
            => new KumiScrollContainer(direction);

        protected override DrawableDropdownMenuItem CreateDrawableDropdownMenuItem(MenuItem item)
            => new DrawableKumiDropdownMenuItem(item);

        public KumiDropdownMenu()
        {
            Margin = new MarginPadding { Top = 4 };
        }

        private partial class DrawableKumiDropdownMenuItem : DrawableDropdownMenuItem
        {
            public DrawableKumiDropdownMenuItem(MenuItem item)
                : base(item)
            {
                Foreground.Padding = new MarginPadding(2);

                BackgroundColour = Colours.Gray(0.1f);
                BackgroundColourHover = Colours.Gray(0.15f);
                BackgroundColourSelected = Colours.Gray(0.15f);
            }

            protected override Drawable CreateContent()
                => new SpriteText
                {
                    Font = KumiFonts.GetFont(size: 12),
                    Colour = Colours.GRAY_C
                };
        }
    }
}
