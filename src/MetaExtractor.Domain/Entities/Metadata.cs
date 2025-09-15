namespace MetaExtractor.Domain.Entities;

public class Metadata
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;

    public int FaceId { get; set; }
    public Face Face { get; set; } = null!;
}
