using System;

namespace Identity
{
    class Program
    {
        static void Main()
        {
            Func<int, Identity<int>> add = x => new Identity<int>(x + 2);
            Func<int, Identity<int>> mult = x => new Identity<int>(x * 3);

            Func<int, Identity<int>> combine = x => add(x).Bind(mult);

            var result = combine(5);
            Console.WriteLine(result.Value);

            var r2 = from x in 5.ToIdentity()
                     from y in 6.ToIdentity()
                     select x + y;
            Console.WriteLine(r2.Value);
        }
    }

    class Identity<T>
    {
        public T Value { get; private set; }

        public Identity(T value)
        {
            Value = value;
        }
    }

    static class IdentityExtensions
    {
        public static Identity<B> Bind<A, B>(this Identity<A> a, Func<A, Identity<B>> func)
        {
            return func(a.Value);
        }

        public static Identity<T> ToIdentity<T>(this T value)
        {
            return new Identity<T>(value);
        }

        public static Identity<V> SelectMany<A, B, V>(this Identity<A> a, Func<A, Identity<B>> func,
            Func<A, B, V> s)
        {
            return s(a.Value, func(a.Value).Value).ToIdentity();
        }
    }
}
