using System.Collections.Concurrent;
using System.Diagnostics;

var cache = new ConcurrentDictionary<long, IEnumerable<long>>();
const int CACHE_LIMIT = 100000;

const string STONES_DIRECTORY = "Stones";
const string FILE_PATH = "block.txt";
const int BLOCK_SIZE = 10000000;

var input = "30 71441 3784 580926 2 8122942 0 291";
//var input = "125 17";
Console.WriteLine();
//total += antinodes.Length;
//InitializeStonesDirectory();
// test
var testStones = parseStones("125 17");
for (int i = 0; i < 6; i++)
{
    testStones = dostep2(testStones);
}
if (testStones.Count() != 22 || string.Join(" ", testStones) != "2097446912 14168 4048 2 0 2 4 40 48 2024 40 48 80 96 2 8 6 7 6 0 3 2") throw new Exception("test non passato");

var stones = parseStones(input);
var N_TIMES = 75;
//var N_TIMES = 6;
for (int i = 0; i < N_TIMES; i++)
{
    Stopwatch sw = Stopwatch.StartNew();
    //if (stones.Count() > BLOCK_SIZE)
    //{
    //    var filePath = WriteBlockToFile(stones.Skip(BLOCK_SIZE).ToList());
    //    stones = stones.Take(BLOCK_SIZE);
    //}
    //var stone = ReadBlockFromFile();
    //while(stone.Count() > 0)
    //{
        stones = dostep2(stones);
    //}
    //var tmpStones = ReadBlockFromFile();
    sw.Stop();
    //Console.WriteLine($"blink {i + 1} in {sw.ElapsedMilliseconds}ms: {stones}");
    Console.WriteLine($"blink {i + 1} ({stones.Count()}) in {sw.ElapsedMilliseconds}ms Memory usage: {GC.GetTotalMemory(false) / 1024 / 1024} MB");
    //Console.WriteLine("");
}

Console.WriteLine("result: \n\n" + stones.Count());
Console.WriteLine("");
Console.WriteLine("== END ==");


IEnumerable<long> parseStones(string input)
{
    return input.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(x => long.Parse(x));
}

IEnumerable<long> dostep2(IEnumerable<long> stones)
{
    return stones
        .AsParallel()
        //.WithDegreeOfParallelism(3)
        .AsOrdered()
        .SelectMany(stone => getOrComputeBlink(stone))
        .ToList()
        ;
}

IEnumerable<long> blink(long stone)
{
    if (stone == 0) return [1];
    var tmp = stone.ToString();
    if (tmp.Length % 2 == 0)
    {
        int half = tmp.Length / 2;
        var part1 = tmp[..half].TrimStart('0');
        var part2 = tmp[half..].TrimStart('0');
        return
        [
            string.IsNullOrEmpty(part1) ? 0 : long.Parse(part1),
            string.IsNullOrEmpty(part2) ? 0 : long.Parse(part2)
        ];
    }
    return [stone * 2024];
}

IEnumerable<long> getOrComputeBlink(long stone)
{
    if (cache.TryGetValue(stone, out IEnumerable<long> tmp))
        return tmp;

    var result = blink(stone);

    // Limita la dimensione della cache
    if (cache.Count >= CACHE_LIMIT)
        cache.TryRemove(cache.First());

    cache[stone] = result;
    return result;
}

string WriteBlockToFile(List<long> block)
{
    string filePath = Path.Combine(STONES_DIRECTORY, FILE_PATH);
    File.WriteAllLines(filePath, block.Select(x => x.ToString()));
    return filePath;
}

IEnumerable<long> ReadBlockFromFile()
{
    string tempFilePath = FILE_PATH + ".tmp";
    string tempFilePath2 = FILE_PATH + "2.tmp";
    using (var reader = new StreamReader(FILE_PATH))
    using (var writer = new StreamWriter(tempFilePath))
    using (var writer2 = new StreamWriter(tempFilePath2))
    {
        int readCount = 0; // Tracks progress in the eventDelimiter
        int removedCount = 0; // Tracks the number of characters removed
        bool eventOccurred = false;

        var BUFFER_SIZE = 10;
        var LIMIT = 10;
        var file1 = true;
        char[] buffer = new char[BUFFER_SIZE];
        int readInBuffer = -1;
        while (readInBuffer == 0)
        {
            readInBuffer = reader.Read(buffer, 0, BUFFER_SIZE);
            readCount += readInBuffer;
            if (file1)
            {
                writer.Write(buffer);
                if (buffer[buffer.Length - 1] == ' ' && readCount > LIMIT) file1 = false;
            }
            if (!file1)
            {
                writer2.Write(buffer);
            }
        }
    }
    var res = File.ReadLines(tempFilePath).Select(long.Parse);
    File.Delete(tempFilePath);
    File.Delete(FILE_PATH);
    File.Move(tempFilePath2, FILE_PATH);
    return res;
}

void InitializeStonesDirectory()
{
    if (!Directory.Exists(STONES_DIRECTORY))
    {
        Directory.CreateDirectory(STONES_DIRECTORY);
    }
    else
    {
        // Pulisce eventuali file residui
        foreach (var file in Directory.GetFiles(STONES_DIRECTORY))
        {
            File.Delete(file);
        }
    }
}