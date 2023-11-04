using Kumi.Game.Charts.Timings;
using NUnit.Framework;

namespace Kumi.Tests.Charts;

[TestFixture]
public class TimingPointTest
{
    private TimingPointHandler _handler = new TimingPointHandler();
    
    [Test]
    public void TestTimingPoints()
    {
        addTimingPoints();
        
        Assert.AreEqual(180.0f, _handler.GetTimingPointAt<UninheritedTimingPoint>(0, TimingPointType.Uninherited).BPM);
        Assert.AreEqual(1.0f, _handler.GetTimingPointAt<UninheritedTimingPoint>(0, TimingPointType.Uninherited).RelativeScrollSpeed);
        Assert.AreEqual(2.0f, _handler.GetTimingPointAt(1000).RelativeScrollSpeed);
        Assert.AreEqual(180.0f, _handler.GetTimingPointAt<UninheritedTimingPoint>(200, TimingPointType.Uninherited).BPM);
        Assert.AreEqual(240.0f, _handler.GetTimingPointAt<UninheritedTimingPoint>(250, TimingPointType.Uninherited).BPM);
        
        Assert.AreEqual(180.0f, _handler.GetBPMAt(0));
        Assert.AreEqual(333.333333333f, _handler.GetBeatLengthAt(0));
        Assert.AreEqual(1.0f, _handler.GetScrollSpeedAt(0));
        Assert.AreEqual(187.5f, _handler.GetBeatLengthAt(250));
        Assert.AreEqual(1.0f, _handler.GetScrollSpeedAt(500));
        Assert.AreEqual(240.0f, _handler.GetBPMAt(500));
        Assert.AreEqual(250.0f, _handler.GetBeatLengthAt(500));
        Assert.AreEqual(1.0f, _handler.GetScrollSpeedAt(500));
        Assert.AreEqual(2.0f, _handler.GetScrollSpeedAt(1000));
    }

    private void addTimingPoints()
    {
        _handler.Clear(); // just in case
        
        _handler.TimingPoints.Add(new UninheritedTimingPoint(0)
        {
            BPM = 180.0f,
            RelativeScrollSpeed = 1.0f
        });
        _handler.TimingPoints.Add(new UninheritedTimingPoint(250)
        {
            BPM = 240.0f,
            TimeSignature = TimeSignature.WALTZ_TIME,
            RelativeScrollSpeed = 1.0f
        });
        _handler.TimingPoints.Add(new UninheritedTimingPoint(500)
        {
            BPM = 240.0f,
            RelativeScrollSpeed = 1.0f
        });
        _handler.TimingPoints.Add(new InheritedTimingPoint(1000)
        {
            RelativeScrollSpeed = 2.0f
        });
    }
    
}
