namespace SharedKernel.Commons;
public class PagedList<T>
{
    public IReadOnlyCollection<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalCount { get; }
    public int TotalPages { get; }
    public bool HasPreviousPage => Page > 1;
    public bool HasNextPage => Page < TotalPages;

    public PagedList(IReadOnlyCollection<T> items, int page, int pageSize, int totalCount)
    {
        Items = items;
        Page = page <= 0 ? 1 : page;
        PageSize = pageSize <= 0 ? 10 : pageSize;
        TotalCount = totalCount;
        TotalPages = (int)Math.Ceiling(TotalCount / (double)PageSize);
    }

    public static PagedList<T> Create(IEnumerable<T> source, int pageIndex, int pageSize)
    {
        pageIndex = pageIndex <= 0 ? 1 : pageIndex;
        pageSize = pageSize <= 0 ? 10 : pageSize;
        var enumerable = source as T[] ?? source.ToArray();
        var count = enumerable.Length;
        var items = enumerable.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        return new PagedList<T>(items, pageIndex, pageSize, count);
    }
}
