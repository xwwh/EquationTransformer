using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace СanonicalFormTransformationTool
{
    /// <summary>
    ///     Equation summand
    /// </summary>
    public class Term : IEnumerable<KeyValuePair<char, int>>
    {
        public static readonly Term Empty = new Term(new Dictionary<char, int>());

        private readonly ImmutableDictionary<char, int> _powers;
        private readonly string _text;

        public Term(Dictionary<char, int> powers)
        {
            _powers = powers.ToImmutableDictionary();
            _text = GetText(_powers);
        }

        public bool IsEmpty => ReferenceEquals(this, Empty) || _powers.All(kvp => kvp.Value == 0);

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<char, int>> GetEnumerator()
        {
            return _powers.GetEnumerator();
        }

        public static Term GetProduct(Term arg1, Term arg2)
        {
            var powers = new Dictionary<char, int>(arg1._powers);
            foreach (var p in arg2._powers)
                if (powers.TryGetValue(p.Key, out var value))
                    powers[p.Key] = p.Value + value;
                else
                    powers.Add(p.Key, p.Value);
            return new Term(powers);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is Term t && Equals(t);
        }

        protected bool Equals(Term other)
        {
            return _text == other._text;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return _text.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _text;
        }

        private static string GetText(IEnumerable<KeyValuePair<char,int>> powers)
        {
            var sb = new StringBuilder(10);
            foreach (var kvp in powers
                .OrderBy(x => x.Key)
                .ThenBy(x => x.Value))
            {
                if (kvp.Value == 0)
                    continue;
                sb.Append(kvp.Key);
                if (kvp.Value == 1)
                    continue;
                sb.Append('^').Append(kvp.Value);
            }

            return sb.ToString();
        }
    }
}