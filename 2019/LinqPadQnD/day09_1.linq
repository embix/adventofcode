<Query Kind="Program">
  <Namespace>System.Collections.Concurrent</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <Namespace>System.Numerics</Namespace>
</Query>

void Main()
{
	var testCases = new[]{
		quine,
        //test2,
        //test3
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
        ram.DumpStoredAddresses("Ram before");
        Process(ram, 0, inputs, outputs);
        ram.DumpStoredAddresses("Ram after");
        
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
public class ConsoleOutputChannel : IOutputChannel
{
    public void Add(BigInteger output){
        Console.WriteLine(output.ToString());
    }
}

static void Process(BigRam ram, BigInteger ip, IInputChannel inputChannel, IOutputChannel outputChannel)// where TWord : INumeric WHEN? Microsoft - WHEN?
{
	while (ip >= 0)
	{
        BigInteger relativeBase = 0;
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