<Query Kind="Program" />

const String day10InputPath = @"C:\git\adventofcode\2019\Inputs\day10.input.txt";
void Main()
{
    //foreach(var testCase in testCases){
    //    
    //}
    TestAngleDetermination();
}

// reuse DirectionVector as Coord based on (0,0)
public struct DirectionVector
{
    public Int32 X;
    public Int32 Y;
    public Double GetDirectionInDegrees()
    {
        var angle = ((Math.Atan2(-Y,X))/Math.PI)*180.0;
        if(angle<0.0)angle+=360.0;
        return angle;
    }
    public Double GetLength() {
        var xComp = (Double)X*(Double)X;
        var yComp = (Double)Y*(Double)Y;
        var sqSum = xComp+yComp;
        var length = Math.Sqrt(sqSum);
        return length;
    }
}

public Boolean TestAngleDetermination()
{
    // hope this matches part 2
    //      -Y 90° 
    //         ^
    //         |
    // 180°<---o---> 0° +X
    // -X      |
    //         v
    //        270° +Y
    var results = 
        new []{
            new{
                X=1,Y=0,Angle=0.0
            },
            new {
                X=1,Y=-1,Angle=45.0
            },
            new { // we need at least one non45 divisible test case
                X=1,Y=-2,Angle=63.4349488
            },
            new {
                X=0,Y=-3,Angle=90.0
            },
            new {
                X=-1,Y=-1,Angle=135.0
            },
            new {
                X=-2,Y=0,Angle=180.0
            },
            new {
                X=-3,Y=3,Angle=225.0
            },
            new {
                X=0,Y=4,Angle=270.0
            },
            new {
                X=5,Y=5,Angle=315.0
            },
            new {
                X=6,Y=0,Angle=0.0 // again :-)
            }
        }.Select(testCase=>new{
            TestCase=testCase,
            ActualAngle = new DirectionVector { X=testCase.X, Y=testCase.Y}.GetDirectionInDegrees()
        }).GroupBy(t=>
            DoubleEqualsWithinEpsilon(t.TestCase.Angle,t.ActualAngle, 0.0000001),
            (passed,g)=>new{
                Passed=passed,
                Cases=g.Select(t=>new{
                    t.TestCase.X,
                    t.TestCase.Y,
                    ExpectedAngle=t.TestCase.Angle,
                    t.ActualAngle
                })
        }).ToList().Dump();
    return !results.Where(r=>!r.Passed).Any();
}

Boolean DoubleEqualsWithinEpsilon(Double lhs, Double rhs, Double epsilon)
{
    var diff = lhs-rhs;
    return (Math.Abs(diff)<=epsilon);
}

class Part1Solution
{
    public DirectionVector BasePosition;
    public Int32 AsteroidsWatchable;
}

struct TestCase{
    public String Name;
    public String MapInput;
}

TestCase[] testCases = new TestCase[]{
    new TestCase {
        Name = "test1",
        MapInput = map1,
        
    },
};

// Code: MIT
// Comment: CC-BY-SA
// (C) SLenik, https://stackoverflow.com/a/6755197/303290
// x - a number, from which we need to calculate the square root
// epsilon - an accuracy of calculation of the root from our number.
// The result of the calculations will differ from an actual value
// of the root on less than epslion.
public static Decimal Sqrt(Decimal x, Decimal epsilon = 0.0M)
{
    if (x < 0) throw new OverflowException("Cannot calculate square root from a negative number");

    // seed
    Decimal current = (Decimal)Math.Sqrt((Double)x);
    Decimal previous;
    do
    {
        previous = current;
        if (previous == 0.0M) return 0;
        current = (previous + x / previous) / 2;
    }
    while (Math.Abs(previous - current) > epsilon);
    return current;
}

static String puzzleInput = File.ReadAllText(day10InputPath);
const String map1 = @".#..#
.....
#####
....#
...##";
const String map2 = @"......#.#.
#..#.#....
..#######.
.#.#.###..
.#..#.....
..#....#.#
#..#....#.
.##.#..###
##...#..#.
.#....####";
const String map3 = @"#.#...#.#.
.###....#.
.#....#...
##.#.#.#.#
....#.#.#.
.##..###.#
..#...##..
..##....##
......#...
.####.###.";
const String map4 = @".#..#..###
####.###.#
....###.#.
..###.##.#
##.##.#.#.
....###..#
..#.#..#.#
#..#.#.###
.##...##.#
.....#.#..";
const String map5 = @".#..##.###...#######
##.############..##.
.#.######.########.#
.###.#######.####.#.
#####.##.#.##.###.##
..#####..#.#########
####################
#.####....###.#.#.##
##.#################
#####.##.###..####..
..######..##.#######
####.##.####...##..#
.#####..#.######.###
##...#.##########...
#.##########.#######
.####.#.###.###.#.##
....##.##.###..#####
.#.#.###########.###
#.#.#.#####.####.###
###.##.####.##.#..##";