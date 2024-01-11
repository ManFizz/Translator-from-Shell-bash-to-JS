using Tyapik;

namespace TyapikTester;

[TestClass]
public class Tester
{
    [TestMethod]
    public void Arifmetic_NoParameter_Parser()
    {
        const string code = "let f=1-+2";
        
        var actual = "";
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        try {
            var resultParse = parser.Parse();
            Console.WriteLine(resultParse.ShowStr());
        } catch (Exception e) {
            actual = e.Message;
        }
        Assert.AreNotEqual(actual, "");
    }
    
    [TestMethod]
    public void Arifmetic_ValidExpresssion_Parser()
    {
        const string code = "let f=1-2+2";
        
        var actual = "";
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        try {
            var resultParse = parser.Parse();
            Console.WriteLine(resultParse.ShowStr());
        } catch (Exception e) {
            actual = e.Message;
        }
        Assert.AreEqual("", actual);
    }
    
    [TestMethod]
    public void List_InnerNoise_Parser()
    {
        const string code = "A=(2 4 5~ 6 4);";
        
        var actual = "";
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        try {
            var resultParse = parser.Parse();
            Console.WriteLine(resultParse.ShowStr());
        } catch (Exception e) {
            actual = e.Message;
        }
        Assert.AreNotEqual(actual, "");
    }
    
    [TestMethod]
    public void List_ValidExpresssion_Parser()
    {
        const string code = "A=(2 4 5 6 4);";
        
        var actual = "";
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        try {
            var resultParse = parser.Parse();
            Console.WriteLine(resultParse.ShowStr());
        } catch (Exception e) {
            actual = e.Message;
        }
        Assert.AreEqual("", actual);
    }
    
    [TestMethod]
    public void CallFunction_ValidExpresssion_Parser()
    {
        const string code = "echo $A;";
        
        var actual = "";
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        try {
            var resultParse = parser.Parse();
            Console.WriteLine(resultParse.ShowStr());
        } catch (Exception e) {
            actual = e.Message;
        }
        Assert.AreEqual("", actual);
    }
    
    [TestMethod]
    public void DeclarationFunction_ValidExpresssion_Parser()
    {
        const string code = @"function f() {
	a=3;
	echo $1+$a;
	let c=$1+$a;
	echo $c;
};
f 2;";
        
        var actual = "";
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        try {
            var resultParse = parser.Parse();
            Console.WriteLine(resultParse.ShowStr());
        } catch (Exception e) {
            actual = e.Message;
        }
        Assert.AreEqual("", actual);
    }
    
    [TestMethod]
    public void Break_Parser()
    {
        const string code = "а как вообще 233 =+ sadв ж/\n\ngfb \n rtyebtressertg=g5-b32gfv4\\-=98-	213авё12в231-vbc./dxRWEFGVCawWA+_ \n -./.CXZW\\/78984```21243!#$^&&^*&)(_+-=\\\\\\\r\t\\y\\u\\h\\fgd\r";
        
        var actual = "";
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        try {
            var resultParse = parser.Parse();
            Console.WriteLine(resultParse.ShowStr());
        } catch (Exception e) {
            actual = e.Message;
        }
        Assert.AreNotEqual("", actual);
    }
    
    
    [TestMethod]
    public void UndefFactparam_Semantic()
    {
        var codes = new[]
        {
            "echo $A;",
            "let c=$1+$c",
            "A=( $d )"
        };
        
        var actual = "";
        foreach (var code in codes)
        {
            var lexer = new Lexer(code);
            var parser = new Parser(lexer);
            try {
                var resultParse = parser.Parse();
                Console.WriteLine(resultParse.ShowStr());
                Semantic.Check(resultParse);
            } catch (Exception e) {
                actual = e.Message;
            }
            Assert.AreNotEqual("", actual);
        }
    }
    
    [TestMethod]
    public void Function_CodeGenerator()
    {
        const string code = @"function f() {
	a=3;
	echo $1+$a;
	let c=$1+$a;
	echo $c;
};
f 2;";
        var actual = "";
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        var resultParse = parser.Parse();
        try {
            Console.WriteLine(CodeGenerator.Get(resultParse));
        } catch (Exception e) {
            actual = e.Message;
        }
        Assert.AreEqual("", actual);
    }
    
    
    [TestMethod]
    public void List_CodeGenerator()
    {
        const string code = "A=(2 4 5 6 4);";
        var actual = "";
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        var resultParse = parser.Parse();
        try {
            Console.WriteLine(CodeGenerator.Get(resultParse));
        } catch (Exception e) {
            actual = e.Message;
        }
        Assert.AreEqual("", actual);
    }
    
    
    [TestMethod]
    public void Arifmmetic_CodeGenerator()
    {
        const string code = "let f=1-2+2";
        var actual = "";
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        var resultParse = parser.Parse();
        try {
            Console.WriteLine(CodeGenerator.Get(resultParse));
        } catch (Exception e) {
            actual = e.Message;
        }
        Assert.AreEqual("", actual);
    }
    
    
    [TestMethod]
    public void Function_Optimizer()
    {
        const string code = @"function m() {
	l=4;
};
function f() {
    b=4;
	a=3;
	echo $1+$a;
	let c=$1+$a;
	echo $c;
};
f 2;";
        var actual = "";
        var lexer = new Lexer(code);
        var parser = new Parser(lexer);
        var resultParse = parser.Parse();
        try {
            Optimizer.Optimize(resultParse);
        } catch (Exception e) {
            actual = e.Message;
        }

        var result = CodeGenerator.Get(resultParse);
        Console.WriteLine(result);
        Assert.AreEqual("function f() \n" +
                        "{\n" +
                        "\tvar a = 3;\n" +
                        "\tconsole.log(arguments[0] + a);\n" +
                        "\tvar c = arguments[0] + a;\n" +
                        "\tconsole.log(c);\n" +
                        "}\n" +
                        "f(2);\n", result);
        Assert.AreEqual("", actual);
    }
}