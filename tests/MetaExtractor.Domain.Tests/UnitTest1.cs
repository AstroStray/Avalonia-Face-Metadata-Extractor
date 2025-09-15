using MetaExtractor.Domain.Entities;

namespace MetaExtractor.Domain.Tests;

public class FaceEntityTests
{
    [Fact]
    public void Face_ShouldInitialize_WithDefaultValues()
    {
        // Arrange & Act
        var face = new Face();

        // Assert
        Assert.Equal(0, face.Id);
        Assert.Equal(default(DateTime), face.Timestamp);
        Assert.NotNull(face.Metadata);
        Assert.Empty(face.Metadata);
    }

    [Fact]
    public void Face_ShouldAllowSettingTimestamp()
    {
        // Arrange
        var face = new Face();
        var expectedTimestamp = DateTime.UtcNow;

        // Act
        face.Timestamp = expectedTimestamp;

        // Assert
        Assert.Equal(expectedTimestamp, face.Timestamp);
    }

    [Fact]
    public void Face_ShouldAllowAddingMetadata()
    {
        // Arrange
        var face = new Face();
        var metadata1 = new Metadata { Key = "age", Value = "25" };
        var metadata2 = new Metadata { Key = "gender", Value = "male" };

        // Act
        face.Metadata.Add(metadata1);
        face.Metadata.Add(metadata2);

        // Assert
        Assert.Equal(2, face.Metadata.Count);
        Assert.Contains(metadata1, face.Metadata);
        Assert.Contains(metadata2, face.Metadata);
    }
}

public class MetadataEntityTests
{
    [Fact]
    public void Metadata_ShouldInitialize_WithDefaultValues()
    {
        // Arrange & Act
        var metadata = new Metadata();

        // Assert
        Assert.Equal(0, metadata.Id);
        Assert.Equal(0, metadata.FaceId);
        Assert.Equal(string.Empty, metadata.Key);
        Assert.Equal(string.Empty, metadata.Value);
        // Note: Face is marked with null! which means it will be null until explicitly set
        // This is expected behavior for Entity Framework navigation properties
    }

    [Fact]
    public void Metadata_ShouldAllowSettingKeyAndValue()
    {
        // Arrange
        var metadata = new Metadata();
        const string expectedKey = "emotion";
        const string expectedValue = "happy";

        // Act
        metadata.Key = expectedKey;
        metadata.Value = expectedValue;

        // Assert
        Assert.Equal(expectedKey, metadata.Key);
        Assert.Equal(expectedValue, metadata.Value);
    }

    [Fact]
    public void Metadata_ShouldAllowSettingFaceRelationship()
    {
        // Arrange
        var metadata = new Metadata();
        var face = new Face { Id = 1, Timestamp = DateTime.UtcNow };

        // Act
        metadata.Face = face;
        metadata.FaceId = face.Id;

        // Assert
        Assert.Equal(face, metadata.Face);
        Assert.Equal(face.Id, metadata.FaceId);
    }
}