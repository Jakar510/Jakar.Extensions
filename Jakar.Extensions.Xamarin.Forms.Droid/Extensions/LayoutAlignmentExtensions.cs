﻿#nullable enable
using Android.Runtime;
using Android.Views;
using Xamarin.Forms;



namespace Jakar.Extensions.Xamarin.Forms.Droid;


[Preserve( AllMembers = true )]
public static class LayoutAlignmentExtensions
{
    public static GravityFlags ToNativeHorizontal( this LayoutAlignment forms ) =>
        forms switch
        {
            LayoutAlignment.Start  => GravityFlags.Start,
            LayoutAlignment.Center => GravityFlags.CenterHorizontal,
            LayoutAlignment.End    => GravityFlags.End,
            _                      => GravityFlags.FillVertical,
        };
    public static GravityFlags ToNativeVertical( this LayoutAlignment forms ) =>
        forms switch
        {
            LayoutAlignment.Start  => GravityFlags.Top,
            LayoutAlignment.Center => GravityFlags.CenterVertical,
            LayoutAlignment.End    => GravityFlags.Bottom,
            _                      => GravityFlags.FillHorizontal,
        };
}
