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
    [Register ("PurchasePopUp")]
    partial class PurchasePopUp
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblBtnTitle { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDescription1 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblDescription2 { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UILabel lblTitle { get; set; }

        [Action ("ActionClose:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionClose (UIKit.UIButton sender);

        [Action ("ActionPurchase:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionPurchase (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (lblBtnTitle != null) {
                lblBtnTitle.Dispose ();
                lblBtnTitle = null;
            }

            if (lblDescription1 != null) {
                lblDescription1.Dispose ();
                lblDescription1 = null;
            }

            if (lblDescription2 != null) {
                lblDescription2.Dispose ();
                lblDescription2 = null;
            }

            if (lblTitle != null) {
                lblTitle.Dispose ();
                lblTitle = null;
            }
        }
    }
}