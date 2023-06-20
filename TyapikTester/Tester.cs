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
        var lexer = new Lexer(code, true);
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
        var lexer = new Lexer(code, true);
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
        var lexer = new Lexer(code, true);
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
        var lexer = new Lexer(code, true);
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
        var lexer = new Lexer(code, true);
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
        const string code = "function f() { \n a=3; \n echo $1 + $a; \n let c=$1+$a; \n echo $c; \n }; \n f 2;";
        
        var actual = "";
        var lexer = new Lexer(code, true);
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
        var lexer = new Lexer(code, true);
        var parser = new Parser(lexer);
        try {
            var resultParse = parser.Parse();
            Console.WriteLine(resultParse.ShowStr());
        } catch (Exception e) {
            actual = e.Message;
        }
        Assert.AreNotEqual("", actual);
    }
}