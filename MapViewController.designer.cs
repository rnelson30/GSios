// WARNING
//
// This file has been generated automatically by Visual Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace GSios
{
    [Register ("MapViewController")]
    partial class MapViewController
    {
        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton GroupsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView LocationContainer { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIButton LocationsButton { get; set; }

        [Outlet]
        [GeneratedCode ("iOS Designer", "1.0")]
        UIKit.UIView SearchView { get; set; }

        [Action ("GroupsButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void GroupsButton_TouchUpInside (UIKit.UIButton sender);

        [Action ("LocationsButton_TouchUpInside:")]
        [GeneratedCode ("iOS Designer", "1.0")]
        partial void LocationsButton_TouchUpInside (UIKit.UIButton sender);

        void ReleaseDesignerOutlets ()
        {
            if (GroupsButton != null) {
                GroupsButton.Dispose ();
                GroupsButton = null;
            }

            if (GroupsButton != null) {
                GroupsButton.Dispose ();
                GroupsButton = null;
            }

            if (LocationContainer != null) {
                LocationContainer.Dispose ();
                LocationContainer = null;
            }

            if (LocationsButton != null) {
                LocationsButton.Dispose ();
                LocationsButton = null;
            }

            if (LocationsButton != null) {
                LocationsButton.Dispose ();
                LocationsButton = null;
            }

            if (SearchView != null) {
                SearchView.Dispose ();
                SearchView = null;
            }
        }
    }
}