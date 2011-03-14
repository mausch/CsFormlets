using System;

namespace SampleWebApp {
    public static class StringExtensions {
        public static bool IsUrl(this string s) {
            Uri dummy;
            return Uri.TryCreate(s, UriKind.Absolute, out dummy);
        }
    }
}