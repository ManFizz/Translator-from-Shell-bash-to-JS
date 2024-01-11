using System.Diagnostics.CodeAnalysis;

namespace Tyapik;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class Lexer
{
    private readonly StreamReader _file;
    public int State = Lexem.NONE;
    private char _currentChar;
    public int Row, Col;
    public string Value = "\0";

    public Lexer(string code) //parametr "c" for different constructors
    {
        var path = Path.GetTempFileName();
        var file = new StreamWriter(path);
        file.Write(code);
        file.Close();
        
        _file = new StreamReader(path);
    }
    public Lexer(StreamReader sr)
    {
        _file = sr;
    }
    
    private void Error(string message)
    {
        throw new Exception($"Lexer error: {message} in position ({Row}, {Col})");
    }
    

    private void GetNextChar()
    {
        if (Lexem.IsNewLine(_currentChar))
        {
            Row++;
            Col = 0;
        }
        _currentChar = (char)_file.Read();
        Col++;
    }

    public Lexem GetNextToken()
    {
        State = Lexem.NONE;
        Value = "\0";
        while (State is Lexem.NONE or Lexem.NEWLINE2 or Lexem.SEMICOLON)
        {
            if (_currentChar == '\0')
                GetNextChar();

            if (_currentChar.ToString().Length == 0 || _currentChar == (char)65535)
                State = Lexem.EOF;
            
            else switch (_currentChar)
            {
                case '#': //Comment
                {
                    while (!Lexem.IsNewLine(_currentChar) && _currentChar.ToString().Length > 0 && _currentChar != (char)65535)
                        GetNextChar();

                    if (Lexem.IsNewLine(_currentChar)) //For fix win newline
                        GetNextChar();
                    break;
                }
                case ' ': //Whitespaces
                {
                    var tabulation = "";
                    var col = Col;
                    while (_currentChar == ' ')
                    {
                        tabulation += _currentChar;
                        GetNextChar();
                        if (tabulation.Length != Lexem.n_tabs || col != 1)
                            continue;
                            
                        State = Lexem.TABULATION;
                        Value = tabulation;
                    }
                    if (tabulation.Length != Lexem.n_tabs && tabulation.Length > 1 && col == 1) Error("Incorrect indent");

                    break;
                }
                case '\t': //Tabulation
                {
                    State = Lexem.TABULATION;
                    Value = _currentChar.ToString();
                    GetNextChar();

                    break;
                }
                case '\'':
                {
                    State = Lexem.STRING;
                    Value = "";
                    GetNextChar();
                    while (_currentChar != '\'')
                    {
                        Value += _currentChar;
                        GetNextChar();
                    }
                    GetNextChar();
                    break;
                }
                case '"':
                {
                    State = Lexem.STRING;
                    Value = "";
                    GetNextChar();
                    while (_currentChar != '"')
                    {
                        Value += _currentChar;
                        GetNextChar();
                    }
                    GetNextChar();
                    break;
                }
                default:
                {
                    if (Lexem.SYMBOLS.ContainsKey(_currentChar.ToString()))
                    {
                        State = Lexem.SYMBOLS[_currentChar.ToString()];
                        Value = _currentChar.ToString();
                        GetNextChar();
                    }
                    else if (char.IsDigit(_currentChar))
                    {
                        var number = 0;
                        while (char.IsDigit(_currentChar))
                        {
                            number = number * 10 + int.Parse(_currentChar.ToString());
                            GetNextChar();
                        }
                        if (char.IsLetter(_currentChar) || _currentChar == '_')
                            Error("Invalid identifier");
                        if (_currentChar == '.')
                        {
                            var numberStr = number.ToString();
                            numberStr += '.';
                            GetNextChar();
                            while (char.IsDigit(_currentChar))
                            {
                                numberStr += _currentChar;
                                GetNextChar();
                            }
                            State = Lexem.FLOATNUMBER;
                            Value = numberStr;
                        }
                        else
                        {
                            State = Lexem.INTNUMBER;
                            Value = number.ToString();
                        }
                    }
                    else if (char.IsLetter(_currentChar) || _currentChar == '_')
                    {
                        var identifier = "";
                        while (char.IsLetter(_currentChar) || char.IsDigit(_currentChar) || _currentChar == '_')
                        {
                            identifier += _currentChar;
                            GetNextChar();
                        }
                        if (Lexem.KEYWORDS.ContainsKey(identifier))
                        {
                            State = Lexem.KEYWORDS[identifier];
                            Value = identifier;
                        }
                        else if (Lexem.RESERVEDNAMES.ContainsKey(identifier))
                        {
                            State = Lexem.RESERVEDNAMES[identifier];
                            Value = identifier;
                        }
                        else
                        {
                            State = Lexem.IDENTIFIER;
                            Value = identifier;
                        }
                    }
                    else
                    {
                        Error($"Unexpected symbol: {_currentChar}");
                    }

                    break;
                }
            }
        }

        return new Lexem(Row, Col, State, Value);
    }
}