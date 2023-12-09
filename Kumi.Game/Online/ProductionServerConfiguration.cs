namespace Kumi.Game.Online;

public class ProductionServerConfiguration : ServerConfiguration
{
    public ProductionServerConfiguration()
    {
        APIUri = "https://kumi.kikuyo.dev/api/v1";
        //WebsocketUri = "https://kumi.kikuyo.dev/api/v1";
        WebsiteUri = "https://kumi.kikuyo.dev";
    }
}
