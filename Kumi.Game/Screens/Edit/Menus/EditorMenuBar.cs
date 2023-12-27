using Kumi.Game.Graphics;
using Kumi.Game.Graphics.Containers;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.UserInterface;

namespace Kumi.Game.Screens.Edit.Menus;

public partial class EditorMenuBar : osu.Framework.Graphics.UserInterface.Menu
{
    public EditorMenuBar(Direction direction)
        : base(direction, true)
    {
        MaskingContainer.CornerRadius = 5;
        ItemsContainer.Padding = new MarginPadding();

        BackgroundColour = Colours.Gray(0.05f);
    }

    protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() => new EditorBarSubMenu();

    protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableEditorBarMenuItem(item);

    protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new KumiScrollContainer(direction);

    private partial class EditorBarSubMenu : osu.Framework.Graphics.UserInterface.Menu
    {
        public EditorBarSubMenu()
            : base(Direction.Vertical)
        {
            ItemsContainer.Padding = new MarginPadding();

            MaskingContainer.CornerRadius = 3;
        }

        protected override osu.Framework.Graphics.UserInterface.Menu CreateSubMenu() => new EditorBarSubMenu();

        protected override DrawableMenuItem CreateDrawableMenuItem(MenuItem item) => new DrawableEditorBarMenuItem(item);

        protected override ScrollContainer<Drawable> CreateScrollContainer(Direction direction) => new KumiScrollContainer();

        private partial class EditorMenuItem : DrawableEditorBarMenuItem
        {
            public EditorMenuItem(MenuItem item)
                : base(item)
            {
            }

            protected override void LoadComplete()
            {
                base.LoadComplete();
                Foreground.Padding = new MarginPadding { Vertical = 2 };
                Foreground.RelativeSizeAxes = Axes.None;
                Foreground.AutoSizeAxes = Axes.Both;
            }
        }
    }
}
