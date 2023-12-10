namespace Kumi.Game.Online;

public class ProductionServerConfiguration : ServerConfiguration
{
    private const string base_uri = "https://kumi.kikuyo.dev";
    
    public ProductionServerConfiguration()
    {
        APIUri = base_uri + "/api/v1"; 
        //WebsocketUri = base_uri + "/api/v1";
        WebsiteUri = base_uri;
    }
}
