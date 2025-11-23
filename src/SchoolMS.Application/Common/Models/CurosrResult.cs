namespace SchoolMS.Application.Common.Models;

public class CursorResult<T>
{
    public string? Cursor { get; set; }
    public bool HasMore { get; set; }
    public List<T> Items { get; set; } = [];
    public CursorResult()
    {

    }

    public CursorResult(string? cursor, bool hasMore, List<T> items)
    {
        Cursor = cursor;
        HasMore = hasMore;
        Items = items;
    }

    public static CursorResult<T> Create(string? cursor, bool hasMore, List<T> items)
    {
        return new CursorResult<T>(cursor, hasMore, items);
    }
}
