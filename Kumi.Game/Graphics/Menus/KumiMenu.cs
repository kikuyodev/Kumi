using Kumi.Game.Screens.Edit.Menus;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace Kumi.Game.Graphics.Menus;

public partial class KumiMenu : Menu
{
    public KumiMenu(Direction direction)
        : base(direction, true)
    {
        MaskingContainer.CornerRadius = 5;
        ItemsContainer.Padding = new MarginPadding();

        BackgroundColour = Colours.Gray(0.05f);
    }

    protected override Menu CreateSubMenu() => new KumiSubMenu();

    protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableEditorBarMenuItem(item);

    protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new BasicScrollContainer(direction);

    private partial class KumiSubMenu : Menu
    {
        public KumiSubMenu()
            : base(Direction.Vertical)
        {
            ItemsContainer.Padding = new MarginPadding();

            MaskingContainer.CornerRadius = 3;
        }

        protected override Menu CreateSubMenu() => new KumiSubMenu();

        protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableEditorBarMenuItem(item);

        protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new BasicScrollContainer();
    }
}
