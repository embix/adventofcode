<Query Kind="Statements" />

#define SAMPLE

var content = GetContent();
content.Dump("input");
var pair = GetPair(content);
pair.Dump();
var (a,b,c) = pair;
(a*b*c).Dump("product");

(Int64, Int64, Int64) GetPair(Int64[] input)
{
    const Int64 targetValue = 2020;
    foreach(var candidate in input)
    {
        var remainingB = input.Except(new[] { candidate}).ToArray();
        foreach(var partnerB in remainingB)
        {
            // yep, inefficient: QnD :P
            var remainingC = remainingB.Except(new []{partnerB}).ToArray();
            foreach(var partnerC in remainingC)
            {
                var sum = candidate + partnerB + partnerC;
                if (sum == targetValue)
                {
                    return (candidate, partnerB, partnerC);
                }
            }
        }        
    }
    throw new Exception("Input does not have a matching triple");
}

Int64[] GetContent()
{
    var fileName = 
    #if SAMPLE
        @"C:\git\adventofcode\2020\Inputs\day01.sample";
    #else
        @"C:\git\adventofcode\2020\Inputs\day01.personalized";
    #endif
    
    return File
        .ReadAllLines(fileName)
        .Where(l=>!String.IsNullOrWhiteSpace(l))
        .Select(s=>Int64.Parse(s)).ToArray();
}
