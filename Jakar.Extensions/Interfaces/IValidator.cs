// Jakar.Extensions :: Jakar.Extensions
// 05/17/2022  4:19 PM

namespace Jakar.Extensions.Interfaces;


public interface IValidator
{
    public bool IsValid { get; }
}




public interface INotifyValidator : IValidator, INotifyPropertyChanged { }