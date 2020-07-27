using System.Collections.Generic;
using NUnit.Framework;
using СanonicalFormTransformationTool;

namespace СanonicalFormTransformationToolTests
{
    public class TermTests
    {
        [Test]
        public void same_terms_should_have_same_hashcode()
        {
            var t1 = new Term(new Dictionary<char, int>
            {
                {'a', 1 },
                {'b', 2 },
                {'c', 3 },
                {'d', 0 },
            });

            var t2 = new Term(new Dictionary<char, int>
            {
                {'c', 3 },
                {'b', 2 },
                {'a', 1 }
            });

            Assert.AreEqual(t1.GetHashCode(), t2.GetHashCode());
            
        }

        [Test]
        public void should_multiply_terms()
        {
            var t1 = new Term(new Dictionary<char, int>
            {
                {'a', 1 },
                {'b', 2 }
            });

            var t2 = new Term(new Dictionary<char, int>
            {
                {'b', 2 },
                {'c', -3 }
            });

            var expected = new Term(new Dictionary<char, int>
            {
                {'a', 1 },
                {'b', 4 },
                {'c', -3 },
            });

            var actual = Term.GetProduct(t1, t2);

            Assert.AreEqual(expected, actual);
        }
    }
}
