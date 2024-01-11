using Tyapik;
// ReSharper disable ArrangeTrailingCommaInMultilineLists

try
{
    var tests = new List<string>
    {
        "break",
        "arifmetic", //1
        "list", //2
        "def",
    };
    const int testNumber = 1;
    var file = tests[testNumber] + ".txt";
    var l = new Lexer( new StreamReader(file));
    var result = "\0";
    var state = -1;
    while (state != Lexem.EOF)
    {
        result += l.GetNextToken() + "\n";
        state = l.State;
    }

    Console.WriteLine(result);
    l = new Lexer(new StreamReader(file));

    var parser = new Parser(l);
    var resultParse = parser.Parse();
    Console.WriteLine(resultParse.ShowStr());

    var code = CodeGenerator.Get(resultParse);
    Console.WriteLine(code);
}
catch (Exception e) {
    Console.WriteLine(e.Message);
}
