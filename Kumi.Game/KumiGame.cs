using Kumi.Game.Screens;
using Kumi.Game.Screens.Menu;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace Kumi.Game;

public partial class KumiGame : KumiGameBase
{
    private KumiScreenStack screenStack = null!;
    private LoaderScreen loaderScreen = null!;

    [BackgroundDependencyLoader]
    private void load()
    {
        AddRange(new Drawable[]
        {
            screenStack = new KumiScreenStack { RelativeSizeAxes = Axes.Both }
        });
        
        DependencyContainer.CacheAs(screenStack.BackgroundStack);
    }

    protected override void LoadComplete()
    {
        base.LoadComplete();
        
        LoadComponent(loaderScreen = new LoaderScreen());
        screenStack.Push(loaderScreen);
        
        // simulate loading for now
        using (BeginDelayedSequence(1000))
        {
            loaderScreen.SetProgress(0.1f);
            
            using (BeginDelayedSequence(1000))
            {
                loaderScreen.SetProgress(0.3f);
                
                using (BeginDelayedSequence(1000))
                {
                    loaderScreen.SetProgress(0.5f);
                    
                    using (BeginDelayedSequence(1000))
                    {
                        loaderScreen.SetProgress(0.6f);
                    
                        using (BeginDelayedSequence(500))
                        {
                            loaderScreen.SetProgress(0.9f);
                    
                            using (BeginDelayedSequence(1000))
                            {
                                loaderScreen.SetProgress(1f);
                            }
                        }
                    }
                }
            }
        }

        Scheduler.AddDelayed(() =>
        {
            LoadComponentAsync(new MenuScreen(), screenStack.Push);
        }, 6500);
    }
}
