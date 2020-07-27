using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace СanonicalFormTransformationTool
{
    /// <summary>
    ///     Parser for transforming equation text representation of summands
    /// </summary>
    public class TermSumParser
    {
        private static readonly CultureInfo Culture = CultureInfo.InvariantCulture;

        /// <summary>
        ///     Parses text representation of summands in the following form:
        ///     P1 + P2 + ... + PN
        ///     where P1..PN - summands, which look like: ax^k
        ///     where a - floating point value;
        ///     k - integer value;
        ///     x - variable(each summand can have many variables).
        ///     Can include brackets
        /// </summary>
        public TermSum Parse(string text)
        {
            return Parse(text.AsSpan());
        }

        private TermSum Parse(ReadOnlySpan<char> text)
        {
            var next = 0;
            TermSum arg = null;
            while (next < text.Length)
            {
                if (text[next] == ' ')
                {
                    next++;
                    continue;
                }

                if (TryGetGroup(text, ref next, out var arg2))
                {
                    SetOrMultiply(ref arg, arg2);
                    continue;
                }

                // (?<sign>+-)?(?<coef>[\.0-9]*)
                GetSign(text, next, out var sign, out var slen);
                next += slen;
                if (slen == 0 && arg != null)
                    throw new ArgumentException("Unexpected input value");

                GetDouble(text, next, out var coef, out var coefLength);
                coef *= sign;
                next += coefLength;

                //(?<group>\(.*\))
                if (TryGetGroup(text, ref next, out arg2))
                {
                    SetOrAdd(ref arg, TermSum.GetProduct(arg2, coef));
                }
                //(?<x1>[a-zA-Z](^\d)?)(?<x2>[a-zA-Z](^\d)?)...
                else if (TryGetTerm(text, next, out var term, out var tlen))
                {
                    if (arg == null)
                        arg = new TermSum();
                    arg.Add(term, coef);
                    next += tlen;
                }
                else if (coefLength > 0)
                {
                    if (arg == null)
                        arg = new TermSum();
                    arg.Add(Term.Empty, coef);
                }
                else if (slen > 0)
                {
                    throw new ArgumentException();
                }
            }

            return arg;
        }

        private bool TryGetTerm(ReadOnlySpan<char> text, int next, out Term result, out int length)
        {
            result = ParseTerm(text, next, out length);
            return length > 0;
        }

        private bool TryGetGroup(ReadOnlySpan<char> text, ref int index, out TermSum result)
        {
            if (index < text.Length && text[index] == '(')
            {
                var endIndex = GetClosingBracketIndex(text, index);
                var len = endIndex - index - 1;
                result = Parse(text.Slice(index + 1, len));
                index += len + 2;
                return true;
            }

            result = null;
            return false;
        }

        private Term ParseTerm(ReadOnlySpan<char> value, int start, out int length)
        {
            var powers = new Dictionary<char, int>();
            var end = start;
            while (end < value.Length && char.IsLetter(value[end]))
            {
                var p = value[end];
                end++;
                if (end == value.Length || value[end] != '^')
                {
                    powers.Add(p, 1);
                }
                else
                {
                    end++;
                    GetInt(value, end, out var power, out var len);
                    end += len;
                    powers.Add(p, power);
                    powers[p] = power;
                }
            }

            length = end - start;
            return length == 0 ? Term.Empty : new Term(powers);
        }

        private void GetDouble(ReadOnlySpan<char> source, int index, out double d, out int length)
        {
            var end = index;
            while (end < source.Length && source[end] == ' ')
                end++;
            var start = end;
            while (end < source.Length && "1234567890.".Contains(source[end]))
                end++;
            var dlen = end - index;
            length = dlen;
            d = start == end ? 1 : double.Parse(source.Slice(index, dlen), NumberStyles.Any, Culture);
        }

        private void GetSign(ReadOnlySpan<char> source, int index, out int sign, out int length)
        {
            var end = index;
            while (end < source.Length && source[end] == ' ')
                end++;
            length = end - index;
            if (end == source.Length)
            {
                sign = 1;
            }
            else if (source[end] == '-')
            {
                length++;
                sign = -1;
            }
            else if (source[end] == '+')
            {
                length++;
                sign = 1;
            }
            else
            {
                sign = 1;
            }
        }

        private void GetInt(ReadOnlySpan<char> source, int index, out int i, out int len)
        {
            var end = index;
            while (end < source.Length && source[end] == ' ')
                end++;
            if ("+-".Contains(source[end]))
                end++;
            while (end < source.Length && "1234567890".Contains(source[end]))
                end++;
            len = end - index;
            i = int.Parse(source.Slice(index, len), NumberStyles.Any, Culture);
        }

        private static int GetClosingBracketIndex(ReadOnlySpan<char> text, int openIndex)
        {
            var cnt = 1;
            for (var i = openIndex + 1; i < text.Length && cnt > 0; i++)
                switch (text[i])
                {
                    case '(':
                        cnt++;
                        break;
                    case ')':
                        cnt--;
                        if (cnt == 0)
                            return i;
                        break;
                }

            throw new ArgumentException();
        }

        private static void SetOrMultiply(ref TermSum value, TermSum arg)
        {
            value = value == null ? arg : TermSum.GetProduct(value, arg);
        }

        private static void SetOrAdd(ref TermSum value, TermSum arg)
        {
            if (value == null)
                value = arg;
            else
                value.Add(arg);
        }
    }
}