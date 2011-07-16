using System;

namespace continuation
{
    static class Extensions
    {
        public delegate TAnswer K<out T, TAnswer>(Func<T, TAnswer> k);

        public static K<T, TAnswer> ToContinuation<T, TAnswer>(this T value)
        {
            return c => c(value);
        }

        public static K<TU, TAnswer> Bind<T, TU, TAnswer>(this K<T, TAnswer> m, Func<T, K<TU, TAnswer>> k)
        {
            return c => m(x => k(x)(c));
        }

        public static K<TV, TAnswer> SelectMany<T, TU, TV, TAnswer>(this K<T, TAnswer> m, Func<T, K<TU, TAnswer>> k,
            Func<T, TU, TV> s)
        {
            return c => m(x => k(x)(u => c(s(x, u))));
        }
    }

    class Program
    {
        static void Main()
        {
            var res = from x in 7.ToContinuation<int, string>()
                      from y in 5.ToContinuation<int, string>()
                      from z in 11.ToContinuation<int, string>()
                      select x + y + z;

            Console.WriteLine(res(x => x.ToString().Replace('2', '-')));
        }
    }
}
