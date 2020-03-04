namespace DataMonitoring.Model
{
    public interface IIndicatorColumn
    {
        string Code { get; set; }

        bool Filtered { get; set; }

        string FilteredValue { get; set; }

        bool IsNumericFormat { get; set; }
    }
}
