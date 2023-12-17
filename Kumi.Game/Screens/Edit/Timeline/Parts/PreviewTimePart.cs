using Kumi.Game.Graphics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace Kumi.Game.Screens.Edit.Timeline.Parts;

public partial class PreviewTimePart : TimelinePart
{
    private readonly BindableInt previewTime = new BindableInt();

    protected override void LoadChart(EditorChart chart)
    {
        base.LoadChart(chart);
        
        previewTime.UnbindAll();
        previewTime.BindTo(chart.PreviewTime);
        previewTime.BindValueChanged(t =>
        {
            Clear();

            if (t.NewValue >= 0)
                Add(new DrawablePreviewTime(t.NewValue));
        }, true);
        
        Track.BindValueChanged(t =>
        {
            if (t.NewValue == null)
                return;
            
            if (previewTime.Value >= 0)
                return;
            
            Clear();
            Add(new DrawablePreviewTime(t.NewValue.Length * 0.4f));
        }, true);
    }
    
    private partial class DrawablePreviewTime : DrawableTick
    {
        public DrawablePreviewTime(double time)
            : base(time)
        {
            Colour = Colours.SEAFOAM_ACCENT_LIGHT;
            Origin = Anchor.TopCentre;
            Height = 0.25f;
        }
    }
}
