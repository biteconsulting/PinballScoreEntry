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
	[Register ("ConfirmScoreController")]
	partial class ConfirmScoreController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnAccept { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch cbxBonus { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtMachine { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtPlayer { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtRound { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtScore { get; set; }

		[Action ("OnAccept:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void OnAccept (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnAccept != null) {
				btnAccept.Dispose ();
				btnAccept = null;
			}
			if (cbxBonus != null) {
				cbxBonus.Dispose ();
				cbxBonus = null;
			}
			if (txtMachine != null) {
				txtMachine.Dispose ();
				txtMachine = null;
			}
			if (txtPlayer != null) {
				txtPlayer.Dispose ();
				txtPlayer = null;
			}
			if (txtRound != null) {
				txtRound.Dispose ();
				txtRound = null;
			}
			if (txtScore != null) {
				txtScore.Dispose ();
				txtScore = null;
			}
		}
	}
}
