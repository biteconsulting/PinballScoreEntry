using System;
using System.Text;

namespace PinballScoreEntry.iOS
{
    public class RoundPlayer
    {
        public int Id { get; set; }
        public int RoundId { get; set; }
        public int PlayerId { get; set; }
        public int PlayerNumber { get; set; }
		public int? EntryNumber { get; set; }
    }
}
