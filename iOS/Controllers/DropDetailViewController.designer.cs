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

        [Action ("ActionGoToLink:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionGoToLink (UIKit.UIButton sender);

        [Action ("ActionPlayVideo:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionPlayVideo (UIKit.UIButton sender);

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
        }
    }
}