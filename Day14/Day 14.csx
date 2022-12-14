record Point
{
    public int x;
    public int y;
    
    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
    
    public Point(Point o)
    {
        this.x = o.x;
        this.y = o.y;
    }
    
    public override string ToString()
    {
        return $"({x}, {y})";
    }
}

enum Cell
{
    Air,
    Stone,
    Sand
}

class Board
{
    public Dictionary<Point, Cell> Resolver { get; } = new Dictionary<Point, Cell>();

    public int FloorY { get; }
    
    public bool HardFloor { get; }
    
    public Board(List<List<Point>> lines, int? hardFloorOffset = null)
    {
        foreach (var line in lines)
        {
            var last = line.First();
            foreach (var p in line.Skip(1))
            {
                if (last.x == p.x)
                {
                    var min = Math.Min(last.y, p.y);
                    var max = Math.Max(last.y, p.y);
                    for (var y = min; y <= max; y++)
                    {
                        Set(new Point(p.x, y), Cell.Stone);
                    }
                }
                if (last.y == p.y)
                {
                    var min = Math.Min(last.x, p.x);
                    var max = Math.Max(last.x, p.x);
                    for (var x = min; x <= max; x++)
                    {
                        Set(new Point(x, p.y), Cell.Stone);
                    }
                }
                last = p;
            }
            
            FloorY = Resolver.Keys.Select(p => p.y).Max();
            if (hardFloorOffset > 0)
            {
                FloorY += hardFloorOffset.Value;
                HardFloor = true;
            }
        }
    }
    
    public Cell Get(int x, int y)
    {
        return Get(new Point(x, y));
    }
    
    public Cell Get(Point p)
    {
        if (HardFloor && p.y == FloorY)
        {
            return Cell.Stone;
        }
        if (Resolver.TryGetValue(p, out var i))
        {
            return i;
        }
        return Cell.Air;
    }
    
    public void Set(Point p)
    {
        Set(p, Cell.Sand);
    }
    
    protected void Set(Point p, Cell c)
    {
        Resolver[p] = c;
    }
}

async Task<List<List<Point>>> Parse(string path)
{
    var res = new List<List<Point>>();
    await foreach (var line in File.ReadLinesAsync(path))
    {
        var ln = new List<Point>();
        foreach (var entry in line.Split(" -> "))
        {
            var comp = entry.Split(",");
            var p = new Point(int.Parse(comp[0]), int.Parse(comp[1]));
            ln.Add(p);
        }
        res.Add(ln);
    }
    return res;
}

void Simulate(Board board, Point source, Action<Point> callback)
{
    var done = false;
    while (!done)
    {
        var s = new Point(source);
 
        // Simulate Fall.
        while (true)
        {
            // Check if we exceeded the Floor.
            if (!board.HardFloor && board.FloorY == s.y)
            {
                done = true;
                break;
            }
            
            // Check Down.
            if (board.Get(s.x, s.y + 1) == Cell.Air)
            {
                s.y += 1;
            }
            // Check Left
            else if (board.Get(s.x - 1, s.y + 1) == Cell.Air)
            {
                s.x -= 1;
                s.y += 1;
            }
            // Check Right
            else if (board.Get(s.x + 1, s.y + 1) == Cell.Air)
            {
                s.x += 1;
                s.y += 1;
            }
            // Check for No Sand at all.
            else if (s == source)
            {
                done = true;
                break;
            }
            else
            {
                board.Set(s);
                callback(s);
                break;
            }
        }
        
        if (done)
        {
            break;
        }
    }
}

// Part 1
var cnt = 0;
var board = new Board(await Parse("input"));
Simulate(board, new Point(500, 0), _ => cnt++);
Console.WriteLine(cnt);

// Part 2
cnt = 1;
board = new Board(await Parse("input"), 2);
Simulate(board, new Point(500, 0), _ => cnt++);
Console.WriteLine(cnt);