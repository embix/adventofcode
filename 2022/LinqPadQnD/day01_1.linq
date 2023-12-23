<Query Kind="Program" />

//#define SAMPLE
//#define TRACE
void Main()
{
    var input = GetContent().DumpTrace("Input");
    var cargoByElf = GetCargoByElf(input).DumpTrace("cargo by elf");
    var ordered =
    cargoByElf
    .Select(e=>new{
        ElfSum=e.Sum(),
        ElfItems=e
    }).DumpTrace("elfsum")
    .OrderByDescending(e=>e.ElfSum);
    ordered
    .First()
    .ElfSum
    .Dump("Result part 1");
    ordered
    .Take(3)
    .Sum(e=>e.ElfSum)
    .Dump("Result part 2");
}

List<List<Int32>> GetCargoByElf(IEnumerable<String> input)
{
    var elfes = new List<List<Int32>>();
    var currentElf = new List<Int32>();
    elfes.Add(currentElf);
    
    foreach(var currentItem in input)
    {
        if(Int32.TryParse(currentItem, out var value)){
            currentElf.Add(value);
        }else{
            currentElf = new List<Int32>();
            elfes.Add(currentElf);
        }
    }
    return elfes;
}

// You can define other methods, fields, classes and namespaces here
String[] GetContent()
{
    var fileName =
#if SAMPLE
            @"C:\git\adventofcode\2022\Inputs\day01.sample";
#else
            @"C:\git\adventofcode\2022\Inputs\day01.personalized";
#endif

    return File
        .ReadAllLines(fileName)
        .ToArray();
}
