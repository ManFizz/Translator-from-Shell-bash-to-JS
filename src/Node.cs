using System.Diagnostics.CodeAnalysis;

namespace Tyapik;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class Node {
    public readonly int pattern;
    public readonly string value;
    public readonly List<Node> childrens;

    public Node(int pattern, string value = "", List<Node>? childrens = null)
    {
        this.pattern = pattern;
        this.value = value;
        this.childrens = childrens ?? new List<Node>();
    }

    public void Show(int level = 0)
    {
        Console.WriteLine($"{Parser.PRESENTATION[pattern]} : {value}");

        foreach (var child in childrens)
        {
            Console.Write(string.Concat(Enumerable.Repeat("|   ", level)));
            Console.Write("|+-");
            child.Show(child.childrens.Count > 0 ? level + 1 : level);
        }
    }

    public string ShowStr(int level = 0)
    {
        var str = $"{Parser.PRESENTATION[pattern]} : {value}\n";
        foreach (var child in childrens)
        {
            str += string.Concat(Enumerable.Repeat("|   ", level));
            str += "|+-";
            str += child.ShowStr(child.childrens.Count > 0 ? level + 1 : level);
        }
        return str;
    }

    public override string ToString()
    {
        return ShowStr();
    }

    public string GetPattern()
    {
        return Parser.PRESENTATION[pattern];
    }
}