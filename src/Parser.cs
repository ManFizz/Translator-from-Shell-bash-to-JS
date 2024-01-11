using System.Diagnostics.CodeAnalysis;
// ReSharper disable ArrangeTrailingCommaInMultilineLists

namespace Tyapik;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Parser
{
    #region Definitions
    // Patterns
    public const int PROGRAM = 0;
    public const int LIST = 1;
    public const int STATEMENT = 2;
    public const int MODIFICATION = 3;
    public const int FORMULA = 4;
    public const int FUNCTION = 5;
    public const int IDENTIFIER = 6;
    public const int FORMALPARAMETERS = 7;
    public const int FACTPARAMETERS = 8;
    public const int ADD = 9;
    public const int SUB = 10;
    public const int SET = 11;
    public const int LESS = 12;
    public const int GREATER = 13;
    public const int MUL = 14;
    public const int DIV = 15;
    public const int REM = 16;
    public const int IFCONSTRUCTION = 17;
    public const int ELIFCONSTRUCTION = 18;
    public const int ELSECONSTRUCTION = 19;
    public const int WHILECONSTRUCTION = 20;
    public const int FORCONSTRUCTION = 21;
    public const int DEFCONSTRUCTION = 22;
    public const int BLOCK = 23;
    public const int INTNUMBER = 24;
    public const int FLOATNUMBER = 25;
    public const int STRING = 26;
    public const int RETURN = 27;
    public const int LISTELEMENT = 28;
    public const int SEMICOLON = 29;
    public const int LET = 30;
    public const int PARAM = 31;

    public static Dictionary<int, string> PRESENTATION = new Dictionary<int, string>()
    {
        {PROGRAM, "PROGRAM"},
        {LIST, "LIST"},
        {STATEMENT, "STATEMENT"},
        {MODIFICATION, "MODIFICATION"},
        {FORMULA, "FORMULA"},
        {FUNCTION, "FUNCTION"},
        {IDENTIFIER, "IDENTIFIER"},
        {FORMALPARAMETERS, "FORMALPARAMETERS"},
        {FACTPARAMETERS, "FACTPARAMETERS"},
        {ADD, "ADD"},
        {SUB, "SUB"},
        {SET, "SET"},
        {LESS, "LESS"},
        {GREATER, "GREATER"},
        {MUL, "MUL"},
        {DIV, "DIV"},
        {REM, "REM"},
        {IFCONSTRUCTION, "IFCONSTRUCTION"},
        {ELIFCONSTRUCTION, "ELIFCONSTRUCTION"},
        {ELSECONSTRUCTION, "ELSECONSTRUCTION"},
        {WHILECONSTRUCTION, "WHILECONSTRUCTION"},
        {FORCONSTRUCTION, "FORCONSTRUCTION"},
        {DEFCONSTRUCTION, "DEFCONSTRUCTION"},
        {BLOCK, "BLOCK"},
        {INTNUMBER, "INTNUMBER"},
        {FLOATNUMBER, "FLOATNUMBER"},
        {STRING, "STRING"},
        {RETURN, "RETURN"},
        {LISTELEMENT, "LISTELEMENT"},
        {SEMICOLON, "SEMICOLON"},
        {LET, "LET"},
        {PARAM, "PARAM"},
    };
 
    #endregion

    private readonly Lexer _lexer;

    public Parser(Lexer lexer)
    {
        _lexer = lexer;
    }

    public void Error(string message)
    {
        throw new Exception($"Parser error: {message} in position {_lexer.Row}, {_lexer.Col}");
    }

    public Node List()
    {
        var formulas = new List<Node>();
        if (_lexer.State == Lexem.LRBRACKET) _lexer.GetNextToken();

        while (_lexer.State != Lexem.RRBRACKET)
        {
            formulas.Add(Formula());
            if (_lexer.State == Lexem.WHITESPACE)
                _lexer.GetNextToken();
        }

        return new Node(LIST, childrens: formulas);
    }

    public Node Term()
    {
        switch (_lexer.State)
        {
            case Lexem.LRBRACKET: {
                var node = List();
                if (_lexer.State != Lexem.RRBRACKET) 
                    Error("Expected ')'");

                _lexer.GetNextToken();
                return node;
            }

            case Lexem.DOLLAR:
            {
                _lexer.GetNextToken();
                if (_lexer.State == Lexem.INTNUMBER)
                    _lexer.State = Lexem.IDENTIFIER;
                
                return Term();
                //return new Node(FUNCTION, childrens: new List<Node> {identifier, factparameters});
            }
            case Lexem.IDENTIFIER:
            {
                var identifier = new Node(IDENTIFIER, _lexer.Value);
                _lexer.GetNextToken();

                switch (_lexer.State)
                {
                    case Lexem.LSBRACKET: // list element
                        _lexer.GetNextToken();
                        var index = Formula();
                        if (_lexer.State != Lexem.RSBRACKET) 
                            Error("Expected ']'");

                        _lexer.GetNextToken();
                        return new Node(LISTELEMENT, childrens: new List<Node> {identifier, index});

                    default: // identifier
                        return identifier;
                }
            }

            case Lexem.INTNUMBER:
                var intNode = new Node(INTNUMBER, _lexer.Value);
                _lexer.GetNextToken();
                return intNode;

            case Lexem.FLOATNUMBER:
                var floatNode = new Node(FLOATNUMBER, _lexer.Value);
                _lexer.GetNextToken();
                return floatNode;

            case Lexem.STRING:
                var stringNode = new Node(STRING, _lexer.Value);
                _lexer.GetNextToken();
                return stringNode;

            /*
             case Lexem.LRBRACKET:
                _lexer.GetNextToken();
                var formula = Formula();
                if (_lexer.State != Lexem.RRBRACKET) 
                    Error("Expected ')'");

                _lexer.GetNextToken();
                return formula;
            */
            default:
                Error("Unexpected symbol");
                return null!; // This line will not be reached, added to satisfy the compiler
        }
    }

    public Node Sum()
    {
        var left = Product();
        switch (_lexer.State)
        {
            case Lexem.PLUS:
            {
                _lexer.GetNextToken();
                return new Node(ADD, childrens: new List<Node> {left, Sum()});
            }

            case Lexem.MINUS: // noncommutative operation (1 - 2 - 3 и 1 + 1 - 1 + 1)
            {
                while (_lexer.State is Lexem.MINUS or Lexem.PLUS)
                {
                    if (_lexer.State == Lexem.MINUS)
                    {
                        _lexer.GetNextToken();
                        var right = Product();
                        left = new Node(SUB, childrens: new List<Node> {left, right});
                    }
                    else
                    {
                        _lexer.GetNextToken();
                        var right = Product();
                        left = new Node(ADD, childrens: new List<Node> {left, right});
                    }
                }

                break;
            }
        }
        return left;
    }

    public Node Product()
    {
        var left = Term();

        switch (_lexer.State)
        {
            case Lexem.MULTIPLY:
                _lexer.GetNextToken();
                return new Node(MUL, childrens: new List<Node> {left, Product()});

            case Lexem.DIVISION:
            case Lexem.REMAINDER: // noncommutative operation with same priority
            {
                while (_lexer.State is Lexem.DIVISION or Lexem.MULTIPLY or Lexem.REMAINDER)
                {
                    switch (_lexer.State)
                    {
                        case Lexem.DIVISION:
                        {
                            _lexer.GetNextToken();
                            var right = Term();
                            left = new Node(DIV, childrens: new List<Node> {left, right});
                            break;
                        }

                        case Lexem.MULTIPLY:
                        {
                            _lexer.GetNextToken();
                            var right = Term();
                            left = new Node(MUL, childrens: new List<Node> {left, right});
                            break;
                        }

                        case Lexem.REMAINDER:
                        {
                            _lexer.GetNextToken();
                            var right = Term();
                            left = new Node(REM, childrens: new List<Node> {left, right});
                            break;
                        }
                    }
                }

                break;
            }
        }
        return left;
    }

    public Node Formula()
    {
        var left = Sum();
        switch (_lexer.State)
        {
            case Lexem.LESS: {
                _lexer.GetNextToken();
                return new Node(LESS, childrens: new List<Node> {left, Formula()});
            }

            case Lexem.GREATER: {
                _lexer.GetNextToken();
                return new Node(GREATER, childrens: new List<Node> {left, Formula()});
            }
        }
        return left;
    }

    public Node FactParameters()
    {
        var formulas = new List<Node>();
        //_lexer.GetNextToken();

        while (_lexer.State != Lexem.SEMICOLON && _lexer.State != Lexem.NEWLINE1 && _lexer.State != Lexem.NEWLINE2 && _lexer.State != Lexem.EOF) 
        {
            formulas.Add(Formula());

            if (_lexer.State == Lexem.WHITESPACE) 
                _lexer.GetNextToken();
        }
        _lexer.GetNextToken();

        return new Node(FACTPARAMETERS, childrens: formulas);
    }

    public Node Block()
    {
        if (_lexer.State == Lexem.LBRACE)
        {
            var zeroblocks = new List<Node>();
            while (_lexer.State != Lexem.RBRACE) {
                _lexer.GetNextToken();
                while (_lexer.State is Lexem.NEWLINE1 or Lexem.NEWLINE2 or Lexem.TABULATION)
                    _lexer.GetNextToken();
                
                if (_lexer.State == Lexem.LBRACE)
                    zeroblocks.Add(Block());
                else
                    zeroblocks.Add(ZeroBlock());
            }
            _lexer.GetNextToken();
            while (_lexer.State is Lexem.NEWLINE1 or Lexem.NEWLINE2 or Lexem.TABULATION)
                _lexer.GetNextToken();

            return new Node(BLOCK, childrens: zeroblocks);
        }

        Error("Expected indent");
        return null!; // This line will not be reached, added to satisfy the compiler
    }

    public Node ZeroBlock()
    {
        switch (_lexer.State)
        {
            // IF pattern (if FORMULA: \n BLOCK)
            case Lexem.IF:
                _lexer.GetNextToken();
                var statement = Formula();
                if (_lexer.State == Lexem.COLON)
                {
                    _lexer.GetNextToken();
                    if (_lexer.State is Lexem.NEWLINE1 or Lexem.NEWLINE2)
                        return new Node(IFCONSTRUCTION, childrens: new List<Node> {statement, Block()});

                    Error("Expected new line");
                }
                else Error("Expected ':'");

                break;

            // ELIF pattern (elif FORMULA: \n BLOCK)
            case Lexem.ELIF:
                _lexer.GetNextToken();
                statement = Formula();
                if (_lexer.State == Lexem.COLON)
                {
                    _lexer.GetNextToken();
                    if (_lexer.State is Lexem.NEWLINE1 or Lexem.NEWLINE2)
                        return new Node(ELIFCONSTRUCTION, childrens: new List<Node> {statement, Block()});

                    Error("Expected new line");
                }
                else Error("Expected ':'");

                break;

            // ELSE pattern (else: \n BLOCK)
            case Lexem.ELSE:
                _lexer.GetNextToken();
                if (_lexer.State == Lexem.COLON)
                {
                    _lexer.GetNextToken();
                    if (_lexer.State is Lexem.NEWLINE1 or Lexem.NEWLINE2)
                        return new Node(ELSECONSTRUCTION, childrens: new List<Node> {Block()});

                    Error("Expected new line");
                }
                else Error("Expected ':'");

                break;

            // WHILE pattern (while FORMULA: \n BLOCK)
            case Lexem.WHILE:
                _lexer.GetNextToken();
                statement = Formula();
                if (_lexer.State == Lexem.COLON)
                {
                    _lexer.GetNextToken();
                    if (_lexer.State is Lexem.NEWLINE1 or Lexem.NEWLINE2)
                    {
                        _lexer.GetNextToken();
                        return new Node(WHILECONSTRUCTION, childrens: new List<Node> {statement, Block()});
                    }

                    Error("Expected new line");
                }
                else Error("Expected ':'");

                break;

            // FOR pattern (for ID in FORMULA: \n BLOCK)
            case Lexem.FOR:
            {
                _lexer.GetNextToken();
                if (_lexer.State == Lexem.IDENTIFIER)
                {
                    var identifier = new Node(IDENTIFIER, _lexer.Value);
                    _lexer.GetNextToken();
                    if (_lexer.State == Lexem.IN)
                    {
                        _lexer.GetNextToken();
                        var formula = Formula();
                        if (_lexer.State == Lexem.COLON)
                        {
                            _lexer.GetNextToken();
                            if (_lexer.State is Lexem.NEWLINE1 or Lexem.NEWLINE2)
                            {
                                _lexer.GetNextToken();
                                return new Node(FORCONSTRUCTION,
                                    childrens: new List<Node> {identifier, formula, Block()});
                            }

                            Error("Expected new line");
                        }
                        else
                        {
                            Error("Expected ':'");
                        }
                    }
                    else
                    {
                        Error("Expected 'in'");
                    }
                }
                else
                {
                    Error("Expected identifier");
                }

                break;
            }

            // FUNCTION pattern (def ID(FORMALPARAMETERS): \n BLOCK)
            case Lexem.FUNCTION:
            {
                _lexer.GetNextToken();
                if (_lexer.State == Lexem.IDENTIFIER)
                {
                    var identifier = new Node(IDENTIFIER, _lexer.Value);
                    _lexer.GetNextToken();
                    _lexer.GetNextToken();
                    _lexer.GetNextToken();
                    return new Node(DEFCONSTRUCTION, childrens: new List<Node> {identifier, Block()});
                }
                else Error("Expected function identifier");

                break;
            }

            case Lexem.DOLLAR:
            {
                _lexer.GetNextToken();
                if (_lexer.State == Lexem.INTNUMBER)
                    _lexer.State = Lexem.IDENTIFIER;
                
                return ZeroBlock();
            }
            case Lexem.LET:
            {
                _lexer.GetNextToken();
                return ZeroBlock();
            }
            // MODIFICATION (ID = FORMULA)
            case Lexem.IDENTIFIER:
            {
                var identifier = new Node(IDENTIFIER, _lexer.Value);
                _lexer.GetNextToken();
                
                if (_lexer.State == Lexem.SET) //
                {
                    _lexer.GetNextToken();
                    var formula = Formula();
                    if (_lexer.State is Lexem.NEWLINE1 or Lexem.NEWLINE2)
                        _lexer.GetNextToken(); // newline skip

                    return new Node(MODIFICATION, childrens: new List<Node> {identifier, formula});
                }

                var factparameters = FactParameters();
                return new Node(FUNCTION, childrens: new List<Node> {identifier, factparameters});

                break;
            }

            // RETURN for FUNCTION-construction
            case Lexem.RETURN:
                _lexer.GetNextToken();
                return new Node(RETURN, childrens: new List<Node> {Formula()});
            
            case Lexem.SEMICOLON:
                _lexer.GetNextToken();
                return new Node(SEMICOLON, childrens: new List<Node> {Formula()});
            case Lexem.TABULATION:
            {
                _lexer.GetNextToken();
                return ZeroBlock();
            }
            default:
                Error("Unexpected syntax");
                break;
        }

        return null!; // This line will not be reached, added to satisfy the compiler
    }

    public Node Program()
    {
        var zeroblocks = new List<Node>();
        while (_lexer.State != Lexem.EOF)
        {
            while (_lexer.State is Lexem.NEWLINE1 or Lexem.NEWLINE2) 
                _lexer.GetNextToken(); // skip empty lines
            if (_lexer.State == Lexem.LBRACE)
                zeroblocks.Add(Block());
            zeroblocks.Add(ZeroBlock());
        }

        return new Node(PROGRAM, childrens: zeroblocks);
    }

    public Node Parse()
    {
        _lexer.GetNextToken();
        return Program();
    }
}