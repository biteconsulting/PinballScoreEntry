using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Drawing;
using Refractored.Xam.Settings;
using ServiceStack;
using BigTed;
using System.Threading.Tasks;

namespace PinballScoreEntry.iOS
{
	partial class LoginController : UIViewController
	{
		public event EventHandler<EventArgs> UserLoggedIn;

		public LoginController (IntPtr handle) : base (handle)
		{
		}

		public override void DidRotate (UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate (fromInterfaceOrientation);

			if(fromInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || fromInterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
				View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("Default-Portrait.png"));
			else
				View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("Default-Landscape.png"));

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			if(InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
				View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("Default-Landscape.png"));
			else
				View.BackgroundColor = UIColor.FromPatternImage(UIImage.FromBundle("Default-Portrait.png"));

			lblVersion.Text = string.Format("Version {0} Build {1}", 
				NSBundle.MainBundle.InfoDictionary ["CFBundleShortVersionString"], 
				NSBundle.MainBundle.InfoDictionary ["CFBundleVersion"]);
				
			this.txtServer.ShouldReturn += (textField) => { 
				txtUser.BecomeFirstResponder(); 
				return true;
			};
			this.txtUser.ShouldReturn += (textField) => { 
				txtPassword.BecomeFirstResponder(); 
				return true;
			};
			this.txtPassword.ShouldReturn += (textField) => { 
				txtPassword.ResignFirstResponder();
				LoginClicked(btnLogin);
				return true;
			};

			var demo = CrossSettings.Current.GetValueOrDefault<string>("DemoServer");
			var server = CrossSettings.Current.GetValueOrDefault<string>("Server");
			if (server == null)
				server = demo;

			txtServer.Text = server;
			txtUser.Enabled = (server != demo);
			txtPassword.Enabled = (server != demo);
			if (server == demo) {
				txtUser.Text = "ref";
				txtPassword.Text = "ref";
			} else {
				txtUser.Text = CrossSettings.Current.GetValueOrDefault<string>("User");
				txtPassword.Text = "";
				if (string.IsNullOrEmpty (txtUser.Text))
					txtUser.BecomeFirstResponder ();
				else
					txtPassword.BecomeFirstResponder ();
			}
		}
			
		partial void LoginClicked (UIButton sender)
		{
			try
			{
				if(string.IsNullOrEmpty(txtServer.Text))
					throw new Exception("Please enter server");
				if(string.IsNullOrEmpty(txtUser.Text))
					throw new Exception("Please enter user");
				if(string.IsNullOrEmpty(txtPassword.Text))
					throw new Exception("Please enter password");

				var server = txtServer.Text.ToLower();
				if(!server.StartsWith("http://"))
					server = "http://" + server;

				Uri test = new Uri(server);
				if(!Reachability.IsHostReachable(test.Host)) {
					throw new Exception("Unreachable host.");
				}

				AppDelegate.Self.ApiClient = new JsonServiceClient { Timeout = new TimeSpan(0, 0, 15) };
				AppDelegate.Self.ApiClient.AlwaysSendBasicAuthHeader = true;
				AppDelegate.Self.ApiClient.SetBaseUri (server);
				var authResponse = AppDelegate.Self.ApiClient.Post<AuthenticateResponse>
					(string.Format("auth/credentials?username={0}&password={1}", txtUser.Text, txtPassword.Text), null);
				AppDelegate.Self.ApiClient.SetCredentials (txtUser.Text, txtPassword.Text);

				CrossSettings.Current.AddOrUpdateValue("Server", txtServer.Text);
				CrossSettings.Current.AddOrUpdateValue("User", txtUser.Text);

				int? value = null;
				if(AppDelegate.Self.CurrentTournamentId > 0)
				{
					var tournament = AppDelegate.Self.ApiClient.Get (new GetTournament { Id = AppDelegate.Self.CurrentTournamentId });
					if(tournament != null)
						value = tournament.Id;
				}
				if(!value.HasValue)
					value = AppDelegate.Self.ApiClient.Get (new GetCurrentTournament { });
				if (value.HasValue)
					AppDelegate.Self.CurrentTournamentId = (int)value;
				else
					throw new Exception("The tournament ID has not been set.");

				if(AppDelegate.Self.CurrentRoundId > 0)
				{
					var round = AppDelegate.Self.ApiClient.Get (new GetRound { Id = AppDelegate.Self.CurrentRoundId });
					if(round != null)
						value = round.Id;
				}
				if(!value.HasValue)
					value = AppDelegate.Self.ApiClient.Get (new GetDefaultRound { TournamentId = AppDelegate.Self.CurrentTournamentId });
				if (value.HasValue)
					AppDelegate.Self.CurrentRoundId = (int)value;
				else
					throw new Exception("The round ID can not be determined.");

				DismissViewController(true, null);
				if (this.UserLoggedIn != null)
					this.UserLoggedIn (this, new EventArgs ());
			}
			catch (WebServiceException ex) 
			{
				AppDelegate.Self.ShowAlert(string.Format("{0} {1}", ex.StatusCode, ex.ErrorMessage));
			}			
			catch (Exception ex) 
			{
				AppDelegate.Self.ShowAlert(ex.Message);
			}
		}
	}
}
