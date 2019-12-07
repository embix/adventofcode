<Query Kind="Program">
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
</Query>

void Main()
{
	var testCases = new[]{
		test1, test2 // both tests took 1m07s on my i3770
		,part2
	};
	foreach(var test in testCases){
		var permutations = GetAllPermutations();
		var maxPerfomer = permutations.AsParallel().Select(s => new
		{
			Sequence = s,
			FinalOutput = GetFinalOutputOf(s, test.Program),
		}).OrderByDescending(p => p.FinalOutput).First();
		
		var result = new {
			test.ExpectedSequence,
			maxPerfomer.Sequence,
			SequencesMatch = String.Join("-",test.ExpectedSequence)==String.Join("-",maxPerfomer.Sequence),
			test.ExpectedMax,
			maxPerfomer.FinalOutput,
			MaxMatch = test.ExpectedMax==maxPerfomer.FinalOutput,
		};
		var testOkay = result.SequencesMatch&&result.MaxMatch;
		result.Dump(testOkay?"Test Passed":"Test Failed");
	}
}

public List<Int32[]> GetAllPermutations()
{
	var permuatitions = Permutate(new[]{5,6,7,8,9}, new Int32[0]);
	return permuatitions.Select(p=>p.ToArray()).ToList();
}

public List<List<Int32>> Permutate(Int32[] remainingItems, Int32[] currentChain)
{
	var results = new List<List<Int32>>();
	if(remainingItems.Length==0){
		results.Add(currentChain.ToList());
		return results;
	}

	foreach(var item in remainingItems)
	{
		var itemChain = currentChain.ToList();// copy
		itemChain.Add(item);
		var itemRemainings = remainingItems.Except(new []{item}).ToArray();
		var partResults = Permutate(itemRemainings, itemChain.ToArray());
		results.AddRange(partResults);
	}
	return results;
}

public Int32 GetFinalOutputOf(Int32[] sequence, Int32[] program)
{
	Int32 latestAmp5Output=0;// its also amp1's first input aka value input
	Boolean shovelRequired = true;// so we can cancel our shovel task
	
	// wire up by hand so we dont mess it up with off-by-1 automation
	var qShovelA = new BlockingCollection<Int32>();
	var qAB = new BlockingCollection<Int32>();
	var qBC = new BlockingCollection<Int32>();
	var qCD = new BlockingCollection<Int32>();
	var qDE = new BlockingCollection<Int32>();
	var qEShovel = new BlockingCollection<Int32>();
		
	// set up sequence inputs:
	qShovelA.Add(sequence[0]);
	qAB.Add(sequence[1]);
	qBC.Add(sequence[2]);
	qCD.Add(sequence[3]);
	qDE.Add(sequence[4]);
	qEShovel.Add(latestAmp5Output);
	
	var shovelTask = Task.Run(()=>{
		while(shovelRequired)// stop if all cpu's haltet
		{
			var eOutput = qEShovel.Take();
			latestAmp5Output = eOutput;// save for later
			qShovelA.Add(eOutput);
		}
	});
	
	var aTask = Task.Run(()=>{
		Int32[] ram = program.ToList().ToArray();// work on copy
		ProcessConcurrently(ram, 0, qShovelA, qAB);
	});
	var bTask = Task.Run(() =>	{
		Int32[] ram = program.ToList().ToArray();// work on copy
		ProcessConcurrently(ram, 0, qAB, qBC);
	});
	var cTask = Task.Run(() => {
		Int32[] ram = program.ToList().ToArray();// work on copy
		ProcessConcurrently(ram, 0, qBC, qCD);
	});
	var dTask = Task.Run(() => {
		Int32[] ram = program.ToList().ToArray();// work on copy
		ProcessConcurrently(ram, 0, qCD, qDE);
	});
	var eTask = Task.Run(() => {
		Int32[] ram = program.ToList().ToArray();// work on copy
		ProcessConcurrently(ram, 0, qDE, qEShovel);
	});

	Task.WaitAll(new[]{aTask,bTask,cTask,dTask,eTask});
	shovelRequired = false;
	return latestAmp5Output;
}

struct TestCase{
	public Int32[] Program;
	public Int32 ExpectedMax;
	public Int32[] ExpectedSequence;	
}

TestCase test1 = new TestCase
{
	Program = new[]{3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5},
	ExpectedMax = 139629729,
	ExpectedSequence = new []{9,8,7,6,5},
};

TestCase test2 = new TestCase
{
	Program = new[] { 3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10},
	ExpectedMax = 18216,
	ExpectedSequence = new[] {9,7,8,5,6 },
};
TestCase part2 = new TestCase{
	Program = new []{3,8,1001,8,10,8,105,1,0,0,21,34,51,68,89,98,179,260,341,422,99999,3,9,1001,9,4,9,102,4,9,9,4,9,99,3,9,1002,9,5,9,1001,9,2,9,1002,9,2,9,4,9,99,3,9,1001,9,3,9,102,3,9,9,101,4,9,9,4,9,99,3,9,102,2,9,9,101,2,9,9,1002,9,5,9,1001,9,2,9,4,9,99,3,9,102,2,9,9,4,9,99,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,3,9,1001,9,2,9,4,9,99,3,9,101,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1002,9,2,9,4,9,3,9,102,2,9,9,4,9,99,3,9,1001,9,1,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,1,9,4,9,3,9,1002,9,2,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,1,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,101,2,9,9,4,9,3,9,101,2,9,9,4,9,99,3,9,1002,9,2,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,102,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,101,2,9,9,4,9,3,9,1001,9,2,9,4,9,3,9,102,2,9,9,4,9,3,9,1002,9,2,9,4,9,3,9,101,1,9,9,4,9,99},
	ExpectedMax = -1,
	ExpectedSequence = Array.Empty<Int32>(),
};

// ram, ip=instruction pointer (index in ram)
//void ProcessStep(Int32[] ram, Int32 ip, ref List<Int32> inputChannel, ref List<Int32> outputChannel)
void ProcessConcurrently(Int32[] ram, Int32 ip, BlockingCollection<Int32> inputChannel, BlockingCollection<Int32> outputChannel)
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
				var input = inputChannel.Take();// remember: now it's blocking!
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


const String spec = @"--- Day 7: Amplification Circuit ---
Based on the navigational maps, you're going to need to send more power to your ship's thrusters to reach Santa in time. To do this, you'll need to configure a series of amplifiers already installed on the ship.

There are five amplifiers connected in series; each one receives an input signal and produces an output signal. They are connected such that the first amplifier's output leads to the second amplifier's input, the second amplifier's output leads to the third amplifier's input, and so on. The first amplifier's input value is 0, and the last amplifier's output leads to your ship's thrusters.

    O-------O  O-------O  O-------O  O-------O  O-------O
0 ->| Amp A |->| Amp B |->| Amp C |->| Amp D |->| Amp E |-> (to thrusters)
    O-------O  O-------O  O-------O  O-------O  O-------O
The Elves have sent you some Amplifier Controller Software (your puzzle input), a program that should run on your existing Intcode computer. Each amplifier will need to run a copy of the program.

When a copy of the program starts running on an amplifier, it will first use an input instruction to ask the amplifier for its current phase setting (an integer from 0 to 4). Each phase setting is used exactly once, but the Elves can't remember which amplifier needs which phase setting.

The program will then call another input instruction to get the amplifier's input signal, compute the correct output signal, and supply it back to the amplifier with an output instruction. (If the amplifier has not yet received an input signal, it waits until one arrives.)

Your job is to find the largest output signal that can be sent to the thrusters by trying every possible combination of phase settings on the amplifiers. Make sure that memory is not shared or reused between copies of the program.

For example, suppose you want to try the phase setting sequence 3,1,2,4,0, which would mean setting amplifier A to phase setting 3, amplifier B to setting 1, C to 2, D to 4, and E to 0. Then, you could determine the output signal that gets sent from amplifier E to the thrusters with the following steps:

Start the copy of the amplifier controller software that will run on amplifier A. At its first input instruction, provide it the amplifier's phase setting, 3. At its second input instruction, provide it the input signal, 0. After some calculations, it will use an output instruction to indicate the amplifier's output signal.
Start the software for amplifier B. Provide it the phase setting (1) and then whatever output signal was produced from amplifier A. It will then produce a new output signal destined for amplifier C.
Start the software for amplifier C, provide the phase setting (2) and the value from amplifier B, then collect its output signal.
Run amplifier D's software, provide the phase setting (4) and input value, and collect its output signal.
Run amplifier E's software, provide the phase setting (0) and input value, and collect its output signal.
The final output signal from amplifier E would be sent to the thrusters. However, this phase setting sequence may not have been the best one; another sequence might have sent a higher signal to the thrusters.

Here are some example programs:

Max thruster signal 43210 (from phase setting sequence 4,3,2,1,0):

3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0
Max thruster signal 54321 (from phase setting sequence 0,1,2,3,4):

3,23,3,24,1002,24,10,24,1002,23,-1,23,
101,5,23,23,1,24,23,23,4,23,99,0,0
Max thruster signal 65210 (from phase setting sequence 1,0,4,3,2):

3,31,3,32,1002,32,10,32,1001,31,-2,31,1007,31,0,33,
1002,33,7,33,1,33,31,31,1,32,31,31,4,31,99,0,0,0
Try every combination of phase settings on the amplifiers. What is the highest signal that can be sent to the thrusters?";

const String specPart2 = @"--- Part Two ---
It's no good - in this configuration, the amplifiers can't generate a large enough output signal to produce the thrust you'll need. The Elves quickly talk you through rewiring the amplifiers into a feedback loop:

      O-------O  O-------O  O-------O  O-------O  O-------O
0 -+->| Amp A |->| Amp B |->| Amp C |->| Amp D |->| Amp E |-.
   |  O-------O  O-------O  O-------O  O-------O  O-------O |
   |                                                        |
   '--------------------------------------------------------+
                                                            |
                                                            v
                                                     (to thrusters)
Most of the amplifiers are connected as they were before; amplifier A's output is connected to amplifier B's input, and so on. However, the output from amplifier E is now connected into amplifier A's input. This creates the feedback loop: the signal will be sent through the amplifiers many times.

In feedback loop mode, the amplifiers need totally different phase settings: integers from 5 to 9, again each used exactly once. These settings will cause the Amplifier Controller Software to repeatedly take input and produce output many times before halting. Provide each amplifier its phase setting at its first input instruction; all further input/output instructions are for signals.

Don't restart the Amplifier Controller Software on any amplifier during this process. Each one should continue receiving and sending signals until it halts.

All signals sent or received in this process will be between pairs of amplifiers except the very first signal and the very last signal. To start the process, a 0 signal is sent to amplifier A's input exactly once.

Eventually, the software on the amplifiers will halt after they have processed the final loop. When this happens, the last output signal from amplifier E is sent to the thrusters. Your job is to find the largest output signal that can be sent to the thrusters using the new phase settings and feedback loop arrangement.

Here are some example programs:

Max thruster signal 139629729 (from phase setting sequence 9,8,7,6,5):

3,26,1001,26,-4,26,3,27,1002,27,2,27,1,27,26,
27,4,27,1001,28,-1,28,1005,28,6,99,0,0,5
Max thruster signal 18216 (from phase setting sequence 9,7,8,5,6):

3,52,1001,52,-5,52,3,53,1,52,56,54,1007,54,5,55,1005,55,26,1001,54,
-5,54,1105,1,12,1,53,54,53,1008,54,0,55,1001,55,1,55,2,53,55,53,4,
53,1001,56,-1,56,1005,56,6,99,0,0,0,0,10
Try every combination of the new phase settings on the amplifier feedback loop. What is the highest signal that can be sent to the thrusters?";