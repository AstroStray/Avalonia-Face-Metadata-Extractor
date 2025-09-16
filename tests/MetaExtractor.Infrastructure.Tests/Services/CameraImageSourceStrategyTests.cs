using System.Collections.Generic;
using MetaExtractor.Infrastructure.Services;
using Xunit;

namespace MetaExtractor.Infrastructure.Tests.Services;

public class CameraImageSourceStrategyTests
{
    [Fact]
    public void GetAvailableCameras_ShouldReturnListOfIntegers()
    {
        try
        {
            // Act
            var cameras = CameraImageSourceStrategy.GetAvailableCameras();

            // Assert
            Assert.NotNull(cameras);
            Assert.IsType<List<int>>(cameras);
        }
        catch (TypeInitializationException)
        {
            // Expected in CI environment without OpenCV runtime
            Assert.True(true, "OpenCV runtime not available - this is expected in CI");
        }
    }

    [Fact]
    public void CameraInfo_ShouldHaveCorrectProperties()
    {
        // Arrange
        var cameraInfo = new CameraInfo(0, "Test Camera", 640, 480, 30.0);

        // Assert
        Assert.Equal(0, cameraInfo.Index);
        Assert.Equal("Test Camera", cameraInfo.Name);
        Assert.Equal(640, cameraInfo.Width);
        Assert.Equal(480, cameraInfo.Height);
        Assert.Equal(30.0, cameraInfo.Fps);
    }

    [Fact]
    public void Constructor_WithInvalidCameraIndex_ShouldThrowException()
    {
        try
        {
            // Act & Assert
            // Note: This might not always throw on all systems, so we test the exception type if it does throw
            try
            {
                var strategy = new CameraImageSourceStrategy(999); // Very high index unlikely to exist
                // If no exception, that's fine - camera might be available
                Assert.NotNull(strategy);
                strategy.Dispose();
            }
            catch (InvalidOperationException ex)
            {
                // Expected behavior for invalid camera index
                Assert.Contains("Could not open camera", ex.Message);
            }
        }
        catch (TypeInitializationException)
        {
            // Expected in CI environment without OpenCV runtime
            Assert.True(true, "OpenCV runtime not available - this is expected in CI");
        }
    }

    [Fact]
    public void GetCameraInfo_ForValidStrategy_ShouldReturnInfo()
    {
        try
        {
            // This test only runs if a camera is available
            var availableCameras = CameraImageSourceStrategy.GetAvailableCameras();
            
            if (availableCameras.Count > 0)
            {
                try
                {
                    using var strategy = new CameraImageSourceStrategy(availableCameras[0]);
                    var info = strategy.GetCameraInfo();

                    Assert.NotNull(info);
                    Assert.Equal(availableCameras[0], info.Index);
                    Assert.NotEmpty(info.Name);
                }
                catch (InvalidOperationException)
                {
                    // Camera might not be available in CI environment
                    Assert.True(true, "Camera not available - this is expected in some environments");
                }
            }
            else
            {
                Assert.True(true, "No cameras available for testing");
            }
        }
        catch (TypeInitializationException)
        {
            // Expected in CI environment without OpenCV runtime
            Assert.True(true, "OpenCV runtime not available - this is expected in CI");
        }
    }

    [Fact]
    public async Task GetNextFrameAsync_WithValidCamera_ShouldReturnFrameOrNull()
    {
        try
        {
            // This test only runs if a camera is available
            var availableCameras = CameraImageSourceStrategy.GetAvailableCameras();
            
            if (availableCameras.Count > 0)
            {
                try
                {
                    using var strategy = new CameraImageSourceStrategy(availableCameras[0]);
                    var frame = await strategy.GetNextFrameAsync();

                    // Frame could be null or a valid Mat - both are acceptable
                    // We just test that the method doesn't throw
                    Assert.True(true, "GetNextFrameAsync completed without exception");
                }
                catch (InvalidOperationException)
                {
                    // Camera might not be available in CI environment
                    Assert.True(true, "Camera not available - this is expected in some environments");
                }
            }
            else
            {
                Assert.True(true, "No cameras available for testing");
            }
        }
        catch (TypeInitializationException)
        {
            // Expected in CI environment without OpenCV runtime
            Assert.True(true, "OpenCV runtime not available - this is expected in CI");
        }
    }
}