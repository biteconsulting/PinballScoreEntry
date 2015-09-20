using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace PinballScoreEntry.iOS
{
    public class CreateGame : IReturn<Game>
    {
        public int PlayerId { get; set; }
        public int RoundId { get; set; }
        public int MachineNumber { get; set; }
		public int? EntryNumber { get; set; }
        public long Score { get; set; }
        public bool? IsBonusGame { get; set; }
    }

    public class UpdateGame : IReturn<Game>
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public int Position { get; set; }
		public bool? IsEntryComplete { get; set; }
    }

    public class DeleteGame
    {
        public int Id { get; set; }
    }

	public class DeleteEntry
	{
		public int RoundId { get; set; }
		public int EntryNumber { get; set; }
	}

	public class FindGames : IReturn<List<Game>>
    {
        public int? RoundId { get; set; }
        public int? PlayerId { get; set; }
        public int? MachineNumber { get; set; }
		public int? EntryNumber { get; set; }
		public bool? IsEntryComplete { get; set; }
    }

    public class GetGame : IReturn<Game>
    {
        public int Id { get; set; }
    }

	public class CountGames : IReturn<GameCount>
	{
		public int Id { get; set; }
	}

	public class CountGamesOnMachine : IReturn<long>
	{
		public int RoundId { get; set; }
		public int? MachineId { get; set; }
		public int? MachineNumber { get; set; }
	}
}
