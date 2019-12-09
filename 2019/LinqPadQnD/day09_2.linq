<Query Kind="Program">
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Numerics</Namespace>
</Query>

//#define DUMP
void Main()
{
	var testCases = new[]{
		//quine,
        //test2,
        //test3,
        //part1,
        part2
	};
    foreach(var testCase in testCases)
    {
        var passed = testCase.HasTestPassed();        
    }
}

static BlockingCollection<TWord> GetNewChannel<TWord>(Int32 blockingBufferSize=1)
{
	return new BlockingCollection<TWord>(new ConcurrentQueue<TWord>(), blockingBufferSize);
}

// could be made compatible with our old Int32[] ram
//interface IRam<TWord>
//{
//	TWord GetAt(TWord index);
//	void SetAt(TWord index, TWord content);
//	
//	TWord this[TWord index]
//	{
//		// requires C#8/netcore3.0+ (supported in LINQPad 6, VS2019.4, VSCode)
//		get { return GetAt(index); }
//		set { SetAt(index, value); }
//	}
//	
//	IRam<TWord> CreateCopy();
//}

class BigRam // : IRam<BigInteger>
{
	Dictionary<BigInteger,BigInteger> _content;
	
	public BigRam(BigInteger[] content)
	{
		_content = content
            .Select((cellValue,cellIndex)=>new{
                CellValue=cellValue,
                CellIndex=(BigInteger)cellIndex}
            )
		    .ToDictionary(cell=>cell.CellIndex, cell=>cell.CellValue);
	}
	
	public BigRam(Dictionary<BigInteger,BigInteger> content)
	{
		_content = content;
	}
	
	public BigRam CreateCopy()
	{
		return new BigRam(
			_content.Select(kvp=>new{kvp.Key,kvp.Value})
			        .ToDictionary(cell=>cell.Key, cell=>cell.Value));
	}

	public BigInteger GetAt(BigInteger index)
	{
		// persist every touched cell (even if 0==empty)
		BigInteger value = 0;
		if(!_content.TryGetValue(index, out value))
		{
			_content[index] = value;
		}
		return value;
	}

	public void SetAt(BigInteger index, BigInteger content)
	{
		// persist every touched cell (even if 0==empty)
		_content[index] = content;
	}

	public BigInteger this[BigInteger index]
	{
		// requires C#8/netcore3.0+ (supported in LINQPad 6, VS2019.4, VSCode)
		get { return GetAt(index); }
		set { SetAt(index, value); }
	}
    
    public void DumpStoredAddresses(String msg = null){
        _content.Select(kvp=>new{
            Address=kvp.Key,
            Value=kvp.Value
        }).OrderBy(a=>a.Address).Dump(msg);
    }
}

struct TestCase{
	public BigInteger[] Program;
    public Func<BigInteger[], Boolean> _testPassed;
    public Boolean HasTestPassed(){return _testPassed(Program);}
}

TestCase quine = new TestCase{
    Program = new BigInteger[]{109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99},
    _testPassed = (prg)=>{
        var ram = new BigRam(prg);
        IInputChannel inputs = new EmptyInputChannel();
        IOutputChannel outputs = new ConsoleOutputChannel();
        #if DUMP
        ram.DumpStoredAddresses("Ram before");
        #endif
        Process(ram, 0, inputs, outputs);
        #if DuMP
        ram.DumpStoredAddresses("Ram after");
        #endif
        
        //foreach(var expected in prg){
        //    if(outputs.TryTake(out var got)){
        //        if (expected != got)
        //        {
        //            $"expected {expected} but got {got}".Dump("FAIL");
        //            return false;
        //        }
        //    }
        //    else{
        //        "expected more outputs".Dump("FAIL");
        //        return false;
        //    }            
        //}
        //var passed = !outputs.Any();
        //return passed;
        return true;
    },    
};
TestCase test2 = new TestCase{
    Program = new BigInteger[]{1102,34915192,34915192,7,4,7,99,0},
    _testPassed = (prg) =>
    {
        var ram = new BigRam(prg);
        IInputChannel inputs = new EmptyInputChannel();
        IOutputChannel outputs = new ConsoleOutputChannel();
        Process(ram, 0, inputs, outputs);
        return true;
    }
};
TestCase test3 = new TestCase{
    Program = new BigInteger[]{104,1125899906842624,99},
    _testPassed = (prg)=>{
        var ram = new BigRam(prg);
        IInputChannel inputs = new EmptyInputChannel();
        IOutputChannel outputs = new ConsoleOutputChannel();
        Process(ram, 0, inputs, outputs);
        return true;
    }
};

static BigInteger[] _puzzleInput = new BigInteger[]{1102,34463338,34463338,63,1007,63,34463338,63,1005,63,53,1101,3,0,1000,109,988,209,12,9,1000,209,6,209,3,203,0,1008,1000,1,63,1005,63,65,1008,1000,2,63,1005,63,904,1008,1000,0,63,1005,63,58,4,25,104,0,99,4,0,104,0,99,4,17,104,0,99,0,0,1101,37,0,1013,1101,426,0,1027,1101,36,0,1000,1101,0,606,1023,1102,34,1,1011,1102,1,712,1029,1102,1,27,1007,1101,831,0,1024,1102,32,1,1002,1102,1,1,1021,1101,429,0,1026,1102,1,826,1025,1101,0,717,1028,1102,1,20,1018,1101,0,24,1004,1102,31,1,1009,1101,22,0,1015,1102,38,1,1014,1102,613,1,1022,1102,29,1,1017,1102,0,1,1020,1102,1,21,1008,1102,33,1,1012,1101,0,30,1006,1101,0,28,1016,1102,1,26,1005,1102,35,1,1019,1101,25,0,1003,1102,1,23,1001,1102,1,39,1010,109,-3,2102,1,5,63,1008,63,34,63,1005,63,205,1001,64,1,64,1106,0,207,4,187,1002,64,2,64,109,-2,1201,7,0,63,1008,63,34,63,1005,63,227,1105,1,233,4,213,1001,64,1,64,1002,64,2,64,109,21,21102,40,1,3,1008,1019,37,63,1005,63,257,1001,64,1,64,1106,0,259,4,239,1002,64,2,64,109,-4,21101,41,0,2,1008,1014,38,63,1005,63,279,1105,1,285,4,265,1001,64,1,64,1002,64,2,64,109,-10,1201,4,0,63,1008,63,30,63,1005,63,307,4,291,1105,1,311,1001,64,1,64,1002,64,2,64,109,6,1207,0,22,63,1005,63,329,4,317,1105,1,333,1001,64,1,64,1002,64,2,64,109,-5,1207,5,20,63,1005,63,353,1001,64,1,64,1106,0,355,4,339,1002,64,2,64,109,8,2108,29,-5,63,1005,63,375,1001,64,1,64,1105,1,377,4,361,1002,64,2,64,109,15,1206,-6,395,4,383,1001,64,1,64,1105,1,395,1002,64,2,64,109,-11,21107,42,43,4,1005,1019,413,4,401,1106,0,417,1001,64,1,64,1002,64,2,64,109,6,2106,0,6,1105,1,435,4,423,1001,64,1,64,1002,64,2,64,109,-15,1208,-3,24,63,1005,63,455,1001,64,1,64,1105,1,457,4,441,1002,64,2,64,109,-13,1208,10,25,63,1005,63,475,4,463,1106,0,479,1001,64,1,64,1002,64,2,64,109,21,21108,43,42,3,1005,1017,495,1106,0,501,4,485,1001,64,1,64,1002,64,2,64,109,-14,2107,31,2,63,1005,63,519,4,507,1106,0,523,1001,64,1,64,1002,64,2,64,109,-4,1202,8,1,63,1008,63,24,63,1005,63,549,4,529,1001,64,1,64,1105,1,549,1002,64,2,64,109,1,2108,23,4,63,1005,63,567,4,555,1105,1,571,1001,64,1,64,1002,64,2,64,109,2,2101,0,5,63,1008,63,21,63,1005,63,591,1105,1,597,4,577,1001,64,1,64,1002,64,2,64,109,28,2105,1,-4,1001,64,1,64,1105,1,615,4,603,1002,64,2,64,109,-10,1205,4,633,4,621,1001,64,1,64,1106,0,633,1002,64,2,64,109,2,1206,2,645,1106,0,651,4,639,1001,64,1,64,1002,64,2,64,109,-4,1202,-6,1,63,1008,63,28,63,1005,63,671,1105,1,677,4,657,1001,64,1,64,1002,64,2,64,109,-9,21102,44,1,4,1008,1010,44,63,1005,63,699,4,683,1105,1,703,1001,64,1,64,1002,64,2,64,109,31,2106,0,-9,4,709,1105,1,721,1001,64,1,64,1002,64,2,64,109,-30,21108,45,45,6,1005,1013,743,4,727,1001,64,1,64,1106,0,743,1002,64,2,64,109,2,21101,46,0,3,1008,1012,46,63,1005,63,765,4,749,1106,0,769,1001,64,1,64,1002,64,2,64,109,-5,2101,0,0,63,1008,63,24,63,1005,63,795,4,775,1001,64,1,64,1105,1,795,1002,64,2,64,109,6,2107,32,-1,63,1005,63,815,1001,64,1,64,1106,0,817,4,801,1002,64,2,64,109,19,2105,1,-5,4,823,1106,0,835,1001,64,1,64,1002,64,2,64,109,-12,21107,47,46,-1,1005,1016,851,1105,1,857,4,841,1001,64,1,64,1002,64,2,64,109,-2,1205,5,873,1001,64,1,64,1105,1,875,4,863,1002,64,2,64,109,-6,2102,1,-8,63,1008,63,23,63,1005,63,897,4,881,1105,1,901,1001,64,1,64,4,64,99,21101,0,27,1,21101,0,915,0,1106,0,922,21201,1,44808,1,204,1,99,109,3,1207,-2,3,63,1005,63,964,21201,-2,-1,1,21101,942,0,0,1105,1,922,21201,1,0,-1,21201,-2,-3,1,21102,957,1,0,1105,1,922,22201,1,-1,-2,1106,0,968,21202,-2,1,-2,109,-3,2105,1,0};
TestCase part1 = new TestCase{
    Program = _puzzleInput,
    _testPassed = (prg)=>{
        var ram = new BigRam(prg);
        IInputChannel inputs = new StaticInputChannel(new BigInteger[]{1});
        IOutputChannel outputs = new ConsoleOutputChannel();
        Process(ram, 0, inputs, outputs);
        return true;
    }
};
TestCase part2 = new TestCase
{
    Program = _puzzleInput,
    _testPassed = (prg) =>
    {
        var ram = new BigRam(prg);
        IInputChannel inputs = new StaticInputChannel(new BigInteger[] { 2 });
        IOutputChannel outputs = new ConsoleOutputChannel();
        Process(ram, 0, inputs, outputs);
        return true;
    }
};

interface IInputChannel { 
    BigInteger Take();
    Boolean TryTake(out BigInteger input);
}
interface IOutputChannel { 
    void Add(BigInteger output);
}
class EmptyInputChannel : IInputChannel {
    public BigInteger Take()
    {
        throw new Exception("Empty input channel");
    }
    public Boolean TryTake(out BigInteger input)
    {
        input = 0;
        return false;
    }
}
class StaticInputChannel : IInputChannel 
{
    Int32 _nextPos;
    BigInteger[] _inputs;
    
    public StaticInputChannel(BigInteger[] inputs)
    {
        _inputs = inputs;
        _nextPos = 0;
    }
    public BigInteger Take()
    {
        return _inputs[_nextPos++];
    }
    public Boolean TryTake(out BigInteger input)
    {
        input = 0;
        if(_nextPos<_inputs.Length){
            input = _inputs[_nextPos++];
            return true;
        }
        return false;
    }

}
public class ConsoleOutputChannel : IOutputChannel
{
    public void Add(BigInteger output){
        Console.WriteLine(output.ToString());
    }
}

static void Process(BigRam ram, BigInteger ip, IInputChannel inputChannel, IOutputChannel outputChannel)// where TWord : INumeric WHEN? Microsoft - WHEN?
{
    BigInteger relativeBase = 0;
	while (ip >= 0)
	{        
		BigInteger fullOpCode = ram[ip];
        ip+=1;
		BigInteger Arg1Mode = 0;
		BigInteger Arg2Mode = 0;
		BigInteger Arg3Mode = 0;
		// ABCDE
		//  1002
		// 
		// DE - two - digit opcode,      02 == opcode 2
		//  C - mode of 1st parameter,  0 == position mode
		//  B - mode of 2nd parameter,  1 == immediate mode
		//  A - mode of 3rd parameter,  0 == position mode,
        //                                   omitted due to being a leading zer
		var bigOpCode = fullOpCode%100;
		var flags = fullOpCode/100;
		Arg1Mode = flags%10;
		flags /= 10;
		Arg2Mode = flags%10;
		flags /= 10;
		Arg3Mode = flags%10;
		flags /= 10;// yagni - just in case we might get more parameters
		
		BigInteger ReadByMode(BigInteger bigMode){
            var mode = 0;
            if(bigMode<Int32.MaxValue && bigMode>=0){
                mode = (Int32) bigMode;
            }
			switch(mode){
				case 0:
					// Indirect Mode
                    var indirectPos = ip;
                    ip+=1;
					return ram[ram[indirectPos]];
				case 1:
                    // Immediate Mode
                    var immediatePos = ip;
                    ip += 1;
                    return ram[immediatePos];
				case 2:
                    // Relative Mode
                    var relativePos = ip;
                    ip += 1;
                    return ram[relativeBase+ram[relativePos]];
				default:
					mode.Dump("invalid read mode");
					throw new Exception("Halt and Catch fire");
			}
		}
		void WriteByMode(BigInteger bigMode, BigInteger value)
        {
            var mode = 0;
            if (bigMode < Int32.MaxValue && bigMode >= 0)
            {
                mode = (Int32)bigMode;
            }
            switch (mode)
			{
				case 0:
                    var indirectPos = ip;
                    ip+=1;
					ram[ram[indirectPos]] = value;
					return;
				case 1:
					"Parameters that an instruction writes to will never be in immediate mode.".Dump();
					throw new Exception("Halt and Catch fire");
                    //var immediatePos = ip;
                    //ip+=1;
					//ram[immediatePos] = value;
					//return;
                case 2:
                    var relativePos = ip;
                    ip+=1;
                    ram[relativeBase+ram[relativePos]] = value;
                    return;
				default:
					mode.Dump("invalid read mode");
					throw new Exception("Halt and Catch fire");
			}
		}
        
        var opCode = 0;
        if(bigOpCode>=0 && bigOpCode<=Int32.MaxValue){
            opCode = (Int32) bigOpCode;
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
            case 9:
                // adjust relative base
                var increment = ReadByMode(Arg1Mode);
                relativeBase += increment;
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


const String spec = @"--- Day 9: Sensor Boost ---
You've just said goodbye to the rebooted rover and left Mars when you receive a faint distress signal coming from the asteroid belt. It must be the Ceres monitoring station!

In order to lock on to the signal, you'll need to boost your sensors. The Elves send up the latest BOOST program - Basic Operation Of System Test.

While BOOST (your puzzle input) is capable of boosting your sensors, for tenuous safety reasons, it refuses to do so until the computer it runs on passes some checks to demonstrate it is a complete Intcode computer.

Your existing Intcode computer is missing one key feature: it needs support for parameters in relative mode.

Parameters in mode 2, relative mode, behave very similarly to parameters in position mode: the parameter is interpreted as a position. Like position mode, parameters in relative mode can be read from or written to.

The important difference is that relative mode parameters don't count from address 0. Instead, they count from a value called the relative base. The relative base starts at 0.

The address a relative mode parameter refers to is itself plus the current relative base. When the relative base is 0, relative mode parameters and position mode parameters with the same value refer to the same address.

For example, given a relative base of 50, a relative mode parameter of -7 refers to memory address 50 + -7 = 43.

The relative base is modified with the relative base offset instruction:

Opcode 9 adjusts the relative base by the value of its only parameter. The relative base increases (or decreases, if the value is negative) by the value of the parameter.
For example, if the relative base is 2000, then after the instruction 109,19, the relative base would be 2019. If the next instruction were 204,-34, then the value at address 1985 would be output.

Your Intcode computer will also need a few other capabilities:

The computer's available memory should be much larger than the initial program. Memory beyond the initial program starts with the value 0 and can be read or written like any other memory. (It is invalid to try to access memory at a negative address, though.)
The computer should have support for large numbers. Some instructions near the beginning of the BOOST program will verify this capability.
Here are some example programs that use these features:

109,1,204,-1,1001,100,1,100,1008,100,16,101,1006,101,0,99 takes no input and produces a copy of itself as output.
1102,34915192,34915192,7,4,7,99,0 should output a 16-digit number.
104,1125899906842624,99 should output the large number in the middle.
The BOOST program will ask for a single input; run it in test mode by providing it the value 1. It will perform a series of checks on each opcode, output any opcodes (and the associated parameter modes) that seem to be functioning incorrectly, and finally output a BOOST keycode.

Once your Intcode computer is fully functional, the BOOST program should report no malfunctioning opcodes when run in test mode; it should only output a single value, the BOOST keycode. What BOOST keycode does it produce?";

const String spec2 = @"--- Part Two ---
You now have a complete Intcode computer.

Finally, you can lock on to the Ceres distress signal! You just need to boost your sensors using the BOOST program.

The program runs in sensor boost mode by providing the input instruction the value 2. Once run, it will boost the sensors automatically, but it might take a few seconds to complete the operation on slower hardware. In sensor boost mode, the program will output a single value: the coordinates of the distress signal.

Run the BOOST program in sensor boost mode. What are the coordinates of the distress signal?";