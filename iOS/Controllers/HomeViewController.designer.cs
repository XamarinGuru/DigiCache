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
    [Register ("HomeViewController")]
    partial class HomeViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView dropContent { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.NSLayoutConstraint dropContentWidth { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView virtualDropContent { get; set; }

        [Action ("ActionDrop:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionDrop (UIKit.UIButton sender);

        [Action ("ActionHome:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionHome (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (dropContent != null) {
                dropContent.Dispose ();
                dropContent = null;
            }

            if (dropContentWidth != null) {
                dropContentWidth.Dispose ();
                dropContentWidth = null;
            }

            if (virtualDropContent != null) {
                virtualDropContent.Dispose ();
                virtualDropContent = null;
            }
        }
    }
}