<Query Kind="Program" />

void Main()
{
	var lines = input.Split("\n").Where(l=>l!="").ToList();
	UInt64 sum = 0;
	
	foreach(var line in lines){
		Decimal moduleWeight = AsDecimal(line);
		UInt64 moduleFuelRequirements = GetFuelRequirements(moduleWeight);
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
	UInt64 GetFuelRequirements(Decimal moduleWeight){
		if(moduleWeight<=0m||moduleWeight>UInt64.MaxValue){moduleWeight.Dump("unexpected module weight");}
		// divide by 3
		var div3 = moduleWeight/3m;		
		// abs
		var abs = Math.Abs(div3);
		var absUint64 = (UInt64)abs;
		// sub2
		var sub2 = absUint64 - 2L;
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