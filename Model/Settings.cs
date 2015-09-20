using ServiceStack;
using System;
using System.Text;

namespace PinballScoreEntry.iOS
{
    public class DropDatabase
    {
    }

    public class InitDatabase
    {
        public bool DropFirst { get; set; }
    }

    public class GetCurrentTournament : IReturn<int?>
    {
    }

    public class SetCurrentTournament
    {
        public int TournamentId { get; set; }
    }

	public class GetDefaultRound : IReturn<int?>
	{
		public int TournamentId { get; set; }
	}
}
