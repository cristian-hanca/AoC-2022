var crt = 0;
var res = new List<int>();

await foreach (var line in File.ReadLinesAsync("input")) {
    if (string.IsNullOrWhiteSpace(line)) {
        res.Add(crt);
        crt = 0;
        continue;
    }
    
    if (int.TryParse(line, out var i)) {
        crt += i;
    }
}

res.Sort((a, b) => -1 * a.CompareTo(b));

// Part 1 
Console.WriteLine(res.Take(1).Sum());

// Part 2
Console.WriteLine(res.Take(3).Sum());