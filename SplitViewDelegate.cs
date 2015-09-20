using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace PinballScoreEntry.iOS
{
	public class SplitViewDelegate : UISplitViewControllerDelegate {

		bool hiddenMaster = false;

		public SplitViewDelegate() : base ()
		{
		}

		public void HideMaster(UISplitViewController ctrl, bool? hide = null)
		{
			ctrl.Delegate = this;
			if (hide.HasValue) {
				this.hiddenMaster = (bool)hide;
				ctrl.WillRotate (ctrl.InterfaceOrientation, 0);
				ctrl.View.SetNeedsLayout ();
				// Ugly code
				UINavigationController nav = ctrl.ViewControllers [1] as UINavigationController;
				DetailViewController detail = nav.ViewControllers [0] as DetailViewController;
				if (detail.tableView != null) {
					RectangleF rect;
					if (detail.InterfaceOrientation == UIInterfaceOrientation.LandscapeLeft || detail.InterfaceOrientation == UIInterfaceOrientation.LandscapeRight)
						rect = detail.RectLandscape;
					else
						rect = detail.RectPortrait;
					detail.tableView.Bounds = rect;
					detail.tableView.ScrollRectToVisible (new RectangleF (0, 0, 1, 1), true);
				}
			}
		}

		public override bool ShouldHideViewController (UISplitViewController svc, UIViewController viewController, UIInterfaceOrientation inOrientation)
		{
			if (inOrientation == UIInterfaceOrientation.Portrait || inOrientation == UIInterfaceOrientation.PortraitUpsideDown)
				return true;
			return this.hiddenMaster;
		}

	}
}
