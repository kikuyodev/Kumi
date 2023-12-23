using DiscordRPC;
using Kumi.Game;
using Kumi.Game.Online;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;

namespace Kumi.Desktop;

public partial class Presence : Component
{
    [Resolved]
    private KumiGameBase game { get; set; } = null!;

    private DiscordRpcClient client;

    private RichPresence presence = new RichPresence()
    {
        Assets = new Assets()
    };

    [BackgroundDependencyLoader]
    private void load()
    {
        client = new DiscordRpcClient("1188229230136414248");
        client.Initialize();

        game.Activity.BindValueChanged(e => onActivityChanged(e), true);
    }
    
    private void onActivityChanged(ValueChangedEvent<PlayerActivity> args)
    {
        if (!client.IsInitialized || args.NewValue == null)
            return;
        
        presence.Assets.LargeImageKey = args.NewValue.GetLargeImage();
        presence.Details = args.NewValue.GetDetails();
        presence.State = args.NewValue.GetState();
        
        client.SetPresence(presence);
    }
}
