using System;
using System.Collections.Generic;

namespace MetaExtractor.Domain.Entities;

public class Face
{
    public int Id { get; set; }
    public DateTime Timestamp { get; set; }

    public ICollection<Metadata> Metadata { get; set; } = new List<Metadata>();
}
