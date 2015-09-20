using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace PinballScoreEntry.iOS
{
    public class GetGlobalRanking : IReturn<List<Ranking>>
    {
        public int RoundId { get; set; }
    }

    public class GetMachineRanking : IReturn<List<Ranking>>
    {
        public int RoundId { get; set; }
        public int? MachineId { get; set; }
        public int? MachineNumber { get; set; }
    }

    public class GetPlayerRanking : IReturn<List<Ranking>>
    {
        public int RoundId { get; set; }
        public int? PlayerId { get; set; }
        public int? PlayerNumber { get; set; }
		public int? EntryNumber { get; set; }
    }

	public class GetMissingRanking : IReturn<List<Ranking>>
	{
		public int RoundId { get; set; }
	}
}
