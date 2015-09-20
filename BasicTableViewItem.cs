using System;
using MonoTouch.UIKit;

namespace PinballScoreEntry.iOS
{
	public class BasicTableViewItem
	{
		public int? Id { get; set; }

		public string Name { get; set; }

		public string SubHeading { get; set; }

		public UITableViewCellAccessory Accessory { get; set; }

		public BasicTableViewItem ()
		{
		}
	}}

