
using System;

namespace core
{
    [Serializable]
    public class Pair<T, U>
    {
        public T x;
        public U y;

        public T First
        {
            get
            {
                return x;
            }
        }
        public U Second
        {
            get
            {
                return y;
            }
        }

        public Pair()
        {
        }

        public Pair(T _x, U _y)
        {
            x = _x;
            y = _y;
        }
    }
}
