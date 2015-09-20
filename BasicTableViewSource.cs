using System;
using MonoTouch.UIKit;
using System.Collections.Generic;
using MonoTouch.Foundation;

namespace PinballScoreEntry.iOS
{
	public class BasicTableViewSource : UITableViewSource
	{
		//---- declare vars
		protected List<BasicTableViewItemGroup> _tableItems;
		protected bool _useColumns;
		string _cellIdentifier = "BasicTableViewCell";
		public event EventHandler<EventArgs> ItemSelected;
		public BasicTableViewItem SelectedItem { get; private set; }

		public BasicTableViewSource (List<BasicTableViewItemGroup> items, bool useColumns = false)
		{
			this._tableItems = items;
			_useColumns = useColumns;
		}

		/// <summary>
		/// Called by the TableView to determine how many sections(groups) there are.
		/// </summary>
		public override int NumberOfSections (UITableView tableView)
		{
			return this._tableItems.Count;
		}

		/// <summary>
		/// Called by the TableView to determine how many cells to create for that particular section.
		/// </summary>
		public override int RowsInSection (UITableView tableview, int section)
		{
			return this._tableItems[section].Items.Count;
		}

		/// <summary>
		/// Called by the TableView to retrieve the header text for the particular section(group)
		/// </summary>
		public override string TitleForHeader (UITableView tableView, int section)
		{
			return this._tableItems[section].Name;
		}

		/// <summary>
		/// Called by the TableView to retrieve the footer text for the particular section(group)
		/// </summary>
		public override string TitleForFooter (UITableView tableView, int section)
		{
			return this._tableItems[section].Footer;
		}

		/// <summary>
		/// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
		/// </summary>
		public override UITableViewCell GetCell (UITableView tableView, NSIndexPath indexPath)
		{
			//---- declare vars
			UITableViewCell cell = tableView.DequeueReusableCell (this._cellIdentifier);
			if (cell == null)
				cell = new UITableViewCell (UITableViewCellStyle.Subtitle, this._cellIdentifier);

			//---- create a shortcut to our item
			BasicTableViewItem item = this._tableItems[indexPath.Section].Items[indexPath.Row];

			cell.TextLabel.Text = item.Name;
			cell.DetailTextLabel.Text = item.SubHeading;
			cell.Accessory = item.Accessory;

			return cell;
		}

		public override void RowSelected (UITableView tableView, NSIndexPath indexPath)
		{
			SelectedItem = this._tableItems[indexPath.Section].Items[indexPath.Row];
			if (this.ItemSelected != null)
				this.ItemSelected (this, new EventArgs ());
		}
		}
}

