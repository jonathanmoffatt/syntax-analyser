using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace JackAnalyser
{
    public class Tokeniser : IDisposable
    {
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

        private bool IsWhitespace => Regex.IsMatch(((char)streamReader.Peek()).ToString(), @"\s");
        private bool AtEnd => streamReader.EndOfStream;

        public Token GetNextToken()
        {
            SkipWhitespace();
            if (AtEnd) return null;
            string chunk = GetChunk();
            if (IsSymbol(chunk)) return new SymbolToken(chunk);
            if (IsInteger(chunk)) return new IntegerConstantToken(chunk);
            return new KeywordToken(chunk);
        }

        private string GetChunk()
        {
            var chunk = new StringBuilder();
            while (!AtEnd && !IsWhitespace)
            {
                chunk.Append((char)streamReader.Read());
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
