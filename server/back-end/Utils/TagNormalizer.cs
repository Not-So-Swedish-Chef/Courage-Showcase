namespace back_end.Utils
{
    public static class TagNormalizer
    {
        public static string Normalize(string input)
        {
            return (input ?? string.Empty).Trim().ToUpperInvariant();
        }
    }
}
