// Jakar.Extensions :: Jakar.Extensions
// 11/18/2025  22:48

namespace Jakar.Extensions;


public sealed class ProgressTracker<TNumber>() : Progress<TNumber>()
    where TNumber : INumber<TNumber>
{
    private TNumber __number = TNumber.Zero;
    public  TNumber Min  { get; init; } = TNumber.Zero;
    public  TNumber Max  { get; init; } = TNumber.CreateTruncating(100);
    public  TNumber Step { get; init; } = TNumber.One;


    public void Tick()
    {
        __number += Step;
        Report(__number);
    }
    public void Report( TNumber value ) => OnReport(value);
    protected override void OnReport( TNumber value )
    {
        TNumber result = TNumber.Clamp(value, Min, Max);
        base.OnReport(result);
    }
}
