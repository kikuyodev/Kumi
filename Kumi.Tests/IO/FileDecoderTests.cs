using System.Text.RegularExpressions;
using Kumi.Game.IO.Formats;
using Kumi.Tests.Resources;
using NUnit.Framework;

namespace Kumi.Tests.IO;

[TestFixture]
public class FileDecoderTests
{
    private Stream testResource => TestResources.OpenResource("Formats/test_format.tst");
    private TestDecoder decoder => new TestDecoder();
    
    [Test]
    public void TestDecoding()
    {
        var input = decoder.Decode(testResource);

        Assert.IsTrue(input.IsProcessed);
        Assert.IsTrue(input.Sections.ContainsKey(TestSection.SectionOne));

        for (var i = 0; i < input.Sections[TestSection.SectionOne].Count; i++)
        {
            var pair = input.Sections[TestSection.SectionOne][i];
            Assert.AreEqual($"Key{i + 1}", pair.Key);
            Assert.AreEqual($"Value{i + 1}", pair.Value);
        }
    }

    private class TestDecoder : FileDecoder<TestInput, TestSection>
    {
        public TestDecoder()
            : base(1)
        {
            
        }

        protected override void ProcessLine(string line)
        {
            if (!Current.Sections.ContainsKey(CurrentSection))
            {
                Current.Sections.Add(CurrentSection, new List<KeyValuePair<string, string>>());
            }
            
            // split pair by ':'
            var pair = new Regex(@":\s?").Split(line);
            
            // add pair to current section
            Current.Sections[CurrentSection].Add(new KeyValuePair<string, string>(pair[0], pair[1]));
        }
    }
    
    private class TestInput : IDecodable
    {
        public Dictionary<TestSection, List<KeyValuePair<string, string>>> Sections { get; set; } = new();
        
        public int Version { get; set; }
        public bool IsProcessed { get; set; }
    }

    private enum TestSection
    {
        Unknown,
        SectionOne
    }
}
