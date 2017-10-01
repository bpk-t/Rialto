using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rialto.Util
{
    public class CompareSelector<T, TKey> : IEqualityComparer<T>
    {
        private Func<T, TKey> selector;

        public CompareSelector(Func<T, TKey> selector)
        {
            this.selector = selector;
        }

        public bool Equals(T x1, T x2)
        {
            return selector(x1).Equals(selector(x2));
        }

        public int GetHashCode(T obj)
        {
            return selector(obj).GetHashCode();
        }
    }
}
