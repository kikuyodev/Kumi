using Kumi.Game.IO.Formats;
using Kumi.Tests.Resources;
using NUnit.Framework;

namespace Kumi.Tests.IO;

[TestFixture]
public class FileEncoderTests
{
    private Stream? testResource => TestResources.OpenResource("Formats/encoded_format_assert.tst");
    private TestEncoder? encoder;
    
    [SetUp]
    public void Setup()
    {
        encoder?.Dispose();
        encoder = new TestEncoder();
    }

    [Test]
    public void TestEncoding()
    {
        var fileName = TestResources.GetTemporaryFilename("tst");
        using var writable = TestResources.OpenWritableTemporaryFile(fileName);
        encoder!.Encode(testInput, writable);
        
        string contents = new StreamReader(TestResources.OpenReadableTemporaryFile(fileName)).ReadToEnd();
        string expected = new StreamReader(testResource!).ReadToEnd();
        
        Assert.AreEqual(expected, contents);
    }

    [Test]
    public void TestEmptyEncoding()
    {
        var fileName = TestResources.GetTemporaryFilename("tst");
        using var writable = TestResources.OpenWritableTemporaryFile(fileName);
        encoder!.Encode(new TestInput(), writable);
        
        string contents = new StreamReader(TestResources.OpenReadableTemporaryFile(fileName)).ReadToEnd().Trim();
        string expected = """
#KUMI v1
[#SECTION_ONE]
[#SECTION_TWO]
""";
        
        Assert.AreEqual(expected, contents);
    }

    private readonly TestInput testInput = new TestInput
    {
        Sections =
        {
            {
                TestSection.SectionOne,
                new Dictionary<string, string>
                {
                    { "Key1", "Value1" },
                    { "Key2", "Value2" }
                }
            },
            {
                TestSection.SectionTwo,
                new Dictionary<string, string>
                {
                    { "Key1", "Value1" },
                    { "Key2", "Value2" }
                }
            },
        }
    };
    
    private class TestEncoder : FileEncoder<TestInput, TestSection>
    {
        protected override IFileHandler<TestInput, TestSection>.SectionHeaderValues SectionHeader => new IFileHandler<TestInput, TestSection>.SectionHeaderValues
        {
            Start = "[#",
            End = "]"
        };

        public TestEncoder()
            : base(1)
        {
        }

        protected override void HandleSection(TestSection section)
        {
            if (!Current.Sections.ContainsKey(section))
                return;

            foreach (var pair in Current.Sections[section])
                WriteLine($"{pair.Key}: {pair.Value}");
        }
    }
    
    private class TestInput
    {
        public Dictionary<TestSection, Dictionary<string, string>> Sections { get; set; } = new Dictionary<TestSection, Dictionary<string, string>>();

        public bool Processed { get; set; }
    }
    
    private enum TestSection
    {
        SectionOne,
        SectionTwo
    }
}
