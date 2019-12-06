<Query Kind="Program" />

void Main()
{
	var testCase = TestCases
		.Where(t=>t.Name==
			"Part 1 Test"
		).Single();
	
	var soiTree = ParseAsSoiTree(testCase.MapInput);
	soiTree.Dump();
}

// "Sphere of influence" model
// see: https://wiki.kerbalspaceprogram.com/wiki/Sphere_of_influence
// should evaluate to the following graph:
//         G - H       J - K - L
//        /           /
// COM - B - C - D - E - F
//                \
//                 I
const String TestMapStringInput = @"COM)B
B)C
C)D
D)E
E)F
B)G
G)H
D)I
E)J
J)K
K)L";

struct TestCase {
	public String Name;
	public String MapInput;
	public int ExpectedOrbitCount;
}

TestCase[] TestCases = new TestCase[]{
	new TestCase{
		Name = "Part 1 Test",
		MapInput = TestMapStringInput,
		ExpectedOrbitCount = 42
	}
};

SoiTree ParseAsSoiTree(string multiLineInput){
	var root = CelestialBody.CreateUniversalCenterOfMass();
	var soiTree = new SoiTree{Root = root};
	var lines = multiLineInput.Split("\n");
	foreach(var line in lines)
	{
		var fromTo = line.Split(")");
		var from = fromTo[0].Trim();
		var to = fromTo[1].Trim();// solve \r\n related issues
		
		CelestialBody parent;
		var parentExists = soiTree.TryGet(from, out parent);
		if(!parentExists)
		{
			line.Dump($"Parent {from} does not exist in soi tree");
			soiTree.Dump("DEBUG: SOI Tree");
			throw new Exception("Halt and Catch Fire");
		}
		
		CelestialBody newBody;
		var newBodyExists = soiTree.TryGet(to, out newBody);
		if(newBodyExists)
		{
			line.Dump($"Cannot add {to} to orbit of {from} as it already directly orbits {newBody.Parent}");
			soiTree.Dump("DEBUG: SOI Tree");
			throw new Exception("Halt and Catch Fire");
		}
		
		parent.AddNewOrbitter(to);
	}
	return soiTree;
}

struct SoiTree
{
	public CelestialBody Root;
	
	public Boolean TryGet(String name, out CelestialBody body)
	{
		return Root.TryGet(name, out body);
	}
}

class CelestialBody {
	
	public readonly String Name;

	public readonly CelestialBody Parent;

	// only to help debigging using Dump()
	public String ParentName => Parent?.Name;

	// or planets, moons, (natural) satellites
	public IEnumerable<CelestialBody> Orbitters => _orbitters;
	private readonly List<CelestialBody> _orbitters;
	
	public static CelestialBody CreateUniversalCenterOfMass()
	{
		return new CelestialBody("COM", null);
	}
	
	private CelestialBody(String name, CelestialBody parent)
	{
		Name = name;
		Parent = parent;
		_orbitters = new List<CelestialBody>();
	}
	
	private void AddOrbitter(CelestialBody orbitter)
	{
		_orbitters.Add(orbitter);
	}
	
	public CelestialBody AddNewOrbitter(String name)
	{
		var newOrbitter = new CelestialBody(name, this);
		this.AddOrbitter(newOrbitter);
		return newOrbitter;
	}
	
	public Boolean TryGet(String name, out CelestialBody body)
	{
		// breadth first
		if(name == Name){
			body = this;
			return true;
		}
		
		var directMatch = _orbitters.Where(o=>o.Name==name).SingleOrDefault();
		if(directMatch!=null)
		{
			body = directMatch;
			return true;
		}
		
		// no longer required due to self check
		foreach(var directOrbitter in Orbitters)
		{
			var isIndirectMatch = directOrbitter.TryGet(name, out body);
			if(isIndirectMatch) return true;
		}
		
		// needed in case we haven't not even a single orbitter
		body = null;		
		return false;
	}
}