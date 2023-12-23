using Newtonsoft.Json;

namespace Kumi.Game.Online.API.Chat;

public class APIChatChannel
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonProperty("tag")]
    public string Tag { get; set; } = string.Empty;
    
    [JsonProperty("description")]
    public string Description { get; set; } = string.Empty;
    
    [JsonProperty("type")]
    public int TypeInt { get; set; }
    
    [JsonIgnore]
    public ChatChannelType Type
    {
        get => (ChatChannelType) TypeInt;
        set => TypeInt = (int) value;
    }
}

public enum ChatChannelType
{
    Public,
    Private,
    System,
    PrivateMessage
}