using Kumi.Game.Online.API.Accounts;
using Kumi.Game.Online.API.Charts;
using Kumi.Game.Overlays.Listing.Cards;
using Kumi.Game.Tests;

namespace Kumi.Tests.Visual.UserInterface;

public partial class ChartSetCardTestScene : KumiTestScene
{
    public ChartSetCardTestScene()
    {
        Add(new ChartSetCard(new APIChartSet
        {
            Title = "Title",
            Artist = "Artist",
            Status = APIChartSetStatus.Ranked,
            Creator = new GuestAccount(),
            Romanised = new APIRomanisedMetadata
            {
                TitleRomanised = "Title Romanised",
                ArtistRomanised = "Artist Romanised"
            }
        }));
    }
}
