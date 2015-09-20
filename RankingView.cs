using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Drawing;

namespace PinballScoreEntry.iOS
{
	public class RankingView : UITableViewSource {
		Ranking[] tableList;
		string cellIdentifier = "TableCell";
		public event EventHandler<EventArgs> ItemSelected;
		public Ranking SelectedItem { get; private set; }
		public float FontSize { get; private set; }

		public RankingView (List<Ranking> list, float fontSize = 12f)
		{
			var copy = new List<Ranking> (list);
			FontSize = fontSize;
			if (copy.Count == 0)
				copy.Add (new Ranking { Name = "No data found." });
			tableList = copy.ToArray ();
		}

		public override int RowsInSection (UITableView tableview, int section)
		{
			return tableList.Length;
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			RankingCell cell = tableView.DequeueReusableCell (cellIdentifier) as RankingCell;
			if (cell == null)
				cell = new RankingCell ((NSString)NSObject.FromObject(cellIdentifier), FontSize);

			cell.UpdateCell (tableList[indexPath.Row] );
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			SelectedItem = tableList[indexPath.Row];
			if (this.ItemSelected != null)
				this.ItemSelected (this, new EventArgs ());
		}
	}

	public class RankingCell : UITableViewCell  {
		UILabel nameLabel, positionLabel, scoreLabel, pointsLabel;

		public RankingCell (NSString cellId, float fontSize) : base (UITableViewCellStyle.Default, cellId)
		{
			SelectionStyle = UITableViewCellSelectionStyle.Default;
			var font = UIFont.SystemFontOfSize(fontSize);
			positionLabel = new UILabel () { Font = font };
			nameLabel = new UILabel () {	Font = font };
			scoreLabel = new UILabel () { Font = font, TextAlignment = UITextAlignment.Right };
			pointsLabel = new UILabel () { Font = font, TextAlignment = UITextAlignment.Right };
			ContentView.Add (nameLabel);
			ContentView.Add (positionLabel);
			ContentView.Add (scoreLabel);
			ContentView.Add (pointsLabel);
		}

		public void UpdateCell (Ranking record)
		{
			bool isBonus = false;
			if (record.IsBonusGame.HasValue)
				isBonus = (bool)record.IsBonusGame;

			long score;
			if (record.TournamentId > 0) {
				if (record.RankingType == RankingType.Global || record.RankingType == RankingType.Missing)
					score = (long)record.GamesPlayed;
				else
					score = (long)record.Score;
			}
			else
				score = 0;

			if (isBonus || (record.RankingType == RankingType.Global && record.BonusPlayed > 0)) {
				positionLabel.TextColor = UIColor.FromRGB(0.4f, 1.0f, 0.4f);
				nameLabel.TextColor = UIColor.FromRGB(0.4f, 1.0f, 0.4f);
				scoreLabel.TextColor = UIColor.FromRGB(0.4f, 1.0f, 0.4f);
				pointsLabel.TextColor = UIColor.FromRGB(0.4f, 1.0f, 0.4f);
			} else {
				positionLabel.TextColor = UIColor.Black;
				nameLabel.TextColor = UIColor.Black;
				scoreLabel.TextColor = UIColor.Black;
				pointsLabel.TextColor = UIColor.Black;
			}
			if (record.RankingType == RankingType.Missing) {
				positionLabel.Text = string.Format ("{0}.", record.Position);
				scoreLabel.Text = string.Format ("{0:n0}", record.GamesPlayed);
				pointsLabel.Text = string.Format ("{0:n0}", record.BonusPlayed);
			} else if (score > 0) {
				scoreLabel.Text = string.Format ("{0:n0}", score);
				if (record.Position > 0) {
					positionLabel.Text = string.Format ("{0}.", record.Position);
					pointsLabel.Text = record.Points.ToString ();
				} else {
					positionLabel.Text = "";
					pointsLabel.Text = "";
				}
			}
			else
			{
				scoreLabel.Text = "";
				positionLabel.Text = "";
				pointsLabel.Text = "";
			}
			if (record.EntryNumber.HasValue) {
				nameLabel.Text = string.Format("{0} ({1})", record.Name, record.EntryNumber);
			} else {
				nameLabel.Text = record.Name;
			}
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			pointsLabel.Frame = new RectangleF(ContentView.Bounds.Width - 30, 4, 25, 26);
			scoreLabel.Frame = new RectangleF(ContentView.Bounds.Width - 150, 4, 105, 26);
			positionLabel.Frame = new RectangleF(20, 4, 30, 26);
			nameLabel.Frame = new RectangleF(50, 4, ContentView.Bounds.Width - 180, 26);
		}
	}

}

