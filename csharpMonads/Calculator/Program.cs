using System;

namespace Calculator
{
    // inspired by bartosz milewski, see: http://bartoszmilewski.wordpress.com/2011/03/17/monads-for-the-curious-programmer-part-3/

    // compromised on Pop and Peek that use default(T) instead of throwing
    // todo (htoma): combine with enriched type that swallows exceptions

    // can be templated only on T, but used second generic parameter to emphasize correct processing inside Bind
    class Tuple<T, U>
    {
        public U Value;
        public IState<T> Calculator;

        public Tuple(U value, IState<T> calculator)
        {
            Value = value;
            Calculator = calculator;
        }
    }

    static class Extensions
    {
        public static Func<IState<T>, Tuple<T, T>> Pop<T>()
        {
            return x => new Tuple<T, T>(x.Peek(), x.Pop());
        }

        // basic type constructor using Push
        public static Func<IState<T>, Tuple<T, T>> Push<T>(T toBePushed)
        {
            return x => new Tuple<T, T>(x.Peek(), x.Push(toBePushed));
        }

        public static Func<IState<T>, Tuple<T, T>> AddTwo<T>(Func<T, T, T> add)
        {
            return x => new Tuple<T, T>(default(T),
                x.Push(add(x.Peek(), x.Pop().Peek())));
        }

        // binder
        public static Func<IState<T>, Tuple<T, B>> Bind<T, A, B>(this Func<IState<T>, Tuple<T, A>> act,
            Func<A, Func<IState<T>, Tuple<T, B>>> func)
        {
            return x =>
            {
                var tmp = act(x);
                var tmpFunc = func(tmp.Value);
                return tmpFunc(tmp.Calculator);
            };
        }
    }

    class Program
    {
        static void Main()
        {
            // empty list
            IState<int> s1 = State<int>.Empty;

            // list having two elements
            IState<int> s2 = s1.Push(10).Push(2);

            // add an element, add stack(top) + stack(top - 1) and extract top
            var funcFirst = Extensions.Push(1).
                Bind(x => Extensions.AddTwo<int>((p, q) => p + q)).Bind(x => Extensions.Pop<int>());
            Console.WriteLine(funcFirst(s1).Value);
            Console.WriteLine(funcFirst(s2).Value);

            // add two elements, add stack(top) + stack(top - 1) and extract top 
            var funcSecond = Extensions.Push(1).Bind(x => Extensions.Push(4))
                .Bind(x => Extensions.AddTwo<int>((p, q) => p + q)).Bind(x => Extensions.Pop<int>());

            Console.WriteLine(funcSecond(s2).Value);
        }
    }

}
