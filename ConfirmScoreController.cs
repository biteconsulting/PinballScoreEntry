using System;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.CodeDom.Compiler;

namespace PinballScoreEntry.iOS
{
	partial class ConfirmScoreController : UIViewController
	{
		public event EventHandler AcceptClicked;

		public ConfirmScoreController (IntPtr handle) : base (handle)
		{
		}

		public void SetInfo(string round, string player, string machine, long score, bool isBonus)
		{
			txtRound.Text = round;
			txtPlayer.Text = player;
			txtMachine.Text = machine;
			txtScore.Text = string.Format ("{0:n0}", score);
			cbxBonus.On = isBonus;
		}

		partial void OnAccept (UIButton sender)
		{
			if (this.AcceptClicked != null)
				this.AcceptClicked (this, new EventArgs ());
		}
	}
}
