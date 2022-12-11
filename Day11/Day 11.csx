using System.Numerics;

class Monkey 
{
    public int Id { get; set; }

    public Queue<ulong> Items { get; } = new Queue<ulong>();
    
    public Func<ulong, ulong> Operation { get; set; } = x => x;
    
    public int BoredFactor { get; set; } = 3;
    
    public int TestDivisibilityFactor { get; set; }
    
    public int OnTestTrue { get; set; }
    
    public int OnTestFalse { get; set; }
   
}

async Task<List<Monkey>> Parse(string path, int boredFactor)
{
    var res = new List<Monkey>();
    var crt = new Monkey();
    await foreach (var line in File.ReadLinesAsync(path))
    {
        if (line.StartsWith("Monkey "))
        {
            var id = int.Parse(line.Replace("Monkey ", "").Replace(":", ""));
            crt = new Monkey
            {
                Id = id,
                BoredFactor = boredFactor
            };
            res.Add(crt);
        }
        else if (line.StartsWith("  Starting items: "))
        {
            line.Replace("  Starting items: ", "")
                .Split(",")
                .Select(x => ulong.Parse(x))
                .ToList()
                .ForEach(x => crt.Items.Enqueue(x));
        }
        else if (line.StartsWith("  Operation: new = old "))
        {
            crt.Operation = ParseOperation(line);
        }
        else if (line.StartsWith("  Test: divisible by "))
        {
            crt.TestDivisibilityFactor = int.Parse(line.Replace("  Test: divisible by ", ""));
        }
        else if (line.StartsWith("    If true: throw to monkey "))
        {
            crt.OnTestTrue = int.Parse(line.Replace("    If true: throw to monkey ", ""));
        }
        else if (line.StartsWith("    If false: throw to monkey "))
        {
            crt.OnTestFalse = int.Parse(line.Replace("    If false: throw to monkey ", ""));
        }
    }
   
    return res; 
    
    Func<ulong, ulong> ParseOperation(string line)
    {
        var comp = line.Replace("  Operation: new = old ", "").Split();
        var val = comp[1] == "old" ? (ulong?)null : ulong.Parse(comp[1]);
        switch (comp[0])
        {
            case "+" :
                if (val == null)
                {
                    return x => x + x;
                }
                else 
                {
                    return x => x + val.Value;
                }
            case "*" :
                if (val == null)
                {
                    return x => x * x;
                }
                else 
                {
                    return x => x * val.Value;
                }
        }
        throw new Exception("wat");
    }
}

async Task Process(string path, int rounds, int boredFactor, Action<Monkey> callback)
{
    var mks = await Parse(path, boredFactor);
    
    // Thanks @elpeka0
    var magicMod = (ulong)mks.Select(x => x.TestDivisibilityFactor).Aggregate(1, (a, x) => a * x);
    
    for (var i = 0; i < rounds; i++)
    {
        foreach (var monkey in mks)
        {
            callback(monkey);
            while (monkey.Items.Any())
            {
                // Get the Item.
                var level = monkey.Items.Dequeue();
                
                // Apply Magic Mod
                level %= magicMod;
                
                // Apply Operation.
                level = monkey.Operation(level);
                
                // Bored.
                if (monkey.BoredFactor != 1)
                {
                    level /= (ulong)monkey.BoredFactor;
                }
                
                // Test + Throw.
                if (level % (ulong)monkey.TestDivisibilityFactor == 0)
                {
                    mks.First(x => x.Id == monkey.OnTestTrue).Items.Enqueue(level);
                }
                else
                {
                    mks.First(x => x.Id == monkey.OnTestFalse).Items.Enqueue(level);
                }
            }
        }
    }
}

async Task<ulong> DeterminemonkeyBusiness(string path, int rounds, int boredFactor)
{
    var itemsInspected = new Dictionary<int, ulong>();
    await Process(path, rounds, boredFactor, m =>
    {
        if (!itemsInspected.ContainsKey(m.Id))
        {
            itemsInspected[m.Id] = 0;
        }
        itemsInspected[m.Id] += (ulong)m.Items.Count;
    });
    var mostActive = itemsInspected.Values.OrderDescending().Take(2).ToList();
    return mostActive[0] * mostActive[1];
}

// Part 1
Console.WriteLine(await DeterminemonkeyBusiness("input", 20, 3));

// Part 2
Console.WriteLine(await DeterminemonkeyBusiness("input", 10000, 1));