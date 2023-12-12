using System.Configuration;

namespace Kumi.Game.Online;

public class DevelopmentServerConfiguration : ServerConfiguration
{
    private static readonly string base_uri = ConfigurationManager.AppSettings["development:baseUri"] ?? "localhost";
    private static readonly int port = int.Parse(ConfigurationManager.AppSettings["development:port"] ?? "8080");
    private static readonly int websocket_port = int.Parse(ConfigurationManager.AppSettings["development:websocketPort"] ?? "8080");
    
    public DevelopmentServerConfiguration()
    {
        APIUri = "http://" + base_uri + $":{port}/api/v1"; 
        WebsocketUri = "ws://" + base_uri + $":{websocket_port}/gateway";
        WebsiteUri = "http://" + base_uri;
    }
}
