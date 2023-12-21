using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;

namespace Kumi.Game.Graphics.Containers;

public partial class AlwaysLastFillFlowContainer : FillFlowContainer
{
    private Func<Drawable>? createComponent;

    public Func<Drawable>? CreateComponent
    {
        get => createComponent;
        set
        {
            component?.Expire();
            component = value?.Invoke();

            createComponent = value;
        }
    }

    private Drawable? component;

    public AlwaysLastFillFlowContainer(Func<Drawable>? createComponent = null)
    {
        CreateComponent = createComponent;
    }

    private bool isBatch;

    public new void AddRange(IEnumerable<Drawable> drawables)
    {
        isBatch = true;
        
        base.AddRange(drawables);
        
        component?.Expire();
        component = CreateComponent?.Invoke();

        if (component != null)
            base.Add(component);
    }

    public override void Add(Drawable drawable)
    {
        if (!isBatch)
        {
            component?.Expire();
            component = CreateComponent?.Invoke();
        }

        base.Add(drawable);

        if (component != null && !isBatch)
            base.Add(component);
    }
}
