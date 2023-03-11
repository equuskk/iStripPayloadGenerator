using IStripLight;

var lightController = new PayloadGenerator();
var result = lightController.GetRgbPayload(0, 0, 255, 5);
Console.WriteLine(result);