var crt = 0;

bool FullContains(int minA, int maxA, int minB, int maxB) {
    return minA >= minB && maxA <= maxB;
}

bool PartialContains(int minA, int maxA, int minB, int maxB) {
    return minA <= maxB && maxA >= minB;
}

// Part 1
await foreach (var line in File.ReadLinesAsync("input")) {
    var components = line.Split(",");
    
    var rangeA = components[0].Split("-").Select(x => int.Parse(x)).ToList();
    var rangeB = components[1].Split("-").Select(x => int.Parse(x)).ToList();
    
    if (FullContains(rangeA[0], rangeA[1], rangeB[0], rangeB[1]) ||
        FullContains(rangeB[0], rangeB[1], rangeA[0], rangeA[1])) {
        crt += 1;
    }
}
Console.WriteLine(crt);

// Part 2
crt = 0;
await foreach (var line in File.ReadLinesAsync("input")) {
    var components = line.Split(",");
    
    var rangeA = components[0].Split("-").Select(x => int.Parse(x)).ToList();
    var rangeB = components[1].Split("-").Select(x => int.Parse(x)).ToList();
    
    if (PartialContains(rangeA[0], rangeA[1], rangeB[0], rangeB[1]) ||
        PartialContains(rangeB[0], rangeB[1], rangeA[0], rangeA[1])) {
        crt += 1;
    }
}
Console.WriteLine(crt);