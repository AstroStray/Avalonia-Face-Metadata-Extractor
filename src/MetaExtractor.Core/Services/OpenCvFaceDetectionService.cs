using MetaExtractor.Core.Interfaces;
using MetaExtractor.Core.Models;
using OpenCvSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace MetaExtractor.Core.Services;

public class OpenCvFaceDetectionService : IFaceDetectionService, IDisposable
{
    private CascadeClassifier? _faceCascade;
    private bool _isInitialized = false;
    private bool _disposed = false;

    public async Task<bool> IsInitializedAsync()
    {
        if (_isInitialized) return true;

        return await Task.Run(() =>
        {
            try
            {
                // Initialize face cascade classifier with Haar cascades
                _faceCascade = new CascadeClassifier();
                
                // Try to load the face cascade from various possible locations
                var possiblePaths = new[]
                {
                    "haarcascade_frontalface_alt.xml", // Current directory
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "haarcascade_frontalface_alt.xml"), // App base directory
                    Path.Combine(Directory.GetCurrentDirectory(), "haarcascade_frontalface_alt.xml"), // Working directory
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "haarcascade_frontalface_alt.xml") // Resources folder
                };

                string? cascadePath = null;
                foreach (var path in possiblePaths)
                {
                    if (File.Exists(path))
                    {
                        cascadePath = path;
                        break;
                    }
                }
                
                if (cascadePath == null || !_faceCascade.Load(cascadePath))
                {
                    // Cascade file not found or failed to load
                    _faceCascade?.Dispose();
                    _faceCascade = null;
                    return false;
                }

                _isInitialized = true;
                return true;
            }
            catch (Exception)
            {
                _faceCascade?.Dispose();
                _faceCascade = null;
                return false;
            }
        });
    }

    public async Task<FaceDetectionResult> DetectFacesAsync(Mat image, string? imagePath = null)
    {
        if (!await IsInitializedAsync())
        {
            return new FaceDetectionResult
            {
                Success = false,
                ErrorMessage = "Face detection service not initialized. Haar cascade file not found."
            };
        }

        if (image.Empty())
        {
            return new FaceDetectionResult
            {
                Success = false,
                ErrorMessage = "Input image is empty"
            };
        }

        return await Task.Run(() =>
        {
            try
            {
                var result = new FaceDetectionResult
                {
                    Success = true,
                    ProcessingMethod = "OpenCV-HaarCascades"
                };

                // Convert to grayscale for face detection
                using var grayImage = new Mat();
                Cv2.CvtColor(image, grayImage, ColorConversionCodes.BGR2GRAY);

                // Detect faces
                var faces = _faceCascade!.DetectMultiScale(
                    grayImage,
                    scaleFactor: 1.1,
                    minNeighbors: 3,
                    flags: HaarDetectionTypes.ScaleImage,
                    minSize: new Size(30, 30),
                    maxSize: new Size()
                );

                // Convert OpenCV rectangles to our DetectedFace objects
                foreach (var face in faces)
                {
                    var detectedFace = new DetectedFace
                    {
                        BoundingBox = face,
                        Confidence = 1.0, // Haar cascades don't provide confidence scores
                        ImagePath = imagePath,
                        Landmarks = new() // Basic landmark detection would require additional models
                    };

                    result.Faces.Add(detectedFace);
                }

                return result;
            }
            catch (Exception ex)
            {
                return new FaceDetectionResult
                {
                    Success = false,
                    ErrorMessage = $"Face detection failed: {ex.Message}"
                };
            }
        });
    }

    public async Task<FaceDetectionResult> DetectFacesAsync(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
        {
            return new FaceDetectionResult
            {
                Success = false,
                ErrorMessage = "Image path is null or empty"
            };
        }

        try
        {
            using var image = Cv2.ImRead(imagePath);
            return await DetectFacesAsync(image, imagePath);
        }
        catch (Exception ex)
        {
            return new FaceDetectionResult
            {
                Success = false,
                ErrorMessage = $"Failed to load image: {ex.Message}"
            };
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _faceCascade?.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}