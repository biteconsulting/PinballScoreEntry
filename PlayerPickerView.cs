using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;
using System.Drawing;
using System.Linq;

namespace PinballScoreEntry.iOS
{
	public class PlayerPickerView : UITableViewSource {
		string cellIdentifier = "TableCell";
		Dictionary<string, List<Player>> indexedTableItems;
		string[] keys;
		public event EventHandler<EventArgs> ItemSelected;
		public Player SelectedItem { get; private set; }

		public PlayerPickerView (List<Player> list)
		{
			if (list.Count == 0)
				list.Add (new Player { Name = "No players found" });

			indexedTableItems = new Dictionary<string, List<Player>>();
			foreach (var t in list) {
				if (indexedTableItems.ContainsKey (t.Name[0].ToString ())) {
					indexedTableItems[t.Name[0].ToString ()].Add(t);
				} else {
					indexedTableItems.Add (t.Name[0].ToString (), new List<Player>() {t});
				}
			}
			keys = indexedTableItems.Keys.ToArray();
		}

		public override int NumberOfSections (UITableView tableView)
		{
			return keys.Length;
		}
		public override int RowsInSection (UITableView tableview, int section)
		{
			return indexedTableItems[keys[section]].Count;
		}
		public override string[] SectionIndexTitles (UITableView tableView)
		{
			return keys;
		}
		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			PlayerPickerCell cell = tableView.DequeueReusableCell (cellIdentifier) as PlayerPickerCell;
			if (cell == null)
				cell = new PlayerPickerCell ((NSString)NSObject.FromObject(cellIdentifier));

			var player = indexedTableItems [keys [indexPath.Section]].ElementAt (indexPath.Row);
			cell.UpdateCell (player);
			if (player.Id > 0)
				cell.Accessory = UITableViewCellAccessory.DisclosureIndicator;
			else
				cell.Accessory = UITableViewCellAccessory.None;
			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			SelectedItem = indexedTableItems [keys [indexPath.Section]].ElementAt (indexPath.Row);
			if (this.ItemSelected != null)
			{
				this.ItemSelected (this, new EventArgs ());
			}	
		}
	}

	public class PlayerPickerCell : UITableViewCell  {
		UILabel playerLabel;

		public PlayerPickerCell (NSString cellId) : base (UITableViewCellStyle.Default, cellId)
		{
			SelectionStyle = UITableViewCellSelectionStyle.Blue;
			playerLabel = new UILabel ();
			ContentView.Add (playerLabel);
		}

		public void UpdateCell (Player record)
		{
			playerLabel.Text = string.Format ("{0} {1} ({2})", record.Name, record.FirstNames, record.PlayerNumber);
		}

		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();
			playerLabel.Frame = new RectangleF(15, 4, ContentView.Bounds.Width, 44);
		}
	}
}

