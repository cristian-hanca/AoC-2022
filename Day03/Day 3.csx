var crt = 0;

static int Score(char c) {
    if (c >= 'a' && c <= 'z') {
        return c - 'a' + 1;
    }
    if (c >= 'A' && c <= 'Z') {
        return c - 'A' + 1 + 26;
    }
    throw new Exception("wat");
}

// Part 1
await foreach (var line in File.ReadLinesAsync("input")) {
    var compA = new HashSet<char>(line.Substring(0, line.Length / 2).ToCharArray());
    var compB = new HashSet<char>(line.Substring(line.Length / 2).ToCharArray());

    var intersection = compA.Intersect(compB);
    crt += intersection.Select(x => Score(x)).Sum();
}
Console.WriteLine(crt);

// Part 2
crt = 0;
var group = new List<string>();
await foreach (var line in File.ReadLinesAsync("input")) {
    group.Add(line);
    if (group.Count < 3) {
        continue;
    }
    
    var set = new HashSet<char>(group[0]);
    set.IntersectWith(group[1]);
    set.IntersectWith(group[2]);
    
    crt += set.Select(x => Score(x)).Sum();
    group.Clear();
}
Console.WriteLine(crt);