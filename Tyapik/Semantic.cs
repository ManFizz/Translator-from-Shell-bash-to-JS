namespace Tyapik;

public static class Semantic
{
    public static void Check(Node tree, List<string>? variables = null)
    {
        if (tree.childrens.Count == 0)
            return;
        
        variables = variables == null ? new List<string>() : new List<string>(variables);
        foreach (var node in tree.childrens)
        {
            if(node.pattern is Parser.DEFCONSTRUCTION or Parser.MODIFICATION)
                Check(node.childrens[1], variables);
            else 
                Check(node, variables);
            
            if (node.pattern is Parser.MODIFICATION or Parser.DEFCONSTRUCTION && !variables.Contains(node.childrens[0].value))
                variables.Add(node.childrens[0].value); //add identifier
            
            if (node.pattern == Parser.IDENTIFIER 
                && !Lexem.KEYWORDS.ContainsKey(node.value) 
                && !variables.Contains(node.value) 
                && !int.TryParse(node.value, out _)
                )
                throw new Exception($"Semantic error: Undefined varable - \"{node.value}\"");
        }
    }
}