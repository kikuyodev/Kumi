using Kumi.Game.Utils;
using NUnit.Framework;

namespace Kumi.Tests.Utils;

[TestFixture]
public class StringUtilsTest
{
    [Test]
    public void SplitStringWithDelimiterInAString()
    {
        const string input = "1,\"a, string\",enumValue,200,5000";
        var result = input.SplitComplex(',').ToArray();
        
        Assert.AreEqual(5, result.Length);
        Assert.AreEqual("1", result[0]);
        Assert.AreEqual("a, string", result[1]);
        Assert.AreEqual("enumValue", result[2]);
        Assert.AreEqual("200", result[3]);
        Assert.AreEqual("5000", result[4]);
    }

    [Test]
    public void AssertValues()
    {
        Assert.IsTrue(StringUtils.AssertProperty<bool>("true"));
        Assert.IsTrue(StringUtils.AssertProperty<bool>("false"));
        Assert.IsFalse(StringUtils.AssertProperty<int>("false"));
        Assert.IsTrue(StringUtils.AssertProperty<int>("1"));
        Assert.IsFalse(StringUtils.AssertProperty<int>("1.1"));
        Assert.IsTrue(StringUtils.AssertProperty<float>("1.1"));
    }

    [Test]
    public void AssertValuesAndFetch()
    {
        Assert.AreEqual(true, StringUtils.AssertAndFetch<bool>("true"));
        Assert.AreEqual(false, StringUtils.AssertAndFetch<bool>("false"));
        
        Assert.AreEqual(1, StringUtils.AssertAndFetch<int>("1"));
        Assert.Throws<InvalidDataException>(() => StringUtils.AssertAndFetch<int>("1.1"));
    }
}
