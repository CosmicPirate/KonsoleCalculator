using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace ConsoleCalculator
{
    class Lexer
    {
        private string _input;
        private int _inputIndex;
        private List<TokenReader> _readers;

        public Lexer(string input, TokenReader[] readers)
        {
            _input = input;
            _readers = new List<TokenReader>(readers);
        }

        public bool IsEnd()
        {
            return _inputIndex == _input.Length;
        }

        public Token NextToken()
        {
            Token token =  _nextToken();

            return token;
        }

        public Token _nextToken()
        {
            Token maxToken = new Token{PayLoad = "", Type = TokenType.NULL};

            if (_inputIndex < _input.Length)
            {
                string s = _input.Substring(_inputIndex);
                foreach(TokenReader reader in _readers)
                {
                    Token t = reader.TryReadToken(s);

                    if (t.PayLoad.Length > maxToken.PayLoad.Length)
                        maxToken = t;
                }

                if (maxToken.Type != TokenType.NULL)
                    _inputIndex += maxToken.PayLoad.Length;
            }

            return maxToken;
        }
    }

}