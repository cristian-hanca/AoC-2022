interface IEntry : IComparable<IEntry>
{
}

class ListEntry : IEntry
{
    public List<IEntry> Values { get; } = new List<IEntry>();

    public ListEntry(params IEntry[] values)
    {
        Values.AddRange(values);
    }

    public int CompareTo(IEntry? other)
    {
        return other switch
        {
            IntEntry i => this.CompareTo(i.ToListEntry()),
            ListEntry l => CompareInternal(Values, l.Values),
            _ => 0,
        };
        
        static int CompareInternal(List<IEntry> a, List<IEntry> b)
        {
            var idx = 0;
            while (true)
            {
                var itmA = idx >= a.Count ? null : a[idx];
                var itmB = idx >= b.Count ? null : b[idx];
                
                // Both Lists are of the Same Size, and Ended.
                if (itmA == null && itmB == null)
                {
                    return 0;
                }
                
                // Left Ended, Right still going on.
                if (itmA == null)
                {
                    return -1;
                }
                
                // Right End, Left still going on.
                if (itmB == null)
                {
                    return 1;
                }
                
                var cmp = itmA.CompareTo(itmB);
                if (cmp == -1)
                {
                    return -1;
                } 
                if (cmp == 1)
                {
                    return 1;
                }
                idx++;
            }
        }
    }
    
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append("[");
        var first = true;
        foreach (var v in Values)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sb.Append(",");
            }
            sb.Append(v.ToString());
        }
        sb.Append("]");
        return sb.ToString();
    }
}

class IntEntry : IEntry
{
    public int Value { get; }

    public IntEntry(int value)
    {
        this.Value = value;
    }

    public ListEntry ToListEntry()
    {
        return new ListEntry(this);
    }

    public int CompareTo(IEntry? other)
    {
        return other switch
        {
            IntEntry i => Value == i.Value
                               ? 0
                               : Value < i.Value
                                   ? -1
                                   : 1,
            ListEntry l => ToListEntry().CompareTo(l),
            _ => 0,
        };
    }

    public override string ToString()
    {
        return Value.ToString();
    }
}

async Task<List<(IEntry A, IEntry B)>> Parse(string path)
{
    var res = new List<(IEntry A, IEntry B)>();
    var itmA = (IEntry?)null;
    var itmB = (IEntry?)null;
    await foreach (var line in File.ReadLinesAsync(path))
    {
        if (String.IsNullOrWhiteSpace(line))
        {
            if (itmA == null || itmB == null)
            {
                throw new Exception("wat");
            }
            res.Add((itmA, itmB));
            itmA = null;
            itmB = null;
        }
        else
        {
            var itm = ParseInternal(new Queue<char>(line), null);
            if (itmA == null)
            {
                itmA = itm;
            }
            else if (itmB == null)
            {
                itmB = itm;
            }
            else
            {
                //Console.WriteLine("A: " + itmA.ToString() + "\nB:" + itmB.ToString());
                throw new Exception("wat");
            }
        }
    }
    if (itmA != null && itmB != null)
    {
        res.Add((itmA, itmB));
    }
    return res;
    
    static IEntry ParseInternal(Queue<char> q, ListEntry? parent)
    {
        var num = (int?) null;
        while (q.Any()) 
        {
            var c = q.Dequeue();
            //Console.WriteLine(c + " ZZZ " + q.Aggregate("", (a, x) => a + x));
            
            if (c == '[')
            {
                var n = ParseInternal(q, new ListEntry());
                if (n != null)
                {
                    if (parent == null)
                    {
                        if (n is ListEntry le)
                        {
                            parent = le;
                        }
                        else
                        {
                            parent = new ListEntry(n);
                        }
                    }
                    else 
                    {
                        parent.Values.Add(n);
                    }
                }
            }
            else if (c == ']') 
            {
                if (parent == null)
                {
                    throw new Exception("wat");
                }
                
                if (num != null)
                {
                    parent.Values.Add(new IntEntry(num.Value));
                }
                return parent;
            }
            else if (c == ',')
            {
                if (num != null) 
                {
                    if (parent == null)
                    {
                        throw new Exception("wat");
                    }
                    
                    parent.Values.Add(new IntEntry(num.Value));
                    num = null;
                }
            }
            else if (c >= '0' && c <= '9')
            {
                if (num == null)
                {
                    num = 0;
                }
                
                num = num * 10 + (c - '0');
            }
        }
        return parent ?? new ListEntry();
    }
}

// Part 1
async Task<int> Part1(string path) 
{
    var cnt = 0;
    var pairs = await Parse(path);
    for (var idx = 0; idx < pairs.Count; idx++) 
    {
        var (itmA, itmB) = pairs[idx];
        //Console.WriteLine("I: " + (idx+1) + "\nA: " + itmA.ToString() + "\nB: " + itmB.ToString() + "\nR: " + itmA.CompareTo(itmB));
        
        if (itmA.CompareTo(itmB) <= 0)
        {
            cnt += idx + 1;
        }
    }
    return cnt;
}
Console.WriteLine(await Part1("input"));

// Part 2
async Task<int> Part2(string path) 
{
    var items = (await Parse(path)).SelectMany(x => new [] { x.A, x.B }).ToList();
    
    var p1 = new ListEntry(new ListEntry(new IntEntry(2)));
    items.Add(p1);
    
    var p2 = new ListEntry(new ListEntry(new IntEntry(6)));
    items.Add(p2);
    
    // He He, knew using Comparable would pay off ;)
    items.Sort();
    
    var i1 = items.IndexOf(p1) + 1;
    var i2 = items.IndexOf(p2) + 1;
    
    return i1 * i2;
}
Console.WriteLine(await Part2("input"));