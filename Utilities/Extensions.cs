using System.Collections.ObjectModel;

namespace AlarmCompanyManager.Utilities
{
    public static class Extensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }

        public static void AddRange<T>(this ObservableCollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static void Sort<T>(this ObservableCollection<T> collection, Comparison<T> comparison)
        {
            var sortedList = collection.OrderBy(x => x, Comparer<T>.Create(comparison)).ToList();
            collection.Clear();
            foreach (var item in sortedList)
            {
                collection.Add(item);
            }
        }

        public static string ToDisplayString(this DateTime? dateTime, string format = "MM/dd/yyyy")
        {
            return dateTime?.ToString(format) ?? "N/A";
        }

        public static string ToDisplayString(this TimeSpan? timeSpan, string format = @"hh\:mm")
        {
            return timeSpan?.ToString(format) ?? "N/A";
        }

        public static bool IsNullOrEmpty(this string? value)
        {
            return string.IsNullOrEmpty(value);
        }

        public static bool IsNullOrWhiteSpace(this string? value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}