using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ExpressionTree
{
    // helper for monads of having an enriched type of Func<A,B>
    static class FuncTwoArgsGlue
    {
        // binder for function composition 
        // not used in the current workflow, but I preferred to code it as an inspiration for SelectMany
        public static Func<T, U> Bind<T, U>(this Func<T, U> first, Func<U, Func<T, U>> second)
        {
            return x => second(first(x))(x);
        }

        // permits expressiveness when composing Plus and Times
        public static Func<T, V> SelectMany<T, U, V>(this Func<T, U> first,
            Func<U, Func<T, U>> second,
            Func<U, U, V> s)
        {
            return x => s(first(x), second(first(x))(x));
        }
    }
}
