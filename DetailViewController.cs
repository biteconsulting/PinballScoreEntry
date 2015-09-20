using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;
using System.Threading.Tasks;
using BigTed;
using System.Collections.Generic;
using System.Drawing;

namespace PinballScoreEntry.iOS
{
	partial class DetailViewController : UIViewController
	{
		public bool IShide { get; set; }
		public UITableView tableView { get; set; }
		public RectangleF RectLandscape = new RectangleF(0, 75, 683, 673);
		public RectangleF RectPortrait = new RectangleF (0, 0, 723, 1024);

		protected LoginController controllerLogin;

		public DetailViewController (IntPtr handle) : base (handle)
		{
		}

		public override void WillRotate (UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillRotate (toInterfaceOrientation, duration);

			if (tableView != null) {
				RectangleF rect;
				if (toInterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || toInterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
					rect = RectLandscape;
				else
					rect = RectPortrait;
				tableView.Bounds = rect;
				tableView.ScrollRectToVisible (new RectangleF (0, 0, 1, 1), true);
			}
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();

			// Perform any additional setup after loading the view, typically from a nib.
			var refreshButton = new UIBarButtonItem(UIBarButtonSystemItem.Refresh, RefreshView);
			NavigationItem.LeftBarButtonItems = new UIBarButtonItem[] { refreshButton };

			var addButton = new UIBarButtonItem(UIBarButtonSystemItem.Add, AddScore);
			NavigationItem.RightBarButtonItem = addButton;

			if (!AppDelegate.Self.IsAuthenticated) {
				Login ();
			}
		}

		public void Login()
		{
			controllerLogin = Storyboard.InstantiateViewController("LoginController") as LoginController;
			controllerLogin.UserLoggedIn += RefreshView;
			controllerLogin.ModalPresentationStyle = UIModalPresentationStyle.FullScreen;
			this.NavigationController.PresentViewController(controllerLogin, false, null);
		}

		public void RefreshView (object sender, EventArgs args)
		{
			UINavigationController ctrl = this.SplitViewController.ViewControllers [0] as UINavigationController;
			MasterViewController master = ctrl.ViewControllers [0] as MasterViewController;

			BTProgressHUD.Show("Loading data ...", -1, ProgressHUD.MaskType.Black);
			master.RefreshData ();
			if (tableView != null) {
				tableView.RemoveFromSuperview ();
				tableView.Dispose ();
				tableView = null;
			}
			BTProgressHUD.Dismiss();
		}

		public void LoadTable(List<Ranking> list)
		{
			if (tableView != null) {
				tableView.RemoveFromSuperview ();
				tableView.Dispose ();
				tableView = null;
			}

			RectangleF rect;
			if (InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || InterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
				rect = RectLandscape;
			else
				rect = RectPortrait;
			tableView = new UITableView (rect, UITableViewStyle.Plain);
			//tableView.AutoresizingMask = UIViewAutoresizing.All;
			tableView.Source = new RankingView (list, 14f);
			tableView.RowHeight = 30f;
			this.View.AddSubview (tableView);
		}
			
		void AddScore (object sender, EventArgs args)
		{
			BTProgressHUD.Show("Please wait ...", -1, ProgressHUD.MaskType.Gradient);
			//ScoreEntryController scoreEntry = this.Storyboard.InstantiateViewController ("ScoreEntryController") as ScoreEntryController;
			QuickEntryController scoreEntry = this.Storyboard.InstantiateViewController ("QuickEntryController") as QuickEntryController;
			if (scoreEntry != null) {
				new SplitViewDelegate ().HideMaster (this.SplitViewController, true);
				this.NavigationController.PushViewController (scoreEntry, true);
			} else
				BTProgressHUD.Dismiss ();
		}
	}
}
