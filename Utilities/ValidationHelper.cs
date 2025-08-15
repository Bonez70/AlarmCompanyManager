using System.Text.RegularExpressions;

namespace AlarmCompanyManager.Utilities
{
    public static class ValidationHelper
    {
        private static readonly Regex EmailRegex = new(@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$", RegexOptions.Compiled);
        private static readonly Regex PhoneRegex = new(@"^[\d\s\-\(\)\+\.]{10,}$", RegexOptions.Compiled);
        private static readonly Regex ZipCodeRegex = new(@"^\d{5}(-\d{4})?$", RegexOptions.Compiled);

        public static bool IsValidEmail(string? email)
        {
            return !string.IsNullOrWhiteSpace(email) && EmailRegex.IsMatch(email);
        }

        public static bool IsValidPhoneNumber(string? phone)
        {
            return !string.IsNullOrWhiteSpace(phone) && PhoneRegex.IsMatch(phone);
        }

        public static bool IsValidZipCode(string? zipCode)
        {
            return !string.IsNullOrWhiteSpace(zipCode) && ZipCodeRegex.IsMatch(zipCode);
        }

        public static bool IsValidRequired(string? value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static string FormatPhoneNumber(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return string.Empty;

            var digits = new string(phone.Where(char.IsDigit).ToArray());

            return digits.Length switch
            {
                10 => $"({digits.Substring(0, 3)}) {digits.Substring(3, 3)}-{digits.Substring(6, 4)}",
                11 when digits.StartsWith("1") => $"+1 ({digits.Substring(1, 3)}) {digits.Substring(4, 3)}-{digits.Substring(7, 4)}",
                _ => phone
            };
        }

        public static string CleanPhoneNumber(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone)) return string.Empty;
            return new string(phone.Where(char.IsDigit).ToArray());
        }
    }
}