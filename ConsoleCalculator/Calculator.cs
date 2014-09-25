using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleCalculator
{

    class Calculator
    {
        private static readonly Dictionary<string, Func<double, double, double>> _operations =
                            new Dictionary<string, Func<double, double, double>>
                            {
                                { "+", (x, y) => x + y },
                                { "-", (x, y) => x - y },
                                { "*", (x, y) => x * y },
                                { "/", (x, y) => x / y },
                                { "^", (x, y) => Math.Pow(x, y) },
                                { "%", (x, y) => x % y }
                            };

        private static readonly Dictionary<string, int> _operationPrecedences =
                            new Dictionary<string, int>
                            {
                                { "+", 2 },
                                { "-", 2 },
                                { "*", 3 },
                                { "/", 3 },
                                { "^", 4 },
                                { "%", 3 },
                            };
        private static readonly Dictionary<string, bool> _rightAssociativeOperations =
                            new Dictionary<string, bool>
                            {
                                { "^", true },
                            };

        private readonly TokenReader[] _readers;

        public Calculator()
        {
            _readers = new TokenReader[]
            {
                new NumberReader(),
                new WordsReader(_operations.Keys.Concat(new string[] {"(", ")"}).ToArray()),
                new WhiteSpaceReader()
            };
        }

        public double Calculate(string expression)
        {
            return evaluateRpn(toPrefixNotation(expression));
        }

        private IEnumerable<Token> toPrefixNotation(string expression)
        {
            Lexer lexer = new Lexer(expression, _readers);

            List<Token> rpn = new List<Token>(10);
            Stack<Token> opStack = new Stack<Token>(10);

            Token t = lexer.NextToken();
            while (t.Type != TokenType.NULL)
            {
                if (t.Type == TokenType.NUMBER)
                {
                    rpn.Add(t);
                }
                else if (t.Type == TokenType.WORD)
                {
                    if (isOperation(t.PayLoad))
                    {
                        while (opStack.Count > 0 && isOperation(opStack.Peek().PayLoad) &&
                                (
                                  operationPrecedence(t.PayLoad) < operationPrecedence(opStack.Peek().PayLoad) ||
                                  (isLeftAssociative(t.PayLoad) && operationPrecedence(t.PayLoad) == operationPrecedence(opStack.Peek().PayLoad))
                                )
                              )
                        {
                            rpn.Add(opStack.Pop());
                        }

                        opStack.Push(t);
                    }
                    else if (t.PayLoad == "(")
                    {
                        opStack.Push(t);
                    }
                    else if (t.PayLoad == ")")
                    {
                        if (opStack.Count == 0)
                            throw new ArgumentException("Mismatched parenteses");

                        while (opStack.Peek().PayLoad != "(")
                        {
                            rpn.Add(opStack.Pop());
                            if (opStack.Count == 0)
                                throw new ArgumentException("Mismatched parenteses");
                        }

                        opStack.Pop();

                        //if (isOperation(opStack.Peek().PayLoad))
                        //    rpn.Add(opStack.Pop());

                    }
                }
                else if (t.Type != TokenType.WSPACE)
                {
                    throw new ArgumentException("Unexpected token");
                }

                t = lexer.NextToken();
            }

            while (opStack.Count > 0)
            {
                Token top = opStack.Peek();
                if (top.PayLoad == "(" || top.PayLoad == ")")
                    throw new ArgumentException("Mismatched parenteses");

                rpn.Add(opStack.Pop());
            }

            return rpn;
        }

        private double evaluateRpn(IEnumerable<Token> rpn)
        {
            Stack<double> stack = new Stack<double>(10);

            foreach (Token t in rpn)
            {
                if (t.Type == TokenType.NUMBER)
                {
                    stack.Push(double.Parse(t.PayLoad));
                }
                else
                {
                    if (stack.Count < 2)    //   2 - арность операций
                        throw new ArgumentException("Incorrect expression");

                    double a2 = stack.Pop();
                    double v = _operations[t.PayLoad](stack.Pop(), a2);
                    stack.Push(v);
                }
            }

            if (stack.Count != 1)
                throw new ArgumentException("Incorrect expression");

            return stack.Pop();
        }

        private bool isOperation(string op)
        {
            return _operations.ContainsKey(op);
        }

        private bool isLeftAssociative(string op)
        {
            if (!isOperation(op))
                throw new ArgumentException("Argument is not operation");

            return !_rightAssociativeOperations.ContainsKey(op);
        }

        private int operationPrecedence(string op)
        {
            if (!_operationPrecedences.ContainsKey(op))
                throw new ArgumentException("Argument is not operation");

            return _operationPrecedences[op];
        }
    }

}