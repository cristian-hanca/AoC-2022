abstract class Node
{
    public ChildrenNode? Parent { get; }
    public String Name { get; }
    public abstract long GetSize();
    
    protected Node(ChildrenNode? parent, string name)
    {
        Parent = parent;
        Name = name;
    }
}

abstract class ChildrenNode : Node
{
    public List<Node> Children { get; } = new List<Node>();
    public override long GetSize() => Children.Select(x => x.GetSize()).Sum();
    
    protected ChildrenNode(ChildrenNode? parent, string name) : base(parent, name)
    {
    
    }
}

class RootNode : ChildrenNode 
{
    public RootNode() : base(null, "/")
    {
    
    }
}

class DirectoryNode : ChildrenNode
{
    public DirectoryNode(ChildrenNode parent, string name) : base(parent, name)
    {
    
    }
}

class FileNode : Node
{
    public long Size { get; }
    public override long GetSize() => Size;
    
    public FileNode(ChildrenNode parent, string name, long size) : base(parent, name)
    {
        Size = size;
    }
}


static async Task<RootNode> Process(string filePath)
{
    var root = new RootNode();
    var crtNode = (ChildrenNode) root;

    await foreach (var line in File.ReadLinesAsync(filePath))
    {
        var components = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
        if (components[0] == "$")
        {
            if (components[1] == "cd")
            {
                if (components[2] == "/")
                {
                    crtNode = root;
                }
                else if (components[2] == "..")
                {
                    crtNode = crtNode.Parent ?? root;
                }
                else
                {
                    crtNode = (ChildrenNode)crtNode.Children.First(x => x.Name == components[2]);
                }
            }
            else if (components[1] == "ls")
            {
                // De-Nada
            }
        }
        else if (components[0] == "dir")
        {
            crtNode.Children.Add(new DirectoryNode(crtNode, components[1]));
        }
        else
        {
            crtNode.Children.Add(new FileNode(crtNode, components[1], long.Parse(components[0])));
        }
    }
    
    return root;
}

// Common
var root = await Process("input");

// Part 1
long Traverse1(ChildrenNode node)
{
    var res = 0L;
    
    var size = node.GetSize();
    if (size <= 100000)
    {
        res += size;
    }
    
    foreach (var child in node.Children.OfType<DirectoryNode>())
    {
        res += Traverse1(child);
    }
    
    return res;
}
Console.WriteLine(Traverse1(root));

// Part 2
long Traverse2(ChildrenNode node, long needed, long crtMin)
{
    var res = crtMin;
    
    var size = node.GetSize();
    if (size >= needed && size < res)
    {
        res = size;
    }
    
    foreach (var child in node.Children.OfType<DirectoryNode>())
    {
        res = Traverse2(child, needed, res);
    }
    
    return res;
}

var currentSize = root.GetSize();
var currentFreeSize = 70000000L - currentSize;
var neededFreeSize = 30000000L - currentFreeSize;
Console.WriteLine(Traverse2(root, neededFreeSize, long.MaxValue));