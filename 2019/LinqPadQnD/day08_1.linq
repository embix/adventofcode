<Query Kind="Program" />

void Main()
{
	var testCase = test1;
	var decodedImage = Decode(test1.EncodedImage);
	decodedImage.Dump("Decoded");
	GetCode(decodedImage).Dump("Code");
}

struct EncodedImage
{
	public Int32 SizeX;
	public Int32 SizeY;
	public String EncodedPixels;
}

struct TestCase
{
	public EncodedImage EncodedImage;
	public DecodedImage DecodedImage;
}

TestCase test1 = new TestCase{
	EncodedImage = new EncodedImage{
		SizeX = 3,
		SizeY = 2,
		EncodedPixels = "123456789012"
	},
	DecodedImage = new DecodedImage{
		new PixelLayer{
			new PixelLine{1,2,3},
			new PixelLine{4,5,6}
		},
		new PixelLayer{
			new PixelLine{7,8,9},
			new PixelLine{0,1,2}
		}
	}
};

// glorified typedefs
class DecodedImage : List<PixelLayer>{}
class PixelLayer : List<PixelLine>{}
class PixelLine : List<Int32>{}

DecodedImage Decode(EncodedImage encoded)
{
	var lineCount = encoded.SizeY;
	var rowCount = encoded.SizeX;
	var layers = new DecodedImage();
	var inputStream = new Queue<Int32>();
	var parsedInput = encoded.EncodedPixels.Select(c=>Int32.Parse(c.ToString())).ToList();
	foreach(var element in parsedInput){
		// we could "draw" the image right here
		inputStream.Enqueue(element);
	}
	
	while(inputStream.Any()){
		var currentLayer = new PixelLayer();
		for (var lineIndex = 0; lineIndex != lineCount; ++lineIndex)
		{
			var currentLine = new PixelLine();
			for (var rowIndex = 0; rowIndex != rowCount; ++rowIndex)
			{
				if(!inputStream.Any()){
					new {
						ParsedLayers = layers,
						CurrentLayer = currentLayer,
						CurrentLine = currentLine,
						encoded.SizeX,
						encoded.SizeY,
						encoded.EncodedPixels,
					}.Dump("expected more pixels in input stream");
					throw new Exception("Halt and catch fire");
				}
				var currentPixel = inputStream.Dequeue();
				currentLine.Add(currentPixel);
			}
			currentLayer.Add(currentLine);
		}			
		// add layer when its finished
		layers.Add(currentLayer);
	}	
	return layers;
}

Int32 GetCode(DecodedImage image)
{
	var mappedLayers = image.Select(layer=>new{
		layer = layer,
		digits = layer.SelectMany(line=>line.Select(row=>row))
	}).ToList()
	.Dump("layers");
	// do it for all layers, althoug only required for code layer
	var layerStats = mappedLayers.Select(layer=>new{
		layer.layer,
		layer.digits,
		zeroCount = layer.digits.Where(d=>d==0).Count(),
		oneCount = layer.digits.Where(d=>d==1).Count(),
		twoCount = layer.digits.Where(d=>d==2).Count(),
	}).ToList()
	.Dump("layer stats");
	var codeLayer = layerStats
		.OrderBy(l=>l.zeroCount)
		.Take(2).Dump("top 2 candidates - should have different 0-digit count")
		.First();
	return codeLayer.oneCount*codeLayer.twoCount;
}