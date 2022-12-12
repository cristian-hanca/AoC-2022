static V TryGet<K, V>(this IDictionary<K, V> dict, K key, V def = default)
{
    if (dict == null || !dict.ContainsKey(key))
    {
        return def;
    }
    return dict[key];
}

class Node
{
    public int Id { get; }

    public int Value { get; set; }

    public List<Node> Children { get; } = new List<Node>();

    public Node(int id, int value, params Node[] n)
    {
        this.Id = id;
        this.Value = value;
        Children.AddRange(n);
    }
    
    public override bool Equals(object? obj)
    {
        return Equals(obj as Node);
    }

    public bool Equals(Node? other)
    {
        return other != null &&
               Id == other.Id;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Id);
    }
}

async Task<(Node Start, Node End, List<Node> Mins)> Parse(string path)
{
    var startNode = (Node?)null;
    var endNode = (Node?)null;
    var minNodes = new List<Node>();

    // Read Nodes
    var id = 0;
    var inp = new List<List<Node>>();
    await foreach (var line in File.ReadLinesAsync(path))
    {
        var ln = new List<Node>();
        foreach (var c in line)
        {
            var n = new Node(id++, c switch
            {
                'S' => -1,
                'E' => -2,
                _ => c - 'a'
            });
            if (n.Value == -1)
            {
                n.Value = 'a' - 'a';
                startNode = n;
            }
            if (n.Value == -2)
            {
                n.Value = 'z' - 'a';
                endNode = n;
            }
            if (n.Value == 0)
            {
                minNodes.Add(n);
            }
            ln.Add(n);
        }
        inp.Add(ln);
    }

    // Create Links.
    for (var x = 0; x < inp.Count; x++)
    {
        var line = inp[x];
        for (var y = 0; y < line.Count; y++)
        {
            var cell = line[y];

            // Left
            if (y > 0)
            {
                Check(inp[x][y - 1]);
            }
            // Right
            if (y < line.Count - 1)
            {
                Check(inp[x][y + 1]);
            }
            // Up
            if (x > 0)
            {
                Check(inp[x - 1][y]);
            }
            // Down
            if (x < inp.Count - 1)
            {
                Check(inp[x + 1][y]);
            }
            
            void Check(Node c)
            {
                if (c.Value <= cell.Value + 1)
                {
                    cell.Children.Add(c);
                }
            }
        }
    }

    if (startNode == null || endNode == null)
    {
        throw new Exception("wat");
    }
    return (startNode, endNode, minNodes);
}

List<Node> Traverse(Node start, Node end)
{
    var queue = new PriorityQueue<Node, int>();
    var path = new Dictionary<Node, Node>();
    var score = new Dictionary<Node, int>();

    queue.Enqueue(start, start.Value);
    score[start] = 0;
    
    while (queue.Count != 0)
    {
        var current = queue.Dequeue();
        if (current == end)
        {
            return CreatePath();
        }

        var s = score.TryGet(current, int.MaxValue - 2);
        foreach (var n in current.Children)
        {
            var newScore = s + 1;
            if (newScore < score.TryGet(n, int.MaxValue - 2))
            {
                path[n] = current;
                score[n] = newScore;
                queue.Enqueue(n, s + n.Value);
            }
        }
    }
    return new List<Node>();

    List<Node> CreatePath()
    {
        var res = new List<Node>() { end };
        var crt = end;
        while (path.ContainsKey(crt))
        {
            crt = path[crt];
            res.Insert(0, crt);
        }
        return res;
    }
}

// Common
var g = await Parse("input");

// Part 1
Console.WriteLine(Traverse(g.Start, g.End).Count - 1);

// Part 2
var r = new List<int>();
foreach (var n in g.Mins)
{
    var p = Traverse(n, g.End);
    if (p.Any())
    {
        r.Add(p.Count - 1);
    }
}
Console.WriteLine(r.Min());