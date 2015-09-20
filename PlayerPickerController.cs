using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace PinballScoreEntry.iOS
{
	partial class PlayerPickerController : UIViewController
	{
		int RoundId;
		bool Distinct;
		List<Player> Players;
		ScoreEntryController ParentController;

		public PlayerPickerController (IntPtr handle) : base (handle)
		{
		}

		public void SetParent(ScoreEntryController parent)
		{
			ParentController = parent;
		}

		public override void ViewDidAppear (bool animated)
		{
			if (ParentController.InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || ParentController.InterfaceOrientation == UIInterfaceOrientation.LandscapeRight) {
				playerPickerView.Frame = new RectangleF (playerPickerView.Frame.Location, new SizeF (400f, 310f));
				//playerPickerView.ContentSize = new SizeF (400f, 310f);
			} else {
				playerPickerView.Frame = new RectangleF (playerPickerView.Frame.Location, new SizeF (400f, 645f));
				//playerPickerView.ContentSize = new SizeF (400f, 645f);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			playerSearchBar.TextChanged += Search;
		}

		void Search(object sender, UISearchBarTextChangedEventArgs e)
		{
			IEnumerable<Player> searchResults = new List<Player>();
			int nbr;
			if (int.TryParse (e.SearchText, out nbr))
				searchResults = Players.Where (p => p.PlayerNumber.Equals(nbr));
			else
				searchResults = Players.Where (p => string.Format ("{0} {1}", p.Name, p.FirstNames).ToLower().Contains (e.SearchText.ToLower()));
			var source = new PlayerPickerView (searchResults.ToList ());
			source.ItemSelected += HandleItemSelected;
			playerPickerView.Source = source;
			playerPickerView.ReloadData ();
		}

		public void SetRoundId(int id, bool distinct = true)
		{
			RoundId = id;
			Distinct = distinct;
		}

		void HandleItemSelected (object sender, EventArgs e)
		{
			playerSearchBar.ResignFirstResponder ();
			var source = playerPickerView.Source as PlayerPickerView;
			ParentController.PlayerPicked (source.SelectedItem);
		}

		public void ShowView ()
		{
			if (Players == null) {
				Players = AppDelegate.Self.ApiClient.Get (new GetPlayersInRound { Id = RoundId, Distinct = Distinct });
				// Show list alphabetically
				Players = Players.OrderBy (p => p.Name).ThenBy (p => p.FirstNames).ToList ();
			}
			var source = new PlayerPickerView (Players);
			source.ItemSelected += HandleItemSelected;
			playerPickerView.Source = source;
			playerPickerView.ReloadData ();
			playerSearchBar.Text = "";
			playerSearchBar.BecomeFirstResponder ();
		}
	}
}
