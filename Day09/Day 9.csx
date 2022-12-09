(int, int) TrailHead(int hx, int hy, int tx, int ty, HashSet<(int, int)> visited)
{
    // Veriy if Still Touching.
    if (Math.Abs(hx - tx) > 1 || Math.Abs(hy - ty) > 1)
    {
        // Move Vertically.
        if (hx == tx)
        {
            MoveY();
        }
        // Move Horizontally.
        else if (hy == ty)
        {
            MoveX();
        }
        // Move Diagonally.
        else
        {
            MoveX();
            MoveY();
        }
    }
    visited.Add((tx, ty));
    return (tx, ty);
    
    void MoveX()
    {
        if (hx < tx)
        {
            tx--;
        }
        else
        {
            tx++;
        }
    }
    
    void MoveY()
    {
        if (hy < ty)
        {
            ty--;
        }
        else
        {
            ty++;
        }
    }
}

// Part 1
async Task Part1()
{
    int hx = 0, hy = 0, tx = 0, ty = 0;
    var visited = new HashSet<(int, int)>();
    
    await foreach (var line in File.ReadLinesAsync("input"))
    {
        var comp = line.Split();
        var cnt = int.Parse(comp[1]);
        switch (comp[0])
        {
            case "U":
                for (var i = 0; i < cnt; i++)
                {
                    hy++;
                    (tx, ty) = TrailHead(hx, hy, tx, ty, visited);
                }
                break;
            case "D":
                for (var i = 0; i < cnt; i++)
                {
                    hy--;
                    (tx, ty) = TrailHead(hx, hy, tx, ty, visited);
                }
                break;
            case "L":
                for (var i = 0; i < cnt; i++)
                {
                    hx--;
                    (tx, ty) = TrailHead(hx, hy, tx, ty, visited);
                }
                break;
            case "R":
                for (var i = 0; i < cnt; i++)
                {
                    hx++;
                    (tx, ty) = TrailHead(hx, hy, tx, ty, visited);
                }
                break;
        }
    }
    
    Console.WriteLine(visited.Count);
}
await Part1();

async Task Part2()
{
    var knots = 9;
    int hx = 0, hy = 0;
    int[] tx = new int[knots], ty = new int[knots];
    var visited = Enumerable.Range(0, knots).Select(x => new HashSet<(int, int)>()).ToArray();
    
    await foreach (var line in File.ReadLinesAsync("input"))
    {
        var comp = line.Split();
        var cnt = int.Parse(comp[1]);
        switch (comp[0])
        {
            case "U":
                for (var i = 0; i < cnt; i++)
                {
                    hy++;
                    TrailHeadInternal();
                }
                break;
            case "D":
                for (var i = 0; i < cnt; i++)
                {
                    hy--;
                    TrailHeadInternal();
                }
                break;
            case "L":
                for (var i = 0; i < cnt; i++)
                {
                    hx--;
                    TrailHeadInternal();
                }
                break;
            case "R":
                for (var i = 0; i < cnt; i++)
                {
                    hx++;
                    TrailHeadInternal();
                }
                break;
        }
        
        void TrailHeadInternal()
        {
            (tx[0], ty[0]) = TrailHead(hx, hy, tx[0], ty[0], visited[0]);
            for (var k = 1; k < knots; k++)
            {
                (tx[k], ty[k]) = TrailHead(tx[k-1], ty[k-1], tx[k], ty[k], visited[k]);
            }
        }
    }
    
    Console.WriteLine(visited[8].Count);
}
await Part2();