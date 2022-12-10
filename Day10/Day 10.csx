async Task<List<(int, Func<int, int>)>> ParseCommands(string path)
{
    var res = new List<(int, Func<int, int>)>();
    await foreach (var line in File.ReadLinesAsync(path))
    {
        var comp = line.Split();
        switch (comp[0])
        {
            case "noop":
                res.Add((0, x => x));
                break;
            case "addx":
                res.Add((1, x => x + int.Parse(comp[1])));
                break;
        }
    }
    return res;
}

async Task Process(string path, Action<int, int> callback)
{
    var cmd = new Queue<(int Delay, Func<int, int> Apply)>(await ParseCommands(path));
    
    var c = 1;
    var x = 1;
    var crt = cmd.Dequeue();
    while (true)
    {
        callback(c, x);
        
        if (crt.Delay == 0)
        {
            x = crt.Apply(x);
            if (cmd.Any())
            {
                crt = cmd.Dequeue();
            } 
            else 
            {
                break;
            }
        }
        else
        {
            crt.Delay -= 1;
        }
        c++;
    }
}

// Part 1
var crt = 0;
await Process("input", (c, x) =>
{
    if (c == 20 || (c - 20) % 40 == 0) {
        crt += c * x;
        //Console.WriteLine(c + " * " + x + " = " + c * x + " | " + crt);
    }
});
Console.WriteLine(crt);

// Part 1
var sb = new StringBuilder();
await Process("input", (c, x) =>
{
    var idx = (c - 1) % 40;
    if (c != 1 && idx == 0) {
        sb.AppendLine();
    }
    
    if (x - 1 == idx || x == idx || x + 1 == idx)
    {
        sb.Append('#');
    }
    else
    {
        sb.Append('.');
    }
});
Console.WriteLine(sb.ToString());