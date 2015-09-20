using System;
using System.Collections.Generic;
using System.Text;

namespace PinballScoreEntry.iOS
{
    public class Round
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public string Name { get; set; }
        public int NbrOfGames { get; set; }
        public int NbrOfMachines { get; set; }
		public bool UseEntries { get; set; }
        public bool AutomaticCalculation { get; set; }
        public int? MaxPoints { get; set; }
        public int BonusGames { get; set; }
		public bool IsDefault { get; set; }
		public bool CalculateTotals { get; set; }
    }
}
