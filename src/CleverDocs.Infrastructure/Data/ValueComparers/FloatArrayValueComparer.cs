using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CleverDocs.Infrastructure.Data.ValueComparers;

public class FloatArrayValueComparer : ValueComparer<float[]>
{
    public FloatArrayValueComparer() : base(
        (c1, c2) => c1!.SequenceEqual(c2!),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
        c => c.ToArray())
    {
    }
} 