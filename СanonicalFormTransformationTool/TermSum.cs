using System.Collections;
using System.Collections.Generic;

namespace СanonicalFormTransformationTool
{
    /// <summary>
    ///     Collection of eqution summands and their coefficients
    /// </summary>
    public class TermSum : IEnumerable<KeyValuePair<Term, double>>
    {
        private readonly Dictionary<Term, double> _ratios = new Dictionary<Term, double>();

        public void Add(Term value, double ratio)
        {
            if (_ratios.TryGetValue(value, out var item))
                _ratios[value] = item + ratio;
            else
                _ratios[value] = ratio;
        }

        private void Subtract(Term value, double ratio)
        {
            if (_ratios.TryGetValue(value, out var item))
                _ratios[value] = item - ratio;
            else
                _ratios[value] = -ratio;
        }

        public void Add(TermSum value)
        {
            foreach (var kvp in value)
                Add(kvp.Key, kvp.Value);
        }

        public void Subtract(TermSum value)
        {
            foreach (var kvp in value)
                Subtract(kvp.Key, kvp.Value);
        }

        public static TermSum GetProduct(TermSum arg1, double value)
        {
            var result = new TermSum();
            foreach (var x in arg1._ratios)
            {
                var ratio = x.Value * value;
                result.Add(x.Key, ratio);
            }

            return result;
        }

        public static TermSum GetProduct(TermSum arg1, TermSum arg2)
        {
            var result = new TermSum();
            foreach (var x in arg1._ratios)
            foreach (var y in arg2._ratios)
            {
                var ratio = x.Value * y.Value;
                var term = Term.GetProduct(x.Key, y.Key);
                result.Add(term, ratio);
            }

            return result;
        }

        /// <inheritdoc />
        public IEnumerator<KeyValuePair<Term, double>> GetEnumerator()
        {
            return _ratios.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}