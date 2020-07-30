using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace JackAnalyser
{
    public class Tokeniser : IDisposable
    {
        private string[] keywords = new[] { "class", "constructor", "function", "method", "field", "static", "var", "int", "char", "boolean", "void", "true", "false", "null", "this", "let", "do", "if", "else", "while", "return" };
        private const string symbols = "{}()[].,;+-*/&|<>=~";
        private bool disposedValue;
        private readonly StreamReader streamReader;

        public Tokeniser(string input)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            streamReader = new StreamReader(new MemoryStream(bytes));
        }

        public Tokeniser(Stream stream)
        {
            streamReader = new StreamReader(stream);
        }

        private bool IsWhitespace => Regex.IsMatch(Peek, @"\s");
        private bool AtEnd => streamReader.EndOfStream;
        private string Peek => AtEnd ? null : ((char)streamReader.Peek()).ToString();
        private string Eat() => AtEnd ? null : ((char)streamReader.Read()).ToString();

        public Token GetNextToken()
        {
            SkipWhitespace();
            if (AtEnd) return null;
            string chunk = GetChunk();
            if (IsSymbol(chunk)) return new SymbolToken(chunk);
            if (IsInteger(chunk)) return new IntegerConstantToken(chunk);
            if (IsString(chunk)) return new StringConstantToken(chunk);
            if (IsKeyword(chunk)) return new KeywordToken(chunk);
            return new IdentifierToken(chunk);
        }

        private string GetChunk()
        {
            if (IsSymbol(Peek)) return Eat();
            var chunk = new StringBuilder();
            bool insideStringConstant = false;
            while (!AtEnd && ((!IsWhitespace && !IsSymbol(Peek)) || insideStringConstant))
            {
                char c = (char)streamReader.Read();
                if (c == '"') insideStringConstant = !insideStringConstant;
                chunk.Append(c);
            }
            SkipWhitespace();
            return chunk.ToString();
        }

        private void SkipWhitespace()
        {
            while (!AtEnd && IsWhitespace)
            {
                streamReader.Read();
            }
        }

        private bool IsSymbol(string s) => symbols.Contains(s);
        private bool IsInteger(string s) => Regex.IsMatch(s, @"^\d+$");
        private bool IsString(string s) => s.StartsWith('"') && s.EndsWith('"');
        private bool IsKeyword(string s) => keywords.Any(k => k == s);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    streamReader.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
