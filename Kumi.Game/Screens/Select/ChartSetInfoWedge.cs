using Kumi.Game.Charts;
using Kumi.Game.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;

namespace Kumi.Game.Screens.Select;

public partial class ChartSetInfoWedge : Container
{
    private Container content = null!;
    private ChartSetInfoWedgeContent? displayableContent;

    public ChartSetInfoWedge()
    {
        X = 5;
        Masking = true;
        CornerRadius = 5;
        Margin = new MarginPadding { Right = -5 };
    }

    [BackgroundDependencyLoader]
    private void load(IBindable<WorkingChart> chart)
    {
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Colours.Gray(0.05f)
            },
            content = new Container
            {
                RelativeSizeAxes = Axes.Both
            }
        };
        
        chart.BindValueChanged(c => updateDisplay(c.NewValue), true);
    }

    private ChartSetInfoWedgeContent loadingInfo = null!;

    private void updateDisplay(WorkingChart? newChart)
    {
        Scheduler.AddOnce(perform);
        
        void perform()
        {
            void removeOldInfo()
            {
                displayableContent?.FadeOut(250);
                displayableContent?.Expire();
                displayableContent = null;
            }

            if (newChart == null)
            {
                removeOldInfo();
                return;
            }

            LoadComponentAsync(loadingInfo = new ChartSetInfoWedgeContent(newChart)
            {
                RelativeSizeAxes = Axes.Both,
                Depth = displayableContent?.Depth + 1 ?? 0
            }, loaded =>
            {
                if (loaded != loadingInfo) return;
                
                removeOldInfo();
                content.Add(displayableContent = loaded);
            });
        }
    }
}
