namespace Tyapik;

public class Semantic
{
    public static void Check(Node tree, List<string>? variables = null)
    {
        if (tree.childrens.Count == 0)
            return;
        
        variables = variables == null ? new List<string>() : new List<string>(variables);
        foreach (var node in tree.childrens)
        {
            Check(node, variables);
            if (node.pattern == Parser.MODIFICATION && !variables.Contains(node.childrens[0].value))
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