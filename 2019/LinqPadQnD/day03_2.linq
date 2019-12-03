<Query Kind="Program" />

void Main()
{
	foreach(var input in inputSets)
	{
		var path1points = GetPoints(input.Path1String);
		var path2points = GetPoints(input.Path2String);
		var intersections = GetIntersections(path1points, path2points);
		intersections.Dump($"Intersections in {input.Name}");
		var closestIntersection = GetClosest(intersections);
		closestIntersection.Dump($"Closest intersection in {input.Name}");
		(closestIntersection.SignalTime).Dump($"shortest signal time in {input.Name}");
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
		ExpectedDistance = 30,
	},
	new TestCase{
		Name = "testcase1",
		Path1String = "R75,D30,R83,U83,L12,D49,R71,U7,L72",
		Path2String = "U62,R66,U55,R34,D71,R55,D58,R83",
		ExpectedDistance = 610,
	},
	new TestCase{
		Name = "testcase2",
		Path1String = "R98,U47,R26,D63,R33,U87,L62,D20,R33,U53,R51",
		Path2String = "U98,R91,D20,R16,D67,R40,U7,R15,U6,R7",
		ExpectedDistance = 410,
	},
	//new TestCase{
	//	Name = "my production input",
	//	Path1String="redacted",
	//	Path2String="redacted",
	//	ExpectedDistance = 0,
	//}
};

struct PathPoint2d
{
	public Int32 X;
	public Int32 Y;
	public Int32 ZeroDist => Math.Abs(X)+Math.Abs(Y);
	public Int32 PathDist;
	public Point2d AsPoint2d => new Point2d{X=X, Y=Y};
}

struct Point2d
{
	public Int32 X;
	public Int32 Y;
}

	struct Intersection
{
	public PathPoint2d Path1Point;
	public PathPoint2d Path2Point;
	public Int32 SignalTime => Path1Point.PathDist + Path2Point.PathDist;
}

List<Intersection> GetIntersections(List<PathPoint2d> p1, List<PathPoint2d> p2)
{
	// slow - but faster than me thinking...
	var points1 = p1.Select(p=>p.AsPoint2d).Distinct().ToList();
	var points2 = p2.Select(p=>p.AsPoint2d).Distinct().ToList();
	var commonPoints = points1.Intersect(points2).ToList();
	var intersections = new List<Intersection>();
	PathPoint2d GetNearestTo(List<PathPoint2d> points, Point2d reference){
		return points
			.Where(p=>p.X == reference.X)
			.Where(p=>p.Y == reference.Y)
			.OrderBy(p=>p.PathDist)
			.First();
	}
	foreach(var common in commonPoints){
		var nearestP1 = GetNearestTo(p1, common);
		var nearestP2 = GetNearestTo(p2, common);
		intersections.Add(new Intersection{
			Path1Point = nearestP1,
			Path2Point = nearestP2,
		});
	}
	return intersections;
}

Intersection GetClosest(List<Intersection> intersections)
{
	var byDistance = intersections
		.OrderBy(i=>i.SignalTime)
		.ToList();
	byDistance.Take(2).Dump("closest two");
	return byDistance[1];// don't return start 0:0
}

List<PathPoint2d> GetPoints(String pathString)
{
	var tokens = pathString.Split(",");
	var currentPoints = new List<PathPoint2d>();
	var currentX = 0;
	var currentY = 0;
	var currentPathLength = 0;
	
	void AddPath(Int32 incX, Int32 incY, Int32 dist)
	{
		while (dist > 0)
		{
			currentX += incX;
			currentY += incY;
			--dist;
			++currentPathLength;
			currentPoints.Add(
				new PathPoint2d{
					X = currentX,
					Y = currentY,
					PathDist = currentPathLength});
		}
	}
	
	currentPoints.Add(
		new PathPoint2d{
			X = currentX, 
			Y = currentY,
			PathDist = currentPathLength});
	
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