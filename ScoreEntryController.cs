using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Drawing;
using System.Linq;
using BigTed;
using ServiceStack;

namespace PinballScoreEntry.iOS
{
	partial class ScoreEntryController : UIViewController
	{
		protected Round Round { get; set; }
		protected List<Machine> Machines { get; set; }
		protected List<Ranking> Games { get; set; }
		protected Player Player { get; set; }
		protected int OpenEntry { get; set; }
		protected int PlayerNumberValue { get; set; }
		protected long ScoreValue { get; set; }
		protected int MachineNumber { get; set; }

		protected UIPopoverController uipocConfirm;
		protected ConfirmScoreController controllerConfirm;
		protected UIPopoverController uipocPlayerPicker;
		protected PlayerPickerController controllerPicker;
		protected UIBarButtonItem btnSave;

		protected bool BackPressed { get; set; }

		public ScoreEntryController (IntPtr handle) : base (handle)
		{
		}
		public ScoreEntryController () : base ()
		{
		}

		public override void ViewWillDisappear (bool animated)
		{
			new SplitViewDelegate().HideMaster(this.SplitViewController, false);
			base.ViewWillDisappear (animated);
		}

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			if (fromInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || fromInterfaceOrientation == UIInterfaceOrientation.LandscapeRight) {
				viewPlayer.Frame = new RectangleF(lblScore.Frame.X, lblScore.Frame.Y + 50, viewPlayer.Frame.Width, viewPlayer.Frame.Height);
			}
		}

		protected async Task ShowMessage(string message, string title = "", Boolean closeView = false) {
			var result = await AppDelegate.Self.ShowModalAlertViewAsync (message, title);
			if(closeView)
				NavigationController.PopViewControllerAnimated(true);
		}
			
		public override void WillMoveToParentViewController (UIViewController parent)
		{
			if (parent == null) {
				BackPressed = true;
			}
			base.WillMoveToParentViewController (parent);
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			// Perform any additional setup after loading the view, typically from a nib.
			try
			{
				btnSave = new UIBarButtonItem(UIBarButtonSystemItem.Save, SaveClicked);
				btnSave.Enabled = false;
				NavigationItem.RightBarButtonItem = btnSave;

				Round = AppDelegate.Self.ApiClient.Get (new GetRound { Id = AppDelegate.Self.CurrentRoundId });
				ScoreEntryNavigation.Title = string.Format("{0} ({1})", "Score Entry", Round.Name);
				if(Round.UseEntries)
				{
					lblPlayer.Text = "Entry:";
					txtPlayer.Placeholder = "Entry Number";
					btnSearch.SetTitle("New Entry", UIControlState.Normal);
					lblBonusEntry.Hidden = true;
					lblPlayerBonus.Hidden = true;
				}

				Machines = AppDelegate.Self.ApiClient.Get (new GetMachinesInRound { Id = Round.Id });
				if (Machines.Count == 0) 
					throw new Exception("No machines were found.");
				var model = new PickerMachineModel(Machines);
				pickMachine.Model = model;
				pickMachine.Select ((Machines.Count / 2) - 1, 0, false);
				model.Selected (pickMachine, (Machines.Count / 2) - 1, 0);
				model.ValueChanged += MachineChanged;

				controllerConfirm = Storyboard.InstantiateViewController("ConfirmScoreController") as ConfirmScoreController;
				uipocConfirm = new UIPopoverController(controllerConfirm);
				uipocConfirm.PopoverContentSize = new SizeF(520f, 385f);
				controllerConfirm.AcceptClicked += ScoreConfirmed;

				controllerPicker = Storyboard.InstantiateViewController("PlayerPickerController") as PlayerPickerController;
				controllerPicker.SetParent(this);
				controllerPicker.SetRoundId(Round.Id);
				uipocPlayerPicker = new UIPopoverController(controllerPicker);

				this.txtPlayer.ShouldReturn += (textField) => { 
					textField.ResignFirstResponder(); 
					return true;
				};
				this.txtScore.ShouldReturn += (textField) => { 
					textField.ResignFirstResponder(); 
					return true;
				};

				txtPlayer.BecomeFirstResponder();
				BTProgressHUD.Dismiss();
			}
			catch (Exception ex) 
			{
				BTProgressHUD.Dismiss();
				ShowMessage(ex.Message, "Error", true);
			}
		}

		protected void HideAll(bool value)
		{
			HidePlayer (value);
			HideMachine (value);
		}
		protected void HidePlayer(bool value)
		{
			viewPlayer.Hidden = value;
			if(value)
				btnVoid.Hidden = value;
			btnSave.Enabled = false;
		}
		protected void HideMachine(bool value)
		{
			if (value) {
				txtScore.Text = "";
				ScoreValue = 0;
				cbxBonus.On = false;
			}
			lblMachine.Hidden = value;
			pickMachine.Hidden = value;
			lblScore.Hidden = value;
			txtScore.Hidden = value;
			if (!Round.UseEntries) {
				lblBonus.Hidden = value;
				cbxBonus.Hidden = value;
			}
		}

		protected void CheckInputField()
		{
			btnSave.Enabled = false;
			var bonusPlayed = Games.Count (x => x.IsBonusGame.HasValue ? (bool)x.IsBonusGame : false);
			if (Games.Count == 0) {
				txtScore.Enabled = true;
				cbxBonus.On = false;
			} else if (Games.Count < Round.NbrOfGames) {
				var model = pickMachine.Model as PickerMachineModel;
				var machine = model.SelectedItem;
				var exists = Games.Exists (x => x.MachineNumber == machine.MachineNumber);
				cbxBonus.On = (exists && bonusPlayed < Round.BonusGames);
				txtScore.Enabled = (bonusPlayed < Round.BonusGames || !exists);
			} else {
				cbxBonus.On = true;
				txtScore.Enabled = (bonusPlayed < Round.BonusGames);
			}
		}

		partial void PlayerEditChanged (UITextField sender)
		{
			int value;
			if(int.TryParse(sender.Text, out value))
				PlayerNumberValue = value;
			else if(sender.Text.Trim().Equals(string.Empty))
			{
				HideAll(true);
				PlayerNumberValue = 0;
			}
			else
				sender.Text = PlayerNumberValue.ToString();
		}
			
		partial void PlayerNumberChanged (UITextField sender)
		{
			if(BackPressed)
				return;
			if(PlayerNumberValue == 0)
			{
				HideAll(true);
				return;
			}

			if(Round.UseEntries)
				Player = AppDelegate.Self.ApiClient.Get(new GetPlayerInRound { RoundId = Round.Id, EntryNumber = PlayerNumberValue });
			else
				Player = AppDelegate.Self.ApiClient.Get(new GetPlayerInRound { RoundId = Round.Id, PlayerNumber = PlayerNumberValue });
			if(Player == null)
			{
				AppDelegate.Self.ShowModalAlertViewAsync (string.Format("{0} number {1} not found.", (Round.UseEntries ? " Entry" : "Player"), PlayerNumberValue));
				HideAll(true);
				return;
			}

			txtPlayerName.Text = string.Format("{0} {1}", Player.Name, Player.FirstNames);
			if(Round.UseEntries)
			{
				Games = AppDelegate.Self.ApiClient.Get(new GetPlayerRanking { RoundId = Round.Id, PlayerId = Player.Id, EntryNumber = PlayerNumberValue });
				btnVoid.Hidden = (Games.Count == 0 || Games.Count == Round.NbrOfGames);
//				var query = Games.GroupBy((item => item.EntryNumber),
//					(key, elements) => new { key = key, count = elements.Distinct().Count()});
			}
			else
			{
				Games = AppDelegate.Self.ApiClient.Get(new GetPlayerRanking { RoundId = Round.Id, PlayerId = Player.Id });
				lblBonusEntry.TextColor = lblPlayerBonus.TextColor;
				lblBonusEntry.Text = "joker";
				lblBonusEntry.TextColor = UIColor.FromRGB(0.4f, 1.0f, 0.4f);
				var count = Games.Where(x => (bool)x.IsBonusGame).Count();
				lblPlayerBonus.Text = string.Format("{0} / {1}", (Games.Count == 0 ? 0 : count), Round.BonusGames);
			}
			lblPlayerPlayed.Text = string.Format("{0} / {1}", Games.Count, Round.NbrOfGames);
			gridPlayer.Source = new RankingView (Games);
			gridPlayer.ReloadData();
			if(Games.Count == Round.NbrOfGames)
			{
				var count = Games.Where(x => (bool)x.IsBonusGame).Count();
				if(count >= Round.BonusGames)
				AppDelegate.Self.ShowModalAlertViewAsync (string.Format("This player has played all games{0}. Score entry is disabled.", Round.UseEntries ? " in this entry" : ""), "Warning");
			}
			txtScore.Text = "";
			ScoreValue = 0;
			CheckInputField();
			HidePlayer(false);
			HideMachine(false);
		}

		partial void ScoreEditChanged (UITextField sender)
		{
			long value;
			if(long.TryParse(sender.Text.Replace(",","").Replace(".",""), out value))
			{
				ScoreValue = value;
				sender.Text = string.Format("{0:n0}", ScoreValue);
			}
			else if(sender.Text.Trim().Equals(string.Empty))
			{
				ScoreValue = 0;
				sender.Text = "";
			}
			else
				sender.Text = string.Format("{0:n0}", ScoreValue);
			btnSave.Enabled = (ScoreValue > 0);
		}

		protected void MachineChanged(object sender, EventArgs e)
		{
			txtScore.Text = "";
			ScoreValue = 0;
			CheckInputField ();
		}

		partial void SearchClicked (UIButton sender)
		{
			try
			{
				if (uipocPlayerPicker.PopoverVisible)
					uipocPlayerPicker.Dismiss(true);
				else
				{
					if(txtPlayer.IsFirstResponder)
					{
						BackPressed = true;
						txtPlayer.ResignFirstResponder();
						BackPressed = false;
					}
					else if(txtScore.IsFirstResponder)
						txtScore.ResignFirstResponder();
					controllerPicker.ShowView();
					if(this.InterfaceOrientation ==  UIInterfaceOrientation.LandscapeLeft || this.InterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
						uipocPlayerPicker.PopoverContentSize = new SizeF(440f, 375f);
					else
						uipocPlayerPicker.PopoverContentSize = new SizeF(440f, 720f);
					uipocPlayerPicker.PresentFromRect(sender.Frame, this.View, UIPopoverArrowDirection.Left, true);
				}
			}
			catch (Exception ex)
			{
				AppDelegate.Self.ShowModalAlertViewAsync(ex.Message);
			}
		}

		partial void VoidClicked (UIButton sender)
		{
			OpenEntry = PlayerNumberValue;
			ProcessEntry(null, new UIButtonEventArgs(0));
		}
			
		public void PlayerPicked(Player player)
		{
			try
			{
				uipocPlayerPicker.Dismiss(true);
				if(player.Id > 0)
				{
					if(Round.UseEntries)
					{
						// Check if player has unfinished entries.
						int? entryId = AppDelegate.Self.ApiClient.Get(new GetPlayerCurrentEntry { RoundId = Round.Id, PlayerId = player.Id });
						if(entryId.HasValue) {
							OpenEntry = entryId.Value;
							ProcessEntry(null, new UIButtonEventArgs(1));
//							var games = AppDelegate.Self.ApiClient.Get(new FindGames { RoundId = Round.Id, EntryNumber = OpenEntry});
//							if(games.Count == 0)
//								// Entry has no games sofar, use it
//								ProcessEntry(null, new UIButtonEventArgs(1));
//							else
//							{
//								// Ask to void or use
//								UIActionSheet actionSheet = new UIActionSheet(
//									string.Format("An open entry was found ({0}).{1}Please pick an option:", entryId, Environment.NewLine),
//									null, "Cancel", "Void this entry", new string[] {"Use this entry", "Cancel"});
//								actionSheet.Clicked += ProcessEntry;
//								actionSheet.ShowInView(this.View);
//							}
							return;
						}
						else
						{
							var tp = AppDelegate.Self.ApiClient.Post(new RegisterPlayer { RoundId = Round.Id, PlayerId = player.Id });
							PlayerNumberValue = (int)tp.EntryNumber;
							txtPlayer.Text = PlayerNumberValue.ToString();
						}
					}
					else
					{
						PlayerNumberValue = player.PlayerNumber;
						txtPlayer.Text = PlayerNumberValue.ToString();
					}
					PlayerNumberChanged(txtPlayer);
				}
			}
			catch (Exception ex)
			{
				ShowMessage(string.Format("There was a problem registering the score. Please try again.{0}{1}", Environment.NewLine, ex.Message));
			}
		}

		void ProcessEntry(object sender, UIButtonEventArgs args)
		{
			try
			{
				switch (args.ButtonIndex) {
				case 0:
					// Delete
					var task = AppDelegate.Self.ShowModalAlertViewAsync("Are you sure you wish to void this entry ?", Round.Name, new[] { "Yes", "No" });
					task.ContinueWith((t) =>
						{
							if(t.Result == 0)
							{
								AppDelegate.Self.ApiClient.Post (new VoidEntry { RoundId = Round.Id, EntryNumber = OpenEntry });
								PlayerNumberValue = 0;
								txtPlayer.Text = string.Empty;
								PlayerNumberChanged(txtPlayer);
							}
							else
							{
								txtPlayer.BecomeFirstResponder();
							}
							OpenEntry = 0;
						}, TaskScheduler.FromCurrentSynchronizationContext());
					break;
				case 1:
					// Use
					PlayerNumberValue = OpenEntry;
					txtPlayer.Text = PlayerNumberValue.ToString();
					PlayerNumberChanged(txtPlayer);
					OpenEntry = 0;
					break;
				default:
					break;
				}
			}
			catch (Exception ex)
			{
				ShowMessage(string.Format("There was a problem creating a new entry. Please try again.{0}{1}", Environment.NewLine, ex.Message));
			}
		}

		void SaveClicked (object sender, EventArgs e)
		{
			try
			{
				if (uipocConfirm.PopoverVisible)
					uipocConfirm.Dismiss(true);
				else
				{
					if(txtPlayer.IsFirstResponder)
					{
						BackPressed = true;
						txtPlayer.ResignFirstResponder();
						BackPressed = false;
					}
					else if(txtScore.IsFirstResponder)
						txtScore.ResignFirstResponder();
					if(!btnSave.Enabled)
						return;
					var model = pickMachine.Model as PickerMachineModel;
					controllerConfirm.SetInfo(Round.Name, txtPlayerName.Text, model.SelectedItem.Name, ScoreValue, cbxBonus.On);
					uipocConfirm.PresentFromBarButtonItem (sender as UIBarButtonItem, UIPopoverArrowDirection.Up, true);
				}
			}
			catch (Exception ex)
			{
				AppDelegate.Self.ShowModalAlertViewAsync(ex.Message);
			}
		}

		protected void ScoreConfirmed (object sender, EventArgs e)
		{
			try
			{
				uipocConfirm.Dismiss(true); 
				var model = pickMachine.Model as PickerMachineModel;
				var newGame = new CreateGame { RoundId = Round.Id, MachineNumber = model.SelectedItem.MachineNumber, PlayerId = Player.Id, Score = ScoreValue, IsBonusGame = cbxBonus.On };
				if(Round.UseEntries)
					newGame.EntryNumber = PlayerNumberValue;
				var game = AppDelegate.Self.ApiClient.Post(newGame);
				ShowMessage("Score was submitted successfully.", Round.Name).ContinueWith((t) =>
					{
						PlayerNumberChanged (txtPlayer);
						//HideMachine(true);
					}, TaskScheduler.FromCurrentSynchronizationContext());
			}
			catch (WebServiceException ex) 
			{
				ShowMessage(string.Format("There was a problem registering the score. Please try again.{0}{1} {2}", 
					Environment.NewLine, ex.StatusCode, ex.ErrorCode));
			}			
			catch (Exception ex)
			{
				ShowMessage(string.Format("There was a problem registering the score. Please try again.{0}{1}", 
					Environment.NewLine, ex.Message));
			}
		}
	}
}
