using System;
using System.Collections.Generic;
using static interpreter.TokenType;
using static System.Char;

namespace interpreter
{
    internal class Scanner
    {
        private readonly string source;
        private readonly List<Token> tokens = new List<Token>();
        private int start = 0;
        private int current = 0;
        private int line = 1;

        private static readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
        {
            ["and"] = AND,
            ["class"] = CLASS,
            ["else"] = ELSE,
            ["false"] = FALSE,
            ["for"] = FOR,
            ["fun"] = FUN,
            ["if"] = IF,
            ["nil"] = NIL,
            ["or"] = OR,
            ["print"] = PRINT,
            ["return"] = RETURN,
            ["super"] = SUPER,
            ["this"] = THIS,
            ["true"] = TRUE,
            ["var"] = VAR,
            ["while"] = WHILE,
        };

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
                case '"': AddString(); break;
                case var digit when IsDigit(c): AddNumber(); break;
                case var alpha when isAllowedAlpha(c): AddIdentifier(); break;

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

        private void AddIdentifier()
        {
            while (isAllowedAlphaOrDigit(Peek()))
            {
                Advance();
            }

            var text = source.Substring(start, current - start);
            if (!keywords.TryGetValue(text, out var tokenType))
            {
                tokenType = IDENTIFIER;
            }
            AddToken(tokenType);
        }

        private void AddNumber()
        {
            while (IsDigit(Peek()))
            {
                Advance();
            }

            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();

                while (IsDigit(Peek()))
                {
                    Advance();
                }
            }

            AddToken(NUMBER, Double.Parse(source.Substring(start, current - start)));
        }

        private void AddString()
        {
            while (Peek() != '"' && !isAtEnd())
            {
                if (Peek() == '\n')
                {
                    line++;
                }
                Advance();
            }

            if (isAtEnd())
            {
                Program.Error(line, -1, "Unterminated string detected");
                return;
            }

            Advance();

            var value = source.Substring(start + 1, current - start - 1);
            AddToken(STRING, value);
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

        private char PeekNext() => current + 1 > source.Length ? '\0' : source[current + 1];

        private bool isAtEnd() => current >= source.Length;

        private bool isAllowedAlpha(char c) => (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_';
        private bool isAllowedAlphaOrDigit(char c) => IsDigit(c) || isAllowedAlpha(c);
    }
}