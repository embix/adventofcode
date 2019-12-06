<Query Kind="Program" />

void Main()
{
	var testCase = TestCases
		.Where(t=>t.Name==
			//"Part 1 Test"
			//"Part 1 Production",
			//"Part 2 Test"
			"Part 2 Production"
		).Single();
	
	//var soiTree = ParseAsSoiTree(testCase.MapInput);
	//soiTree.Dump("Parsed SOI tree");
	testCase.RunSelfCheck();
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

const String Part2TestMapStringInput = @"COM)B
B)C
C)D
D)E
E)F
B)G
G)H
D)I
E)J
J)K
K)L
K)YOU
I)SAN";

struct TestCase {
	public String Name;
	public String MapInput;
	public int ExpectedOrbitCount;
	public Action RunSelfCheck;
}

TestCase[] TestCases = new TestCase[]{
	new TestCase{
		Name = "Part 1 Test",
		MapInput = TestMapStringInput,
		ExpectedOrbitCount = 42,
		RunSelfCheck = ()=>{
			var soiTree = ParseAsSoiTree(TestMapStringInput);
			var checks = new Action[]{
				()=>{
					CelestialBody d;
					var hasD = soiTree.TryGet("D", out d);
					var dOrbitsCDirectly = d?.ParentName == "C";
					//todo: var indirectParents = d?.GetIndirectParents();
					
					var parentCount = d?.GetParents()?.Count();
					var isParentCountEqualTo3 = parentCount == 3;
												
					var testPassed = 
						hasD
						&& dOrbitsCDirectly
						&& isParentCountEqualTo3;
					if(!testPassed){
						new{
							hasD,
							dOrbitsCDirectly,
							parentCount,
							isParentCountEqualTo3,
							testPassed
						}.Dump("TEST 1 FAILED ( D Node) ");
					}
				},
				()=>{
					var totalOrbitCount = soiTree.GetTotalOrbitCount();
					var is42 = totalOrbitCount==42;
					var testPassed = is42;
					if(!testPassed){
						new {
							totalOrbitCount,
							is42,
						}.Dump("TEST Integration FAILED");
					}
				}
			};
			foreach(var check in checks){
				check();
			}
		}
	},
	new TestCase{
		Name = "Part 1 Production",
		MapInput = ProductionMapStringInput,
		ExpectedOrbitCount = 0,
		RunSelfCheck = ()=>{
			var soiTree = ParseAsSoiTree(ProductionMapStringInput);
			var totalOrbits = soiTree.GetTotalOrbitCount();
			totalOrbits.Dump("Part 1: total orbit count");
		}
	},
	new TestCase{
		Name = "Part 2 Test",
		MapInput = Part2TestMapStringInput,
		RunSelfCheck = ()=>{
			var soiTree = ParseAsSoiTree(Part2TestMapStringInput);
			soiTree.TryGet("YOU", out var you);
			soiTree.TryGet("SAN", out var san);
			var currentYouSoi = you.Parent;
			var targetYouSoi = san.Parent;
			var path = soiTree.GetPathFromTo(currentYouSoi,targetYouSoi);
			path.Dump();
			(path.Count()-1).Dump("orbital transfers required");
			4.Dump("EXPECTED");
		}
	},
	new TestCase{
		Name = "Part 2 Production",
		MapInput = ProductionMapStringInput,
		RunSelfCheck = () =>{
			var soiTree = ParseAsSoiTree(ProductionMapStringInput);
			soiTree.TryGet("YOU", out var you);
			soiTree.TryGet("SAN", out var san);
			var currentYouSoi = you.Parent;
			var targetYouSoi = san.Parent;
			var path = soiTree.GetPathFromTo(currentYouSoi,targetYouSoi);
			String.Join("-", path.Select(p=>p.Name)).Dump();
			(path.Count()-1).Dump("orbital transfers required");
		}
	}
};

static SoiTree ParseAsSoiTree(string multiLineInput){
	var root = CelestialBody.CreateUniversalCenterOfMass();
	var soiTree = new SoiTree{Root = root};
	var lines = multiLineInput.Split("\n");
	// TODO: use directory from-to, then starting from COM instead
	// - any remaining mappings would be other trees in a forrest => hacf
	
	var fromToMapping = lines
		.Select(l=>l.Split(")"))
		.Select(fromTo=>new{
			// solve \r\n related issues
			from = fromTo[0].Trim(),
			to = fromTo[1].Trim()
		})
		.ToLookup(m=>m.from, m=>m.to);
	var unfinisheds = new Queue<CelestialBody>();
	unfinisheds.Enqueue(root);
	while (unfinisheds.Any())
	{
		CelestialBody currentNode = unfinisheds.Dequeue();
		if(fromToMapping.Contains(currentNode.Name)){
			var unconnectedChildren = fromToMapping[currentNode.Name];

			foreach (var child in unconnectedChildren)
			{
				CelestialBody parent = currentNode;
				CelestialBody newBody;
				var newBodyExists = soiTree.TryGet(child, out newBody);// still slow - another dictionary would help
				if (newBodyExists)
				{
					$"Cannot add {child} to orbit of {parent.Name} as it already directly orbits {newBody.Parent}".Dump("Error");
					soiTree.Dump("DEBUG: SOI Tree");
					throw new Exception("Halt and Catch Fire");
				}
				var childBody = parent.AddNewOrbitter(child);
				unfinisheds.Enqueue(childBody);
			}
		}
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
	
	public Int32 GetTotalOrbitCount()
	{
		// todo: move into nodes (aka CelestialBody)
		// todo: IterateOverChildren(lambda)
		var totalOrbitCount = 0;
		var nodes = new Dictionary<CelestialBody,Int32>();
		Queue<CelestialBody> toVisit = new Queue<CelestialBody>();
		toVisit.Enqueue(Root);
		while(toVisit.Any()){
			var visiting = toVisit.Dequeue();
			var visitingParentCount = visiting.GetParents().Count();
			totalOrbitCount += visitingParentCount;
			var directSatellites = visiting.Orbitters;
			foreach(var satellite in directSatellites){
				toVisit.Enqueue(satellite);
			}
		}
		nodes.Dump();
		return totalOrbitCount;
	}
	
	public List<CelestialBody> GetPathFromTo(CelestialBody from, CelestialBody to)
	{
		var currentPath = new CelestialBody[]{from};
		var route = GetPathFromTo(from, to, currentPath);
		if(route!=null){
			return route.ToList();
		}
		return null;		
	}
	
	// recursion
	public CelestialBody[] GetPathFromTo(CelestialBody from, CelestialBody to, CelestialBody[] route)
	{
		if(from==to) return route;

		// we're lucky enough a tree only has one possible route (using each node no more than once)
		// so the one we find is always the shortest
		foreach (var option in from.GetTransferOptions().Except(route))
		{
			var optionRoute = route.ToList();
			optionRoute.Add(option);
			var fullRoute = GetPathFromTo(option, to, optionRoute.ToArray());
			if(fullRoute!=null) return fullRoute;
		}
		return null; // no route found
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
	
	public IEnumerable<CelestialBody> GetIndirectParents()
	{
		if(Parent!=null){
			return Parent.GetParents();
		}
		return Array.Empty<CelestialBody>();
	}
	
	public IEnumerable<CelestialBody> GetParents()
	{
		// not efficient
		var parents = new List<CelestialBody>();
		if(Parent!=null){
			parents.Add(Parent);
			parents.AddRange(Parent.GetParents());
		}
		return parents;
	}
	
	public IEnumerable<CelestialBody> GetTransferOptions()
	{
		var transferOptions = new List<CelestialBody>();
		if(Parent!=null) transferOptions.Add(Parent);
		transferOptions.AddRange(_orbitters);
		return transferOptions;
	}
}

const String ProductionMapStringInput = @"XLG)95G
W3V)ZZ3
ZM3)Q4Q
4S9)KCL
HX2)MLM
NV7)7TZ
MQZ)JDD
CRQ)3HY
TLZ)7F3
S3N)6C5
4JP)KYD
6Z9)Q1R
XJF)JZY
1M3)64D
NB1)72T
Q4Q)PD8
2B1)JJY
6Z6)C2R
R5W)B4X
T6K)N6R
C78)D21
K33)YC7
FD1)CCX
J4Y)LKJ
HNZ)WCM
C3N)5BV
TTM)974
N6D)SC7
WVR)NRH
7CC)BDH
49M)HNJ
YJ9)NJC
NHP)PHR
8NP)N6J
27G)739
YNG)QJ2
B3B)TL2
D6Z)2PZ
41K)RRK
FPM)7VR
V1T)HLP
XQZ)MQZ
BCV)51Q
HRN)QNC
7MM)YDV
7ND)W79
BLP)PZJ
CPF)DH3
YYZ)FP2
GB9)6RB
JFQ)41H
QQH)D6C
ZQ6)B9W
D2J)KC3
ZCJ)1LN
NXT)1SX
6L8)GMQ
NYY)MN9
5YY)D1W
9Z7)T65
K5G)T6K
WZ2)XMD
HZT)BMY
6VJ)HG5
COM)PWZ
DW2)ZPK
RSC)SBQ
61K)L9W
DH3)XC8
2KB)9DJ
D7R)BG7
9Q8)529
8SX)99N
NJ1)K98
14L)W66
HQD)MMY
ZDS)45V
H96)YTP
W83)FPX
2BF)ZXR
WVD)HY5
SXJ)CBS
KLV)WC9
XJM)K33
Y5K)J49
K9R)4H5
WPL)HQD
9W4)J4Y
C89)2NN
XQM)VDF
QBM)F29
BHC)47P
Y9P)YNG
ZNG)KLJ
YM9)D3Q
9Z9)ZPQ
SGC)C6X
5M6)GCL
L9L)QLD
22X)GJ8
8QP)LJF
CMR)LLC
88Q)ZG9
PWG)PGJ
H2T)1JK
ZKV)TPX
5MZ)X7L
XBJ)DCF
7VL)63J
1VZ)WVR
VPN)XQQ
QGR)LNP
W3J)ZDS
6ZM)1VZ
JBY)C68
9Y9)F8B
XZK)GKT
QJ2)9F2
KQT)V1T
KJF)8LD
X2X)DP4
82X)PK5
9Q1)48V
YJS)Y9P
ZBK)YJ2
XVS)S95
22D)CL2
NV6)GM4
FB1)JMQ
HZP)4M3
TKC)CKB
N7Q)ZBH
K34)XV3
82L)T4S
VSP)XG5
MV2)1HT
TLD)XWF
PZT)C25
Q7G)2BF
GW2)5ZJ
XJV)YZL
QQP)B6R
ZX8)6Z4
JB4)RKT
KL1)XKK
8QR)1SZ
VJ8)HCG
3XG)LF3
T93)S6H
DY6)CKP
WNB)7B1
PP7)MRN
J4D)BTV
6NT)XGP
VVX)NM7
8QK)G9F
BYF)R34
58Q)9ND
85X)Y1T
T1Q)8GB
FL9)4DH
GJZ)VJ8
D35)8F3
73G)6FK
CBR)YT6
GG1)SLQ
VM3)W33
N6J)Y4M
Q9X)B76
576)55D
CCT)NHP
P7R)466
VN3)BZZ
YTP)XLD
QH8)QDV
W2L)1QD
B33)XG6
CLD)XBJ
GBK)QP1
C1V)Q72
WYW)M12
3FD)Z1L
Q72)Q2G
2GH)N1P
YR1)3FV
XG5)5Z7
TSC)HCL
FLQ)B5D
VF3)BLG
M6W)Z6R
4JV)K15
Z7J)C3N
YT6)Q7G
99P)DKM
RSB)T9Z
PZC)VM1
K98)2KL
D3Q)46F
MRR)6HF
YW4)TSC
TVD)XR1
XR1)QHZ
Z6Y)GRX
FWD)WGV
Z19)JTX
N8B)T84
QGL)MWZ
YD3)YLR
S3S)MJD
W66)84B
ZZH)HHQ
89G)MDW
W5R)3W4
VS8)H3G
MST)844
7K3)YWB
9KP)FY9
7YK)TYT
PGW)J6D
DCZ)TMC
7YC)TLZ
12X)TVW
Z7Q)P5Q
ZTF)GSC
63L)NCL
JJY)5LW
YBR)WVD
LLC)7N9
DP4)3X4
GSN)DR5
VPR)49R
HWK)X16
HJB)F1K
V1Y)M6Q
V47)7ZL
5XN)FB1
G8T)J81
MSZ)RW4
RF6)7VL
39M)T7V
CNM)443
ZV5)2K1
X2Z)8CV
5HW)K55
6X1)1GR
DCF)81X
4F8)RQB
XWF)N6D
9WG)5LV
Z2J)D7R
VZW)8L1
3FV)YW4
ZXR)8KK
ZQL)JB4
S2R)7DH
PWN)YJS
7FP)FNX
X1D)RDH
CXS)618
G72)9CZ
3GK)H2Z
9ZC)4PX
FBL)22R
43T)VX4
N64)Z1K
6D5)7JR
ZWM)R9Y
BSC)2RT
KJF)41J
VG2)Q9X
Q1R)75G
RWG)769
7S3)M9C
87D)5V9
ZHM)MQR
LWF)CLD
39M)Z6L
4XC)KR5
LRZ)PG6
23H)JZZ
GX7)W15
Z3L)RT3
ZRJ)X38
PND)1BZ
LPN)9WG
41J)L4B
5JM)X29
4M3)D2J
XJN)GMB
SRK)2W8
TD6)FL9
2WD)2Q3
5JW)F77
239)BSF
7ZL)W1P
P9P)D3F
7BS)ZWM
MLM)53G
MDW)TSX
PQ1)KWL
GTF)SV1
VMM)GJZ
PNJ)75C
NR3)9D4
TNB)LNH
488)XVS
LFJ)4P4
J6D)X2W
NCL)VT6
H9L)S3H
4H9)239
RKT)X2Z
PVQ)QMK
JWH)LGW
Z3F)8LW
4MJ)21N
49J)88B
D19)LDZ
LKJ)DZQ
59Z)GV3
YQK)VQ2
RJY)FL5
3BF)1DD
HP6)PX8
LVM)MRH
L65)ZCF
664)RJY
GZV)ZFX
N23)5V4
GFZ)Q79
LL7)PFT
BZZ)FT9
S6P)Z3F
9ND)L33
T1T)SAN
WJ9)DZY
JFP)GC5
33R)7S3
BPQ)S5S
LLC)S4N
TW5)Q69
C3D)N1D
4L5)4VS
VWX)XM6
37R)2KR
X29)N7Q
382)JT3
D66)JQ2
CCX)5PM
N1D)CKT
Q5J)QQH
ZHL)7GF
SV1)1FZ
BR3)2NG
C5W)VG2
J5P)N86
7MG)JFQ
RP1)JFD
PZR)ZPR
7PQ)2RL
NWM)PWG
79L)NBL
G83)LVM
MNS)HTV
XBH)5JW
Z98)FGH
4JB)FDJ
YQK)VWZ
GCG)LN4
QQJ)R5W
6C4)NRY
LW2)CZ8
MMY)81K
LNS)X4C
R14)1TV
JG2)HPN
TS6)PLY
7J8)VV4
6RB)3SF
LXM)BF1
7QY)X5B
362)9YP
7JR)JJT
RHZ)2WD
LKC)8SV
47P)KND
Y3W)H5L
MWM)YBR
2F6)DQR
S4L)546
4H5)TF1
NDP)Y2H
5DB)JN3
N7J)8D9
SV2)W83
DZZ)T5Y
W94)S3N
Z98)44M
N28)43T
2NG)D1C
RMF)SWN
NWM)4QR
YJ2)PCK
7NS)81Z
ML8)HRN
XQ2)ZHG
Q7Z)299
Z1H)QFW
J81)KHY
HQD)PFN
GSN)L6Q
D21)5H9
QDV)M97
JVR)BR3
PZJ)H8P
4MJ)L3S
GRX)W5R
1K5)6ZM
BDM)8QR
5MZ)7YK
66X)VF2
81K)XH6
H2N)VZ7
4GT)M6W
Q69)WWT
SK2)P85
3ZQ)RF6
PKM)5DZ
81C)MG2
WN5)CCT
14B)FBL
SC7)F7Q
YZB)P5C
218)3T9
2RT)HF2
TSX)R6X
MT4)9CD
NM7)S4L
VX8)RQ2
DW7)TD6
9Q8)9WZ
J5K)7B9
QZV)P99
FVY)XFN
XSZ)DV4
T65)ZJL
TVD)6CL
3XN)8F7
FNY)NFP
VMD)SYP
X2W)ZBD
5TQ)82L
BG7)39M
C89)T8Y
SSH)GCB
SVQ)1MN
7SC)7BT
HTV)JX8
PG6)T17
DZQ)Q36
48V)J4D
6L4)8NP
RLK)8XM
KL1)LT6
5H9)YBP
JQG)SMD
VVG)WWF
Z6Y)DBK
MRD)W8Q
BDH)FBF
P85)32T
5LW)J2J
Y9P)QGC
VFP)14J
PPY)8WK
FC5)H5K
KSG)PNJ
63J)3XK
6H6)ZXX
S9M)NGJ
55D)8MD
64D)NWM
C6H)V1X
T9Z)ZY6
P35)68X
XK1)R49
VJX)W7K
33X)BBN
GBK)G98
XS2)4KD
7F4)8KV
M8M)S77
RD5)WYW
T4S)FNQ
WCX)KCR
1SY)BPV
VPW)1SY
NBB)J5M
1GV)L9L
391)ZSG
N28)488
ZHG)TR9
SWW)B69
V6C)ZWN
8XY)1T2
81Q)XZK
L7D)LJK
HF2)17Y
H5L)815
6MM)S3C
LGW)N8B
LSZ)7MM
1JW)FD6
DQR)JW1
17N)JTF
9J2)FC3
XR3)HZS
LJK)H3Q
XZC)183
WZJ)NL7
M9B)NJL
ZBD)WR8
KMT)4JV
9YJ)6LR
5V9)KVY
RRG)MT4
VMT)GXG
49W)NDP
3F7)CJV
XVR)89G
YQS)R1G
GSC)KKF
8DH)J3B
LWC)H4W
TL9)NVX
5LW)YQK
Y9V)XBH
5L7)VMD
KSJ)B3B
CKT)JST
22R)13Z
B2H)ZV1
RCZ)6Z6
92Q)XF3
XN2)2P6
7MM)1RS
8BP)4MJ
6Y5)5L3
RW4)XZB
VXZ)7ND
G72)N23
8BP)YH5
37X)1TH
SHX)NXN
8SM)Z1W
L2T)89M
NL7)Q39
6FK)SWW
VDF)DLQ
BMY)RBJ
QQL)MMH
M83)C8G
183)34F
P5C)33R
81Z)7SC
8KV)BSZ
L6Q)7ZP
8F3)J5K
1B6)423
DBK)TNB
L12)R8J
Z66)49W
XKK)X63
49M)VRL
93Y)PZR
PK5)XQ2
H1G)ZYN
XM6)9S1
W33)CHL
VD5)M4W
G98)24R
R49)N53
1WY)D6Z
13Z)G27
JP7)1LT
ZFX)Z7J
2F6)G9K
QHZ)WXC
F8M)Y27
KND)ZTF
Q1L)14L
XH6)GQQ
J7J)87N
FY5)Q59
15J)2JJ
ZF2)383
F9Z)SMR
QH8)GZW
HD9)7F4
FHG)7CC
7L3)T37
HYP)8YV
1FX)BD4
6CG)MRD
53S)SNN
88M)C5Q
S4N)1KF
Q89)ZH1
BN6)3C9
TQ7)9CV
SMP)J69
Q3T)RDG
QC8)16G
JNB)3GK
SDJ)7LT
NM7)498
15W)P8Q
9TX)ZHP
PCK)WGL
B8F)7CB
5M3)SR9
7VR)NJ6
XG6)HVW
6CL)S24
287)MZX
55B)18F
F7Q)WQS
4PX)3BZ
YCC)D19
KGB)Z2J
JKN)FMP
GT4)VCT
VQ2)1B6
FF2)55B
ZZL)22X
S3C)7NK
WZL)2Y2
XKC)KJH
Y8C)CPW
66Z)Q89
YWB)JP7
C7F)SYN
9N4)PGW
TL9)9NR
6C4)GW2
YC7)42T
4KG)YDQ
66Z)WHM
CZG)HJB
QHH)QGR
KN2)LHB
J64)R9R
618)L12
GKT)SW2
SW2)YCC
H6M)Z4X
34F)772
WHM)RW8
3HY)34D
H55)RMR
NJL)27G
1HT)4XD
4VS)3MG
CHL)VW4
H3Q)3FD
LDK)2K5
44X)ZXV
FNB)195
RFG)6X1
ZZ9)WQQ
JG2)R2T
W4J)WNK
XC8)J5P
443)Y5X
VRL)BBH
CPX)HNG
LRF)K6F
TC7)ZHL
R2R)13Y
867)24C
GRQ)8XP
W94)TW5
FDJ)M8W
QJ2)7L6
KJ9)93Y
353)Z3L
P6C)BJF
53L)HYP
MQY)8T6
SBL)ZHX
FGH)8J4
HLP)YYZ
XG6)4YX
WFL)FVY
6HF)8SX
4MC)QGL
VV4)YQG
5LX)4WY
67S)3WD
42Q)L4F
8KG)3XN
T6P)9RY
1TV)ZRJ
N63)39Y
LGJ)824
41H)YZB
LTP)TC7
9DJ)SLK
1NC)PKY
FBJ)116
5DZ)BLP
9S1)FMH
3X4)JVV
1JL)VWX
87N)6KV
GZW)NB1
F6G)GJ5
R9Y)L9H
D7K)NV6
PHR)49J
RT1)ZNG
Q1B)VZQ
V1X)8DH
XV9)VJX
F5V)X9X
7F3)QNY
R1G)VMT
D3F)X2X
19J)GH4
33B)S9M
DR5)ZQ6
4TD)3F7
KWM)L83
769)NBB
TKC)9SK
2CX)VS8
B3B)SMP
YC2)49M
2Y2)S3S
X3L)BJQ
SWP)JY7
XDS)YR4
NZ5)Q5J
XM9)12X
8NN)2KB
1MN)47M
1JL)1MB
RBJ)6C4
8HS)G2J
T4F)RP1
J2J)KJF
QP1)25C
CWX)4JB
TF5)DKC
5YT)7Z4
WYW)NC6
78H)LC4
7ZN)MC9
QQL)9W4
YQG)M9B
M97)11H
82L)T1Q
Z1L)8WY
WXC)8TM
L4F)XV9
ZXX)6MM
24R)88M
7M5)FHM
R8J)7J8
Z26)7NS
F4N)K7T
S69)RWY
MRN)5XS
ZYX)TCV
65Z)KJ9
51Q)WZR
GH4)71Y
SNN)LS8
84B)FY5
RWY)KDH
B5D)SHX
Q1B)FC5
BSZ)FWD
YZL)CZG
QGL)W6L
QQH)Y9Y
YM8)4MC
LNP)BK7
WWF)XS2
B9W)63L
4LM)P7R
QB5)TL9
8YV)5DK
N51)3BF
R6X)PYV
PRX)439
21N)8HD
TVS)933
R4S)15S
XLG)9YJ
SQ6)JS3
BSC)6MY
ZSG)PWN
JWZ)XHK
FQ3)1J2
JBX)DB4
3ZS)CRW
T37)S69
QC2)YJV
5MH)65Z
1NM)M39
GC5)NZW
QW4)F6T
K15)MWM
18F)4S9
5BV)N96
VHB)ZKV
1DD)2XW
1JG)XC9
472)LPN
9CZ)8BP
LVR)NJ1
7DP)G5D
5JW)Q1B
ZJL)FHG
DB4)KWM
1XW)SGX
MT4)SDJ
KYD)1JX
FN9)GNV
G36)Q3T
9NR)29F
6RZ)HDZ
Q4Q)9Z7
L5P)ZTC
FD6)5QJ
1SZ)M1C
KQT)1K5
M9B)17C
2SJ)YD3
1QD)82X
1SX)YNF
GCZ)N51
BBN)T2X
GPZ)14X
BZY)WDY
YT6)BHC
7PN)W2K
XJC)WJ9
LHB)PNP
29Q)FR8
99L)BLR
P7C)KWB
Y5X)PRX
XN2)MWW
N53)R2R
FLQ)9TX
24J)BZY
792)6RZ
JXH)BNF
CXQ)Q2K
D6G)P6C
VDJ)N8C
K55)P7C
C5Q)F9Z
GK1)GB5
3DG)LFJ
N86)NL2
95G)SQ6
5ZJ)8BM
5DR)JB1
7TZ)ZLJ
7NK)QZV
68X)2F6
XQS)CT7
7L6)RHZ
RT3)XDS
RQB)MJK
C2R)X1D
LLV)MNS
2SX)M5H
FC9)LDK
F78)44X
CRW)BQJ
K3K)LHL
FNQ)W98
11H)4JP
DFY)C78
W8Q)PMQ
4QR)SK2
KSK)MQY
PFN)353
SYP)J2F
BJF)TKC
WQQ)QQJ
8HD)FLQ
8HD)QM6
G2J)B2H
CKB)8CC
8WY)FBJ
G5D)5P8
83N)JG2
LS8)VHB
1HD)KGM
8BM)QQP
Q79)G2T
DFB)4LK
ZLH)R82
T8Y)66Z
YLR)1K6
933)YM8
GMB)FSV
5PM)BJS
47C)TQ7
MKS)5FT
WWC)P8Z
HG5)CXJ
LDZ)K3W
88N)1JL
2T6)DLX
H4W)KGR
7DH)LNS
MWZ)BCV
45V)382
RML)5MF
QX4)NXZ
CFP)792
4SP)PPY
J5M)SV8
NC6)1CG
MJD)Y2X
ZKG)FK1
Q2G)WCX
WMD)BG3
VWZ)QH8
99N)NXT
Y8P)YKH
Q28)LNY
8FN)V47
NG7)C6H
2NN)D7K
Q3R)V6C
Z3W)GCG
RC9)WMD
M5W)XBT
NV1)WNT
P8Z)41K
T2X)JJW
NL2)D6G
JV9)Q1L
CF2)VXZ
W85)GTW
DKM)WFL
PNY)K3K
SJW)B33
9V2)Q51
HWK)KG9
DLQ)ZKG
CCQ)83N
42K)LW2
X4C)2ZW
KGM)8KG
FNX)FDK
64D)37R
JB1)T93
VPR)6Z9
L4B)73G
LPW)XJM
DLQ)22Z
1T2)PZT
XGP)VX8
CWF)MH6
2K5)QQL
7J8)YFY
195)XLG
8D9)XK1
423)BNB
JW1)QTP
24J)GHH
L83)7MG
1LY)J7P
7B1)33X
Y1H)4TB
6VF)TC6
BD4)Z26
ZHX)37X
7S5)WXR
D1C)DTR
JVV)LWC
QCX)Z29
2XW)TCB
93Z)68L
ZBH)99L
G1D)MST
L9H)JQG
W6L)28G
NVX)RZ8
L57)14B
MX5)L5P
FMP)QBM
PGJ)B6D
71Y)7FP
C25)G1D
QMK)K5G
LNH)RSB
3TD)Y8C
9F2)W9F
JS3)6Y5
TF1)FN1
FNY)9FH
33X)PND
1JX)CCQ
25C)85X
STD)RTX
YDQ)9ZC
SMR)1NM
TVW)VDJ
HT5)JYH
VCT)Y3W
VQ2)LHT
75C)BRG
557)XKC
66F)VFP
QGC)J2C
8LW)Y8P
NJ3)QGY
G2T)FD1
2SJ)VD5
BPV)PXF
ZXV)SCD
5WB)C8P
HDZ)VMM
FSV)2Y1
GGT)7JX
JDD)Y9V
8MD)VDX
VXL)5MZ
WDY)GZV
XQZ)Y1H
WXR)RXN
96M)JXH
9CD)53S
546)JVR
NZW)C89
MMH)4GT
YKH)SV2
HNG)82B
25K)HH4
ZD4)F8M
KJH)8NN
6G5)YQS
8LD)PQ1
M4B)283
X5B)J2S
LGX)Y9Q
XFN)CCG
C8G)L57
Z4X)8Y7
S29)YR1
VF2)P5M
ZSJ)4SP
WXX)TJF
SGX)VCY
7QY)4H9
J3B)P88
NBL)ZZ9
W15)4T6
1LT)LGT
NRH)1CY
KHY)HX2
LFN)XQS
1J5)H7M
NMG)WNX
ZHP)Y5K
FY9)NDW
3C9)JKN
JST)PBV
W79)VPN
X63)RSC
YH5)C1V
8F7)STD
ZM4)Q28
S95)5XN
W2K)4TD
S7H)C6V
218)R4S
1K6)5WB
DMS)GXZ
SWN)5FV
WCM)V99
GHH)78C
H2N)5DB
L6M)KVN
RW8)T6P
873)WPL
8XM)F78
BGX)KMT
1CG)WVB
N3J)9KP
9XM)MSS
39Y)81Q
Q2K)6XN
P8Q)867
PWZ)CPF
XV5)L65
PYV)XSZ
8L1)QCX
HHN)PP7
CBS)MN4
DCM)29Q
GMB)6CG
BQJ)GG1
5FV)GTF
TMB)Q7Z
KWL)WWC
GJ5)ZBK
6C4)99P
VDX)5MH
DZY)GBQ
4QR)91Y
P3D)QX4
PPY)CWX
2RL)CMR
7FP)1T5
6MY)PNY
X16)655
J2C)3DG
655)MGG
6LR)V5L
Y5K)XQZ
Z6L)9Y9
4XD)GBK
XF3)TP4
9S1)GK1
F6T)7YC
X38)1S3
ZH1)JBY
8N5)K9S
MN9)HN8
SBQ)L5S
N1G)TVD
Q51)CF2
9H1)NSF
833)4RK
PNP)QWH
PD8)NXM
D7K)QW4
JDL)N5H
YSM)BPQ
3MG)NPW
J49)1QP
WQS)GN4
VFP)L2T
2KR)GX7
9WZ)T4F
JJT)SWP
8KV)KL1
3XK)87D
739)RPS
JWZ)8P4
Q72)LWF
PBV)P3V
XLD)M8M
D6C)6L4
498)8XY
N6R)WZL
K6F)25K
VK8)WQH
YJV)2SX
L7D)66F
VM1)RWG
439)QTX
Q59)2GH
Q36)5YT
RRK)GPZ
5QJ)XJC
WVB)CXS
5MF)CTB
C6X)P35
R9R)833
BNB)96M
2Q3)4LM
V99)1GF
J2F)12H
HLJ)5M6
MGG)8N5
12H)1YK
GJ5)M4B
8QR)DY6
YR4)3XG
7N9)Q2T
M8H)NBW
LN4)8FN
QPX)ZX8
QNY)K83
X9K)ZV5
MP1)93Z
Y9Q)1WY
Q5Q)9CG
2F5)TTM
JT3)6D5
BTV)H96
3ZN)LRZ
YFY)ZLH
K9R)Z98
13Z)55G
H4W)1FX
BNF)S7H
P5Q)576
TVY)7S5
RQB)58H
ZFX)M8H
S77)NZ5
29F)N64
6YD)8JC
LJH)N28
RDV)DW2
NTZ)G72
F77)2T6
JTF)FF2
14J)15W
JZZ)VN3
PXF)6J1
4DD)LXM
WNZ)6L8
XMX)4XC
JMQ)58Q
BJS)ZYX
XGY)276
GJF)TF5
MQR)J61
B6R)H7L
1GF)6H6
MZX)HNZ
8WK)LGX
NPW)GCZ
78T)ZCJ
1JK)XR3
ZY6)DCM
TMB)362
FCG)D35
KCR)WZJ
SYN)9H1
93S)SVQ
34D)GFZ
9CG)5JM
24C)1KY
5QK)JJC
51Q)XJF
VZ7)VM3
D1W)9JZ
LCB)NTK
T5Y)MRR
PBX)9J2
8B4)GT4
32T)RLT
H8P)93H
2P6)RBG
529)VK8
P5M)6NT
1J2)JWH
GXZ)78H
N96)RSJ
WNK)C71
VT6)67S
88B)Q7K
8KK)KN2
78C)9Q8
L3S)MX5
LF3)19J
LPJ)2CX
BG3)GSN
HZS)7DP
ZLW)47C
1KY)5NC
7NQ)C5W
4RK)T1T
215)287
QFW)RD5
ZTY)LL7
GJZ)7ZN
75G)4F8
NXM)SSH
QW8)7PQ
KLJ)MP1
YDV)6YD
GB5)8HS
1BZ)2H9
4W5)PKM
Y27)2B1
BSF)VVG
8P4)NR3
9NX)RT1
T84)KSJ
844)4W5
W98)TV9
2W8)WN5
JY7)77G
XHY)D7H
GQQ)CBR
3J9)D5M
YNF)P9P
17D)KMX
K9S)Z7Q
61K)NYY
283)ZQL
8D8)7PN
GNV)J64
TP4)RCZ
JZY)JWZ
LRD)RLK
LJF)K34
WC9)Z1H
3X8)L9Z
ZPQ)LJH
ZWN)7PX
CT7)H55
RLK)6VJ
GV3)LVR
GBQ)Z66
3XG)QVK
J7P)LGJ
TMC)88N
299)C7F
CP3)5HW
3BZ)FNK
L9W)B7G
NXN)KQT
SMD)GJF
Y8C)NMG
N1G)XJN
17N)DMS
WRH)TVS
XMD)JRT
1FZ)TQ4
89M)JNB
46F)25Z
HN8)J7J
824)CMW
CKP)HLJ
8J4)MKS
KGR)WF9
V5L)F5V
FF3)1M3
KKF)1J5
1S3)ZZL
772)3X8
KG9)XM9
9SK)61K
F8B)F6G
13Y)R61
Q91)5LX
FP2)RMF
22Z)215
J69)W3V
4TB)NG7
J3B)B8F
MZX)SRK
8T6)VVX
FMH)FF9
QQJ)LPW
GJ8)9Q1
Y1T)VCG
9CB)177
6C5)7K3
C3N)DFB
ZSJ)RZK
7TK)N63
DKC)9V2
FPX)5F6
W9F)78T
YTP)PVQ
H5K)2JB
SCD)ZTY
91Y)K9R
WGL)M6T
8JC)8QK
GP6)VBV
D3C)QPX
P3V)N1G
XBT)CFP
V1X)XVR
53G)3J9
17Y)ZNC
JJW)HWK
8R3)1NC
FBF)1JG
DH3)ZSJ
RSJ)XN2
H7M)VPW
R5W)KGB
RPS)XZC
MWW)HP5
8J4)9N4
44M)TLD
5V4)KSG
8CC)4DY
Z6R)MSZ
HVW)SBL
QWH)DZZ
4T6)PZC
Y4M)ML8
WJR)6G5
WR8)X3L
XQ2)WXX
5JM)M5W
LJF)QC2
5XS)8R3
Q24)RDV
WGV)YJ9
QP1)FF3
7NQ)6VF
YNG)8D8
M4W)NV1
G27)LFN
8GB)SXJ
ZCF)HZT
S3H)W85
89N)CNM
SWN)7VJ
JQ2)X9K
TPX)V1Y
YM8)7TK
28G)S2R
M1C)Q5Q
JDQ)JFP
B18)KSK
ZPR)X9V
PLY)YM9
VZQ)23H
5XS)7QY
7D9)92Q
8Y7)KLV
7B9)HQK
S6H)M83
HJP)9CB
5NW)RML
1YK)FNB
HP5)33B
JN3)YOU
HQK)Q3R
49R)CPX
LGT)K45
5S8)9YL
M12)HJS
DQL)3ZN
LHL)2CT
1TV)L7D
VBV)RRG
2H9)6LW
3W4)B7B
Q39)5QK
99P)W3J
JRT)YC2
WNT)JDL
2JJ)BDM
1GB)ZM4
RZ8)2SJ
DZ3)RXP
WF9)79L
RDH)CRQ
8YN)15J
C3D)ZF2
NFP)CP3
4DY)G36
NBW)KH7
6KV)XQM
XR1)B18
KCL)3TD
TL2)WJR
5L3)PBX
Z29)17D
ZV1)81C
6LW)HD9
BF1)H1G
CCG)VF3
GCB)HHN
JKP)S8G
6J1)Q24
DW9)J1D
B7B)JKP
7BT)QBF
TCV)W4J
11H)1XW
QB5)HWD
F1K)DCZ
55G)24J
HH4)CXQ
HCG)DQL
B18)89N
WZR)LCB
1TH)CPT
RZK)YNR
FR8)BSC
7PX)JY6
G9F)WNZ
QTX)PSQ
M5H)XMX
22R)LRF
2JJ)S6P
8TM)8SM
G9K)MV2
6ZM)VPR
5DK)HJP
JTX)VJR
GCL)TVY
4P4)ZZH
HNJ)658
8SV)JV9
RDG)L6M
4D5)42K
NRY)XHY
1QP)RFG
FN1)218
F29)4D5
C6V)WZ2
FL5)G83
QBF)D3C
VX4)NTZ
1MB)NV7
TJF)4XM
RZ8)4KG
R82)VSP
TQ4)GGT
P13)W7D
ZNC)Q91
2JB)3ZS
XHK)VZW
QM6)HZP
WXC)5M3
D5S)17N
LNH)QW8
FK1)FNY
JYH)ZD4
X7L)8BL
PWN)RGL
TCB)GP6
25Z)LKC
RGL)312
81X)FN9
9W4)LLV
H7M)5TQ
9FH)D66
K3W)7NQ
T17)7BS
G98)JBX
L9Z)F4N
YMX)G8T
HPN)5NW
1CY)XGY
X9V)7D9
ZG9)2F5
72T)LTW
B6D)HT5
9YL)4DD
6Z4)4L5
J61)BN6
KH7)Z19
QTP)472
4XM)7M5
RTX)H2N
ZZ3)7MN
MJK)4TK
L5S)P3D
P99)5L7
ZLJ)TMB
MC9)42Q
D1W)9SF
TC6)1LY
J1D)GB9
58Q)9XM
X9X)3ZQ
9RY)4PG
T6K)391
LHT)FQ3
FF9)LPJ
89N)5DR
T7V)R83
WWT)BYF
HJS)DFY
XPK)9Z9
1RS)JDQ
77G)5YY
17C)YMX
J2S)S29
1T5)BGX
D5M)DW9
PQ1)ZLW
K3W)1HD
YNR)7L3
F8B)1GV
Y2H)N7J
9J2)HP6
974)4LN
P88)557
3FD)H2T
7N9)D5S
GZV)KBK
Z1W)YSM
R2T)FYW
BBH)C3D
LC4)CWF
4DH)GRQ
M9C)1JW
T93)DZ3
5M3)W2L
15J)9NX
R34)4MK
NTK)N2M
LNY)FPM
4MK)Z6Y
4PG)66X
QNC)8B4
9SF)SJW
M12)1GB
D35)XPK
R61)XJV
177)Z3W
DMS)QC8
2Y1)8QP
4WY)TS6
KBK)53L
9JZ)8YN
82B)NJ3
CPW)664
14X)ZHM
M39)BBL
D7H)R14
KC3)VXL
58H)59Z
JJC)522
W66)873
F8M)LRD
C71)DW7
8BL)88Q
8XP)SGC
1KF)QHH
3SF)FCG
L33)RC9
7LT)H6M
MG2)WRH
9CV)P13
FC3)LTP
GRQ)FC9
PX8)XV5
KMX)QB5
C8P)5S8
3T9)N3J
WNX)H9L
Y2X)WNB
VCG)22D
37R)ZM3
B7G)LSZ
JFD)93S
BLG)RG4
XQQ)W94";