//#define PRINT_DEBUG

namespace Tyapik;

public static class Optimizer
{
    private static void Log(string msg)
    {
        #if PRINT_DEBUG
        Console.WriteLine(msg);
        #endif
    }
    
    public static void Optimize(Node tree)
    {
        Log("Start optimize");
        var usedVariablesAndFunctions = new HashSet<string>();
        
        Log("in OptimizeNode");
        OptimizeNode(tree, usedVariablesAndFunctions);
        Log("end OptimizeNode");
        
        Log("Used variables and functions:");
        usedVariablesAndFunctions.ToList().ForEach(Log);
        
        Log("in RemoveUnusedVariablesAndFunctions");
        RemoveUnusedVariablesAndFunctions(tree, usedVariablesAndFunctions);
        Log("end RemoveUnusedVariablesAndFunctions");
        
        Log("End optimize");
    }
    
    private static void OptimizeNode(Node node, ISet<string> usedVariablesAndFunctions)
    {
        //skip DEFCONSTRUCTION, MODIFICATION - IDENTIFIER
        if (node.pattern is Parser.MODIFICATION or Parser.DEFCONSTRUCTION)
        {
            OptimizeNode(node.childrens[1], usedVariablesAndFunctions);
            return;
        }

        if (node.pattern == Parser.IDENTIFIER) //variables and functions
        {
            Log($"found {node.value}");
            usedVariablesAndFunctions.Add(node.value);
            return;
        }

        foreach (var child in node.childrens) 
            OptimizeNode(child, usedVariablesAndFunctions);
    }

    private static void RemoveUnusedVariablesAndFunctions(Node node, IReadOnlySet<string> usedVariablesAndFunctions)
    {
        for (var i = node.childrens.Count - 1; i >= 0; i--)
        {
            var child = node.childrens[i];

            if (child.pattern is Parser.MODIFICATION or Parser.DEFCONSTRUCTION)
            {
                if (usedVariablesAndFunctions.Contains(child.childrens[0].value))
                {
                    Log($"Is used {child.GetPattern()} with id {child.childrens[0].value}");
                    RemoveUnusedVariablesAndFunctions(child, usedVariablesAndFunctions);
                }
                else
                {
                    Log($"Remove {child.GetPattern()} with id {child.childrens[0].value}");
                    node.childrens.RemoveAt(i);
                }
            }
            else
            {
                RemoveUnusedVariablesAndFunctions(child, usedVariablesAndFunctions);
            }
        }
    }

}
