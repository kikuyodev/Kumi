namespace Kumi.Game.Online;

public class ProductionServerConfiguration : ServerConfiguration
{
    private const string base_uri = "kumi.kikuyo.dev";
    
    public ProductionServerConfiguration()
    {
        APIUri = "https://" + base_uri + "/api/v1"; 
        WebsocketUri = "wss://" + base_uri + "/gateway";
        WebsiteUri = "https://" + base_uri;
    }
}
