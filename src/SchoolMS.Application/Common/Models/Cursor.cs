using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Json;

namespace SchoolMS.Application.Common.Models;

public sealed record Cursor(DateTimeOffset Date, Guid LastId)
{
    public static string Encode(DateTimeOffset date, Guid lastId)
    {
        var cursor = new Cursor(date, lastId);
        string json = JsonSerializer.Serialize(cursor);
        return Base64UrlTextEncoder.Encode(Encoding.UTF8.GetBytes(json));
    }


    public static Cursor? Decode(string encodedCursor)
    {
        if (string.IsNullOrWhiteSpace(encodedCursor))
        {
            return null;
        }

        try
        {
            string json = Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(encodedCursor));
            return JsonSerializer.Deserialize<Cursor>(json);
        }
        catch
        {
            return null;
        }

    }

}
