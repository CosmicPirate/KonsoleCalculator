using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleCalculator
{
    class NumberReader : TokenReader
    {

        public NumberReader()
            : base(TokenType.NUMBER)
        {
            State fractPart = new State(new Transition[]   //  дробная часть
                {
                    new Transition { Condition = ch => !char.IsDigit(ch), Destination = State.Finish }
                });
            State intergerPart = new State(new Transition[]   //  целая часть
                {
                    new Transition { Condition = ch => ch == '.' || ch == ',', Destination = fractPart },
                    new Transition { Condition = ch => !char.IsDigit(ch), Destination = State.Finish }
                });
            State start = new State(new Transition[]   //  начало
                {
                    new Transition { Condition = ch => char.IsDigit(ch), Destination = intergerPart },
                    new Transition { Condition = ch => ch == '.' || ch == ',', Destination = fractPart },
                    new Transition { Condition = ch => true, Destination = State.Error }    //  default
                });

            InitialState = start;
        }

        public override Token TryReadToken(string input)
        {
            StringBuilder sb = _tryReadToken(input);

            if (sb.Length > 0 && (sb[sb.Length - 1] == '.' || sb[sb.Length - 1] == ','))
                sb.Remove(sb.Length - 1, 1);

            return new Token
            {
                PayLoad = sb.ToString(),
                Type = Type
            };
        }
    }

    class WordsReader : TokenReader
    {
        private string[] _words;

        public WordsReader(string[] words)
            : base(TokenType.WORD)
        {
            _words = words;
        }

        public override Token TryReadToken(string input)
        {
            string token = "";
            bool founded = false;

            foreach (string op in _words)
            {
                token = input.Substring(0, op.Length);
                if (token == op)
                {
                    founded = true;
                    break;
                }
            }

            return new Token
            {
                PayLoad = founded ? token : "",
                Type = founded ? Type : TokenType.NULL
            };
        }
    }

    class WhiteSpaceReader : TokenReader
    {
        public WhiteSpaceReader() : base(TokenType.WSPACE) { }

        public override Token TryReadToken(string input)
        {
            int i = 0;
            while (i < input.Length && char.IsWhiteSpace(input[i])) ++i;

            return new Token
            {
                PayLoad = input.Substring(0, i),
                Type = Type
            };
        }
    }

}

