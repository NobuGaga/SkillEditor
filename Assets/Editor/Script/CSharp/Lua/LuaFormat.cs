namespace Lua {

    internal static class LuaFormat {

        public const char EqualSymbol = '=';
        public const char SpaceSymbol = ' ';
        public const char CommaSymbol = ',';
        public const string PointSymbol = ".";

        private const char QuotationSymbol = '"';
        public static readonly PairChar QuotationPair = new PairChar(QuotationSymbol, QuotationSymbol);

        private const char SquareBraceStart = '[';
        private const char SquareBraceEnd = ']';
        public static readonly PairStringChar SquareBracketPair = new PairStringChar(SquareBraceStart, SquareBraceEnd);

        private const char CurlyBracesStart = '{';
        private const char CurlyBracesEnd = '}';
        public static readonly PairChar CurlyBracesPair = new PairChar(CurlyBracesStart, CurlyBracesEnd);

        public const char NotesSymbolStart = '-';
        private const string NotesSymbol = "--";
        public const char LineSymbol = '\n';
        public static readonly PairStringChar NotesLinePair = new PairStringChar(NotesSymbol, LineSymbol);

        public static readonly PairString HashKeyPair = new PairString(SquareBraceStart, QuotationSymbol,
                                                                        SquareBraceEnd, QuotationSymbol);

        public const ushort IntMin = '0';
        public const ushort IntMax = '9';
        public const ushort NumberPoint = '.';
        public const string TabString = "    ";
        public const string ArrayKeyFormat = "[{0}]=";
        public const string ArrayKeyValueFormat = "{0}[{1}] = {2}\n{3}{0}{4},\n";
        public const string StringKeyValueFormat = "{0}[\"{1}\"] = {2}\n{3}{0}{4},\n";
        public const string FixedKeyValueFormat = "{0}{1} = {2}\n{3}{0}{4},\n";
        public const string RequireFunction = "require(\"{0}\")";

        public struct PairChar {

            public char start;
            public char end;

            public PairChar(char start, char end) {
                this.start = start;
                this.end = end;
            }
        }

        public struct PairStringChar {

            public string start;
            public char end;

            public PairStringChar(string start, char end) {
                this.start = start;
                this.end = end;
            }

            public PairStringChar(char start, char end) {
                this.start = new string(new char[] { start });
                this.end = end;
            }
        }

        public struct PairString {
            
            public string start;
            public string end;

            public PairString(char startChar1, char startChar2, char endChar1, char endChar2) {
                start = new string(new char[] { startChar1, startChar2 });
                end = new string(new char[] { endChar1, endChar2 });
            }
        }
    }
}