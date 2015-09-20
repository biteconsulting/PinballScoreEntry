using MRoundedButton;
using BigTed;
using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.ObjCRuntime;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

using CGRect = global::System.Drawing.RectangleF;
using CGSize = global::System.Drawing.SizeF;
using CGPoint = global::System.Drawing.PointF;
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;
using System.Globalization;
using System.Text.RegularExpressions;
using ServiceStack;
using System.Threading.Tasks;

namespace PinballScoreEntry.iOS
{
	partial class QuickEntryController : UIViewController
	{
		protected Round Round { get; set; }
		protected List<Ranking> Games { get; set; }
		protected List<Machine> Machines { get; set; }
		protected Player Player { get; set; }

		enum ActiveField {
			Player,
			Game,
			Score
		};

		ActiveField activeField;
		nfloat _backgroundViewHeight;
		nfloat _backgroundViewWidth;
		nfloat labelh;

		HollowBackgroundView backgroundViewToken;
		UILabel tokenLabel;

		HollowBackgroundView backgroundViewConfirm;
		UILabel confirmLabel;

		HollowBackgroundView backgroundViewKeyPad;

		UILabel playerLabel;
		UILabel playerNumberLabel;
		UILabel playerNameLabel;
		UILabel gameLabel;
		UILabel gameNumberLabel;
		UILabel gameNameLabel;
		UILabel scoreLabel;
		UILabel scoreNumberLabel;
		UILabel savedLabel;
		UITextField inputField;
		UIInterfaceOrientation _fromRotation;
		float? _heightFix;

		int? PlayerNumber;
		int? GameNumber;
		long? ScoreValue;
		int? EntryNumber;

		public QuickEntryController (IntPtr handle) : base (handle)
		{
		}
		public QuickEntryController () : base ()
		{
		}

		public override void ViewWillDisappear (bool animated)
		{
			new SplitViewDelegate().HideMaster(this.SplitViewController, false);
			base.ViewWillDisappear (animated);
		}

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);

			if (fromInterfaceOrientation == _fromRotation)
				return;
			_fromRotation = fromInterfaceOrientation;

			int multipl = -1;
			int val = 100;
			if (fromInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || fromInterfaceOrientation == UIInterfaceOrientation.LandscapeRight) {
				val = 100;
				multipl = 1;
			}
			_backgroundViewWidth = View.Bounds.Width;
			_backgroundViewHeight = _heightFix.HasValue ? _heightFix.Value : View.Bounds.Height;
			_heightFix = null;

			playerLabel.Frame = new CGRect (playerLabel.Frame.X, playerLabel.Frame.Y, playerLabel.Frame.Width, playerLabel.Frame.Height);
			playerNumberLabel.Frame = new CGRect ((_backgroundViewWidth / 2) - 130, playerNumberLabel.Frame.Y, playerNumberLabel.Frame.Width, playerNumberLabel.Frame.Height);
			playerNameLabel.Frame = new CGRect ((_backgroundViewWidth / 2) - 20, playerNameLabel.Frame.Y, playerNameLabel.Frame.Width, playerNameLabel.Frame.Height);
			gameLabel.Frame = new CGRect (gameLabel.Frame.X, gameLabel.Frame.Y + (50 * multipl), gameLabel.Frame.Width, gameLabel.Frame.Height);
			gameNumberLabel.Frame = new CGRect ((_backgroundViewWidth / 2) - 130, gameNumberLabel.Frame.Y + (50 * multipl), gameNumberLabel.Frame.Width, gameNumberLabel.Frame.Height);
			gameNameLabel.Frame = new CGRect ((_backgroundViewWidth / 2) - 20, gameNameLabel.Frame.Y + (50 * multipl), gameNameLabel.Frame.Width, gameNameLabel.Frame.Height);
			scoreLabel.Frame = new CGRect (scoreLabel.Frame.X, scoreLabel.Frame.Y + (100 * multipl), scoreLabel.Frame.Width, scoreLabel.Frame.Height);
			scoreNumberLabel.Frame = new CGRect ((_backgroundViewWidth / 2) - 130, scoreNumberLabel.Frame.Y + (100 * multipl), scoreNumberLabel.Frame.Width, scoreNumberLabel.Frame.Height);
			savedLabel.Frame = new CGRect ((_backgroundViewWidth / 2) - 130, savedLabel.Frame.Y + (150 * multipl), savedLabel.Frame.Width, savedLabel.Frame.Height);
			backgroundViewConfirm.Frame = new CGRect ((_backgroundViewWidth / 2) - 130, (_backgroundViewHeight / 2) + (val * multipl), backgroundViewConfirm.Frame.Width, backgroundViewConfirm.Frame.Height);
			backgroundViewToken.Frame = new CGRect ((_backgroundViewWidth / 2) - 130, (_backgroundViewHeight / 2) + (val * multipl), backgroundViewToken.Frame.Width, backgroundViewToken.Frame.Height);
			backgroundViewKeyPad.Frame = new CGRect ((_backgroundViewWidth / 2) - 130, _backgroundViewHeight - 300 - val, backgroundViewKeyPad.Frame.Width, backgroundViewKeyPad.Frame.Height);
			switch (activeField) {
				case ActiveField.Player:
					inputField.Frame = new CGRect ((_backgroundViewWidth / 2) - 130, inputField.Frame.Y, inputField.Frame.Width, inputField.Frame.Height);
					break;
					case ActiveField.Game:
				inputField.Frame = new CGRect ((_backgroundViewWidth / 2) - 130, inputField.Frame.Y + (50 * multipl), inputField.Frame.Width, inputField.Frame.Height);
					break;
				case ActiveField.Score:
				inputField.Frame = new CGRect ((_backgroundViewWidth / 2) - 130, inputField.Frame.Y + (100 * multipl), inputField.Frame.Width, inputField.Frame.Height);
					break;
				default:
					break;
			}
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad ();

			BTProgressHUD.Dismiss ();

			activeField = ActiveField.Player;
			Round = AppDelegate.Self.ApiClient.Get (new GetRound { Id = AppDelegate.Self.CurrentRoundId });
			Machines = AppDelegate.Self.ApiClient.Get (new GetMachinesInRound { Id = Round.Id });
			if (Machines.Count == 0) 
				AppDelegate.Self.ShowModalAlertViewAsync ("No games found.");

			_backgroundViewHeight = UIScreen.MainScreen.Bounds.Height;
			_backgroundViewWidth = UIScreen.MainScreen.Bounds.Width;

			labelh = 50.0f;

			nfloat labelx = 70;
			nfloat labely = 120;

			playerLabel = new UILabel {
				Text = "Player",
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.White,
				Font = UIFont.FromName ("Arial", 16F),
				Frame = new CGRect(labelx, labely, _backgroundViewWidth - labelx, labelh),
				UserInteractionEnabled = true
			};
			playerLabel.AddGestureRecognizer (
				new UITapGestureRecognizer (() => {
					setFocus(playerLabel);
				}));

			playerNumberLabel = new UILabel {
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.FromRGB(84, 164, 224),
				Font = UIFont.FromName ("Arial-BoldMT", 35F),
				Frame = new CGRect((_backgroundViewWidth / 2) - 130, labely, 100, labelh),
				Hidden = true
			};
			playerNameLabel = new UILabel {
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.FromRGB(84, 164, 224),
				Font = UIFont.FromName ("Arial", 16F),
				Frame = new CGRect((_backgroundViewWidth / 2) - 20, labely, 400, labelh),
				Hidden = true
			};

			inputField = new UITextField
			{
				Placeholder = "|",
				BackgroundColor = UIColor.FromRGB(41, 57, 69),
				TextColor = UIColor.White,
				Font = UIFont.FromName ("Arial", 30F),
				BorderStyle = UITextBorderStyle.RoundedRect,
				Frame = new CGRect((_backgroundViewWidth / 2) - 130, labely, 100, labelh),
				UserInteractionEnabled = false,
				TextAlignment = UITextAlignment.Center
			};

			labely += 140;
			gameLabel = new UILabel {
				Text = "Game",
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.FromRGB(84, 164, 224),
				Font = UIFont.FromName ("Arial", 16f),
				Frame = new CGRect(labelx, labely, _backgroundViewWidth - labelx, labelh),
				UserInteractionEnabled = true
			};
			gameLabel.AddGestureRecognizer (
				new UITapGestureRecognizer (() => {
					setFocus(gameLabel);
				}));

			gameNumberLabel = new UILabel {
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.FromRGB(84, 164, 224),
				Font = UIFont.FromName ("Arial-BoldMT", 35F),
				Frame = new CGRect((_backgroundViewWidth / 2) - 130, labely, 100, labelh),
				Hidden = false
			};
			gameNameLabel = new UILabel {
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.FromRGB(84, 164, 224),
				Font = UIFont.FromName ("Arial", 16F),
				Frame = new CGRect((_backgroundViewWidth / 2) - 20, labely, 400, labelh),
				Hidden = false
			};

			labely += 140;
			scoreLabel = new UILabel {
				Text = "Score",
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.FromRGB(84, 164, 224),
				Font = UIFont.FromName ("Arial", 16f),
				Frame = new CGRect(labelx, labely, _backgroundViewWidth - labelx, labelh),
				UserInteractionEnabled = true
			};
			scoreLabel.AddGestureRecognizer (
				new UITapGestureRecognizer (() => {
					setFocus(scoreLabel);
				}));

			scoreNumberLabel = new UILabel {
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.FromRGB(84, 164, 224),
				Font = UIFont.FromName ("Arial-BoldMT", 35F),
				Frame = new CGRect((_backgroundViewWidth / 2) - 130, labely, 300, labelh),
				Hidden = true
			};

			labely += 100;
			savedLabel = new UILabel {
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.FromRGB(48, 202, 102),
				Text = "Score submitted successfully",
				Font = UIFont.FromName ("Arial", 16F),
				Frame = new CGRect((_backgroundViewWidth / 2) - 130, labely, 300, labelh),
				Hidden = true
			};

			View.AddSubview(playerLabel);
			View.AddSubview(playerNumberLabel);
			View.AddSubview(playerNameLabel);
			View.AddSubview(gameLabel);
			View.AddSubview(gameNumberLabel);
			View.AddSubview(gameNameLabel);
			View.AddSubview(scoreLabel);
			View.AddSubview(scoreNumberLabel);
			View.AddSubview(savedLabel);
			View.AddSubview(inputField);

			drawButtons ();

			if (InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
			{
				_heightFix = View.Bounds.Width;
				DidRotate (UIInterfaceOrientation.Portrait);
			}
		}

		private void drawButtons()
		{
			UIColor foregroundColorArray = UIColor.FromRGB(30, 30, 30);
			RoundedButtonStyle buttonStyleArray = RoundedButtonStyle.Default;

			CGRect backgroundRect = new CGRect (
				(_backgroundViewWidth / 2) - 130,
				_backgroundViewHeight - 400, //(_backgroundViewHeight / 2) + 100,
				400,
				400
			);

			backgroundViewConfirm = new HollowBackgroundView (backgroundRect);
			backgroundViewConfirm.ForegroundColor = foregroundColorArray;
			backgroundViewConfirm.Hidden = true;
			View.AddSubview (backgroundViewConfirm);

			CGRect tokenRect = new CGRect (
				0,
				0,
				100,
				100
			);

			RoundedButton confirmButton = new RoundedButton (
				tokenRect,
				RoundedButtonStyle.Default,
				"3"
			);
			confirmButton.BackgroundColor = UIColor.FromRGB(84, 164, 224);
			confirmButton.TextLabel.Text = "OK";
			confirmButton.TextLabel.Font = UIFont.FromName ("Arial", 35F);
			confirmButton.TouchUpInside += confirmClicked;

			confirmLabel = new UILabel {
				Text = "Confirm score",
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.White,
				Font = UIFont.FromName ("Arial", 30f),
				Frame = new CGRect(120, 25, 300, labelh),
			};

			backgroundViewConfirm.AddSubview (confirmButton);
			backgroundViewConfirm.AddSubview (confirmLabel);

			backgroundViewToken = new HollowBackgroundView (backgroundRect);
			backgroundViewToken.ForegroundColor = foregroundColorArray;
			backgroundViewToken.Hidden = true;
			View.AddSubview (backgroundViewToken);

			tokenRect = new CGRect (
				0,
				0,
				100,
				100
			);

			RoundedButton chipButton = new RoundedButton (
				tokenRect,
				RoundedButtonStyle.CentralImage,
				"4"
			);
			chipButton.BackgroundColor = UIColor.Clear;
			chipButton.ImageView.Image = UIImage.FromBundle ("chip.png");
			chipButton.TouchUpInside += chipClicked;

			tokenLabel = new UILabel {
				Text = "Token required!",
				BackgroundColor = UIColor.Clear,
				TextColor = UIColor.FromRGB(204, 0, 102),
				Font = UIFont.FromName ("Arial", 30f),
				Frame = new CGRect(120, 25, 300, labelh),
			};

			backgroundViewToken.AddSubview (chipButton);
			backgroundViewToken.AddSubview (tokenLabel);

			backgroundViewKeyPad = new HollowBackgroundView (backgroundRect);
			backgroundViewKeyPad.ForegroundColor = foregroundColorArray;
			View.AddSubview (backgroundViewKeyPad);

			nfloat buttonSize = 70F;
			CGRect buttonRect = new CGRect (
				0,
				0,
				buttonSize,
				buttonSize
			);

			for (var i = 0; i < 12; i++) 
			{
				var style = (i == 9 ? "2" : (i == 11 ? "3" : "1"));
				var color = (i == 11 ? UIColor.FromRGB(30, 30, 30) : UIColor.FromRGB(84, 164, 224));
				RoundedButton button = new RoundedButton (
					buttonRect,
					buttonStyleArray,
					style
				);
				button.TouchUpInside += buttonClicked;
				button.BackgroundColor = color;
				var label = string.Empty;
				switch (i) 
				{
				case 9:
					label = "X";
					break;
				case 10:
					label = "0";
					break;
				case 11:
					label = "↵";
					break;
				default:
					label = (i + 1).ToString ();
					break;
				}
				button.TextLabel.Text = label;
				button.TextLabel.Font = UIFont.FromName ("Arial", 35F);

				backgroundViewKeyPad.AddSubview (button);

				if (i == 2 || i == 5 || i == 8) {
					buttonRect.Y += 80;
					buttonRect.X = 0;
				} else {
					buttonRect.X += buttonSize + 25;
				}
			}
		}

		void setFocus(UILabel label)
		{
			if (label == playerLabel) 
			{
				SelectPlayer ();
			}
			if (label == gameLabel) 
			{
				if (!PlayerNumber.HasValue)
					return;
				if (Round.UseEntries && !EntryNumber.HasValue)
					SelectToken ();
				else
					SelectGame ();
			}
			if (label == scoreLabel) {
				if (!PlayerNumber.HasValue)
					return;
				if (!GameNumber.HasValue)
					return;
				if (Round.UseEntries && !EntryNumber.HasValue)
					SelectToken ();
				else
					SelectScore ();
			}
		}

		void buttonClicked (object sender, EventArgs e)
		{
			var button = sender as RoundedButton;
			var text = button.TextLabel.Text;
			var input = inputField.Text.Replace (".", "");
			long inputNumeric = 0;

			switch (text) 
			{
			case "↵":
					if (inputField.Text == string.Empty)
						return;
					switch (activeField) {
						case ActiveField.Player:
							ProcessPlayer ();
							break;
						case ActiveField.Game:
							ProcessGame ();
							break;
						case ActiveField.Score:
							ProcessScore ();
							break;
						}
					break;
				case "X":
					var len = input.Length;
					input = (len > 0 ? input.Substring (0, input.Length - 1) : string.Empty);
					if (input.Equals (string.Empty)) {
						inputField.Text = input;
					} else {
						inputNumeric = Convert.ToInt64 (input);
						inputField.Text = inputNumeric.ToString("N0", CultureInfo.GetCultureInfo("nl-BE"));
					}
					break;
				default:
					input += text;
					inputNumeric = Convert.ToInt64 (input);
					inputField.Text = inputNumeric.ToString("N0", CultureInfo.GetCultureInfo("nl-BE"));
					break;
			}
		}

		void SelectPlayer()
		{
			activeField = ActiveField.Player;
			backgroundViewToken.Hidden = true;
			backgroundViewConfirm.Hidden = true;
			backgroundViewKeyPad.Hidden = false;
			savedLabel.Hidden = true;

			playerLabel.TextColor = UIColor.White;
			gameLabel.TextColor = UIColor.FromRGB(84, 164, 224);
			scoreLabel.TextColor = UIColor.FromRGB(84, 164, 224);
			playerNumberLabel.Hidden = true;
			playerNameLabel.Hidden = true;

			if (GameNumber.HasValue) {
				gameNumberLabel.Hidden = false;
				gameNameLabel.Hidden = false;
			}
			if (ScoreValue.HasValue) {
				scoreNumberLabel.Hidden = false;
			}
						
			inputField.Frame = new CGRect (inputField.Frame.X, playerLabel.Frame.Y, 100, inputField.Frame.Height);
			inputField.Text = string.Empty;
			inputField.Hidden = false;
		}

		void SelectToken()
		{
			// confirm new entry
			if (PlayerNumber.HasValue) {
				playerNumberLabel.Hidden = false;
				playerNameLabel.Hidden = false;
			}
			backgroundViewKeyPad.Hidden = true;
			backgroundViewConfirm.Hidden = true;
			backgroundViewToken.Hidden = false;
			inputField.Text = string.Empty;
			inputField.Hidden = true;
		}

		void SelectGame()
		{
			activeField = ActiveField.Game;
			backgroundViewConfirm.Hidden = true;
			backgroundViewKeyPad.Hidden = false;

			playerLabel.TextColor = UIColor.FromRGB(84, 164, 224);
			gameLabel.TextColor = UIColor.White;
			scoreLabel.TextColor = UIColor.FromRGB(84, 164, 224);
			gameNumberLabel.Hidden = true;
			gameNameLabel.Hidden = true;

			if (PlayerNumber.HasValue) {
				playerNumberLabel.Hidden = false;
				playerNameLabel.Hidden = false;
			}
			if (ScoreValue.HasValue) {
				scoreNumberLabel.Hidden = false;
			}

			inputField.Frame = new CGRect (inputField.Frame.X, gameLabel.Frame.Y, 100, inputField.Frame.Height);
			inputField.Text = string.Empty;
			inputField.Hidden = false;
		}

		void SelectScore()
		{
			activeField = ActiveField.Score;
			backgroundViewConfirm.Hidden = true;
			backgroundViewKeyPad.Hidden = false;

			playerLabel.TextColor = UIColor.FromRGB(84, 164, 224);
			gameLabel.TextColor = UIColor.FromRGB(84, 164, 224);
			scoreLabel.TextColor = UIColor.White;
			scoreNumberLabel.Hidden = true;

			if (PlayerNumber.HasValue) {
				playerNumberLabel.Hidden = false;
				playerNameLabel.Hidden = false;
			}
			if (GameNumber.HasValue) {
				gameNumberLabel.Hidden = false;
				gameNameLabel.Hidden = false;
			}

			inputField.Frame = new CGRect (inputField.Frame.X, scoreLabel.Frame.Y, 300, inputField.Frame.Height);
			if (ScoreValue.HasValue)
				inputField.Text = ScoreValue.Value.ToString("N0", CultureInfo.GetCultureInfo("nl-BE"));
			else
				inputField.Text = string.Empty;
			inputField.Hidden = false;
		}

		void SelectConfirm()
		{
			inputField.Text = string.Empty;
			inputField.Hidden = true;

			backgroundViewToken.Hidden = true;
			backgroundViewKeyPad.Hidden = true;
			backgroundViewConfirm.Hidden = false;
		}

		void EntryDone()
		{
			PlayerNumber = null;
			playerNumberLabel.Text = string.Empty;
			playerNameLabel.Text = string.Empty;
			GameNumber = null;
			gameNumberLabel.Text = string.Empty;
			gameNameLabel.Text = string.Empty;
			ScoreValue = null;
			scoreNumberLabel.Text = string.Empty;

			backgroundViewToken.Hidden = true;
			backgroundViewConfirm.Hidden = true;

			SelectPlayer ();
			backgroundViewKeyPad.Hidden = true;
			savedLabel.Hidden = false;
			TaskScheduler uiContext = TaskScheduler.FromCurrentSynchronizationContext();
			Task.Delay(3000).ContinueWith((task) =>
				{
					//Do UI stuff
					savedLabel.Hidden = true;
					backgroundViewKeyPad.Hidden = false;
				}, uiContext);

		}

		void ProcessPlayer()
		{
			try
			{
				BTProgressHUD.Show("Please wait ...", -1, ProgressHUD.MaskType.Gradient);
				var _playerNumber = Convert.ToInt32 (inputField.Text.Replace(".", "").Replace(",",""));
				if(Round.UseEntries)
					Player = AppDelegate.Self.ApiClient.Get(new GetPlayer { PlayerNumber = _playerNumber });
				else
					Player = AppDelegate.Self.ApiClient.Get(new GetPlayerInRound { RoundId = Round.Id, PlayerNumber = _playerNumber });
				if(Player == null)
				{
					BTProgressHUD.Dismiss ();
					AppDelegate.Self.ShowModalAlertViewAsync (string.Format("Player number {0} not found.", _playerNumber));
					return;
				}

				if(Round.UseEntries)
				{
					EntryNumber = AppDelegate.Self.ApiClient.Get (new GetPlayerCurrentEntry {
						PlayerId = Player.Id,
						RoundId = Round.Id
					});
					if (EntryNumber.HasValue) {
						Games = AppDelegate.Self.ApiClient.Get (new GetPlayerRanking {
							RoundId = Round.Id,
							PlayerId = Player.Id,
							EntryNumber = EntryNumber
						});
						//btnVoid.Hidden = (Games.Count == 0 || Games.Count == Round.NbrOfGames);
					} else {
						Games = new List<Ranking> ();
						if(Round.NbrOfGames == 1) {
							// Make entry
							var tp = AppDelegate.Self.ApiClient.Post(new RegisterPlayer { RoundId = Round.Id, PlayerId = _playerNumber });
							if (tp == null) {
								AppDelegate.Self.ShowModalAlertViewAsync ("Unable to create new entry.", "Error");
								return;
							}
							EntryNumber = tp.EntryNumber;
						}
					}
				}
				else
				{
					Games = AppDelegate.Self.ApiClient.Get(new GetPlayerRanking { RoundId = Round.Id, PlayerId = Player.Id });
					if(Games.Count == Round.NbrOfGames)
					{
						var count = Games.Where(x => (bool)x.IsBonusGame).Count();
						if (count >= Round.BonusGames) {
							BTProgressHUD.Dismiss ();
							AppDelegate.Self.ShowModalAlertViewAsync (string.Format("{0} {1} has played all games.", Player.Name, Player.FirstNames), "Warning");
							return;
						}
					}
				}

				if (GameNumber.HasValue && Games.Count > 0) {
					var game = Games.Where (x => x.MachineNumber.Equals (GameNumber.Value)).FirstOrDefault ();
					if (game != null) {
						GameNumber = null;
						gameNumberLabel.Text = string.Empty;
						gameNameLabel.Text = string.Empty;
						inputField.Text = string.Empty;
						ScoreValue = null;
						scoreNumberLabel.Text = string.Empty;
						BTProgressHUD.Dismiss ();
						AppDelegate.Self.ShowModalAlertViewAsync (string.Format("{0} {1} already played on {2}.", 
							Player.Name, Player.FirstNames, Machines.Where(x => x.MachineNumber == game.MachineNumber).Select(x => x.Name).FirstOrDefault()), "Warning");
						return;
					}
				}

				PlayerNumber = _playerNumber;
				playerNumberLabel.Text = inputField.Text;
				playerNumberLabel.Hidden = false;
				playerNameLabel.Text = string.Format("{0} {1}", Player.Name, Player.FirstNames);
				playerNameLabel.Hidden = false;
				gameLabel.Text = string.Format ("Game {0}/{1}", Math.Min(Games.Count + 1, Round.NbrOfGames), Round.NbrOfGames);
				if (Round.UseEntries && Round.NbrOfGames > 1 && !EntryNumber.HasValue) {
					SelectToken();
				} else {
					if(GameNumber.HasValue)
						SelectScore ();
					else
						SelectGame ();
				}
			}
			catch (Exception ex) {
				AppDelegate.Self.ShowModalAlertViewAsync (string.Format("An error occured. Please try again.{0}{1}", 
					Environment.NewLine, ex.Message), "Error");
				return;
			}
			finally {
				BTProgressHUD.Dismiss ();
			}
		}

		void chipClicked (object sender, EventArgs e)
		{
			try 
			{
				// Make entry
				var tp = AppDelegate.Self.ApiClient.Post(new RegisterPlayer { RoundId = Round.Id, PlayerId = PlayerNumber.Value });
				if (tp == null) {
					AppDelegate.Self.ShowModalAlertViewAsync ("Unable to create new entry.", "Error");
					return;
				}

				EntryNumber = tp.EntryNumber;
				backgroundViewToken.Hidden = true;
				backgroundViewKeyPad.Hidden = false;
				SelectGame ();
			}
			catch (Exception ex) {
				AppDelegate.Self.ShowModalAlertViewAsync (string.Format("An error occured. Please try again.{0}{1}", 
					Environment.NewLine, ex.Message), "Error");
				return;
			}
			finally {
				BTProgressHUD.Dismiss ();
			}
		}

		void ProcessGame()
		{
			try 
			{
				BTProgressHUD.Show("Please wait ...", -1, ProgressHUD.MaskType.Gradient);
				var _gameNumber = Convert.ToInt32 (inputField.Text.Replace(".", "").Replace(",",""));
				var machine = Machines.Where(x => x.MachineNumber.Equals(_gameNumber)).FirstOrDefault();
				if(machine == null)
				{
					BTProgressHUD.Dismiss ();
					AppDelegate.Self.ShowModalAlertViewAsync (string.Format("Game number {0} not found.", _gameNumber));
					return;
				}
				if (Games.Count > 0) {
					var game = Games.Where (x => x.MachineNumber.Equals (_gameNumber)).FirstOrDefault ();
					if (game != null) {
						GameNumber = null;
						gameNumberLabel.Text = string.Empty;
						gameNameLabel.Text = string.Empty;
						inputField.Text = string.Empty;
						ScoreValue = null;
						scoreNumberLabel.Text = string.Empty;
						BTProgressHUD.Dismiss ();
						AppDelegate.Self.ShowModalAlertViewAsync (string.Format("{0} {1} already played on {2}.", 
							Player.Name, Player.FirstNames, Machines.Where(x => x.MachineNumber == game.MachineNumber).Select(x => x.Name).FirstOrDefault()), "Warning");
						return;
					}
				}

				GameNumber = _gameNumber;
				gameNumberLabel.Text = inputField.Text;
				gameNumberLabel.Hidden = false;
				gameNameLabel.Text = string.Format("{0}", machine.Name);
				gameNameLabel.Hidden = false;

				SelectScore ();
			}
			catch (Exception ex) {
				AppDelegate.Self.ShowModalAlertViewAsync (string.Format("An error occured. Please try again.{0}{1}", 
					Environment.NewLine, ex.Message), "Error");
				return;
			}
			finally {
				BTProgressHUD.Dismiss ();
			}
		}

		void ProcessScore()
		{
			var _scoreValue = Convert.ToInt64 (inputField.Text.Replace(".", "").Replace(",",""));

			ScoreValue = _scoreValue;
			scoreNumberLabel.Text = inputField.Text;
			scoreNumberLabel.Hidden = false;

			SelectConfirm ();
		}

		void confirmClicked (object sender, EventArgs e)
		{
			try
			{
				var newGame = new CreateGame { RoundId = Round.Id, MachineNumber = GameNumber.Value, PlayerId = Player.Id, Score = ScoreValue.Value, IsBonusGame = false };
				if(Round.UseEntries)
					newGame.EntryNumber = EntryNumber;
				var game = AppDelegate.Self.ApiClient.Post(newGame);
			}
			catch (WebServiceException ex) 
			{
				AppDelegate.Self.ShowModalAlertViewAsync (string.Format("An error occured. Please try again.{0}Error code: {1} - {2}", 
					Environment.NewLine, ex.StatusCode, ex.ErrorMessage), "Error");
				return;
			}			
			catch (Exception ex)
			{
				AppDelegate.Self.ShowModalAlertViewAsync (string.Format("An error occured. Please try again.{0}{1}", 
					Environment.NewLine, ex.Message), "Error");
				return;
			}

			EntryDone();
		}
	}
}
