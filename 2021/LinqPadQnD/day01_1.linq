<Query Kind="Program" />

#define SAMPLE
#define TRACE
void Main()
{
    GetContent().DumpTrace("Input")
    .Convolute((prev,curr)=>new{prev,curr,diff=curr-prev}).DumpTrace("convoluted")
    .Where(m=>m.diff>0).DumpTrace("depth increased")
    .Count().Dump("Result");    
}

// You can define other methods, fields, classes and namespaces here
Int64[] GetContent()
{
    var fileName =
#if SAMPLE
            @"C:\git\adventofcode\2021\Inputs\day01.sample";
#else
            @"C:\git\adventofcode\2021\Inputs\day01.personalized";
#endif

    return File
        .ReadAllLines(fileName)
        .Where(l => !String.IsNullOrWhiteSpace(l))
        .Select(s => Int64.Parse(s)).ToArray();
}

// FYI: Convolute from "My Extensions.linq"
//public static IEnumerable<TOut> Convolute<TIn, TOut>(this IEnumerable<TIn> sequence, Func<TIn, TIn, TOut> function)
//{
//    if (sequence == null) throw new ArgumentNullException("sequence");
//    if (function == null) throw new ArgumentNullException("function");
//    if (sequence.Take(2).Count() < 2) throw new ArgumentOutOfRangeException("sequence", "Must have at least 2 value to apply diff convolution.");
//
//    // convolute in separate method due to the argument-check/yield issue (see: where did i find that skeet article?!)
//    return ConvoluteImpl(sequence, function);
//}
//
//private static IEnumerable<TOut> ConvoluteImpl<TIn, TOut>(IEnumerable<TIn> sequence, Func<TIn, TIn, TOut> function)
//{
//    TIn lastValue = default(TIn);// value not used but required for compilation
//    Boolean isInitialized = false;
//
//    foreach (var element in sequence)
//    {
//        if (isInitialized)
//        {
//            var convoluted = function(lastValue, element);
//            lastValue = element;
//            yield return convoluted;
//        }
//        else
//        {
//            lastValue = element;
//            isInitialized = true;
//        }
//    }
//}