using System;

namespace СanonicalFormTransformationTool
{
    public static class TermSumParserExtensions
    {
        /// <summary>
        ///     Transforms equation into canonical form. An equation can be of any order. It may contain any amount of variables
        ///     and brackets.
        ///     The equation will be given in the following form:
        ///     P1 + P2 + ... = ... + PN
        ///     where P1..PN - summands, which look like: ax^k
        ///     where a - floating point value;
        ///     k - integer value;
        ///     x - variable(each summand can have many variables).
        /// </summary>
        public static string ToCanonicalForm(this TermSumParser parser, string text)
        {
            var parts = text.Split('=');
            if (parts.Length != 2)
                throw new ArgumentException("Unexpected equition sign");
            var left = parser.Parse(parts[0]);
            var right = parser.Parse(parts[1]);
            left.Subtract(right);
            return left.AsString() + "=0";
        }
    }
}