namespace Garmetix.Commons.Dashboard.Models
{
    public class ChartData
    {
        public string Title { get; set; } = string.Empty;
        public decimal Count { get; set; } = 0;

        public ChartData(string title, decimal count)
        {
            Title = title;
            Count = count;
        }
    }
}
