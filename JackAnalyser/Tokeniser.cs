using System;
using System.Text;
using System.Text.RegularExpressions;

namespace JackAnalyser
{
    public class Tokeniser
    {
        private readonly string input;
        private int position;

        public Tokeniser(string input)
        {
            this.input = input;
            position = 0;
        }

        public Token GetNextToken()
        {
            if (AtEnd()) return null;
            return new KeywordToken(GetChunk());
        }

        private string GetChunk()
        {
            var chunk = new StringBuilder();
            while(position < input.Length && !IsWhitespace())
            {
                chunk.Append(input[position]);
                position++;
            }
            SkipWhitespace();
            return chunk.ToString();
        }

        private void SkipWhitespace()
        {
            while (position < input.Length && IsWhitespace())
            {
                position++;
            }
        }

        private bool IsWhitespace()
        {
            return Regex.IsMatch(input.Substring(position, 1), @"\s");
        }

        public bool AtEnd()
        {
            SkipWhitespace();
            return position == input.Length;
        }
    }
}
