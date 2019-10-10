namespace SkillEditor {

    internal static class LuaFormat {

        private const char QuotationSymbol = '"';
        public static readonly PairChar QuotationPair = new PairChar(QuotationSymbol, QuotationSymbol);

        private const char SquareBraceStart = '[';
        private const char SquareBraceEnd = ']';
        public static readonly PairChar SquareBracketPair = new PairChar(SquareBraceStart, SquareBraceEnd);

        private const char CurlyBracesStart = '{';
        private const char CurlyBracesEnd = '}';
        public static readonly PairChar CurlyBracesPair = new PairChar(CurlyBracesStart, CurlyBracesEnd);

        public const char NotesSymbolStart = '-';
        private const string NotesSymbol = "--";
        private const char LineSymbol = '\n';
        public static readonly PairStringChar NotesLinePair = new PairStringChar(NotesSymbol, LineSymbol);

        public const string TabString = "    ";
        public const ushort IntMin = '0';
        public const ushort IntMax = '9';

    }

    internal struct PairChar {
        public char start;
        public char end;

        public PairChar(char start, char end) {
            this.start = start;
            this.end = end;
        }
    }

    internal struct PairStringChar {
        public string start;
        public char end;

        public PairStringChar(string start, char end) {
            this.start = start;
            this.end = end;
        }
    }
}