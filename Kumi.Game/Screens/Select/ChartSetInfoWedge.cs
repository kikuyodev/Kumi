using Kumi.Game.Charts;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
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
        X = -5;
        Masking = true;
        CornerRadius = 5;
        Margin = new MarginPadding { Left = -5 };
    }

    [BackgroundDependencyLoader]
    private void load(IBindable<WorkingChart> chart)
    {
        Children = new Drawable[]
        {
            new Box
            {
                RelativeSizeAxes = Axes.Both,
                Colour = Color4Extensions.FromHex("0D0D0D")
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
