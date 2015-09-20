using System;
using System.Collections.Generic;
using System.Text;
using ServiceStack;

namespace PinballScoreEntry.iOS
{
    public class CreatePlayer
    {
        public string Name { get; set; }
        public string FirstNames { get; set; }
        public string Tag { get; set; }
        public int? IfpaId { get; set; }
        public PlayerType PlayerType { get; set; }
    }

    public class UpdatePlayer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstNames { get; set; }
        public string Tag { get; set; }
        public int? IfpaId { get; set; }
        public PlayerType PlayerType { get; set; }
    }

    public class DeletePlayer
    {
        public int Id { get; set; }
    }

	public class FindPlayers : IReturn<List<Player>>
    {
        public string Name { get; set; }
        public string FirstNames { get; set; }
        public PlayerType PlayerType { get; set; }
    }
    
	public class GetPlayer : IReturn<Player>
    {
        public int? Id { get; set; }
        public string Tag { get; set; } //Alternative way to fetch a player
        public int? IfpaId { get; set; }
        public int? PlayerNumber { get; set; }
    }
}
