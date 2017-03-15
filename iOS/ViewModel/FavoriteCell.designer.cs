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
    [Register ("FavoriteCell")]
    partial class FavoriteCell
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton btnX { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgIcon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblLat { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblLong { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblName { get; set; }

        [Action ("ActionMap:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionMap (UIKit.UIButton sender);

        [Action ("OnClickX:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void OnClickX (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (btnX != null) {
                btnX.Dispose ();
                btnX = null;
            }

            if (imgIcon != null) {
                imgIcon.Dispose ();
                imgIcon = null;
            }

            if (lblLat != null) {
                lblLat.Dispose ();
                lblLat = null;
            }

            if (lblLong != null) {
                lblLong.Dispose ();
                lblLong = null;
            }

            if (lblName != null) {
                lblName.Dispose ();
                lblName = null;
            }
        }
    }
}