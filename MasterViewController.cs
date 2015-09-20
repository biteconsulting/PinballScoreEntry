using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Collections.Generic;

namespace PinballScoreEntry.iOS
{
	partial class MasterViewController : UITableViewController
	{
		List<Round> Rounds;
		UIActionSheet actionSheet;

		public MasterViewController (IntPtr handle) : base (handle)
		{
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
			var changeButton = new UIBarButtonItem(UIBarButtonSystemItem.Compose, SHowRounds);
			NavigationItem.RightBarButtonItem = changeButton;

			if(AppDelegate.Self.IsAuthenticated)
				RefreshData ();
		}

		public void RefreshData()
		{
			Rounds = AppDelegate.Self.ApiClient.Get (new FindRounds { TournamentId = AppDelegate.Self.CurrentTournamentId });

			//---- declare vars
			BasicTableViewItemGroup tGroup;
			List<BasicTableViewItemGroup> tableItems = new List<BasicTableViewItemGroup> ();

			var round = AppDelegate.Self.ApiClient.Get (new GetRound { Id = AppDelegate.Self.CurrentRoundId });
			NavigationItem.Title = round.Name;

			//---- Stats
			if (!round.UseEntries) {
				var gamecount = AppDelegate.Self.ApiClient.Get (new CountGames { Id = AppDelegate.Self.CurrentRoundId });
				tGroup = new BasicTableViewItemGroup() { Name = "Stats" };
				tGroup.Items.Add (new BasicTableViewItem() { Name = "Show missing scores", SubHeading = string.Format("Games played: {0} / {1} (Joker: {2})", gamecount.GamesPlayed, gamecount.TotalGames, gamecount.BonusPlayed), Accessory = UITableViewCellAccessory.DisclosureIndicator });
				tableItems.Add (tGroup);
			}

			//---- Global ranking
			if(round.CalculateTotals) {
				tGroup = new BasicTableViewItemGroup() { Name = "Global" };
				tGroup.Items.Add (new BasicTableViewItem() { Name = "Global Ranking", Accessory = UITableViewCellAccessory.DisclosureIndicator });
				tableItems.Add (tGroup);
			}

			//---- Machine rankings
			var machines = AppDelegate.Self.ApiClient.Get (new GetMachinesInRound { Id = AppDelegate.Self.CurrentRoundId });
			tGroup = new BasicTableViewItemGroup() { Name = "Machines", Footer = string.Format("Total machines: {0}", machines.Count) };
			foreach (var machine in machines) {
				var count = AppDelegate.Self.ApiClient.Get (new CountGamesOnMachine { RoundId = AppDelegate.Self.CurrentRoundId, MachineNumber = machine.MachineNumber });
				tGroup.Items.Add (new BasicTableViewItem() { Id = machine.Id, Name = machine.Name, SubHeading = string.Format("Games played: {0}", count), Accessory = UITableViewCellAccessory.DisclosureIndicator });
			}
			tableItems.Add (tGroup);

			var source = new BasicTableViewSource (tableItems);
			TableView.Source = source;
			source.ItemSelected += HandleItemSelected;
			TableView.ReloadData();
		}

		void HandleItemSelected (object sender, EventArgs e)
		{
			List<Ranking> list;
			var source = TableView.Source as BasicTableViewSource;
			if (source.SelectedItem.Name == "Show missing scores")
				list = AppDelegate.Self.ApiClient.Get (new GetMissingRanking { RoundId = AppDelegate.Self.CurrentRoundId });
			else if (source.SelectedItem.Name == "Global Ranking")
				list = AppDelegate.Self.ApiClient.Get (new GetGlobalRanking { RoundId = AppDelegate.Self.CurrentRoundId });
			else if (source.SelectedItem.Id.HasValue)
				list = AppDelegate.Self.ApiClient.Get (new GetMachineRanking {
					RoundId = AppDelegate.Self.CurrentRoundId,
					MachineId = (int)source.SelectedItem.Id
				});
			else
				list = null;
			if (list != null) {
				UINavigationController ctrl = this.SplitViewController.ViewControllers [1] as UINavigationController;
				DetailViewController detail = ctrl.ViewControllers [0] as DetailViewController;
				detail.LoadTable (list);
			}
		}

		void SHowRounds (object sender, EventArgs args)
		{
			if (actionSheet != null)
				if (actionSheet.Visible) {
					actionSheet.DismissWithClickedButtonIndex (0, true);
					return;
				}

			actionSheet = new UIActionSheet("Choose round:");
			int i = 0;
			string[] names = new string[Rounds.Count];
			foreach (var round in Rounds)
				names [i++] = round.Name;
			foreach (string name in names)
				actionSheet.AddButton (name);
			var idx = actionSheet.AddButton ("Cancel");
			actionSheet.DestructiveButtonIndex = idx;
			actionSheet.Clicked += ChangeRound;
			actionSheet.ShowFrom(NavigationItem.RightBarButtonItem, true);
		}

		void ChangeRound(object sender, UIButtonEventArgs args)
		{
			if (args.ButtonIndex >= 0 && args.ButtonIndex <= Rounds.Count - 1) {
				var round = Rounds [args.ButtonIndex];
				if (round.Id == AppDelegate.Self.CurrentRoundId)
					return;
				NavigationItem.Title = round.Name;
				AppDelegate.Self.CurrentRoundId = round.Id;
				UINavigationController ctrl = this.SplitViewController.ViewControllers [1] as UINavigationController;
				DetailViewController detail = ctrl.ViewControllers [0] as DetailViewController;
				detail.RefreshView(sender, null);
			}
		}
	}
}
