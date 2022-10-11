// unset

#nullable enable
using System;
using Android.Runtime;
using Object = Java.Lang.Object;



namespace Jakar.Extensions.Xamarin.Forms.Droid;


public static class JavaObjectExtensions
{
    public static bool IsDisposed( this Object      obj ) => obj.Handle == IntPtr.Zero;
    public static bool IsAlive( this    Object      obj ) => !obj.IsDisposed();
    public static bool IsDisposed( this IJavaObject obj ) => obj.Handle == IntPtr.Zero;
    public static bool IsAlive( this    IJavaObject obj ) => !obj.IsDisposed();
}
