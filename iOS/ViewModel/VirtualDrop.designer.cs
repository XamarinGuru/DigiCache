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
    [Register ("VirtualDrop")]
    partial class VirtualDrop
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIImageView imgIcon { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDistance { get; set; }

        [Action ("ActionDetail:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionDetail (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (imgIcon != null) {
                imgIcon.Dispose ();
                imgIcon = null;
            }

            if (lblDistance != null) {
                lblDistance.Dispose ();
                lblDistance = null;
            }
        }
    }
}