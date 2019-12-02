<Query Kind="Program" />

void Main()
{
	var lines = input.Split("\n").Where(l=>l!="").ToList();
	Int64 sum = 0L;
	
	foreach(var line in lines){
		line.Dump("calculating for");
		Decimal moduleWeight = AsDecimal(line);
		Int64 moduleFuelRequirements = GetTotalFuelRequirements(moduleWeight);
		sum += moduleFuelRequirements;
	}
	sum.Dump("We need the following amount of fuel");
	
	Decimal AsDecimal(String input){
		Decimal parsed = 0m;
		if(!Decimal.TryParse(input, out parsed)){
			input.Dump("Could not parse input of");
		}
		return parsed;
	}
	
	Int64 GetTotalFuelRequirements(Decimal moduleWeight){
		Int64 total = 0L;
		Int64 current = (Int64) moduleWeight;
		do
		{
			current = GetFuelRequirements(current);
			total+=current;
		} while(current!=0L);
		return total;
	}
	
	Int64 GetFuelRequirements(Decimal moduleWeight)
	{
		moduleWeight.Dump();
		if (moduleWeight <= 0m || moduleWeight > Int64.MaxValue) { moduleWeight.Dump("unexpected module weight"); }
		// divide by 3
		var div3 = moduleWeight / 3m;
		// abs
		var abs = Math.Abs(div3);
		var absUint64 = (Int64)abs;
		// sub2 - prevent overflow
		var sub2 = absUint64 - 2L;
		if(sub2<0L) sub2 = 0L;
		return sub2;
	}
}

// Define other methods, classes and namespaces here
const string spec = @"--- Day 1: The Tyranny of the Rocket Equation ---
Santa has become stranded at the edge of the Solar System while delivering presents to other planets! To accurately calculate his position in space, safely align his warp drive, and return to Earth in time to save Christmas, he needs you to bring him measurements from fifty stars.

Collect stars by solving puzzles. Two puzzles will be made available on each day in the Advent calendar; the second puzzle is unlocked when you complete the first. Each puzzle grants one star. Good luck!

The Elves quickly load you into a spacecraft and prepare to launch.

At the first Go / No Go poll, every Elf is Go until the Fuel Counter-Upper. They haven't determined the amount of fuel required yet.

Fuel required to launch a given module is based on its mass. Specifically, to find the fuel required for a module, take its mass, divide by three, round down, and subtract 2.

For example:

For a mass of 12, divide by 3 and round down to get 4, then subtract 2 to get 2.
For a mass of 14, dividing by 3 and rounding down still yields 4, so the fuel required is also 2.
For a mass of 1969, the fuel required is 654.
For a mass of 100756, the fuel required is 33583.
The Fuel Counter-Upper needs to know the total fuel requirement. To find it, individually calculate the fuel needed for the mass of each module (your puzzle input), then add together all the fuel values.

What is the sum of the fuel requirements for all of the modules on your spacecraft?

To begin, get your puzzle input.";

// Define other methods, classes and namespaces here
const string spec2 = @"--- Part Two ---
During the second Go / No Go poll, the Elf in charge of the Rocket Equation Double-Checker stops the launch sequence. Apparently, you forgot to include additional fuel for the fuel you just added.

Fuel itself requires fuel just like a module - take its mass, divide by three, round down, and subtract 2. However, that fuel also requires fuel, and that fuel requires fuel, and so on. Any mass that would require negative fuel should instead be treated as if it requires zero fuel; the remaining mass, if any, is instead handled by wishing really hard, which has no mass and is outside the scope of this calculation.

So, for each module mass, calculate its fuel and add it to the total. Then, treat the fuel amount you just calculated as the input mass and repeat the process, continuing until a fuel requirement is zero or negative. For example:

A module of mass 14 requires 2 fuel. This fuel requires no further fuel (2 divided by 3 and rounded down is 0, which would call for a negative fuel), so the total fuel required is still just 2.
At first, a module of mass 1969 requires 654 fuel. Then, this fuel requires 216 more fuel (654 / 3 - 2). 216 then requires 70 more fuel, which requires 21 fuel, which requires 5 fuel, which requires no further fuel. So, the total fuel required for a module of mass 1969 is 654 + 216 + 70 + 21 + 5 = 966.
The fuel required by a module of mass 100756 and its fuel is: 33583 + 11192 + 3728 + 1240 + 411 + 135 + 43 + 12 + 2 = 50346.
What is the sum of the fuel requirements for all of the modules on your spacecraft when also taking into account the mass of the added fuel? (Calculate the fuel requirements for each module separately, then add them all up at the end.)
";

const string input = @"138486
133535
66101
98143
56639
120814
142212
92654
100061
104095
55169
94082
76014
81109
106237
111930
138463
145843
142133
71154
112809
136465
142342
68794
131804
146345
107935
98577
127456
89612
95710
149792
136982
92773
92303
114637
107447
111815
149603
106822
78811
114120
148773
90259
101612
82220
139301
91121
99366
84070
120713
59311
120435
56106
127426
110465
76167
81199
116298
110064
125674
135698
86792
114228
119794
76683
125698
103450
142435
142297
122593
96177
104287
121379
54729
108057
127334
91718
67009
93304
66907
133910
145775
119241
117492
56351
96171
50449
137815
149308
119003
60320
66853
56648
52003
115137
124759
73799
94731
147480
";