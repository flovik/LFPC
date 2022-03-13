using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lexer
{
    internal class Token
    {
        public TokenType Type { get; set; }
        public string Value { get; set; }

        private static readonly Dictionary<string, TokenType> words = new()
        {
            ["true"] = TokenType.TRUE,
            ["false"] = TokenType.FALSE,
            ["if"] = TokenType.IF,
            ["else"] = TokenType.ELSE,
            ["return"] = TokenType.RETURN,
            ["while"] = TokenType.WHILE,
            ["for"] = TokenType.FOR,
            ["int"] = TokenType.INT_DT,
            ["void"] = TokenType.VOID_DT,
            ["main"] = TokenType.MAIN,
            ["float"] = TokenType.FLOAT_DT
        };

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }

        public enum TokenType
        {
            TRUE,
            FALSE,
            IF,
            ELSE,
            RETURN,
            WHILE,
            FOR,
            PLUS,
            MINUS,
            MULTIPLY,
            DIVIDE,
            NOT,
            GREATER,
            LESSER,
            ASSIGN,
            EQ,
            GREATER_EQ,
            LESSER_EQ,
            NOT_EQ,
            INCREMENT,
            DECREMENT,
            COMMA,
            SEMICOLON,
            LPAREN,
            RPAREN,
            LBRACK,
            RBRACK,
            LBRACE,
            RBRACE,
            INT_DT,
            VOID_DT,
            EOF,
            IDENTIFIER,
            ILLEGAL,
            MAIN,
            FLOAT_DT
        }

        public static TokenType CheckKeyword(string keyword)
        {
            return words.ContainsKey(keyword) ? words[keyword] : TokenType.IDENTIFIER;
        }
    }
}
