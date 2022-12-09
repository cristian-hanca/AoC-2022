static async Task<(List<Stack<char>> Stacks, List<(int Count, int From, int To)> Moves)> Parse(string path)
{
    var stacks = new List<Stack<char>>();
    var moves = new List<(int Count, int From, int To)>();
    var mode = false;
    
    await foreach (var line in File.ReadLinesAsync(path))
    {
        if (!mode)
        {
            // Create Empty Stacks
            if (stacks.Count == 0)
            {
                for (var i = 0; i < (line.Length / 4 + 1); i++)
                {
                    stacks.Add(new Stack<char>());
                }
            }
        
            // Read Stack
            var idx = 0;
            for (var i = 0; i < line.Length; i += 4)
            {
                var c = line[i + 1];
                if (c != ' ' && !(c >= '1' && c <= '9'))
                {
                    stacks[idx].Push(c);
                }
                idx++;
            }
            
            if (string.IsNullOrWhiteSpace(line))
            {
                mode = true;
            }
        }
        else
        {
            // Read Moves
            var nums = line.Replace("move ", "").Replace("from", "").Replace("to", "")
                           .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                           .Select(x => int.Parse(x))
                           .ToList();
            moves.Add((nums[0], nums[1], nums[2]));
        }
    }
   
    stacks = stacks.Select(s => new Stack<char>(s)).ToList();
    return (stacks, moves);
}


// Part 1
var data1 = await Parse("input");
foreach (var m in data1.Moves)
{
    for (var i = 0; i < m.Count; i++)
    {
        data1.Stacks[m.To - 1].Push(data1.Stacks[m.From - 1].Pop());
    }
}
Console.WriteLine(data1.Stacks.Select(x => x.Peek()).Aggregate("", (a, x) => a + x));

// Part 2
var data2 = await Parse("input");
foreach (var m in data2.Moves)
{
    var temp = new List<char>();
    for (var i = 0; i < m.Count; i++)
    {
        temp.Add(data2.Stacks[m.From - 1].Pop());
    }
    foreach (var t in temp.Reverse<char>())
    {
        data2.Stacks[m.To - 1].Push(t);
    }
}
Console.WriteLine(data2.Stacks.Select(x => x.Peek()).Aggregate("", (a, x) => a + x));