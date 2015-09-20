using ServiceStack;
using System;
using System.Collections.Generic;
using System.Text;

namespace PinballScoreEntry.iOS
{
    public class CreateTournament : IReturn<Tournament>
    {
        public string Name { get; set; }
    }

    public class UpdateTournament : IReturn<Tournament>
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class DeleteTournament
    {
        public int Id { get; set; }
    }

    public class FindTournaments : IReturn<List<Tournament>>
    {
        public string Name { get; set; }
    }

    public class GetTournament : IReturn<Tournament>
    {
        public int Id { get; set; }
    }
}
