﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AGridLayout = Android.Widget.GridLayout;
using AContext = Android.Content.Context;
using AView = Android.Views.View;
using AObject = Java.Lang.Object;


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.Droid;


[Preserve( AllMembers = true )]
public enum Layout
{
    Match,
    Wrap
}



[Preserve( AllMembers = true )]
public enum GridSpec
{
    BaselineAlignment,
    BottomAlignment,
    Center,
    End,
    Fill,
    LeftAlignment,
    RightAlignment,
    Start,
    TopAlignment
}



[Preserve( AllMembers = true )]
public static class AndroidLayoutExtensions
{
    private static readonly IReadOnlyDictionary<Layout, int> _layoutMapper = new Dictionary<Layout, int>
                                                                             {
                                                                                 { Layout.Match, ViewGroup.LayoutParams.MatchParent },
                                                                                 { Layout.Wrap, ViewGroup.LayoutParams.WrapContent }
                                                                             };


    private static readonly IReadOnlyDictionary<GridSpec, AGridLayout.Alignment?> _specMapper = new Dictionary<GridSpec, AGridLayout.Alignment?>
                                                                                                {
                                                                                                    { GridSpec.BaselineAlignment, AGridLayout.BaselineAlighment },
                                                                                                    { GridSpec.BottomAlignment, AGridLayout.BottomAlighment },
                                                                                                    { GridSpec.Center, AGridLayout.Center },
                                                                                                    { GridSpec.End, AGridLayout.End },
                                                                                                    { GridSpec.Fill, AGridLayout.Fill },
                                                                                                    { GridSpec.LeftAlignment, AGridLayout.LeftAlighment },
                                                                                                    { GridSpec.RightAlignment, AGridLayout.RightAlighment },
                                                                                                    { GridSpec.Start, AGridLayout.Start },
                                                                                                    { GridSpec.TopAlignment, AGridLayout.TopAlighment }
                                                                                                };

    public static void Add( this AGridLayout           stack,
                            AView                      view,
                            int                        row,
                            int                        column,
                            GridSpec                   columnPos,
                            GridSpec                   rowPos,
                            Layout                     width        = Layout.Wrap,
                            Layout                     height       = Layout.Wrap,
                            GravityFlags?              gravity      = null,
                            int                        bottomMargin = 4,
                            int                        topMargin    = 4,
                            int                        leftMargin   = 10,
                            int                        rightMargin  = 10,
                            [CallerMemberName] string? caller       = default
    ) =>
        stack.Add( view,
                   row,
                   column,
                   _specMapper[columnPos],
                   _specMapper[rowPos],
                   width,
                   height,
                   gravity,
                   bottomMargin,
                   topMargin,
                   leftMargin,
                   rightMargin,
                   caller );

    public static void Add( this AGridLayout           stack,
                            AView                      view,
                            int                        row,
                            int                        column,
                            AGridLayout.Alignment?     columnPos,
                            AGridLayout.Alignment?     rowPos,
                            Layout                     width        = Layout.Wrap,
                            Layout                     height       = Layout.Wrap,
                            GravityFlags?              gravity      = null,
                            int                        bottomMargin = 4,
                            int                        topMargin    = 4,
                            int                        leftMargin   = 10,
                            int                        rightMargin  = 10,
                            [CallerMemberName] string? caller       = default
    ) =>
        stack.Add( view,
                   row,
                   column,
                   AGridLayout.InvokeSpec( column, columnPos ),
                   AGridLayout.InvokeSpec( row,    rowPos ),
                   width,
                   height,
                   gravity,
                   bottomMargin,
                   topMargin,
                   leftMargin,
                   rightMargin,
                   caller );

    public static void Add( this AGridLayout           stack,
                            AView                      view,
                            int                        row,
                            int                        column,
                            AGridLayout.Spec?          columnPos,
                            AGridLayout.Spec?          rowPos,
                            Layout                     width        = Layout.Wrap,
                            Layout                     height       = Layout.Wrap,
                            GravityFlags?              gravity      = null,
                            int                        bottomMargin = 4,
                            int                        topMargin    = 4,
                            int                        leftMargin   = 10,
                            int                        rightMargin  = 10,
                            [CallerMemberName] string? caller       = default
    )
    {
        if (stack is null) { throw new NullReferenceException( nameof(stack) ); }

        Run( () =>
             {
                 using var layoutParams = new GridLayout.LayoutParams
                                          {
                                              ColumnSpec   = columnPos,
                                              RowSpec      = rowPos,
                                              Width        = _layoutMapper[width],
                                              Height       = _layoutMapper[height],
                                              BottomMargin = bottomMargin,
                                              TopMargin    = topMargin,
                                              LeftMargin   = leftMargin,
                                              RightMargin  = rightMargin
                                          };

                 {
                     if (gravity != null) { layoutParams.SetGravity( (GravityFlags)gravity ); }

                     stack.AddView( view, layoutParams );
                 }
             },
             caller );
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------

    public static void Add( this LinearLayout stack, AView view, Layout width, Layout height, GravityFlags? gravity = null, [CallerMemberName] string? caller = default )
    {
        if (stack is null) { throw new NullReferenceException( nameof(stack) ); }

        Run( () =>
             {
                 using var layoutParams = new LinearLayout.LayoutParams( _layoutMapper[width], _layoutMapper[height] );

                 {
                     if (gravity != null) { layoutParams.Gravity = (GravityFlags)gravity; }

                     stack.AddView( view, layoutParams );
                 }
             },
             caller );
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------

    public static void Add( this RelativeLayout stack, AView view, Layout width = Layout.Wrap, Layout height = Layout.Wrap, [CallerMemberName] string? caller = default )
    {
        if (stack is null) { throw new NullReferenceException( nameof(stack) ); }

        Run( () =>
             {
                 using var layoutParams = new RelativeLayout.LayoutParams( _layoutMapper[width], _layoutMapper[height] );

                 {
                     stack.AddView( view, layoutParams );
                 }
             },
             caller );
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------


    public static AView CreateContentView( this AContext context, ViewGroup? root, int id, bool attach = true, [CallerMemberName] string? caller = default )
    {
        AObject? temp     = context.GetSystemService( AContext.LayoutInflaterService );
        var      inflater = (LayoutInflater)(temp ?? throw new NullReferenceException( nameof(AContext.LayoutInflaterService) ));

        return inflater.Inflate( id, root, attach ) ?? throw new InflateException( $"ID: {id} not found. Called from {caller}" );
    }


    // -----------------------------------------------------------------------------------------------------------------------------------------------------

    public static GridLayout.Spec GetSpec( GridSpec spec, float weight ) =>
        AGridLayout.InvokeSpec( AGridLayout.Undefined, _specMapper[spec], weight ) ?? throw new NullReferenceException( nameof(AGridLayout.InvokeSpec) );


    [MethodImpl( MethodImplOptions.AggressiveInlining )]
    private static void Run( Action action, string? caller )
    {
        try { action(); }
        catch (Exception)
        {
            var temp = new StackTrace();
            Console.WriteLine( $"------------------------- {caller} -------------------------\n\n" );
            Console.WriteLine( temp + "\n\n" );
            throw;
        }
    }

    public static void SetSpec( this AView view, GridSpec spec, float weight )
    {
        switch (view.LayoutParameters)
        {
            case null: return;

            case GridLayout.LayoutParams parameters:
                parameters.ColumnSpec = GetSpec( spec, weight );
                break;
        }

        // throw new ArgumentException("view's LayoutParameters is not Android.Widget.GridLayout.LayoutParams", nameof(view));
    }
}
