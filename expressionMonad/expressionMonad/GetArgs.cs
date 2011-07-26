using System;
using System.Collections.Generic;
using System.Linq;

namespace expressionMonad
{
    class GetArgs
    {
        public void Demo()
        {
            var res = 3.GetArg<int>().Bind(GetArgsExtensions.DoubleIt<int>);

            var list = new List<int> { 1, 2, 3 };

            Console.WriteLine(res(list));
        }
    }

    static class GetArgsExtensions
    {
        public static Func<IEnumerable<T>, T> GetArg<T>(this int n)
        {
            return x => x.Skip(n - 1).First();
        }

        public static Func<IList<T>, int> DoubleIt<T>(int n)
        {
            return x => 2 * n;
        }

        public static Func<IList<T>, B> Bind<T, A, B>(this Func<IList<T>, A> a, Func<A, Func<IList<T>, B>> f)
        {
            return x => f(a(x))(x);
        }
    }
}
