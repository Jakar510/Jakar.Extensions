using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;





namespace Jakar.Extensions.Xamarin.Forms.iOS.Extensions.Layout;


public static class LayoutExtensions
{
#region SubViews

    public static IEnumerable<NSLayoutConstraint> AddFull( this UIStackView parent, UIView view )
    {
        parent.AddArrangedSubview(view);
        IEnumerable<NSLayoutConstraint> result = view.SetBounds(parent);
        view.UpdateConstraintsIfNeeded();
        return result;
    }

    public static IEnumerable<NSLayoutConstraint> AddFull( this UIView parent, UIView view )
    {
        parent.AddSubview(view);
        IEnumerable<NSLayoutConstraint> result = view.SetBounds(parent);
        view.UpdateConstraintsIfNeeded();
        return result;
    }

    public static IEnumerable<NSLayoutConstraint> SetContent( this       UITableViewCell cell, in UIView view ) => cell.ContentView.AddFull(view);
    public static void                            SetAccessory( this     UITableViewCell cell, in UIView view ) => cell.AccessoryView = view;
    public static void                            SetEditAccessory( this UITableViewCell cell, in UIView view ) => cell.EditingAccessoryView = view;


#region Popup

    public static IEnumerable<NSLayoutConstraint> SetPopup( this     UITableViewCell cell, in UIView view ) => cell.InputView.AddFull(view);
    public static void                            RemovePopup( this  UITableViewCell cell, UIView    view ) => cell.InputView.Subviews.FirstOrDefault(item => item.Equals(view))?.RemoveFromSuperview();
    public static bool                            CanShowPopup( this UITableViewCell cell ) => cell.CanBecomeFirstResponder;
    public static bool                            ShowPopup( this    UITableViewCell cell ) => cell.BecomeFirstResponder();
    public static bool                            CanHidePopup( this UITableViewCell cell ) => cell.CanResignFirstResponder;
    public static bool                            HidePopup( this    UITableViewCell cell ) => cell.ResignFirstResponder();

#endregion

#endregion


#region Partial Sizes

    public static IEnumerable<NSLayoutConstraint> LeftExtended( this UIView view, in UIView parent, in UIView right ) =>
        view.SetBounds(parent.TopAnchor, parent.BottomAnchor, parent.LeftAnchor, right.LeftAnchor);

    public static IEnumerable<NSLayoutConstraint> RightExtended( this UIView view, in UIView parent, in UIView left ) =>
        view.SetBounds(parent.TopAnchor, parent.BottomAnchor, left.RightAnchor, parent.RightAnchor);


    public static IEnumerable<NSLayoutConstraint> InBetween( this UIView view, in UIView parent, in UIView left, in UIView right ) =>
        view.SetBounds(parent.TopAnchor, parent.BottomAnchor, right.LeftAnchor, left.RightAnchor);


    public static IEnumerable<NSLayoutConstraint> SetBounds( this UIView view, in UIView parent, in bool translate = false ) => view.SetBounds(parent.TopAnchor,
                                                                                                                                               parent.BottomAnchor,
                                                                                                                                               parent.LeftAnchor,
                                                                                                                                               parent.RightAnchor,
                                                                                                                                               translate);

    public static IEnumerable<NSLayoutConstraint> SetBounds( this UIView              view,
                                                             in   NSLayoutYAxisAnchor top,
                                                             in   NSLayoutYAxisAnchor bottom,
                                                             in   NSLayoutXAxisAnchor left,
                                                             in   NSLayoutXAxisAnchor right,
                                                             in   bool                translate = false
    )
    {
        view.TranslatesAutoresizingMaskIntoConstraints = translate;

        var constraints = new List<NSLayoutConstraint>();

        constraints.AddRange(view.SetWidthBounds(left, right));
        constraints.AddRange(view.SetHeightBounds(top, bottom));

        return constraints;
    }

    public static IEnumerable<NSLayoutConstraint> SetWidthBounds( this UIView view, in NSLayoutXAxisAnchor left, in NSLayoutXAxisAnchor right )
    {
        var constraints = new List<NSLayoutConstraint>();

        NSLayoutConstraint leftConstraint = view.LeftAnchor.ConstraintEqualTo(left);
        leftConstraint.Active = true;
        constraints.Add(leftConstraint);

        NSLayoutConstraint rightConstraint = view.RightAnchor.ConstraintEqualTo(right);
        rightConstraint.Active = true;
        constraints.Add(rightConstraint);

        return constraints;
    }

    public static IEnumerable<NSLayoutConstraint> SetHeightBounds( this UIView view, in NSLayoutYAxisAnchor top, in NSLayoutYAxisAnchor bottom )
    {
        var constraints = new List<NSLayoutConstraint>();

        NSLayoutConstraint topConstraint = view.TopAnchor.ConstraintEqualTo(top);
        topConstraint.Active = true;
        constraints.Add(topConstraint);

        NSLayoutConstraint bottomConstraint = view.BottomAnchor.ConstraintEqualTo(bottom);
        bottomConstraint.Active = true;
        constraints.Add(bottomConstraint);

        return constraints;
    }


#region WidthOf

    public static NSLayoutConstraint WidthOf( this UIView view, in UIView other, in nfloat leftFactor, in nfloat rightFactor ) =>
        view.WidthOf(other, Math.Abs(leftFactor - rightFactor).ToNFloat());

    public static NSLayoutConstraint WidthOf( this UIView view, in UIView other, in nfloat factor )
    {
        NSLayoutConstraint anchor = view.WidthAnchor.ConstraintEqualTo(other.WidthAnchor, factor);
        anchor.Active = true;
        return anchor;
    }

    public static NSLayoutConstraint MinimumWidthOf( this UIView view, in UIView other, in nfloat factor )
    {
        NSLayoutConstraint anchor = view.WidthAnchor.ConstraintGreaterThanOrEqualTo(other.WidthAnchor, factor);
        anchor.Active = true;
        return anchor;
    }

    public static NSLayoutConstraint MaximumWidthOf( this UIView view, in UIView other, in nfloat factor )
    {
        NSLayoutConstraint anchor = view.WidthAnchor.ConstraintLessThanOrEqualTo(other.WidthAnchor, factor);
        anchor.Active = true;
        return anchor;
    }

#endregion


#region HeightOf

    public static NSLayoutConstraint HeightOf( this UIView view, in UIView other, in nfloat topFactor, in nfloat bottomFactor ) =>
        view.HeightOf(other, Math.Abs(topFactor - bottomFactor).ToNFloat());

    public static NSLayoutConstraint HeightOf( this UIView view, in UIView other, in nfloat factor )
    {
        NSLayoutConstraint anchor = view.HeightAnchor.ConstraintEqualTo(other.HeightAnchor, factor);
        anchor.Active = true;
        return anchor;
    }

    public static NSLayoutConstraint MinimumHeightOf( this UIView view, in UIView other, in nfloat factor )
    {
        NSLayoutConstraint anchor = view.HeightAnchor.ConstraintGreaterThanOrEqualTo(other.WidthAnchor, factor);
        anchor.Active = true;
        return anchor;
    }

    public static NSLayoutConstraint MaximumHeightOf( this UIView view, in UIView other, in nfloat factor )
    {
        NSLayoutConstraint anchor = view.HeightAnchor.ConstraintLessThanOrEqualTo(other.WidthAnchor, factor);
        anchor.Active = true;
        return anchor;
    }

#endregion

#endregion


#region Positions

    public static NSLayoutConstraint LeftOf( this UIView view, in UIView other )
    {
        NSLayoutConstraint anchor = view.LeftAnchor.ConstraintEqualTo(other.RightAnchor);
        anchor.Active = true;
        return anchor;
    }

    public static NSLayoutConstraint LeftOf( this UIView view, in UIView other, in nfloat factor )
    {
        NSLayoutConstraint anchor = view.LeftAnchor.ConstraintEqualTo(other.RightAnchor, factor);
        anchor.Active = true;
        return anchor;
    }


    public static NSLayoutConstraint RightOf( this UIView view, in UIView other )
    {
        NSLayoutConstraint anchor = view.RightAnchor.ConstraintEqualTo(other.LeftAnchor);
        anchor.Active = true;
        return anchor;
    }

    public static NSLayoutConstraint RightOf( this UIView view, in UIView other, in nfloat factor )
    {
        NSLayoutConstraint anchor = view.RightAnchor.ConstraintEqualTo(other.RightAnchor, factor);
        anchor.Active = true;
        return anchor;
    }


    public static NSLayoutConstraint BelowOf( this UIView view, in UIView other )
    {
        NSLayoutConstraint anchor = view.TopAnchor.ConstraintEqualTo(other.BottomAnchor);
        anchor.Active = true;
        return anchor;
    }

    public static NSLayoutConstraint BelowOf( this UIView view, in UIView other, in nfloat factor )
    {
        NSLayoutConstraint anchor = view.TopAnchor.ConstraintEqualTo(other.BottomAnchor, factor);
        anchor.Active = true;
        return anchor;
    }


    public static NSLayoutConstraint AboveOf( this UIView view, in UIView other )
    {
        NSLayoutConstraint anchor = view.BottomAnchor.ConstraintEqualTo(other.TopAnchor);
        anchor.Active = true;
        return anchor;
    }

    public static NSLayoutConstraint AboveOf( this UIView view, in UIView other, in nfloat factor )
    {
        NSLayoutConstraint anchor = view.BottomAnchor.ConstraintEqualTo(other.TopAnchor, factor);
        anchor.Active = true;
        return anchor;
    }

#endregion
}
