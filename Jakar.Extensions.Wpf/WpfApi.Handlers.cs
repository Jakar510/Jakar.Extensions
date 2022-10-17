#nullable enable
namespace Jakar.Extensions.Wpf;


public static partial class WpfApi
{
    public static readonly long maxTime = new TimeSpan( 23, 59, 59 ).Ticks;
    public static DateTimeOffset? EndDate_OnSelectedDateTimeChanged( in DatePicker picker, in DateTime? startDate )
    {
        DateTime? selected = picker.SelectedDate;

        if (!selected.HasValue) { return null; }

        DateTime dt = selected.Value;

        if (startDate.HasValue && dt < startDate.Value) { picker.SelectedDate = dt = startDate.Value.Date; }

        return dt.AddTicks( maxTime )
                 .ToUniversalTime();
    }


    public static DateTimeOffset? StartDate_OnSelectedDateTimeChanged( in DatePicker picker, in DateTime? endDate )
    {
        DateTime? selected = picker.SelectedDate;

        // ReSharper disable once InvertIf
        if (endDate.HasValue && selected.HasValue)
        {
            if (selected.Value > endDate.Value) { picker.SelectedDate = endDate.Value.Date; }
        }

        return selected?.Date.ToUniversalTime();
    }
}
