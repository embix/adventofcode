<Query Kind="Program" />

void Main()
{
	var cases = testcases;
	var meetingCount = 0;
	foreach(var pw in cases)
	{
		var isMeetingCriteria = IsMeetingCriteria(pw.Password);
		isMeetingCriteria.Dump($"{pw.Password} expected to meet criteria {pw.IsMeetingCriteria}");
		if(isMeetingCriteria){
			++meetingCount;
		}
	}
	meetingCount.Dump("that amount of meeting passwords were found");
}

Int32 puzzleMin = 100000;
Int32 puzzleMax = 999999;

Boolean IsMeetingCriteria(String password)
{
	// six digit number
	if(!Regex.IsMatch(password, @"\d{6}")) return false;
	
	var code = Int32.Parse(password);
	
	// is within range (inclusive?)
	if(code<puzzleMin || code>puzzleMax) return false;
	
	
	// todo: adjacent are same or increased
	if(!IsAdjacentSameOrIncreased(password)) return false;
	
	//never decreases
	if(!IsNeverDecreasing(password)) return false;
	
	return true;
}

Boolean IsAdjacentSameOrIncreased(String password)
{
	var i = 0;
	var currentValue = Int32.Parse(password.Substring(i, 1));
	while (++i < password.Length)
	{
		var nextValue = Int32.Parse(password.Substring(i, 1));
		if (!(
			   (nextValue == currentValue)
			|| (nextValue == currentValue + 1)))
		{
			return false;
		}
		currentValue = nextValue;
	}
	return true;
}

Boolean IsNeverDecreasing(String password)
{
	var i = 0;
	var currentValue = Int32.Parse(password.Substring(i,1));
	while(++i<password.Length)
	{
		var nextValue = Int32.Parse(password.Substring(i,1));
		if(nextValue<currentValue) return false;
		currentValue = nextValue;
	}
	return true;
}

struct TestCase{
	public String Password;
	public Boolean IsMeetingCriteria;
}

TestCase[] testcases = new TestCase[]{
	new TestCase{
		Password = "111111",
		IsMeetingCriteria = true,
	},
	new TestCase{
		Password = "223450",
		IsMeetingCriteria = false,
	},
	new TestCase{
		Password = "123789",
		IsMeetingCriteria = false,
	}
};

// Define other methods, classes and namespaces here
const String spec = @"--- Day 4: Secure Container ---
You arrive at the Venus fuel depot only to discover it's protected by a password. The Elves had written the password on a sticky note, but someone threw it out.

However, they do remember a few key facts about the password:

It is a six-digit number.
The value is within the range given in your puzzle input.
Two adjacent digits are the same (like 22 in 122345).
Going from left to right, the digits never decrease; they only ever increase or stay the same (like 111123 or 135679).
Other than the range rule, the following are true:

111111 meets these criteria (double 11, never decreases).
223450 does not meet these criteria (decreasing pair of digits 50).
123789 does not meet these criteria (no double).
How many different passwords within the range given in your puzzle input meet these criteria?

Your puzzle input is 152085-670283.";