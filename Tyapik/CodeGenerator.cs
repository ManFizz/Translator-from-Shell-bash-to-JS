namespace Tyapik;

public static class CodeGenerator
{
    private static string _outPut = "";

    private static void AppendCode(string code) {
        _outPut += code;
    }

    private static void Error(string error = "") {
        throw new Exception("CodeGenerator error:" + error);
    }
    
    public static string Get(Node tree)
    {
        try {
            Semantic.Check(tree);
        } catch (Exception e) {
            Console.WriteLine(e.Message);
            //Error();
            return "";
        }

        _outPut = "";
        Switcher(tree, "");
        return _outPut;
    }
    
    // ReSharper disable TailRecursiveCall
    private static void Switcher(Node node, string prefix)
    {
        switch (node.pattern) 
        {
            case Parser.PROGRAM:
            {
                Switcher(node.childrens[0], prefix); //nothing to do
                break;
            }
            case Parser.LIST:
            {
                AppendCode("[");
                for (var i = 0; i < node.childrens.Count; i++)
                {
                    Switcher(node.childrens[i], prefix);
                    if(i != node.childrens.Count - 1)
                        AppendCode(", ");
                }
                AppendCode("]");
                break;
            }
            case Parser.DEFCONSTRUCTION:
            {
                AppendCode($"function {node.childrens[0].value}() ");
                Switcher(node.childrens[1], prefix);
                break;
            }
            case Parser.BLOCK:
            {
                prefix += "\t";
                AppendCode("\n{\n" + prefix);
                foreach (var blockNode in node.childrens)
                    Switcher(blockNode, prefix);
                if (_outPut[^1] == '\t')
                    _outPut = _outPut.Remove(_outPut.Length - 1);
                AppendCode("}\n");
                break;
            }
            case Parser.INTNUMBER:
            case Parser.FLOATNUMBER:
            case Parser.STRING:
            {
                AppendCode(node.value);
                break;
            }
            case Parser.MODIFICATION:
            {
                AppendCode($"var {node.childrens[0].value} = ");
                Switcher(node.childrens[1], prefix);
                AppendCode(";\n" + prefix);
                break;
            }
            case Parser.FUNCTION:
            {
                if(node.childrens[0].value.Equals("echo", StringComparison.InvariantCultureIgnoreCase))
                    AppendCode("console.log(");
                else
                    AppendCode($"{node.childrens[0].value}(");
                var fact = node.childrens[1];//FACTPARAMETRS
                for (var i = 0; i < fact.childrens.Count; i++)
                {
                    Switcher(fact.childrens[i], prefix);
                    if(i != fact.childrens.Count - 1)
                        AppendCode(", ");
                }

                AppendCode(");\n" + prefix);
                break;
            }
            case Parser.ADD:
            {
                Switcher(node.childrens[0], prefix);
                AppendCode(" + ");
                Switcher(node.childrens[1], prefix);
                break;
            }
            case Parser.SUB:
            {
                Switcher(node.childrens[0], prefix);
                AppendCode(" - ");
                Switcher(node.childrens[1], prefix);
                break;
            }
            case Parser.MUL:
            {
                Switcher(node.childrens[0], prefix);
                AppendCode(" * ");
                Switcher(node.childrens[1], prefix);
                break;
            }
            case Parser.DIV:
            {
                Switcher(node.childrens[0], prefix);
                AppendCode(" / ");
                Switcher(node.childrens[1], prefix);
                break;
            }
            case Parser.FACTPARAMETERS:
            {
                Error("FACTPARAMETERS must be unreachable");
                break;
            }
            case Parser.IDENTIFIER:
            {
                if(int.TryParse(node.value, out var number))
                    AppendCode($"arguments[{number - 1}]");
                else
                    AppendCode(node.value);
                break;
            }
            case Parser.STATEMENT:
            case Parser.SET:
            case Parser.LESS:
            case Parser.GREATER:
            case Parser.REM:
            case Parser.IFCONSTRUCTION:
            case Parser.ELIFCONSTRUCTION:
            case Parser.ELSECONSTRUCTION:
            case Parser.WHILECONSTRUCTION:
            case Parser.FORCONSTRUCTION:
            case Parser.RETURN:
            case Parser.LISTELEMENT:
            case Parser.SEMICOLON:
            case Parser.LET:
            case Parser.PARAM:
            case Parser.FORMULA:
            {
                throw new NotImplementedException(Parser.PRESENTATION[node.pattern] + " is not implemented");
            }
        }
    }
}