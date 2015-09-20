// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System;
using System.CodeDom.Compiler;

namespace PinballScoreEntry.iOS
{
	[Register ("PlayerPickerController")]
	partial class PlayerPickerController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView playerPickerView { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISearchBar playerSearchBar { get; set; }

		void ReleaseDesignerOutlets ()
		{
			if (playerPickerView != null) {
				playerPickerView.Dispose ();
				playerPickerView = null;
			}
			if (playerSearchBar != null) {
				playerSearchBar.Dispose ();
				playerSearchBar = null;
			}
		}
	}
}
