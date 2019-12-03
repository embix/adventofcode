<Query Kind="Program" />

void Main()
{
	foreach(var input in inputSets)
	{
		var path1points = GetPoints(input.Path1String);
		var path2points = GetPoints(input.Path2String);
		var intersections = path1points.Intersect(path2points).ToList();
		intersections.Dump($"Intersections in {input.Name}");
		var closestIntersection = GetClosest(intersections);
		closestIntersection.Dump($"Closest intersection in {input.Name}");
		(closestIntersection.ZeroDist).Dump($"closest distance in {input.Name}");
	}
}

struct TestCase
{
	public String Name;
	public String Path1String;
	public String Path2String;
	public UInt32 ExpectedDistance;
}

TestCase[] inputSets = new[]{
	new TestCase{
		Name = "sample",
		Path1String = "R8,U5,L5,D3",
		Path2String = "U7,R6,D4,L4",
		ExpectedDistance = 6,		
	},
	new TestCase{
		Name = "testcase1",
		Path1String = "R75,D30,R83,U83,L12,D49,R71,U7,L72",
		Path2String = "U62,R66,U55,R34,D71,R55,D58,R83",
		ExpectedDistance = 159,
	},
	new TestCase{
		Name = "testcase2",
		Path1String = "R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51",
		Path2String = "U98,R91,D20,R16,D67,R40,U7,R15,U6,R7",
		ExpectedDistance = 135,
	}
};

struct Point2d
{
	public Int32 X;
	public Int32 Y;
	public Int32 ZeroDist => Math.Abs(X)+Math.Abs(Y);
}



Point2d GetClosest(List<Point2d> commonPoints)
{
	var byDistance = commonPoints
		.OrderBy(p=>p.ZeroDist)
		.ToList();
	byDistance.Take(2).Dump("closest two");
	return byDistance[1];// dont return 0:0, return closest instead
}

List<Point2d> GetPoints(String pathString)
{
	var tokens = pathString.Split(",");
	var currentPoints = new List<Point2d>();
	var currentX = 0;
	var currentY = 0;
	void AddPath(Int32 incX, Int32 incY, Int32 dist)
	{
		while (dist > 0)
		{
			currentX += incX;
			currentY += incY;
			--dist;
			currentPoints.Add(new Point2d{X=currentX,Y=currentY});
		}
	}
	currentPoints.Add(new Point2d{X=currentX, Y = currentY});
	foreach(var token in tokens)
	{
		var direction = token[0];
		var distance = Int32.Parse(token.Substring(1));
		
		switch(direction){
			case 'R':
				AddPath(1,0,distance);
				break;
			case 'L':
				AddPath(-1,0,distance);
				break;
			case 'U':
				AddPath(0,1,distance);
				break;
			case 'D':
				AddPath(0,-1,distance);
				break;
			default:
				"somethings fucky".Dump();
				break;
		}
	}
	return currentPoints.Distinct().ToList();
}