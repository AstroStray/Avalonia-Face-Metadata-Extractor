using System;
using System.Collections.Generic;

namespace MetaExtractor.Domain.Entities;

public class Face
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }
    
    // Face detection properties
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public double Confidence { get; set; }
    
    // Face landmarks (eyes, nose, mouth corners)
    public string? LandmarkData { get; set; } // JSON serialized landmark points
    
    // Facial analysis properties
    public string? ImagePath { get; set; }
    public string? ProcessingMethod { get; set; } // OpenCV, MediaPipe, etc.

    public ICollection<Metadata> Metadata { get; set; } = new List<Metadata>();
}
