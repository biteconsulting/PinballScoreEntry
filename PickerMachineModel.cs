using System;
using MonoTouch.UIKit;
using System.Collections.Generic;

namespace PinballScoreEntry.iOS
{
	public class PickerMachineModel : UIPickerViewModel 
	{

		public event EventHandler<EventArgs> ValueChanged;

		/// <summary>
		/// The items to show up in the picker
		/// </summary>
		public List<Machine> Items { get; set; }

		/// <summary>
		/// The current selected item
		/// </summary>
		public Machine SelectedItem
		{
			get { return Items[selectedIndex]; }
		}
		protected int selectedIndex = 0;

		/// <summary>
		/// default constructor
		/// </summary>
		public PickerMachineModel (List<Machine> items)
		{
			Items = items;
		}

		/// <summary>
		/// Called by the picker to determine how many rows are in a given spinner item
		/// </summary>
		public override int GetRowsInComponent (UIPickerView picker, int component)
		{
			return Items.Count;
		}

		/// <summary>
		/// called by the picker to get the text for a particular row in a particular 
		/// spinner item
		/// </summary>
		public override string GetTitle (UIPickerView picker, int row, int component)
		{
			return string.Format ("{0}. {1}", Items [row].MachineNumber, Items [row].Name);
		}

		/// <summary>
		/// called by the picker to get the number of spinner items
		/// </summary>
		public override int GetComponentCount (UIPickerView picker)
		{
			return 1;
		}

		/// <summary>
		/// called when a row is selected in the spinner
		/// </summary>
		public override void Selected (UIPickerView picker, int row, int component)
		{
			selectedIndex = row;
			if (this.ValueChanged != null)
			{
				this.ValueChanged (this, new EventArgs ());
			}	
		}
	}		
}

