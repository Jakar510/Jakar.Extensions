using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Android.Views;
using Android.Widget;
using AGridLayout = Android.Widget.GridLayout;
using AContext = Android.Content.Context;
using AView = Android.Views.View;
using AObject = Java.Lang.Object;


#nullable enable
namespace Jakar.Extensions.Xamarin.Forms.Droid;


[global::Android.Runtime.Preserve(AllMembers = true)]
public enum Layout
{
    Match,
    Wrap,
}



[global::Android.Runtime.Preserve(AllMembers = true)]
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



[global::Android.Runtime.Preserve(AllMembers = true)]
public static class AndroidLayoutExtensions
{
    private static readonly Dictionary<Layout, int> LayoutMapper = new()
                                                                   {
                                                                       { Layout.Match, ViewGroup.LayoutParams.MatchParent },
                                                                       { Layout.Wrap, ViewGroup.LayoutParams.WrapContent },
                                                                   };


    private static readonly Dictionary<GridSpec, AGridLayout.Alignment?> SpecMapper = new()
                                                                                      {
                                                                                          { GridSpec.BaselineAlignment, AGridLayout.BaselineAlighment },
                                                                                          { GridSpec.BottomAlignment, AGridLayout.BottomAlighment },
                                                                                          { GridSpec.Center, AGridLayout.Center },
                                                                                          { GridSpec.End, AGridLayout.End },
                                                                                          { GridSpec.Fill, AGridLayout.Fill },
                                                                                          { GridSpec.LeftAlignment, AGridLayout.LeftAlighment },
                                                                                          { GridSpec.RightAlignment, AGridLayout.RightAlighment },
                                                                                          { GridSpec.Start, AGridLayout.Start },
                                                                                          { GridSpec.TopAlignment, AGridLayout.TopAlighment },
                                                                                      };


    private static void Run( Action action, string caller )
    {
        try { action(); }
        catch ( Exception )
        {
            var temp = new StackTrace();
            Console.WriteLine($"------------------------- {caller} -------------------------\n\n");
            Console.WriteLine(temp + "\n\n");
            throw;
        }
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------

    public static AGridLayout.Spec GetSpec( GridSpec spec, float weight ) =>
        AGridLayout.InvokeSpec(AGridLayout.Undefined, SpecMapper[spec], weight) ?? throw new NullReferenceException(nameof(AGridLayout.InvokeSpec));

    public static void SetSpec( this AView view, GridSpec spec, float weight )
    {
        switch ( view.LayoutParameters )
        {
            case null: return;

            case AGridLayout.LayoutParams parameters:
                parameters.ColumnSpec = GetSpec(spec, weight);
                break;
        }

        // throw new ArgumentException("view's LayoutParameters is not Android.Widget.GridLayout.LayoutParams", nameof(view));
    }

    public static void Add( this AGridLayout          stack,
                            AView                     view,
                            int                       row,
                            int                       column,
                            GridSpec                  columnPos,
                            GridSpec                  rowPos,
                            Layout                    width        = Layout.Wrap,
                            Layout                    height       = Layout.Wrap,
                            GravityFlags?             gravity      = null,
                            int                       bottomMargin = 4,
                            int                       topMargin    = 4,
                            int                       leftMargin   = 10,
                            int                       rightMargin  = 10,
                            [CallerMemberName] string caller       = ""
    ) =>
        stack.Add(view,
                  row,
                  column,
                  SpecMapper[columnPos],
                  SpecMapper[rowPos],
                  width,
                  height,
                  gravity,
                  bottomMargin,
                  topMargin,
                  leftMargin,
                  rightMargin,
                  caller);

    public static void Add( this AGridLayout          stack,
                            AView                     view,
                            int                       row,
                            int                       column,
                            AGridLayout.Alignment?    columnPos,
                            AGridLayout.Alignment?    rowPos,
                            Layout                    width        = Layout.Wrap,
                            Layout                    height       = Layout.Wrap,
                            GravityFlags?             gravity      = null,
                            int                       bottomMargin = 4,
                            int                       topMargin    = 4,
                            int                       leftMargin   = 10,
                            int                       rightMargin  = 10,
                            [CallerMemberName] string caller       = ""
    ) =>
        stack.Add(view,
                  row,
                  column,
                  AGridLayout.InvokeSpec(column, columnPos),
                  AGridLayout.InvokeSpec(row,    rowPos),
                  width,
                  height,
                  gravity,
                  bottomMargin,
                  topMargin,
                  leftMargin,
                  rightMargin,
                  caller);

    public static void Add( this AGridLayout          stack,
                            AView                     view,
                            int                       row,
                            int                       column,
                            AGridLayout.Spec?         columnPos,
                            AGridLayout.Spec?         rowPos,
                            Layout                    width        = Layout.Wrap,
                            Layout                    height       = Layout.Wrap,
                            GravityFlags?             gravity      = null,
                            int                       bottomMargin = 4,
                            int                       topMargin    = 4,
                            int                       leftMargin   = 10,
                            int                       rightMargin  = 10,
                            [CallerMemberName] string caller       = ""
    )
    {
        if ( stack is null ) { throw new NullReferenceException(nameof(stack)); }

        Run(() =>
            {
                using var layoutParams = new AGridLayout.LayoutParams()
                                         {
                                             ColumnSpec   = columnPos,
                                             RowSpec      = rowPos,
                                             Width        = LayoutMapper[width],
                                             Height       = LayoutMapper[height],
                                             BottomMargin = bottomMargin,
                                             TopMargin    = topMargin,
                                             LeftMargin   = leftMargin,
                                             RightMargin  = rightMargin,
                                         };

                {
                    if ( gravity != null ) { layoutParams.SetGravity((GravityFlags)gravity); }

                    stack.AddView(view, layoutParams);
                }
            },
            caller);
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------

    public static void Add( this LinearLayout stack, AView view, Layout width, Layout height, GravityFlags? gravity = null, [CallerMemberName] string caller = "" )
    {
        if ( stack is null ) { throw new NullReferenceException(nameof(stack)); }

        Run(() =>
            {
                using var layoutParams = new LinearLayout.LayoutParams(LayoutMapper[width], LayoutMapper[height]);

                {
                    if ( gravity != null ) { layoutParams.Gravity = (GravityFlags)gravity; }

                    stack.AddView(view, layoutParams);
                }
            },
            caller);
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------

    public static void Add( this RelativeLayout stack, AView view, Layout width = Layout.Wrap, Layout height = Layout.Wrap, [CallerMemberName] string? caller = default )
    {
        if ( stack is null ) { throw new NullReferenceException(nameof(stack)); }

        Run(() =>
            {
                using var layoutParams = new RelativeLayout.LayoutParams(LayoutMapper[width], LayoutMapper[height]);

                {
                    stack.AddView(view, layoutParams);
                }
            },
            caller);
    }

    // -----------------------------------------------------------------------------------------------------------------------------------------------------


    public static AView CreateContentView( this AContext context, ViewGroup? root, int id, bool attach = true, [CallerMemberName] string? caller = default )
    {
        AObject? temp     = context.GetSystemService(AContext.LayoutInflaterService);
        var      inflater = (LayoutInflater)( temp ?? throw new NullReferenceException(nameof(AContext.LayoutInflaterService)) );

        return inflater.Inflate(id, root, attach) ?? throw new InflateException($"ID: {id} not found. Called from {caller}");
    }
}
