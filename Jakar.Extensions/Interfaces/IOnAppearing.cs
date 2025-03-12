// Jakar.Extensions :: Jakar.Extensions
// 03/12/2025  15:03

namespace Jakar.Extensions;


public interface IOnAppearing
{
    public void OnAppearing();
}



public interface IOnDisappearing
{
    public void OnDisappearing();
}



public interface IChangeable : IOnAppearing, IOnDisappearing;
