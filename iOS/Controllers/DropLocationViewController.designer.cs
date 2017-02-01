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
    [Register ("DropLocationViewController")]
    partial class DropLocationViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView bgTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UITableView locationListTableView { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UISearchBar txtSearchBar { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView viewMapContent { get; set; }

        [Action ("ActionConfirmLocation:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void ActionConfirmLocation (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (bgTableView != null) {
                bgTableView.Dispose ();
                bgTableView = null;
            }

            if (locationListTableView != null) {
                locationListTableView.Dispose ();
                locationListTableView = null;
            }

            if (txtSearchBar != null) {
                txtSearchBar.Dispose ();
                txtSearchBar = null;
            }

            if (viewMapContent != null) {
                viewMapContent.Dispose ();
                viewMapContent = null;
            }
        }
    }
}