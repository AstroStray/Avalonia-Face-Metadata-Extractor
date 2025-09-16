using System.Collections.Generic;
using OpenCvSharp;

namespace MetaExtractor.Core.Models;

public class FaceDetectionResult
{
    public List<DetectedFace> Faces { get; set; } = new();
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public string ProcessingMethod { get; set; } = "OpenCV";
}

public class DetectedFace
{
    public Rect BoundingBox { get; set; }
    public double Confidence { get; set; }
    public List<Point2f> Landmarks { get; set; } = new();
    public string? ImagePath { get; set; }
}

public class FaceLandmarks
{
    public Point2f LeftEye { get; set; }
    public Point2f RightEye { get; set; }
    public Point2f NoseTip { get; set; }
    public Point2f LeftMouthCorner { get; set; }
    public Point2f RightMouthCorner { get; set; }
}