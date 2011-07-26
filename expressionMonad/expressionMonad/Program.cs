using System;
using System.Collections.Generic;
using System.Linq;

namespace expressionMonad
{
    // implements Expression Tree Monad (EDSL example taken from Bartosz Milewski article, see http://bartoszmilewski.wordpress.com/2011/07/11/monads-in-c/)
    class Program
    {
        static void Main()
        {
            // build expression tree
            var exp = 2.ToConstant() * 3.ToConstant() + 13.ToConstant() * (1.ToArgsN() + 2.ToArgsN());

            // compile expression into program
            // will evaluate to: 2 * 3 + 13 * (Args[1] + Args[2])
            var program = exp.Compile();

            // execute program by passing Args            
            var result = program(new Args(new[] { 2, 3, 4 }));

            // display result
            Console.WriteLine(result);
        }
    }

    // arguments
    class Args
    {
        public IEnumerable<int> List;

        // constructor
        public Args(IEnumerable<int> list)
        {
            List = list.ToList();
        }
    }

    // using abstract class instead of interface because I need operator overloading
    abstract class Exp
    {
        public static Plus operator +(Exp first, Exp second)
        {
            return new Plus(first, second);
        }

        public static Times operator *(Exp first, Exp second)
        {
            return new Times(first, second);
        }
    }

    // expression represented by a constant
    class Constant : Exp
    {
        public int Value;

        // constructor
        public Constant(int value)
        {
            Value = value;
        }
    }

    // addition expression containing two operands
    class Plus : Exp
    {
        public Exp Exp1;
        public Exp Exp2;

        // constructor
        public Plus(Exp exp1, Exp exp2)
        {
            Exp1 = exp1;
            Exp2 = exp2;
        }
    }

    // multiplication expression containing two operands
    class Times : Exp
    {
        public Exp Exp1;
        public Exp Exp2;

        // constructor
        public Times(Exp exp1, Exp exp2)
        {
            Exp1 = exp1;
            Exp2 = exp2;
        }
    }

    // expression that holds a program (action on given list of int)
    class ArgsN : Exp
    {
        public Func<Args, int> Func;

        // constructor
        // extracts n-th element from an IEnumerable
        public ArgsN(int n)
        {
            Func = GetArgs(n);
        }

        // helper
        public static Func<Args, int> GetArgs(int n)
        {
            return x => x.List.Skip(n - 1).First();
        }
    }

    // helper class
    static class Extensions
    {
        // result of compilation when passed a constant is a function returning the constant
        public static Func<Args, int> Compile(this Constant c)
        {
            return x => c.Value;
        }

        // result of compilation when passed an ArgsN is a function returning function waiting to be evaluated
        public static Func<Args, int> Compile(this ArgsN c)
        {
            return c.Func;
        }

        // result of compiling an addition is a function returning the additon of the compiled operands
        public static Func<Args, int> Compile(this Plus plus)
        {
            return x =>
                   (from a in Compile(plus.Exp1)
                    from b in Compile(plus.Exp2)
                    select a + b)(x);
        }

        // result of compiling a multiplication is a function returning the multiplication of compiled operands
        public static Func<Args, int> Compile(this Times times)
        {
            return x =>
                   (from a in Compile(times.Exp1)
                    from b in Compile(times.Exp2)
                    select a * b)(x);
        }

        // I wasn't so inspired here, I had to find a substitute for template specialization
        public static Func<Args, int> Compile(Exp c)
        {
            if (c is ArgsN)
                return Compile(c as ArgsN);
            if (c is Plus)
                return Compile(c as Plus);
            if (c is Times)
                return Compile(c as Times);

            return Compile(c as Constant);
        }

        // binder for function composition 
        // not used in the current workflow, but I preferred to code it as an inspiration for SelectMany
        public static Func<Args, int> Bind(this Func<Args, int> first, Func<int, Func<Args, int>> second)
        {
            return x => second(first(x))(x);
        }

        // permits expressiveness when composing Plus and Times
        public static Func<Args, int> SelectMany(this Func<Args, int> first,
            Func<int, Func<Args, int>> second,
            Func<int, int, int> s)
        {
            return x =>
                       {
                           int res1 = first(x);
                           int res2 = second(first(x))(x);
                           return s(res1, res2);
                       };
        }

        // helper for avoiding explicit creation of Constant variables
        public static Constant ToConstant(this int value)
        {
            return new Constant(value);
        }

        // helper for avoiding explicit creation of ArgsN variables
        public static ArgsN ToArgsN(this int n)
        {
            return new ArgsN(n);
        }
    }
}
