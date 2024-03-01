namespace viki_01.Services;

public interface IMapper<TOriginal, TTransformed>
{
    public TTransformed Map(TOriginal original);
    public TOriginal Map(TTransformed transformed);
    
    public void Map(TOriginal source, TOriginal destination);

    public ICollection<TTransformed> Map(IEnumerable<TOriginal> originals)
    {
        var transformed = new List<TTransformed>();
        
        foreach (var original in originals)
        {
            transformed.Add(Map(original));
        }
        
        return transformed;
    }
    
    public ICollection<TOriginal> Map(IEnumerable<TTransformed> transformed)
    {
        var originals = new List<TOriginal>();
        
        foreach (var transformedItem in transformed)
        {
            originals.Add(Map(transformedItem));
        }
        
        return originals;
    }
}