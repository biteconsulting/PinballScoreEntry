using System;
using System.Collections.Generic;
using System.Text;

namespace PinballScoreEntry.iOS
{
    public class Game
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int RoundId { get; set; }
        public int MachineNumber { get; set; }
		public int EntryNumber { get; set; }
        public long Score { get; set; }
        public int Points { get; set; }
        public int Position { get; set; }
        public bool IsBonusGame { get; set; }
		public bool IsEntryComplete { get; set; }
    }
}
