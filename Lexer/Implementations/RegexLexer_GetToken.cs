﻿using System.Collections.Generic;

namespace Lexer.Implementation
{
    public partial class RegexLexer: Interfaces.IStringLexer<Interfaces.IToken<string, string>>
    {
        /// <summary>
        /// Get the next token off of the input 
        /// base on a set or regex rules
        /// </summary>
        /// <returns>A new token from the input</returns>
        public Interfaces.IToken<string, string> GetNextToken()
        {
            string curtoken;

            // Ignore whitespace and comments
            manager.Get("[ \t\r\v\n]+");
            manager.Get(comment_sequence);
            if((curtoken = manager.Get(include_sequence))!= null)
            {
                curtoken = curtoken.Remove(0, "#include<".Length);
                curtoken = curtoken.Remove(curtoken.Length - 1, 1);
                manager.PutString(System.IO.File.ReadAllText(curtoken));
            }
            manager.Get("[ \t\r\n\n]+");

            curtoken = "";
            foreach (KeyValuePair<string, string> token in allowed_tokens)
            {
                if((curtoken = manager.Get(token.Key)) != null)
                {
                    if (keywords.Contains(curtoken))
                    {
                        return new Token("keyword", curtoken);
                    }
                    return new Token(token.Value, curtoken);
                }
                curtoken = null;
            }

            if((curtoken = manager.Get(".")) != null)
            {
                return new Token("invalid", curtoken);
            }

            return new Token("eof", "EOF");
        }
    }
}
