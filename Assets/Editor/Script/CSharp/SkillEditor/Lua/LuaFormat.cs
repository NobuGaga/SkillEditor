namespace SkillEditor {

    internal static class LuaFormat {

        public const char EqualSymbol = '=';
        public const char SpaceSymbol = ' ';
        public const char CommaSymbol = ',';

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

        public const ushort IntMin = '0';
        public const ushort IntMax = '9';
        public const ushort NumberPoint = '.';
        public const string TabString = "    ";

        public enum Type {
            LuaString,
            LuaInt,
            LuaNumber,
            LuaTable,
        }

        public struct LuaTableKeyValue {
            public string key;
            public Type type;

            public int KeyLength => key.Length;

            public LuaTableKeyValue(string key, Type type) {
                this.key = key;
                this.type = type;
            }
        }
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