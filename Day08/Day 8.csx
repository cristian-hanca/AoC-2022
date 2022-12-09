static IEnumerable<int> Range(int start, int end)
{
    for (var i = start; i <= end; i++)
    {
        yield return i;
    }
}

static int CountUntil(this IEnumerable<int> ls, int lim)
{
    var res = 0;
    foreach (var elem in ls)
    {
        res++;
        if (elem >= lim)
        {
            break;
        }
    }
    return res;
}

static async Task<List<List<int>>> Process(string filePath)
{
    var res = new List<List<int>>();
    await foreach (var line in File.ReadLinesAsync(filePath))
    {
        var ln = new List<int>();
        foreach (var c in line)
        {
            ln.Add(int.Parse(c.ToString()));
        }
        res.Add(ln);
    }
    return res;
}

// Common
var map = await Process("input");
var xLim = map[0].Count - 1;
var yLim = map.Count - 1;

// Part 1
var crt = (long)(map.Count * 2 + map[0].Count * 2 - 4);
for (var xId = 1; xId < xLim; xId++)
{
    for (var yId = 1; yId < yLim; yId++)
    {
        var self = map[yId][xId];
        if (/* Up    */ self > Range(0, yId - 1).Select(y => map[y][xId]).Max() ||
            /* Down  */ self > Range(yId + 1, yLim).Select(y => map[y][xId]).Max() ||
            /* Left  */ self > Range(0, xId - 1).Select(x => map[yId][x]).Max() ||
            /* Right */ self > Range(xId + 1, xLim).Select(x => map[yId][x]).Max())
        {
            crt++;
            continue;
        }
    }
}
Console.WriteLine(crt);

// Part 2
crt = 0;
for (var xId = 1; xId < xLim; xId++)
{
    for (var yId = 1; yId < yLim; yId++)
    {
        var self = map[yId][xId];
        var res = /* Up    */ Range(0, yId - 1).Select(y => map[y][xId]).Reverse().CountUntil(self) * 
                  /* Down  */ Range(yId + 1, yLim).Select(y => map[y][xId]).CountUntil(self) *
                  /* Left  */ Range(0, xId - 1).Select(x => map[yId][x]).Reverse().CountUntil(self) *
                  /* Right */ Range(xId + 1, xLim).Select(x => map[yId][x]).CountUntil(self);
        if (crt < res)
        {
            crt = res;
        }
    }
}
Console.WriteLine(crt);