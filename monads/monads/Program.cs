using System;

namespace monads
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

            // monads with SelectMany
            var r2 = from x in 5.ToIdentity()
                     from y in 6.ToIdentity()
                     select x + y;
            Console.WriteLine(r2.Value);

            var r3 = from x in 5.ToMaybe()
                     from y in 6.ToMaybe()
                     select x + y;
            Console.WriteLine(r3);
        }
    }


    // Maybe monad
    interface Maybe<T> { }

    public class Nothing<T> : Maybe<T>
    {
        public override string ToString()
        {
            return "Nothing";
        }
    }

    class Just<T> : Maybe<T>
    {
        public T Value { get; private set; }
        public Just(T value)
        {
            Value = value;
        }
        public override string ToString()
        {
            return Value.ToString();
        }
    }

    // Identity monad
    class Identity<T>
    {
        public T Value { get; private set; }

        public Identity(T value)
        {
            Value = value;
        }
    }

    static class MaybeExtensions
    {
        public static Maybe<T> ToMaybe<T>(this T value)
        {
            return new Just<T>(value);
        }

        public static Maybe<B> Bind<A, B>(this Maybe<A> a, Func<A, Maybe<B>> func)
        {
            var justa = a as Just<A>;
            if (a == null)
                return new Nothing<B>();
            return func(justa.Value);
        }

        public static Maybe<V> SelectMany<A, B, V>(this Maybe<A> a, Func<A, Maybe<B>> func, Func<A, B, V> s)
        {
            var justa = a as Just<A>;
            if (justa == null)
                return new Nothing<V>();

            var justb = func(justa.Value) as Just<B>;
            if (justb == null)
                return new Nothing<V>();

            return new Just<V>(s(justa.Value, justb.Value));
        }

        public static Maybe<int> Div(this int numerator, int denominator)
        {
            return denominator == 0
                       ? (Maybe<int>)new Nothing<int>()
                       : new Just<int>(numerator / denominator);
        }

        public static Maybe<double> DivDouble(int numerator)
        {
            return new Just<double>(numerator / 4.0);
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
