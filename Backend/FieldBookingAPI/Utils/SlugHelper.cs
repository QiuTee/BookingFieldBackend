namespace FieldBookingAPI.Utils
{
    public static class SlugHelper
    {
        public static string GenerateSlug(string name)
        {
            return name 
                .ToLower()
                .Trim()
                .Replace(" ", "_")
                .Replace("đ", "d")
                .Replace("Đ", "d")
                .Normalize(System.Text.NormalizationForm.FormD)
                .Where(c => char.IsLetterOrDigit(c) || c == '_')
                .Aggregate("", (acc, c) => acc + c);

        }
    }
}