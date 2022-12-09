static async Task<int> Process(string path, int dist) {
    var crt = 0;
    var q = new Queue<char>();
    foreach (var c in await File.ReadAllTextAsync(path))
    {
        if (q.Count == dist) {
            if (q.Distinct().Count() == dist) {
                break;
            }
            q.Dequeue();
        }
        q.Enqueue(c);
        crt++;
    }
    return crt;
}

// Part 1
Console.WriteLine(await Process("input", 4));

// Part 2
Console.WriteLine(await Process("input", 14));