using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MetaExtractor.Core.Models;

public class Landmark
{
    [JsonPropertyName("x")]
    public double X { get; set; }

    [JsonPropertyName("y")]
    public double Y { get; set; }

    [JsonPropertyName("z")]
    public double Z { get; set; }
}

public class FaceDetectionResult
{
    [JsonPropertyName("image_width")]
    public int ImageWidth { get; set; }

    [JsonPropertyName("image_height")]
    public int ImageHeight { get; set; }

    [JsonPropertyName("faces")]
    public List<List<Landmark>> Faces { get; set; } = new();
}
