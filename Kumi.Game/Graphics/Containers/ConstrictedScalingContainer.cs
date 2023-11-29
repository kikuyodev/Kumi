using osu.Framework.Extensions.EnumExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Layout;
using osuTK;

namespace Kumi.Game.Graphics.Containers;

/// <summary>
/// Keeps a constant size relative to the parent size.
/// This is useful for keeping a constant size relative to the parent size, but also allowing the parent to scale.
///
/// Currently, this container only supports scaling with an aspect ratio of 1:1.
/// </summary>
public partial class ConstrictedScalingContainer : Container
{
    private Vector2 preferredSize = Vector2.Zero;

    public Vector2 PreferredSize
    {
        get => preferredSize;
        set
        {
            if (preferredSize == value)
                return;
            
            preferredSize = value;
            parentSize.Invalidate();
        }
    }
    
    private readonly Container content;
    private readonly LayoutValue parentSize;

    protected override Container<Drawable> Content => content;

    public ConstrictedScalingContainer()
    {
        AddLayout(parentSize = new LayoutValue(Invalidation.RequiredParentSizeToFit | Invalidation.DrawInfo));
        
        InternalChild = content = new Container
        {
            Size = PreferredSize,
            Anchor = Anchor.Centre,
            Origin = Anchor.Centre,
        };
    }

    protected override void Update()
    {
        base.Update();
        
        if (parentSize.IsValid)
            return;
        
        content.Size = PreferredSize;
        
        // scale the content relative to the parent size
        var parent = Parent;
        if (parent != null)
        {
            var parentSize = parent.DrawSize;
            var contentSize = content.DrawSize;

            if (RelativeSizeAxes.HasFlagFast(Axes.X))
                parentSize.X *= Size.X;
            if (RelativeSizeAxes.HasFlagFast(Axes.Y))
                parentSize.Y *= Size.Y;
            
            var scale = Math.Min(parentSize.X / contentSize.X, parentSize.Y / contentSize.Y);
            content.Scale = new Vector2(scale);
        }
        
        parentSize.Validate();
    }
}
