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
    [Register ("DropDetailViewController")]
    partial class DropDetailViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgImage { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblName { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblText { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton symbolFavorite { get; set; }

        [Action ("ActionFavorite:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionFavorite (UIKit.UIButton sender);

        [Action ("ActionModifyItems:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionModifyItems (UIKit.UIButton sender);

        [Action ("ActionSaveFile:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionSaveFile (UIKit.UIButton sender);

        [Action ("ActionShareDropLocation:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionShareDropLocation (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (imgImage != null) {
                imgImage.Dispose ();
                imgImage = null;
            }

            if (lblName != null) {
                lblName.Dispose ();
                lblName = null;
            }

            if (lblText != null) {
                lblText.Dispose ();
                lblText = null;
            }

            if (symbolFavorite != null) {
                symbolFavorite.Dispose ();
                symbolFavorite = null;
            }
        }
    }
}