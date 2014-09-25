using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalculator
{
    enum TokenType
    {
        NULL = 0, NUMBER = 1, WORD = 2, WSPACE
    };

    struct Token
    {
        public string PayLoad;
        public TokenType Type;
    };

    class State
    {
        public static readonly State Finish = new State(new Transition[] { });
        public static readonly State Error  = new State(new Transition[] { });

        private List<Transition> _transitions;
        public Transition[] Transitions
        {
            get { return _transitions.ToArray(); }
            set { _transitions = new List<Transition>(value); }
        }

        public State(Transition[] transitions)
        {
            Transitions = transitions;
        }

        public State Transit(char ch)
        {
            State nextState = this;

            foreach(Transition t in _transitions)
            {
                if (t.Condition(ch))
                {
                    nextState = t.Destination;
                    break;
                }
            }

            return nextState;
        }
    }

    struct Transition
    {
        private Condition _condition;

        public Condition Condition
        {
            get { return _condition; }
            set { _condition = value; }
        }

        private State _destination;

        public State Destination
        {
            get { return _destination; }
            set { _destination = value; }
        }
    }

    delegate bool Condition(char ch);

    abstract class TokenReader
    {
        private State _currentState;
        private State _initialState;

        private TokenType _type;

        public virtual Token TryReadToken(string input)
        {
            return new Token
            {
                PayLoad = _tryReadToken(input).ToString(),
                Type = _type
            };
        }

        protected StringBuilder _tryReadToken(string input)
        {
            StringBuilder sb = new StringBuilder();

            _currentState = _initialState;
            foreach(char c in input)
            {
                if (Update(c))
                    break;

                sb.Append(c);
            }

            if (_currentState == State.Error)
                sb.Clear();
            
            return sb;
        }

        protected TokenReader(TokenType type)
        {
            _type = type;
        }

        protected TokenReader(TokenType type, State initialState)
        {
            _currentState = initialState;
            _type = type;
        }

        protected State InitialState
        {
            get { return _initialState; }
            set { _initialState = value; }
        }

        public TokenType Type
        {
            get { return _type; }
            set { _type = value; }
        }

        //  возвращает false при переходе в терминальное состояние
        protected bool Update(char ch)
        {
            State nextState = _currentState.Transit(ch);
            if (nextState != _currentState)
                _currentState = nextState;

            return _currentState == State.Finish || _currentState == State.Error;
        }

    }

}
