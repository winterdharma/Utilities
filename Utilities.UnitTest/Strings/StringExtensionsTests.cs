using NUnit.Framework;
using System;
using Utilities.Strings;

namespace Utilities.Test
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [Test]
        public void StringExtensionScrubRemovesSubstringsCorrectly()
        {
            string s = "[0160c17] [1]大智度論釋初品中讚尸[17]羅波羅蜜義[]第二十[19]三";
            string expected = "[0160c17] 大智度論釋初品中讚尸羅波羅蜜義第二十三";
            string actual = s.Scrub('[', ']',4);
            Console.WriteLine(expected);
            Console.WriteLine(actual);
            Assert.AreEqual(expected, actual);
        }
    }
}
