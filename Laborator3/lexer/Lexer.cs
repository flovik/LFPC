using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lexer
{
    internal class Lexer
    {
        private int _currentPosition; //current position in input (current char)
        private int _readPosition;  //current reading position in input (after current char)
        //readPosition will help to go further into the input and look after the current 
        //character to see what comes up next
        private readonly string _input; 
        private char _currentChar; //current char under examination

        public Lexer(string input)
        {
            _input = input;
        }

        public List<Token> Tokenizer()
        {
            ReadChar(); //initialize char reading
            var tokens = new List<Token>();

            Token token;
            do
            {
                token = NewToken();
                tokens.Add(token);
            } while (token.Type != Token.TokenType.EOF);

            return tokens;
        }

        protected Token NewToken()
        {
            //look at current character and return a token depending on which char it is
            Token token;

            //skip whitespaces
            SkipWhitespaces();

            switch (_currentChar)
            {
                case '+':
                    if (PeekChar() == '+')
                    {
                        ReadChar(); //move one step forward
                        token = new Token(Token.TokenType.INCREMENT, "++");
                    }
                    else token = new Token(Token.TokenType.PLUS, "+");
                    break;
                case '-':
                    if (PeekChar() == '-')
                    {
                        ReadChar();
                        token = new Token(Token.TokenType.DECREMENT, "--");
                    }
                    else token = new Token(Token.TokenType.MINUS, "-");
                    break;
                case '*':
                    token = new Token(Token.TokenType.MULTIPLY, "*");
                    break;
                case '/':
                    token = new Token(Token.TokenType.DIVIDE, "/");
                    break;
                case '!':
                    if (PeekChar() == '=')
                    {
                        ReadChar();
                        token = new Token(Token.TokenType.NOT_EQ, "!=");
                    }
                    else token = new Token(Token.TokenType.NOT, "!");
                    break;
                case '>':
                    if (PeekChar() == '=')
                    {
                        ReadChar();
                        token = new Token(Token.TokenType.GREATER_EQ, ">=");
                    }
                    else token = new Token(Token.TokenType.GREATER, ">");
                    break;
                case '<':
                    if (PeekChar() == '=')
                    {
                        ReadChar();
                        token = new Token(Token.TokenType.LESSER_EQ, "<=");
                    }
                    else token = new Token(Token.TokenType.LESSER, "<");
                    break;
                case '=':
                    if (PeekChar() == '=') //we peek the next char to see if we have EQ token
                    {
                        ReadChar();
                        token = new Token(Token.TokenType.EQ, "==");
                    }
                    else token = new Token(Token.TokenType.ASSIGN, "=");
                    break;
                case ',':
                    token = new Token(Token.TokenType.COMMA, ",");
                    break;
                case ';':
                    token = new Token(Token.TokenType.SEMICOLON, ";");
                    break;
                case '(':
                    token = new Token(Token.TokenType.LPAREN, "(");
                    break;
                case ')':
                    token = new Token(Token.TokenType.RPAREN, ")");
                    break;
                case '[':
                    token = new Token(Token.TokenType.LBRACK, "[");
                    break;
                case ']':
                    token = new Token(Token.TokenType.RBRACK, "]");
                    break;
                case '{':
                    token = new Token(Token.TokenType.LBRACE, "{");
                    break;
                case '}':
                    token = new Token(Token.TokenType.RBRACE, "}");
                    break;
                case '\0':
                    token = new Token(Token.TokenType.EOF, "");
                    break;
                default:
                    if (IsLetter())
                    {
                        var literal = ReadIdentifier();
                        var tokenType = Token.CheckKeyword(literal);
                        return new Token(tokenType, literal); //to skip readChar()
                    }
                    else if (IsDigit())
                    {
                        return ReadNumber();
                    }

                    else
                    {
                        token = new Token(Token.TokenType.ILLEGAL, _currentChar.ToString());
                    }
                    break;
            }

            ReadChar();
            return token;
        }

        protected void SkipWhitespaces()
        {
            //whitespaces have no meaning
            while (_currentChar is ' ' or '\t' or '\n' or '\r')
            {
                ReadChar();
            }
        }

        protected void ReadChar()
        {
            //gives the next character and advance our position in the input string
            //main reason of it is to check if reached end of input
            if (_readPosition >= _input.Length) _currentChar = '\0'; //NUL
            else _currentChar = _input[_readPosition];
            _currentPosition = _readPosition;
            _readPosition++; //always points to the 'next' character in input
        }

        protected string ReadIdentifier()
        {
            //reads an identifier and advances our lexer's positions until it encounters
            //a non-letter char
            var pos = _currentPosition;
            while (IsLetter() || IsDigit())
            {
                ReadChar();
            }

            //return the identifier
            return _input.Substring(pos, _currentPosition - pos);
        }

        protected Token ReadNumber()
        {
            //same as identifier, but for numbers
            int pos = _currentPosition;
            while (IsDigit())
            {
                ReadChar();
            }

            //handle float types
            if (_currentChar == '.')
            {
                ReadChar(); //skip dot
                while (IsDigit())
                {
                    ReadChar();
                }

                return new Token(Token.TokenType.FLOAT_DT, _input.Substring(pos, _currentPosition - pos));
            }

            //return the identifier
            return new Token(Token.TokenType.INT_DT, _input.Substring(pos, _currentPosition - pos));
        }

        protected char PeekChar()
        {
            //similar to ReadChar(), but we don't increment lexer's indexes
            //use to check what ReadChar() could return 
            if (_readPosition > _input.Length) return '\0'; //NUL
            return _input[_readPosition];
        }

        protected bool IsLetter()
        {
            return _currentChar is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or '_';
        }

        protected bool IsDigit()
        {
            return _currentChar is >= '0' and <= '9';
        }


    }
}
