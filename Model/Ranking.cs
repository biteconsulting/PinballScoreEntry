using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PinballScoreEntry.iOS
{
    public enum RankingType { Global, Machine, Player, Missing}

    public class Ranking
    {
        public RankingType RankingType { get; set; }
        public int TournamentId { get; set; }
        public int RoundId { get; set; }
        public string RoundName { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public int PlayerNumber { get; set; }
        public int? MachineNumber { get; set; }
        public int? EntryNumber { get; set; }
        public int Position { get; set; }
        public int? GamesPlayed { get; set; }
        public int? BonusPlayed { get; set; }
        public long? Score { get; set; }
        public int Points { get; set; }
        public bool? IsBonusGame { get; set; }
    }
}
