﻿using System;
using System.Windows.Forms;

namespace MathCompiler
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string Expression = Microsoft.VisualBasic.Interaction.InputBox("Input an expression", "Expression Input", "1+1");
            Lexer.Interfaces.IStringLexer<Lexer.Interfaces.IToken<string, string>> lexer = new Lexer.Implementation.RegexLexer(Expression);
            MathCompiler compiler = new MathCompiler(lexer.GetAllTokens().ToArray());
            MessageBox.Show(compiler.MainMethod());
        }
    }
}
