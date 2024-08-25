using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using Microsoft.Xna.Framework;
namespace SpeletGymnasiearbete;
#nullable enable

public class Node
{
    private Node? _parent; // if null Node is the root node
    private List<Node> _children = new List<Node>();

    public System.Func<GameTime, int>? Script;

    public override System.String ToString() { return this.GetType().Name; }

    private void Update(GameTime deltaTime) {
        if (Script is not null) {
            int ErrCode = Script(deltaTime);
            if (ErrCode != 0) { System.Console.WriteLine("Script Failed: " + ErrCode); }
        }
    }

    public void add_child(Node node) {
        _children.Add(node);
        node._parent = this;
    }

    public List<Node> get_children() {
        return _children;
    }

    public void Update_children(GameTime deltaTime) {
        foreach (Node child in _children)
        {
            child.Update(deltaTime);
            child.Update_children(deltaTime);
        }
    }

    public List<Node> get_siblings()
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
}
