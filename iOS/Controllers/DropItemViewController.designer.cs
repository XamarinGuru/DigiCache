// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace Drop.iOS
{
    [Register ("DropItemViewController")]
    partial class DropItemViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint heightExpiry { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint heightIcon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint heightName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint heightPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint heightPermission { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextView txtDescription { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITextField txtName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewExpiry { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewIcon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewPassword { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewPermission { get; set; }

        [Action ("ActionColleps:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionColleps (UIKit.UIButton sender);

        [Action ("ActionCustomIcon:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionCustomIcon (UIKit.UIButton sender);

        [Action ("ActionDefailtIcon:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionDefailtIcon (UIKit.UIButton sender);

        [Action ("ActionDropItem:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionDropItem (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (heightExpiry != null) {
                heightExpiry.Dispose ();
                heightExpiry = null;
            }

            if (heightIcon != null) {
                heightIcon.Dispose ();
                heightIcon = null;
            }

            if (heightName != null) {
                heightName.Dispose ();
                heightName = null;
            }

            if (heightPassword != null) {
                heightPassword.Dispose ();
                heightPassword = null;
            }

            if (heightPermission != null) {
                heightPermission.Dispose ();
                heightPermission = null;
            }

            if (txtDescription != null) {
                txtDescription.Dispose ();
                txtDescription = null;
            }

            if (txtName != null) {
                txtName.Dispose ();
                txtName = null;
            }

            if (viewExpiry != null) {
                viewExpiry.Dispose ();
                viewExpiry = null;
            }

            if (viewIcon != null) {
                viewIcon.Dispose ();
                viewIcon = null;
            }

            if (viewName != null) {
                viewName.Dispose ();
                viewName = null;
            }

            if (viewPassword != null) {
                viewPassword.Dispose ();
                viewPassword = null;
            }

            if (viewPermission != null) {
                viewPermission.Dispose ();
                viewPermission = null;
            }
        }
    }
}