using System;
using System.Collections;
using System.Collections.Generic;

namespace calculator
{
    // credits to eric lippert, see: http://blogs.msdn.com/b/ericlippert/archive/2007/12/04/immutability-in-c-part-two-a-simple-immutable-stack.aspx

    public interface IState<T> : IEnumerable<T>
    {
        IState<T> Push(T value);
        IState<T> Pop();
        T Peek();
        bool IsEmpty { get; }
    }
    
    // renamed ImmutableStack to State for ease of notation use
    public class State<T> : IState<T>
    {
        private sealed class EmptyStack : IState<T>
        {
            public bool IsEmpty
            {
                get { return true; }
            }

            public T Peek()
            {
                return default(T);
            } //throw new Exception("Empty stack"); 
            
            public IState<T> Push(T value)
            {
                return new State<T>(value, this);
            }

            public IState<T> Pop()
            {
                return this;
            }

            public IEnumerator<T> GetEnumerator()
            {
                yield break;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        private static readonly EmptyStack empty = new EmptyStack();
        public static IState<T> Empty { get { return empty; } }
        private readonly T head;
        private readonly IState<T> tail;
        
        private State(T head, IState<T> tail)
        {
            this.head = head;
            this.tail = tail;
        }
        public bool IsEmpty { get { return false; } }
        public T Peek() { return head; }
        public IState<T> Pop() { return tail; }
        public IState<T> Push(T value) { return new State<T>(value, this); }
        public IEnumerator<T> GetEnumerator()
        {
            for(IState<T> state = this; !state.IsEmpty ; state = state.Pop())
                yield return state.Peek();
        }
        IEnumerator IEnumerable.GetEnumerator() {return GetEnumerator();}
    }
}
