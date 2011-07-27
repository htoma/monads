using System;

namespace Maybe
{
    // implements monad for the Maybe type (similar to Nullable but applyable to any type)
    class Program
    {
        static void Main()
        {
            var r3 = from x in 5.ToMaybe()
                     from y in 6.ToMaybe()
                     select x + y;
            Console.WriteLine(r3);
        }
    }

    interface IMaybe<T> { }

    class Nothing<T> : IMaybe<T>
    {
        public override string ToString()
        {
            return "Nothing";
        }
    }

    class Just<T> : IMaybe<T>
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

    static class MaybeExtensions
    {
        public static IMaybe<T> ToMaybe<T>(this T value)
        {
            return new Just<T>(value);
        }

        public static IMaybe<B> Bind<A, B>(this IMaybe<A> a, Func<A, IMaybe<B>> func)
        {
            var justa = a as Just<A>;
            if (a == null)
                return new Nothing<B>();
            return func(justa.Value);
        }

        public static IMaybe<V> SelectMany<A, B, V>(this IMaybe<A> a, Func<A, IMaybe<B>> func, Func<A, B, V> s)
        {
            var justa = a as Just<A>;
            if (justa == null)
                return new Nothing<V>();

            var justb = func(justa.Value) as Just<B>;
            if (justb == null)
                return new Nothing<V>();

            return new Just<V>(s(justa.Value, justb.Value));
        }

        public static IMaybe<int> Div(this int numerator, int denominator)
        {
            return denominator == 0
                       ? (IMaybe<int>)new Nothing<int>()
                       : new Just<int>(numerator / denominator);
        }

        public static IMaybe<double> DivDouble(int numerator)
        {
            return new Just<double>(numerator / 4.0);
        }
    }
}
