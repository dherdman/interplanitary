
namespace core
{
    public static class Math
    {
        public static float Abs (float num)
        {
            return num < 0 ? -num : num;
        }

        public static bool RoughlyZero(float num)
        {
            return Abs(num) < 0.001;
        }

        public static int Sign(float val, bool allowZero = true)
        {
            return allowZero && RoughlyZero(val) ? 0 : val < 0 ? -1 : 1;
        }

        public static float AbsoluteMax(float a, float b)
        {
            return Abs(a) > Abs(b) ? a : b;
        }
    }
}
