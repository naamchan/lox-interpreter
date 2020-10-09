using System;
using System.Collections.Generic;
using static interpreter.TokenType;

namespace interpreter
{
    internal class Scanner
    {
        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        public Scanner(string source)
        {
            this.source = source;
        }

        internal IList<Token> scanTokens(string source)
        {
            // System.Console.WriteLine($"Scan {source}");
            while (!isAtEnd())
            {
                start = current;
                scanToken();
            }

            tokens.Add(new Token(EOF, "", null, line));
            return tokens;
        }

        private void scanToken()
        {
            char c = Advance();
            // System.Console.WriteLine($"Processing {c}");

            switch (c)
            {
                case '(': AddToken(LEFT_PAREN); break;
                case ')': AddToken(RIGHT_PAREN); break;
                case '{': AddToken(LEFT_BRACE); break;
                case '}': AddToken(RIGHT_BRACE); break;
                case ',': AddToken(COMMA); break;
                case '.': AddToken(DOT); break;
                case '-': AddToken(MINUS); break;
                case '+': AddToken(PLUS); break;
                case ';': AddToken(SEMICOLON); break;
                case '*': AddToken(STAR); break;
                case '!': AddToken(Match('=') ? BANG_EQUAL : BANG); break;
                case '=': AddToken(Match('=') ? EQUAL_EQUAL : EQUAL); break;
                case '<': AddToken(Match('=') ? LESS_EQUAL : LESS); break;
                case '>': AddToken(Match('=') ? GREATER_EQUAL : GREATER); break;
                case '\n': line++; break;
                case ' ': case '\r': case '\t': break;

                case '/':
                    if (Match('/'))
                    {
                        while (Peek() != '\n' && !isAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(SLASH);
                    }
                    break;
                default:
                    Program.Error(line, -1, $"Unexpected character {c}");
                    break;
            }
        }

        private bool Match(char expected)
        {
            if (isAtEnd())
            {
                return false;
            }

            if (source[current] != expected)
            {
                return false;
            }

            current++;
            return true;
        }

        private void AddToken(TokenType tokenType, Object literal = null)
        {
            // System.Console.WriteLine($"{start}:{current}");
            tokens.Add(new Token(tokenType, source.Substring(start, current - start), literal, line));
        }

        private char Advance()
        {
            current++;
            return source[current - 1];
        }

        private char Peek() => isAtEnd() ? '\0' : source[current];

        private bool isAtEnd() => current >= source.Length;
    }
}