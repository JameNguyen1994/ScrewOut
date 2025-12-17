using System;
using System.Collections.Generic;
using System.Text;

public static class UriHelper
{
    public static string BuildUri(string basePath, Dictionary<string, string> queryParams)
    {
        var sb = new StringBuilder();
        sb.Append(basePath);
        if (!basePath.Contains("?"))
            sb.Append("?");

        bool first = true;
        foreach (var kvp in queryParams)
        {
            if (!first) sb.Append("&");
            sb.Append(Uri.EscapeDataString(kvp.Key));
            sb.Append("=");
            sb.Append(Uri.EscapeDataString(kvp.Value));
            first = false;
        }

        return sb.ToString();
    }
}