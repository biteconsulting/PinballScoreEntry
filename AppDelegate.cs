using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Threading.Tasks;
using Refractored.Xam.Settings.Abstractions;
using Refractored.Xam.Settings;
using MRoundedButton;
using MonoTouch.ObjCRuntime;

using CGRect = global::System.Drawing.RectangleF;
using CGSize = global::System.Drawing.SizeF;
using CGPoint = global::System.Drawing.PointF;
using nfloat = global::System.Single;
using nint = global::System.Int32;
using nuint = global::System.UInt32;


namespace PinballScoreEntry.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the
	// User Interface of the application, as well as listening (and optionally responding) to
	// application events from iOS.
	[Register ("AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		public static AppDelegate Self { get; private set; }
		public override UIWindow Window { get; set; }

		public bool IsAuthenticated { get; private set; }
		public JsonServiceClient ApiClient { get; set; }
		public int CurrentTournamentId { get; set; }
		public int CurrentRoundId { get; set; }

		public override void FinishedLaunching (UIApplication application)
		{
			NSDictionary appearanceProxy1 = GetNSDictionary (new Dictionary<string, object> {
				{ RoundedButtonAppearanceKeys.CornerRadius , 60 },
				{ RoundedButtonAppearanceKeys.BorderWidth  , 3 },
				{ RoundedButtonAppearanceKeys.BorderColor  , UIColor.FromRGB(84, 164, 224) },
				{ RoundedButtonAppearanceKeys.ContentColor , UIColor.White },
				{ RoundedButtonAppearanceKeys.ContentAnimateToColor , UIColor.White },
				{ RoundedButtonAppearanceKeys.ForegroundColor , UIColor.FromRGB(30, 30, 30) },
				{ RoundedButtonAppearanceKeys.ForegroundAnimateToColor , UIColor.Clear }
			});

			NSDictionary appearanceProxy2 = GetNSDictionary (new Dictionary<string, object> { 
				{ RoundedButtonAppearanceKeys.CornerRadius , 60 },
				{ RoundedButtonAppearanceKeys.BorderWidth  , 3 },
				{ RoundedButtonAppearanceKeys.BorderColor  , UIColor.FromRGB(84, 164, 224) },
				{ RoundedButtonAppearanceKeys.ContentColor , UIColor.White },
				{ RoundedButtonAppearanceKeys.ContentAnimateToColor , UIColor.White },
				{ RoundedButtonAppearanceKeys.ForegroundColor , UIColor.FromRGB(41, 57, 69) },
				{ RoundedButtonAppearanceKeys.ForegroundAnimateToColor , UIColor.Clear }
			});

			NSDictionary appearanceProxy3 = GetNSDictionary (new Dictionary<string, object> { 
				{ RoundedButtonAppearanceKeys.CornerRadius , 60 },
				{ RoundedButtonAppearanceKeys.BorderWidth  , 0 },
				{ RoundedButtonAppearanceKeys.ContentColor , UIColor.White },
				{ RoundedButtonAppearanceKeys.ContentAnimateToColor , UIColor.FromRGB(84, 164, 224) },
				{ RoundedButtonAppearanceKeys.ForegroundColor , UIColor.FromRGB(84, 164, 224) },
				{ RoundedButtonAppearanceKeys.ForegroundAnimateToColor , UIColor.White }
			});

			NSDictionary appearanceProxy4 = GetNSDictionary (new Dictionary<string, object> { 
				{ RoundedButtonAppearanceKeys.CornerRadius , 60 },
				{ RoundedButtonAppearanceKeys.BorderWidth  ,2 },
				{ RoundedButtonAppearanceKeys.BorderColor  , UIColor.FromRGB(204, 0, 102) },
				//{ RoundedButtonAppearanceKeys.ContentColor , UIColor.White },
				//{ RoundedButtonAppearanceKeys.ContentAnimateToColor , UIColor.White },
				{ RoundedButtonAppearanceKeys.ForegroundColor , UIColor.Clear },
				{ RoundedButtonAppearanceKeys.ForegroundAnimateToColor , UIColor.FromRGB(204, 0, 102) }
			});

			RoundedButtonAppearanceManager.RegisterAppearanceProxy (appearanceProxy1, "1");
			RoundedButtonAppearanceManager.RegisterAppearanceProxy (appearanceProxy2, "2");
			RoundedButtonAppearanceManager.RegisterAppearanceProxy (appearanceProxy3, "3");
			RoundedButtonAppearanceManager.RegisterAppearanceProxy (appearanceProxy4, "4");

			// NOTE: Don't call the base implementation on a Model class
			// see http://docs.xamarin.com/guides/ios/application_fundamentals/delegates,_protocols,_and_events
			AppDelegate.Self = this;
			try
			{
				CrossSettings.Current.AddOrUpdateValue("DemoServer", "http://bop");
				CrossSettings.Current.Save();
				IosPclExportClient.Configure();
			}
			catch (Exception ex) 
			{
				ShowAlert(ex.Message);
			}
		}

		static NSDictionary GetNSDictionary (Dictionary<string,object> source)
		{
			return NSDictionary.FromObjectsAndKeys (
				source.Values.ToArray (), 
				source.Keys.ToArray ());
		}

		public Task<int> ShowModalAlertViewAsync (string message, string title = "Error", params string[] buttons)
		{
			if (buttons.Length == 0) {
				buttons = new string[] {"Ok"};
			}
			var alertView = new UIAlertView (title, message,  null, null, buttons);
			alertView.Show ();
			var tsc = new TaskCompletionSource<int> ();

			alertView.Clicked += (sender, buttonArgs) => {
				//Console.WriteLine ("User clicked on {0}", buttonArgs.ButtonIndex);		
				tsc.TrySetResult(buttonArgs.ButtonIndex);
			};    
			return tsc.Task;
		}

		public void ShowAlert(string message, string title = "Error", List<string> buttons = null )
		{
			if (buttons == null) {
				buttons = new List<string> ();
				buttons.Add ("Ok");
			}
			UIAlertView alert = new UIAlertView () { 
				Title = title, 
				Message = message
			};
			foreach(var button in buttons)
				alert.AddButton(button);
			alert.Show ();
		}

		// This method is invoked when the application is about to move from active to inactive state.
		// OpenGL applications should use this method to pause.
		public override void OnResignActivation (UIApplication application)
		{
		}
		
		// This method should be used to release shared resources and it should store the application state.
		// If your application supports background exection this method is called instead of WillTerminate
		// when the user quits.
		public override void DidEnterBackground (UIApplication application)
		{
			IsAuthenticated = false;
			ApiClient = null;
		}
		
		// This method is called as part of the transiton from background to active state.
		public override void WillEnterForeground (UIApplication application)
		{
			if (!AppDelegate.Self.IsAuthenticated) {
				var ctrl = Window.RootViewController.Storyboard.InstantiateViewController ("SplitViewController") as SplitViewController;
				Window.RootViewController = ctrl;
			}
		}
		
		// This method is called when the application is about to terminate. Save data, if needed.
		public override void WillTerminate (UIApplication application)
		{
		}
	}
}

