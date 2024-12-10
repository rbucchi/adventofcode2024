var input = @".....E........................s.......n.........g.
.............................c............4.......
........................................4.........
....................................5.....e.......
...............p...........c......................
....h................s...................e........
h..................1...........s..Ke............C.
.......................1...............g.....KC...
........8.......B.....p..kc................K..e.X.
...b.........pI...k..................r.........C.X
...........................5.n............R.r.....
j......Z....tApE..............c....5..g.X.........
............E..L......5............X..............
b...................D...................K.....R..4
..k..D.....h..A...........L.1.....................
.j...........h......B.......A.....................
.........I......b..................4.......r....0.
.................B.n..........G...................
..........9.I...............U...................2.
.........Doy........s...............U....R........
..........................G.....V............R....
...z.o.......I..E....t.....G..n....3..............
.Z.........Aj..................W.......M.U........
..Z......k......O....W.....U........M.......0.....
.....z......o.O..........a....ZG..................
........L..........Y............a.................
......D8t...S.......WO............................
......1P..........WO.9..F.w........Q..d....0......
..........y............................x..........
............z..........w.........J................
.o...t..P.........w..B......F....v........x....2..
y..8...........v.......M.................x.......2
.....y..........z..N...H.......6........a.........
....N.S............H...................a..........
N........S..........v........m....................
......8...........H........7x....6.l..............
.............q.P...............w..m...............
.....S......................7.6.......T...........
...............................0....3.6....J......
...N..........v.................m.......3.l.J.....
...........................F..d....7.3............
...............u..................................
.V....Y..u..........H.......J.............T.......
.......V...q...................d..fF.............T
..u................................f.....T......l.
..................................i...............
...Y......M.........................7.............
............Y...........9............f2..m..Q.....
.....................i.9........fd.......l....Q...
V.........q................i......................
";


static char[,] readInput(string input)
{
    string[] lines = input.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

    int rowCount = lines.Length;
    if (rowCount == 0)
    {
        throw new Exception("Input is empty.");
    }

    int colCount = lines[0].Length;
    foreach (var line in lines)
    {
        if (line.Length != colCount)
        {
            throw new Exception("All lines must have the same number of characters.");
        }
    }

    char[,] matrix = new char[rowCount, colCount];

    for (int i = 0; i < rowCount; i++)
    {
        for (int j = 0; j < colCount; j++)
        {
            var tmp = lines[i][j];
            matrix[i, j] = tmp;
        }
    }
    Console.WriteLine("Matrix of Antennas:");
    for (int i = 0; i < rowCount; i++)
    {
        if (i == 0) for (int k = -1; k < colCount; k++) Console.Write(k == -1 ? "\t" : k + " ");
        if (i == 0) Console.WriteLine();
        for (int j = 0; j < colCount; j++)
        {
            if (j == 0) Console.Write((i) + "\t");
            Console.Write(matrix[i, j] + " ");
        }
        Console.WriteLine();
    }
    return matrix;
}

static char[] readFrequencies(char[,] matrix)
{
    HashSet<char> result = [];
    int rowCount = matrix.GetUpperBound(0) + 1;
    int colCount = matrix.GetUpperBound(1) + 1;
    for (int i = 0; i < rowCount; i++)
        for (int j = 0; j < colCount; j++)
            if (matrix[i, j] != '.') result.Add(matrix[i, j]);
    return [.. result];
}

static (int, int)[] getAntennas(char[,] matrix, char frequency)
{
    int rowCount = matrix.GetUpperBound(0) + 1;
    int colCount = matrix.GetUpperBound(1) + 1;
    List<(int, int)> result = [];
    for (int i = 0; i < rowCount; i++)
        for (int j = 0; j < colCount; j++)
            if (matrix[i, j] == frequency) result.Add((i, j));
    return result.ToArray();
}

//input = @"............
//........0...
//.....0......
//.......0....
//....0.......
//......A.....
//............
//............
//........A...
//.........A..
//............
//............";

var matrix = readInput(input);

char[] frequencies = readFrequencies(matrix);
Console.WriteLine("");
Console.Write("Frequencies: ");
Console.WriteLine("");
foreach (char c in frequencies) Console.Write(c + " ");

int rowCount = matrix.GetUpperBound(0) + 1;
int colCount = matrix.GetUpperBound(1) + 1;
var total = 0;
//var antinodes = uniqueAntinodesCalc(matrix, frequencies);
var antinodes = uniqueAntinodesCalcInfinite(matrix, frequencies);
for (int x = 0; x < rowCount; x++)
{
    if (x == 0) for (int k = -1; k < colCount; k++) Console.Write(k == -1 ? "\t" : k + " ");
    if (x == 0) Console.WriteLine();
    for (int y = 0; y < colCount; y++)
    {
        if (y == 0) Console.Write((x) + "\t");
        if (antinodes.Any(_ => _.Item1 == x && _.Item2 == y))
        {
            total++;
            Console.Write("# ");
        }
        else
        {
            Console.Write(matrix[x, y] + " ");
        }
    }
    Console.WriteLine();
}
Console.WriteLine();
//total += antinodes.Length;

Console.WriteLine("total: " + total);
Console.WriteLine("== END ==");

static (int, int)[] uniqueAntinodesCalc(char[,] matrix, char[] frequencies)
{
    int rowCount = matrix.GetUpperBound(0) + 1;
    int colCount = matrix.GetUpperBound(1) + 1;
    List<(int, int)> antinodes = [];
    foreach (var freq in frequencies)
    {
        (int, int)[] antennas = getAntennas(matrix, freq);
        for (int i = 0; i < antennas.Length; i++)
        {
            var antennaToCalc = antennas[i];
            if (matrix[antennaToCalc.Item1, antennaToCalc.Item2] != freq) continue;
            for (int j = 0; j < antennas.Length; j++)
            {
                if (i == j) continue;
                if (matrix[antennas[j].Item1, antennas[j].Item2] != freq) continue;
                (int, int) antinode = (antennaToCalc.Item1 + (antennaToCalc.Item1 - antennas[j].Item1), antennaToCalc.Item2 + (antennaToCalc.Item2 - antennas[j].Item2));
                if (!antinodes.Any(x => x.Item1 == antinode.Item1 && x.Item2 == antinode.Item2)
                   && antinode.Item1 >= 0
                   && antinode.Item2 >= 0
                   && antinode.Item1 <= rowCount
                   && antinode.Item2 <= colCount)
                {
                    antinodes.Add(antinode);
                }
            }
        }
    }
    return antinodes.ToArray();
}

static (int, int)[] uniqueAntinodesCalcInfinite(char[,] matrix, char[] frequencies)
{
    int rowCount = matrix.GetUpperBound(0) + 1;
    int colCount = matrix.GetUpperBound(1) + 1;
    List<(int, int)> antinodes = [];
    foreach (var freq in frequencies)
    {
        (int, int)[] antennas = getAntennas(matrix, freq);
        for (int i = 0; i < antennas.Length; i++)
        {
            var found = false;
            var antennaToCalc = antennas[i];
            if (matrix[antennaToCalc.Item1, antennaToCalc.Item2] != freq) continue;
            for (int j = 0; j < antennas.Length; j++)
            {
                if (i == j) continue;
                if (matrix[antennas[j].Item1, antennas[j].Item2] != freq) continue;
                found = true;
                var imout = false;
                var currentX = antennaToCalc.Item1;
                var currentY = antennaToCalc.Item2;
                while (!imout)
                {
                    (int, int) antinode = (currentX + (antennaToCalc.Item1 - antennas[j].Item1), currentY + (antennaToCalc.Item2 - antennas[j].Item2));
                    currentX += (antennaToCalc.Item1 - antennas[j].Item1);
                    currentY += (antennaToCalc.Item2 - antennas[j].Item2);
                    if (!antinodes.Any(x => x.Item1 == antinode.Item1 && x.Item2 == antinode.Item2)
                       && antinode.Item1 >= 0
                       && antinode.Item2 >= 0
                       && antinode.Item1 <= rowCount
                       && antinode.Item2 <= colCount)
                    {
                        antinodes.Add(antinode);
                    }
                    if (antinode.Item1 < 0
                       || antinode.Item2 < 0
                       || antinode.Item1 > rowCount
                       || antinode.Item2 > colCount)
                        imout = true;
                }
            }
            if (found) antinodes.Add(antennas[i]);
        }
    }
    return antinodes.ToArray();
}
