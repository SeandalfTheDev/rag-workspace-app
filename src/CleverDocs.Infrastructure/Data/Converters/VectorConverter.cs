using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Pgvector;

namespace CleverDocs.Infrastructure.Data.Converters;

public class VectorConverter : ValueConverter<float[], Vector>
{
    public VectorConverter() : base(
        floatArray => new Vector(floatArray),
        vector => vector.ToArray())
    {
    }
} 