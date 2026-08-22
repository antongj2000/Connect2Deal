namespace Connect2Deal.Constants
{
    public static class Languages
    {
        public const string Bcms = "bcms";
        public const string Albanian = "sq";
        public const string Macedonian = "mk";
        public const string English = "en";

        public static readonly Dictionary<string, string> All = new()
        {
            { English,    "English" },
            { Bcms,       "Crnogorski / srpski / hrvatski / bosanski" },
            { Albanian,   "Shqip" },
            { Macedonian, "Македонски" }
        };

        public static bool IsValid(string? code)
        {
            return !string.IsNullOrWhiteSpace(code) && All.ContainsKey(code);
        }

        public static string DisplayName(string? code)
        {
            return code != null && All.TryGetValue(code, out var name) ? name : "English";
        }
    }
}