<Query Kind="Program" />

void Main()
{
	var cases = 
		//testcases;
		Enumerable.Range(puzzleMin, puzzleMax-puzzleMin+1)
		.Select(i=>new TestCase{
			Password = i.ToString()
		}).ToList();
	cases.Last().Dump();
	var meetingCount = 0;
	foreach(var pw in cases)
	{
		var isMeetingCriteria = IsMeetingCriteria(pw.Password);
		//isMeetingCriteria.Dump($"{pw.Password} expected to meet criteria {pw.IsMeetingCriteria}");
		if(isMeetingCriteria){
			++meetingCount;
		}
	}
	meetingCount.Dump("that amount of meeting passwords were found");
}

Int32 puzzleMin = 152085;
Int32 puzzleMax = 670283;

Boolean IsMeetingCriteria(String password)
{
	// six digit number
	if(!Regex.IsMatch(password, @"\d{6}")) return false;
	
	var code = Int32.Parse(password);
	
	// is within range (inclusive?)
	if(code<puzzleMin || code>puzzleMax) return false;
	
	
	// new double rule
	if(!ContainsTrueDouble(password)) return false;
	
	//never decreases
	if(!IsNeverDecreasing(password)) return false;
	
	return true;
}

Boolean ContainsTrueDouble(String password)
{
	var sameChunks = new List<String>();
	var i = 0;
	var currentLetter = password.Substring(i,1);
	var currentString = currentLetter;
	while(++i < password.Length)
	{
		var nextLetter = password.Substring(i,1);
		if(nextLetter == currentLetter)
		{
			// same: add
			currentString+=nextLetter;			
		}
		else
		{
			// different: close
			sameChunks.Add(currentString);
			currentString = nextLetter;			
		}
		// either way: remember
		currentLetter = nextLetter;
	}
	sameChunks.Add(currentString);
	return sameChunks.Any(c=>c.Length==2);
}

// not quite what the spec wants
Boolean HasAdjacentSameButNoTripel(String password)
{
	var i = 0;
	var currentValue = Int32.Parse(password.Substring(i, 1));
	var currentIsDouble = false;
	var doubleCount = 0;
	while (++i < password.Length)
	{
		var nextValue = Int32.Parse(password.Substring(i, 1));
		if(nextValue == currentValue)
		{
			if(currentIsDouble){
				return false;// tripel found!
			}else{
				currentIsDouble = true;
				++doubleCount;
			}
		}else{
			currentIsDouble = false;
		}
		currentValue = nextValue;
	}
	return doubleCount>0;
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

const String spec2 = @"--- Day 4: Secure Container ---
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

Your puzzle answer was 1764.

The first half of this puzzle is complete! It provides one gold star: *

--- Part Two ---
An Elf just remembered one more important detail: the two adjacent matching digits are not part of a larger group of matching digits.

Given this additional criterion, but still ignoring the range rule, the following are now true:

112233 meets these criteria because the digits never decrease and all repeated digits are exactly two digits long.
123444 no longer meets the criteria (the repeated 44 is part of a larger group of 444).
111122 meets the criteria (even though 1 is repeated more than twice, it still contains a double 22).
How many different passwords within the range given in your puzzle input meet all of the criteria?

Your puzzle input is still 152085-670283.";