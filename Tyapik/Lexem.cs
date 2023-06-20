using System.Diagnostics.CodeAnalysis;
// ReSharper disable ArrangeTrailingCommaInMultilineLists

namespace Tyapik;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Lexem
{
    #region Definitions

    // types of lexemes
    public const int NONE = -1;
    public const int TABULATION = 0;
    public const int NEWLINE1 = TABULATION + 1;
    public const int NEWLINE2 = NEWLINE1 + 1;
    public const int IF = NEWLINE2 + 1;
    public const int ELSE = IF + 1;
    public const int FI = ELSE + 1;
    public const int WHILE = FI + 1;
    public const int DO = WHILE + 1;
    public const int DONE = DO + 1;
    public const int FOR = DONE + 1;
    public const int IN = FOR + 1;
    public const int CASE = IN + 1;
    public const int ESAC = CASE + 1;
    public const int BREAK = ESAC + 1;
    public const int CONTINUE = BREAK + 1;
    public const int RETURN = CONTINUE + 1;
    public const int LBRACE = RETURN + 1;
    public const int RBRACE = LBRACE + 1;
    public const int LRBRACKET = RBRACE + 1;
    public const int RRBRACKET = LRBRACKET + 1;
    public const int SEMICOLON = RRBRACKET + 1;
    public const int ID = SEMICOLON + 1;
    public const int DOLLAR = ID + 1;
    public const int PERCENT = DOLLAR + 1;
    public const int EQUALS = PERCENT + 1;
    public const int GREATER_EQUALS = EQUALS + 1;
    public const int LESS_EQUALS = GREATER_EQUALS + 1;
    public const int GREATER = LESS_EQUALS + 1;
    public const int LESS = GREATER + 1;
    public const int ELIF = LESS + 1;
    public const int TRUE = ELIF + 1;
    public const int FALSE = TRUE + 1;
    public const int ECHO = FALSE + 1;
    public const int UNTIL = ECHO + 1;
    public const int EXIT = UNTIL + 1;
    public const int EXCLAMATION_MARK = EXIT + 1;
    public const int LSBRACKET = EXCLAMATION_MARK + 1;
    public const int RSBRACKET = LSBRACKET + 1;
    public const int DSEMICOLON = RSBRACKET + 1;
    public const int AMPERSAND = DSEMICOLON + 1;
    public const int DAMPERSAND = AMPERSAND + 1;
    public const int VERTICAL = DAMPERSAND + 1;
    public const int DVERTICAL = VERTICAL + 1;
    public const int SET = DVERTICAL + 1;
    public const int PLUS = SET + 1;
    public const int MINUS = PLUS + 1;
    public const int MULTIPLY = MINUS + 1;
    public const int DIVISION = MULTIPLY + 1;
    public const int REMAINDER = DIVISION + 1;
    public const int COMMA = REMAINDER + 1;
    public const int COLON = COMMA + 1;
    public const int QUOTE1 = COLON + 1;
    public const int QUOTE2 = QUOTE1 + 1;
    public const int DECIMALPOINT = QUOTE2 + 1;
    
    public const int IDENTIFIER = DECIMALPOINT + 1;
    public const int FLOATNUMBER = IDENTIFIER + 1;
    public const int INTNUMBER = FLOATNUMBER + 1;
    public const int EOF = INTNUMBER + 1;
    public const int STRING = EOF + 1;
    public const int FUNCTION = STRING + 1;
    public const int LET = FUNCTION + 1;

    public const int WHITESPACE = LET + 1;
    public const int PARAM = WHITESPACE + 1;

    public static readonly Dictionary<int, string> PRESENTATION = new()
    {
        {TABULATION, "tab"},
        {NEWLINE1, "newline default"},
        {NEWLINE2, "newline from win"},
        {IF, "if"},
        {ELSE, "else"},
        {FI, "fi"},
        {WHILE, "while"},
        {DO, "do"},
        {DONE, "done"},
        {FOR, "for"},
        {IN, "in"},
        {CASE, "case"},
        {ESAC, "esac"},
        {BREAK, "break"},
        {CONTINUE, "continue"},
        {RETURN, "return"},
        {LBRACE, "left brace"},
        {RBRACE, "right brace"},
        {LRBRACKET, "left paren"},
        {RRBRACKET, "left paren"},
        {SEMICOLON, "semicolon"},
        {ID, "id"},
        {DOLLAR, "dollar"},
        {PERCENT, "percent"},
        {EQUALS, "equals"},
        {GREATER_EQUALS, "greater or equals"},
        {LESS_EQUALS, "less or equals"},
        {GREATER, "greater"},
        {LESS, "less"},
        {ELIF, "elif"},
        {TRUE, "true"},
        {FALSE, "false"},
        {ECHO, "echo"},
        {UNTIL, "until"},
        {EXIT, "exit"},
        {EXCLAMATION_MARK, "exclamation mark"},
        {LSBRACKET, "lsquare bracket"},
        {RSBRACKET, "rsquare bracket"},
        {DSEMICOLON, "double semicolon"},
        {AMPERSAND, "ampersand"},
        {DAMPERSAND, "double ampersand"},
        {VERTICAL, "vertical"},
        {DVERTICAL, "double vertical"},
        {SET, "set"},
        {PLUS, "plus"},
        {MINUS, "minus"},
        {MULTIPLY, "multiply"},
        {DIVISION, "dvision"},
        {REMAINDER, "remainder"},
        {COMMA, "comma"},
        {COLON, "colon"},
        {QUOTE1, "quote1"},
        {QUOTE2, "quote2"},
        {DECIMALPOINT, "decimal point"},
        {IDENTIFIER, "id"},
        {FLOATNUMBER, "float num"},
        {INTNUMBER, "int num"},
        {EOF, "eof"},
        {STRING, "string"},
        {LET, "let"},
        {FUNCTION, "function"},
        {WHITESPACE, "whitespace"},
        {PARAM, "parametr"},
    };

    public static readonly Dictionary<string, int> KEYWORDS = new()
    {
        {"if", IF},
        {"fi", FI},
        {"else", ELSE},
        {"elif", ELIF},
        {"while", WHILE},
        {"for", FOR},
        {"return", RETURN},
        {"done", DONE},
        {"case", CASE},
        {"until", UNTIL},
        {"do", DO},
        {"esac", ESAC},
        {"function", FUNCTION},
        {"exit", EXIT},
        {"echo", IDENTIFIER},
        {"let", LET}
    };

    //Unused
    public static readonly Dictionary<string, int> RESERVEDNAMES = new()
    {
        {"true", TRUE},
        {"false", FALSE},
    };

    public const int n_tabs = 4;

    public static readonly Dictionary<string, int> SYMBOLS = new()
    {
        {"=", SET},
        {"+", PLUS},
        {"-", MINUS},
        {"*", MULTIPLY},
        {"/", DIVISION},
        {"%", REMAINDER},
        {"(", LRBRACKET},
        {")", RRBRACKET},
        {"[", LSBRACKET},
        {"]", RSBRACKET},
        {"\t", TABULATION},
        {string.Concat(Enumerable.Repeat(" ", n_tabs)), TABULATION},
        {",", COMMA},
        {":", COLON},
        {"<", LESS},
        {">", GREATER},
        {"'", QUOTE1},
        {"\"", QUOTE2},
        {".", DECIMALPOINT},
        {"\n", NEWLINE1},
        {"\r", NEWLINE2},
        {"!", EXCLAMATION_MARK},
        {"{", LBRACE},
        {"}", RBRACE},
        {";", SEMICOLON},
        {";;", DSEMICOLON},
        {"&", AMPERSAND},
        {"&&", DAMPERSAND},
        {"|", VERTICAL},
        {"||", DVERTICAL},
        {"$", DOLLAR},
        {" ", WHITESPACE},
    };
    
    #endregion

    public readonly int Row;
    public readonly int Col;
    public readonly int Type;
    public readonly string Value;

    public Lexem(int row, int col, int type, string value)
    {
        Row = row;
        Col = col;
        Type = type;
        Value = value;
    }

    public override string ToString()
    {
        var symobl = Type switch
        {
            -1 => "None",
            _ => PRESENTATION[Type]
        };
        return $"({Row}, {Col})\t{symobl}\t{Value}";
    }

    public static bool IsNewLine(char c)
    {
        return c is '\r' or '\n';
    }
}