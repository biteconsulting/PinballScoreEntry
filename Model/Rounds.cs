using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack;

namespace PinballScoreEntry.iOS
{
    public class CreateRound : IReturn<Round>
    {
        public int TournamentId { get; set; }
        public string Name { get; set; }
        public int NbrOfGames { get; set; }
        public int? NbrOfMachines { get; set; }
		public int? UseEntries { get; set; }
        public bool AutomaticCalculation { get; set; }
        public int? MaxPoints { get; set; }
        public int? BonusGames { get; set; }
    }

    public class UpdateRound : IReturn<Round>
    {
        public int Id { get; set; }
        public int TournamentId { get; set; }
        public string Name { get; set; }
        public int? NbrOfGames { get; set; }
        public int? NbrOfMachines { get; set; }
		public int? UseEntries { get; set; }
        public bool? AutomaticCalculation { get; set; }
        public int? MaxPoints { get; set; }
        public int? BonusGames { get; set; }
    }

    public class DeleteRound
    {
        public int Id { get; set; }
    }

    public class FindRounds : IReturn<List<Round>>
    {
        public int? TournamentId { get; set; }
        public string Name { get; set; }
    }

    public class GetRound : IReturn<Round>
    {
        public int Id { get; set; }
    }

    public class RegisterMachine : IReturn<RoundMachine>
    {
        public int RoundId { get; set; }
        public int MachineId { get; set; }
        public int? MachineNumber { get; set; }
    }

    public class UnRegisterMachine
    {
        public int Id { get; set; }
        public int? RoundId { get; set; }
        public int? MachineId { get; set; }
        public int? MachineNumber { get; set; }
    }

    public class GetMachinesInRound : IReturn<List<Machine>>
    {
        public int Id { get; set; }
    }

    public class GetMachineInRound : IReturn<Machine>
    {
        public int Id { get; set; }
        public int? RoundId { get; set; }
        public int? MachineId { get; set; }
        public int? MachineNumber { get; set; }
    }

	public class RegisterPlayer : IReturn<RoundPlayer>
	{
		public int RoundId { get; set; }
		public int PlayerId { get; set; }
		public int? PlayerNumber { get; set; }
		public int? EntryNumber { get; set; }
	}

	public class UnRegisterPlayer
	{
		public int Id { get; set; }
		public int? RoundId { get; set; }
		public int? PlayerId { get; set; }
		public int? PlayerNumber { get; set; }
		public int? EntryNumber { get; set; }
	}

	public class VoidEntry
	{
		public int Id { get; set; }
		public int? RoundId { get; set; }
		public int? PlayerId { get; set; }
		public int? PlayerNumber { get; set; }
		public int? EntryNumber { get; set; }
	}

	public class GetPlayersInRound : IReturn<List<Player>>
	{
		public int Id { get; set; }
		public bool? Distinct { get; set; }
	}

	public class GetPlayerInRound : IReturn<Player>
	{
		public int Id { get; set; }
		public int? RoundId { get; set; }
		public int? PlayerId { get; set; }
		public int? PlayerNumber { get; set; }
		public int? EntryNumber { get; set; }
	}

	public class CountPlayersInRound : IReturn<long>
	{
		public int Id { get; set; }
	}

	public class GetPlayerCurrentEntry : IReturn<int?>
	{
		public int RoundId { get; set; }
		public int PlayerId { get; set; }
	}
}
