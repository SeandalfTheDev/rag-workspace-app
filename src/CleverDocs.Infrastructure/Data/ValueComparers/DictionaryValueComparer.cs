using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace CleverDocs.Infrastructure.Data.ValueComparers;

public class DictionaryValueComparer : ValueComparer<Dictionary<string, string>>
{
    public DictionaryValueComparer() : base(
        (c1, c2) => c1!.Count == c2!.Count && c1.All(kvp => c2.ContainsKey(kvp.Key) && c2[kvp.Key] == kvp.Value),
        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.Key.GetHashCode(), v.Value.GetHashCode())),
        c => new Dictionary<string, string>(c))
    {
    }
} 