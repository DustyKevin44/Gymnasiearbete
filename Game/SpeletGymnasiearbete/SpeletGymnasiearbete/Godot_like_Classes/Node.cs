using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
namespace SpeletGymnasiearbete;
#nullable enable

public class Node
{
    private Node? _parent; // if null Node is the root node
    private List<Node> _children;
    public override String ToString() { return this.GetType().Name; }

    public Func<GameTime, bool> _process = (delta) => true;
    public void Update(GameTime gameTime) {
        _process(gameTime);
    }

    public Node(List<Node>? children = null)
    { _children = (children is null) ? new() : [.. children]; }

    public void add_child(Node node) {
        _children.Add(node);
        node._parent = this;
    }

    public IReadOnlyList<Node> get_children() {
        return _children.AsReadOnly<Node>();
    }

    public void Update_children(GameTime deltaTime) {
        foreach (Node child in _children)
        {
            child.Update(deltaTime);
            child.Update_children(deltaTime);
        }
    }

    public IReadOnlyList<Node> get_siblings()
    {
        if (get_parent() is Node parent) { return parent.get_children(); }
        return new List<Node>();
    }
    
    public List<T> get_children_of<T>()
    {
        List<T> childrenOfT = new List<T>();
        foreach (Node child in _children)
        {
            if (child is T Tchild) { childrenOfT.Add(Tchild); }
        }
        return childrenOfT;
    }

    public List<T> get_descendants_of<T>()
    {
        List<T> descendantsOfT = new List<T>();
        foreach (Node child in _children)
        {
            if (child is T Tchild) { descendantsOfT.Add(Tchild); }
            descendantsOfT.AddRange(child.get_descendants_of<T>());
        }
        return descendantsOfT;
    }

    public Node? get_parent() { return _parent; }

    public Node get_root()
    {
        Node root = this;
        while (root.get_parent() is Node parent) {
            root = parent;
        }
        return root;
    }

    public void PrintTree(string nl = "|", string indent = "---")
    {
        Console.Write("ROOT");
        Node root = get_root();
        root.PrintBranch();
    }

    public void PrintBranch(string nl = "|", string indent = "---", int depth = 0)
    {
        string ls = " " + ((depth == 0) ? "" : nl);
        string ind = string.Concat(Enumerable.Repeat(indent, depth));
        Console.WriteLine(ls + ind + GetType().Name);
        foreach(Node child in _children)
        {
            child.PrintBranch(nl, indent);
        }
    }
}
