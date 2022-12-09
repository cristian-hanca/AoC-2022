enum Move { Rock, Paper, Scissors }

var moveMap = new Dictionary<string, Move> {
    {"A", Move.Rock},
    {"B", Move.Paper},
    {"C", Move.Scissors},
    {"X", Move.Rock},
    {"Y", Move.Paper},
    {"Z", Move.Scissors},
};

var inpMap = new Dictionary<string, int> {
    {"X", -1},
    {"Y",  0},
    {"Z",  1},
};

var valMap = new Dictionary<Move, int> {
    {Move.Rock    , 1},
    {Move.Paper   , 2},
    {Move.Scissors, 3}
};

var rezMap = new Dictionary<int, int> {
    {-1, 0},
    {0,  3},
    {1,  6},
};

var crt = 0;

static int Compute(Move a, Move b)
{
    switch (a)
    {
        case Move.Rock:
            switch (b)
            {
                case Move.Rock:
                    return 0;
                case Move.Paper:
                    return -1;
                case Move.Scissors:
                    return 1;
            };
            break;
        case Move.Paper:
            switch (b)
            {
                case Move.Rock:
                    return 1;
                case Move.Paper:
                    return 0;
                case Move.Scissors:
                    return -1;
            }
            break;
        case Move.Scissors:
            switch (b)
            {
                case Move.Rock:
                    return -1;
                case Move.Paper:
                    return 1;
                case Move.Scissors:
                    return 0;
            }
            break;
    }
    throw new Exception("wat");
}


// Part 1
await foreach (var line in File.ReadLinesAsync("input")) {
    var moves = line.Split();
    
    var opMove = moveMap[moves[0]];
    var myMove = moveMap[moves[1]];
    
    crt += valMap[myMove] + rezMap[Compute(myMove, opMove)];
}
Console.WriteLine(crt);

// Part 2
crt = 0;
await foreach (var line in File.ReadLinesAsync("input")) {
    var items = line.Split();
    
    var opMove = moveMap[items[0]];
    var shouldRes = inpMap[items[1]];
    
    var map = Enum.GetValues<Move>()
        .Select(x => (Compute(x, opMove), x))
        .ToDictionary(x => x.Item1, x => x.Item2);
    crt += valMap[map[shouldRes]] + rezMap[shouldRes];
}
Console.WriteLine(crt);