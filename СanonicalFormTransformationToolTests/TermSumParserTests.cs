using System;
using NUnit.Framework;
using СanonicalFormTransformationTool;

namespace СanonicalFormTransformationToolTests
{
    public class TermSumParserTests
    {
        private static readonly TermSumParser Parser = new TermSumParser();

        [TestCase("a + b - c", "a+b-c")]
        [TestCase("2a^2b + 3ab + ab^2", "2a^2b+3ab+ab^2")]
        public void should_return_self(string input, string expected)
        {
            var res = Parser.Parse(input);
            var actual = res.AsString();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("ab + ba", "2ab")]
        public void should_ignore_order(string input, string expected)
        {
            var res = Parser.Parse(input);
            var actual = res.AsString();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("1-1", "0")]
        [TestCase("1+2", "3")]
        [TestCase("ax", "ax")]
        [TestCase("1ax", "ax")]
        [TestCase("2ax", "2ax")]
        [TestCase("2ax+ax", "3ax")]
        [TestCase("2ax + ax", "3ax")]
        [TestCase("2ax + 3ax", "5ax")]
        [TestCase("2ax - ax", "ax")]
        [TestCase("2ax - 3ax", "-ax")]
        [TestCase("2ax - 3ax", "-ax")]
        [TestCase("2a^2x - 3a^2x", "-a^2x")]
        [TestCase("2ba + 2a^2x - 3a^2x + ba ", "3ab-a^2x")]
        public void should_combine_terms(string input, string expected)
        {
            var res = Parser.Parse(input);
            var actual = res.AsString();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("a + (b - c)", "a+b-c")]
        [TestCase("a - (b - c)", "a-b+c")]
        [TestCase("a - (b + c)", "a-b-c")]
        [TestCase("-(a - b + c)", "-a+b-c")]
        [TestCase("-(a - (b + c))", "-a+b+c")]
        [TestCase("-2(a - (-3b + 4c))", "-2a-6b+8c")]
        [TestCase("(a - (b + c))", "a-b-c")]
        [TestCase("(a - (-b + c))", "a+b-c")]
        public void should_remove_brackets(string input, string expected)
        {
            var res = Parser.Parse(input);
            var actual = res.AsString();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("a(a+b)", "a^2+ab")]
        [TestCase("(a-b)(a+b)", "a^2-b^2")]
        [TestCase("(a-b)(a-b)", "a^2-2ab+b^2")]
        [TestCase("(a-2b)(a-3b)", "a^2-5ab+6b^2")]
        [TestCase("(a-b)(a-b)(a-b)", "a^3-3a^2b+3ab^2-b^3")]
        public void should_calculate_product(string input, string expected)
        {
            var res = Parser.Parse(input);
            var actual = res.AsString();
            Assert.AreEqual(expected, actual);
        }

        [TestCase("a b")]
        [TestCase("1 b")]
        [TestCase("a+")]
        [TestCase("++a")]
        [TestCase("--a")]
        public void should_throw_when_invalid_equition(string input)
        {
            Assert.Throws<ArgumentException>(() => Parser.Parse(input));
        }
    }
}