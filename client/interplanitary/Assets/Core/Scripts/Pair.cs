
using System;

namespace core
{
    [Serializable]
    public class Pair<T, U>
    {
        public T First { get; set; }
        public U Second { get; set; }

        public Pair()
        {
        }

        public Pair(T _first, U _second)
        {
            First = _first;
            Second = _second;
        }
    }
}
