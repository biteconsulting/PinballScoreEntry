using System;
using System.Text;

namespace PinballScoreEntry.iOS
{
	public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstNames { get; set; }
        public string Tag { get; set; }
        public int? IfpaId { get; set; }
        public int? PlayerTypeId { get; set; }
		public int PlayerNumber { get; set; }
		public int EntryNumber { get; set; }
    }
}