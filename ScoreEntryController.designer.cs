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
	[Register ("ScoreEntryController")]
	partial class ScoreEntryController
	{
		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnSearch { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIButton btnVoid { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UISwitch cbxBonus { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITableView gridPlayer { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblBonus { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblBonusEntry { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblMachine { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblPlayer { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField lblPlayerBonus { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField lblPlayerPlayed { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UILabel lblScore { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIPickerView pickMachine { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UINavigationItem ScoreEntryNavigation { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtPlayer { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtPlayerName { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UITextField txtScore { get; set; }

		[Outlet]
		[GeneratedCode ("iOS Designer", "1.0")]
		UIView viewPlayer { get; set; }

		[Action ("PlayerEditChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void PlayerEditChanged (UITextField sender);

		[Action ("PlayerNumberChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void PlayerNumberChanged (UITextField sender);

		[Action ("ScoreEditChanged:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void ScoreEditChanged (UITextField sender);

		[Action ("SearchClicked:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void SearchClicked (UIButton sender);

		[Action ("VoidClicked:")]
		[GeneratedCode ("iOS Designer", "1.0")]
		partial void VoidClicked (UIButton sender);

		void ReleaseDesignerOutlets ()
		{
			if (btnSearch != null) {
				btnSearch.Dispose ();
				btnSearch = null;
			}
			if (btnVoid != null) {
				btnVoid.Dispose ();
				btnVoid = null;
			}
			if (cbxBonus != null) {
				cbxBonus.Dispose ();
				cbxBonus = null;
			}
			if (gridPlayer != null) {
				gridPlayer.Dispose ();
				gridPlayer = null;
			}
			if (lblBonus != null) {
				lblBonus.Dispose ();
				lblBonus = null;
			}
			if (lblBonusEntry != null) {
				lblBonusEntry.Dispose ();
				lblBonusEntry = null;
			}
			if (lblMachine != null) {
				lblMachine.Dispose ();
				lblMachine = null;
			}
			if (lblPlayer != null) {
				lblPlayer.Dispose ();
				lblPlayer = null;
			}
			if (lblPlayerBonus != null) {
				lblPlayerBonus.Dispose ();
				lblPlayerBonus = null;
			}
			if (lblPlayerPlayed != null) {
				lblPlayerPlayed.Dispose ();
				lblPlayerPlayed = null;
			}
			if (lblScore != null) {
				lblScore.Dispose ();
				lblScore = null;
			}
			if (pickMachine != null) {
				pickMachine.Dispose ();
				pickMachine = null;
			}
			if (ScoreEntryNavigation != null) {
				ScoreEntryNavigation.Dispose ();
				ScoreEntryNavigation = null;
			}
			if (txtPlayer != null) {
				txtPlayer.Dispose ();
				txtPlayer = null;
			}
			if (txtPlayerName != null) {
				txtPlayerName.Dispose ();
				txtPlayerName = null;
			}
			if (txtScore != null) {
				txtScore.Dispose ();
				txtScore = null;
			}
			if (viewPlayer != null) {
				viewPlayer.Dispose ();
				viewPlayer = null;
			}
		}
	}
}
