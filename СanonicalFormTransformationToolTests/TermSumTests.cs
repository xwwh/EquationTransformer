using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using СanonicalFormTransformationTool;

namespace СanonicalFormTransformationToolTests
{
    public class TermSumTests
    {
        private static readonly TermSumParser Parser = new TermSumParser();

        [TestCase("a-b", "a-b", "a^2-2ab+b^2")]
        [TestCase("a+b", "a-b", "a^2-b^2")]
        [TestCase("a^2+b", "a^-2-b", "1-a^2b+a^-2b-b^2")]
        public void should_multiply_sums(string value1, string value2, string expected)
        {
            var sum1 = Parser.Parse(value1);
            var sum2 = Parser.Parse(value2);
            var actual = TermSum.GetProduct(sum1, sum2);

            Assert.AreEqual(expected, actual.AsString());
        }

        [TestCase("a-b", 1, "a-b")]
        [TestCase("a+b^2", 2, "2a+2b^2")]
        [TestCase("a^2+b", -1, "-a^2-b")]
        public void should_multiply_sum_and_number(string value1, double value2, string expected)
        {
            var sum1 = Parser.Parse(value1);
            var actual = TermSum.GetProduct(sum1, value2);

            Assert.AreEqual(expected, actual.AsString());
        }

        [TestCase("a-b", "-a+b", "0")]
        [TestCase("a-b", "a-b", "2a-2b")]
        [TestCase("a-b+c^2", "a-b", "2a-2b+c^2")]
        [TestCase("a-b", "a-b+c^2", "2a-2b+c^2")]
        public void should_add_sums(string value1, string value2, string expected)
        {
            var sum1 = Parser.Parse(value1);
            var sum2 = Parser.Parse(value2);
            
            sum1.Add(sum2);

            Assert.AreEqual(expected, sum1.AsString());
        }


        [TestCase("a-b", "-a+b", "2a-2b")]
        [TestCase("a-b", "a-b", "0")]
        [TestCase("a-b+c^2", "a-b", "c^2")]
        [TestCase("a-b", "a-b+c^2", "-c^2")]
        public void should_subtract_sums(string value1, string value2, string expected)
        {
            var sum1 = Parser.Parse(value1);
            var sum2 = Parser.Parse(value2);

            sum1.Subtract(sum2);

            Assert.AreEqual(expected, sum1.AsString());
        }
    }
}
