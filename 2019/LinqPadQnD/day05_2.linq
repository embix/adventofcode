<Query Kind="Program" />

void Main()
{
	var testCase = appTests.Where(t=>t.Name==
		//"Test 1"
		//"Test 2"
		//"Test 3"
		//"Producion Part 1 Air Conditioner"
		//"Part2 Test 1-1"
		//"Part2 Test 1-2"
		//"Part2 Test 2-1"
		//"Part2 Test 2-2"
		//"Part2 Test 2-3"
		//"Part2 Test 3-1"
		//"Part2 Test 3-2"
		//"Part2 Test 4-1"
		//"Part2 Test 4-2"
		//"Part2 Test 4-3"
		//"Jump1 Test 1"
		//"Jump1 Test 2"
		//"Jump2 Test 1"
		//"Jump2 Test 2"
		//"Jump3 Test 1"
		//"Jump3 Test 2"
		//"Jump3 Test 3"
		//"Producion Part 2 Thermal Radiator"
	).Single();
	
	Int32[] ram = testCase.RamInit.ToList().ToArray();// get fresh copy
	//Init(ram, noun, verb);
	var inputChannel = testCase.InputInit.ToList();// 1 == air conditioner, 5 thermal radiator
	var outputChannel = new List<Int32>();
	ProcessStep(ram, 0, ref inputChannel, ref outputChannel);
	ram.Dump("ram");
	outputChannel.Dump("output");
	testCase.ExpectedOutput.Dump("expected output");
}

struct AppTest
{
	public string Name;
	public int[] RamInit;
	public int[] InputInit;
	public int[] ExpectedOutput;
}

AppTest[] appTests = new AppTest[]{
	new AppTest{
		Name = "Test 1",
		RamInit = new int[]{3,0,4,0,99},// requires input, provides same output
		InputInit = new int []{777},
		ExpectedOutput = new int[]{777}
	},
	new AppTest{
		Name = "Test 2",
		RamInit = new[]{1002,4,3,4,33},
		InputInit = new int[0],
		ExpectedOutput = new int[0],
	},
	new AppTest{
		Name = "Test 3",
		RamInit = new []{1101,100,-1,4,0},
		InputInit = new int[0],
		ExpectedOutput = new int[0]
	},
	new AppTest{
		Name = "Producion Part 1 Air Conditioner",
		RamInit = _puzzleInput,
		InputInit = new int[]{1},
		ExpectedOutput = new int[]{0,0,0,0,0,0,0,0,0,11049715}
	},
	new AppTest{
		Name = "Part2 Test 1-1",
		RamInit = _p2t1Input,
		InputInit = new int[]{8},
		ExpectedOutput = new int[]{1},
	},
	new AppTest{
		Name = "Part2 Test 1-2",
		RamInit = _p2t1Input,
		InputInit = new int[]{4},
		ExpectedOutput = new int[]{0},
	},
	new AppTest{
		Name = "Part2 Test 2-1",
		RamInit = _p2t2Input,
		InputInit = new int[]{7},
		ExpectedOutput = new int[]{1},
	},
	new AppTest{
		Name = "Part2 Test 2-2",
		RamInit = _p2t2Input,
		InputInit = new int[]{8},
		ExpectedOutput = new int[]{0},
	},
	new AppTest{
		Name = "Part2 Test 2-3",
		RamInit = _p2t2Input,
		InputInit = new int[]{9},
		ExpectedOutput = new int[]{0},
	},
	new AppTest{
		Name = "Part2 Test 3-1",
		RamInit = _p2t3Input,
		InputInit = new int[]{8},
		ExpectedOutput = new int[]{1},
	},
	new AppTest{
		Name = "Part2 Test 3-2",
		RamInit = _p2t3Input,
		InputInit = new int[]{4},
		ExpectedOutput = new int[]{0},
	},
	new AppTest{
		Name = "Part2 Test 4-1",
		RamInit = _p2t4Input,
		InputInit = new int[]{7},
		ExpectedOutput = new int[]{1},
	},
	new AppTest{
		Name = "Part2 Test 4-2",
		RamInit = _p2t4Input,
		InputInit = new int[]{8},
		ExpectedOutput = new int[]{0},
	},
	new AppTest{
		Name = "Part2 Test 4-3",
		RamInit = _p2t4Input,
		InputInit = new int[]{9},
		ExpectedOutput = new int[]{0},
	},
	new AppTest{
		Name = "Producion Part 2 Thermal Radiator",
		RamInit = _puzzleInput,
		InputInit = new int[]{5},
		ExpectedOutput = new int[]{}
	},
	new AppTest{
		Name = "Jump1 Test 1",
		RamInit = _pt2j1Input,
		InputInit = new int[]{0},
		ExpectedOutput = new int[]{0},
	},
	new AppTest{
		Name = "Jump1 Test 2",
		RamInit = _pt2j1Input,
		InputInit = new int[]{2},
		ExpectedOutput = new int[]{1},
	},
	new AppTest{
		Name = "Jump2 Test 1",
		RamInit = _pt2j2Input,
		InputInit = new int[]{0},
		ExpectedOutput = new int[]{0},
	},
	new AppTest{
		Name = "Jump2 Test 2",
		RamInit = _pt2j2Input,
		InputInit = new int[]{2},
		ExpectedOutput = new int[]{1},
	},
		new AppTest{
		Name = "Jump3 Test 1",
		RamInit = _pt2j3Input,
		InputInit = new int[]{7},
		ExpectedOutput = new int[]{999},
	},
	new AppTest{
		Name = "Jump3 Test 2",
		RamInit = _pt2j3Input,
		InputInit = new int[]{8},
		ExpectedOutput = new int[]{1000},
	},

	new AppTest{
		Name = "Jump3 Test 3",
		RamInit = _pt2j3Input,
		InputInit = new int[]{9},
		ExpectedOutput = new int[]{1001},
	},
};
static Int32[] _pt2j3Input = new int[]{3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99};
static Int32[] _pt2j2Input = new int[]{3,3,1105,-1,9,1101,0,0,12,4,12,99,1};
static Int32[] _pt2j1Input = new int[]{3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9};
static Int32[] _p2t4Input = new int[] { 3, 3, 1107, -1, 8, 3, 4, 3, 99 };
static Int32[] _p2t3Input = new int[] { 3, 3, 1108, -1, 8, 3, 4, 3, 99 };
static Int32[] _p2t2Input = new int[] { 3, 9, 7, 9, 10, 9, 4, 9, 99, -1, 8 };
static Int32[] _p2t1Input = new int[] { 3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8 };
static Int32[] _puzzleInput = new int[]{3,225,1,225,6,6,1100,1,238,225,104,0,1,192,154,224,101,-161,224,224,4,224,102,8,223,223,101,5,224,224,1,223,224,223,1001,157,48,224,1001,224,-61,224,4,224,102,8,223,223,101,2,224,224,1,223,224,223,1102,15,28,225,1002,162,75,224,1001,224,-600,224,4,224,1002,223,8,223,1001,224,1,224,1,224,223,223,102,32,57,224,1001,224,-480,224,4,224,102,8,223,223,101,1,224,224,1,224,223,223,1101,6,23,225,1102,15,70,224,1001,224,-1050,224,4,224,1002,223,8,223,101,5,224,224,1,224,223,223,101,53,196,224,1001,224,-63,224,4,224,102,8,223,223,1001,224,3,224,1,224,223,223,1101,64,94,225,1102,13,23,225,1101,41,8,225,2,105,187,224,1001,224,-60,224,4,224,1002,223,8,223,101,6,224,224,1,224,223,223,1101,10,23,225,1101,16,67,225,1101,58,10,225,1101,25,34,224,1001,224,-59,224,4,224,1002,223,8,223,1001,224,3,224,1,223,224,223,4,223,99,0,0,0,677,0,0,0,0,0,0,0,0,0,0,0,1105,0,99999,1105,227,247,1105,1,99999,1005,227,99999,1005,0,256,1105,1,99999,1106,227,99999,1106,0,265,1105,1,99999,1006,0,99999,1006,227,274,1105,1,99999,1105,1,280,1105,1,99999,1,225,225,225,1101,294,0,0,105,1,0,1105,1,99999,1106,0,300,1105,1,99999,1,225,225,225,1101,314,0,0,106,0,0,1105,1,99999,1108,226,226,224,102,2,223,223,1005,224,329,101,1,223,223,107,226,226,224,1002,223,2,223,1005,224,344,1001,223,1,223,107,677,226,224,102,2,223,223,1005,224,359,101,1,223,223,7,677,226,224,102,2,223,223,1005,224,374,101,1,223,223,108,226,226,224,102,2,223,223,1006,224,389,101,1,223,223,1007,677,677,224,102,2,223,223,1005,224,404,101,1,223,223,7,226,677,224,102,2,223,223,1006,224,419,101,1,223,223,1107,226,677,224,1002,223,2,223,1005,224,434,1001,223,1,223,1108,226,677,224,102,2,223,223,1005,224,449,101,1,223,223,108,226,677,224,102,2,223,223,1005,224,464,1001,223,1,223,8,226,677,224,1002,223,2,223,1005,224,479,1001,223,1,223,1007,226,226,224,102,2,223,223,1006,224,494,101,1,223,223,1008,226,677,224,102,2,223,223,1006,224,509,101,1,223,223,1107,677,226,224,1002,223,2,223,1006,224,524,1001,223,1,223,108,677,677,224,1002,223,2,223,1005,224,539,1001,223,1,223,1107,226,226,224,1002,223,2,223,1006,224,554,1001,223,1,223,7,226,226,224,1002,223,2,223,1006,224,569,1001,223,1,223,8,677,226,224,102,2,223,223,1006,224,584,101,1,223,223,1008,677,677,224,102,2,223,223,1005,224,599,101,1,223,223,1007,226,677,224,1002,223,2,223,1006,224,614,1001,223,1,223,8,677,677,224,1002,223,2,223,1005,224,629,101,1,223,223,107,677,677,224,102,2,223,223,1005,224,644,101,1,223,223,1108,677,226,224,102,2,223,223,1005,224,659,101,1,223,223,1008,226,226,224,102,2,223,223,1006,224,674,1001,223,1,223,4,223,99,226};

void Init(Int32[] ram, Int32 noun, Int32 verb)
{
	ram[1] = noun;
	ram[2] = verb;
}

void Restore1202Alert(Int32[] ram)
{
	ram[1] = 12;
	ram[2] = 2;
}

// ram, ip=instruction pointer (index in ram)
void ProcessStep(Int32[] ram, Int32 ip, ref List<Int32> inputChannel, ref List<Int32> outputChannel)
{
	while (ip < ram.Length && ip >= 0)
	{
		Int32 fullOpCode = ram[ip++];
		Int32 Arg1Mode = 0;
		Int32 Arg2Mode = 0;
		Int32 Arg3Mode = 0;
		// ABCDE
		//  1002
		// 
		// DE - two - digit opcode,      02 == opcode 2
		//  C - mode of 1st parameter,  0 == position mode
		//  B - mode of 2nd parameter,  1 == immediate mode
		//  A - mode of 3rd parameter,  0 == position mode,
        //                                   omitted due to being a leading zer
		var opCode = fullOpCode%100;
		var flags = fullOpCode/100;
		Arg1Mode = flags%10;
		flags /= 10;
		Arg2Mode = flags%10;
		flags /= 10;
		Arg3Mode = flags%10;
		flags /= 10;// yagni - just in case we might get more parameters
		
		Int32 ReadByMode(Int32 mode){
			switch(mode){
				case 0:
					return ram[ram[ip++]];
				case 1:
					return ram[ip++];
				default:
					mode.Dump("invalid read mode");
					throw new Exception("Halt and Catch fire");
			}
		}
		void WriteByMode(Int32 mode, Int32 value)
		{
			switch (mode)
			{
				case 0:
					ram[ram[ip++]] = value;
					return;
				case 1:
					"Parameters that an instruction writes to will never be in immediate mode.".Dump();
					throw new Exception("Halt and Catch fire");
					//ram[ip++] = value;
					//return;
				default:
					mode.Dump("invalid read mode");
					throw new Exception("Halt and Catch fire");
			}
		}

		switch (opCode)
		{
			case 1:
				// add
				var add_lhs = ReadByMode(Arg1Mode);
				var add_rhs = ReadByMode(Arg2Mode);
				WriteByMode(Arg3Mode, add_lhs + add_rhs);
				break;
			case 2:
				// mul
				var mul_lhs = ReadByMode(Arg1Mode);
				var mul_rhs = ReadByMode(Arg2Mode);
				WriteByMode(Arg3Mode, mul_lhs * mul_rhs);
				break;
			case 3:
				// in
				var input = inputChannel[0];
				inputChannel.RemoveAt(0);
				WriteByMode(Arg1Mode, input);
				break;
			case 4:
				// out
				var output = ReadByMode(Arg1Mode);
				outputChannel.Add(output);
				break;
			case 5:
				// jnz
				var jnz_value = ReadByMode(Arg1Mode);
				if (jnz_value != 0)
				{
					var new_ip = ReadByMode(Arg2Mode);
					ip = new_ip;
				}
				else
				{
					// need to increment ip as we did not read arg2 yet
					++ip;
				}
				break;
			case 6:
				// jz
				var jz_value = ReadByMode(Arg1Mode);
				if (jz_value == 0)
				{
					var new_ip = ReadByMode(Arg2Mode);
					ip = new_ip;
				}else{
					// need to increment ip as we did not read arg2 yet
					++ip;
				}
				break;
			case 7:
				// lt
				var lt_lhs = ReadByMode(Arg1Mode);
				var lt_rhs = ReadByMode(Arg2Mode);
				var lt_cmp = lt_lhs < lt_rhs ? 1 : 0;// no cast - make it explicit
				WriteByMode(Arg3Mode, lt_cmp);
				break;
			case 8:
				// eq
				var eq_lhs = ReadByMode(Arg1Mode);
				var eq_rhs = ReadByMode(Arg2Mode);
				var eq_cmp = eq_lhs == eq_rhs ? 1 : 0;// no cast - make it explicit
				WriteByMode(Arg3Mode, eq_cmp);
				break;
			case 99:
				return;
			default:
				opCode.Dump("illegal opcode");
				throw new Exception("Halt and Catch fire");
		}
	}
	ip.Dump("Error");
}


const String spec = @"--- Day 5: Sunny with a Chance of Asteroids ---
You're starting to sweat as the ship makes its way toward Mercury. The Elves suggest that you get the air conditioner working by upgrading your ship computer to support the Thermal Environment Supervision Terminal.

The Thermal Environment Supervision Terminal (TEST) starts by running a diagnostic program (your puzzle input). The TEST diagnostic program will run on your existing Intcode computer after a few modifications:

First, you'll need to add two new instructions:

Opcode 3 takes a single integer as input and saves it to the position given by its only parameter. For example, the instruction 3,50 would take an input value and store it at address 50.
Opcode 4 outputs the value of its only parameter. For example, the instruction 4,50 would output the value at address 50.
Programs that use these instructions will come with documentation that explains what should be connected to the input and output. The program 3,0,4,0,99 outputs whatever it gets as input, then halts.

Second, you'll need to add support for parameter modes:

Each parameter of an instruction is handled based on its parameter mode. Right now, your ship computer already understands parameter mode 0, position mode, which causes the parameter to be interpreted as a position - if the parameter is 50, its value is the value stored at address 50 in memory. Until now, all parameters have been in position mode.

Now, your ship computer will also need to handle parameters in mode 1, immediate mode. In immediate mode, a parameter is interpreted as a value - if the parameter is 50, its value is simply 50.

Parameter modes are stored in the same value as the instruction's opcode. The opcode is a two-digit number based only on the ones and tens digit of the value, that is, the opcode is the rightmost two digits of the first value in an instruction. Parameter modes are single digits, one per parameter, read right-to-left from the opcode: the first parameter's mode is in the hundreds digit, the second parameter's mode is in the thousands digit, the third parameter's mode is in the ten-thousands digit, and so on. Any missing modes are 0.

For example, consider the program 1002,4,3,4,33.

The first instruction, 1002,4,3,4, is a multiply instruction - the rightmost two digits of the first value, 02, indicate opcode 2, multiplication. Then, going right to left, the parameter modes are 0 (hundreds digit), 1 (thousands digit), and 0 (ten-thousands digit, not present and therefore zero):

ABCDE
 1002

DE - two-digit opcode,      02 == opcode 2
 C - mode of 1st parameter,  0 == position mode
 B - mode of 2nd parameter,  1 == immediate mode
 A - mode of 3rd parameter,  0 == position mode,
                                  omitted due to being a leading zero
This instruction multiplies its first two parameters. The first parameter, 4 in position mode, works like it did before - its value is the value stored at address 4 (33). The second parameter, 3 in immediate mode, simply has value 3. The result of this operation, 33 * 3 = 99, is written according to the third parameter, 4 in position mode, which also works like it did before - 99 is written to address 4.

Parameters that an instruction writes to will never be in immediate mode.

Finally, some notes:

It is important to remember that the instruction pointer should increase by the number of values in the instruction after the instruction finishes. Because of the new instructions, this amount is no longer always 4.
Integers can be negative: 1101,100,-1,4,0 is a valid program (find 100 + -1, store the result in position 4).
The TEST diagnostic program will start by requesting from the user the ID of the system to test by running an input instruction - provide it 1, the ID for the ship's air conditioner unit.

It will then perform a series of diagnostic tests confirming that various parts of the Intcode computer, like parameter modes, function correctly. For each test, it will run an output instruction indicating how far the result of the test was from the expected value, where 0 means the test was successful. Non-zero outputs mean that a function is not working correctly; check the instructions that were run before the output instruction to see which one failed.

Finally, the program will output a diagnostic code and immediately halt. This final output isn't an error; an output followed immediately by a halt means the program finished. If all outputs were zero except the diagnostic code, the diagnostic program ran successfully.

After providing 1 to the only input instruction and passing all the tests, what diagnostic code does the program produce?

To begin, get your puzzle input.";