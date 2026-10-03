using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace SharedKernel.Commons;

public static class SlugHelper
{
    public static string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return string.Empty;
        }

        var normalizedString = text.Trim().Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                if (c is 'đ' or 'Đ')
                {
                    stringBuilder.Append('d');
                }
                else
                {
                    stringBuilder.Append(c);
                }
            }
        }

        var normalized = stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();

        // Replace invalid chars with hyphen
        normalized = Regex.Replace(normalized, @"[^a-z0-9\s-]", "");
        // Convert multiple spaces/hyphens into single hyphen
        normalized = Regex.Replace(normalized, @"[\s-]+", " ").Trim();
        normalized = Regex.Replace(normalized, @"\s", "-");

        return normalized;
    }
}
