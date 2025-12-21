using SportsData.Shared;

namespace SportsData.Modules.Competitions.Domain
{
    public class Player : Entity
    {
        public string Name { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public string Nationality { get; private set; }
        public string Position { get; private set; }
        public int? Height { get; private set; } // in cm
        public int? Weight { get; private set; } // in kg

        private Player() { }

        public Player(string name, DateTime? dateOfBirth, string nationality, string position, int? height, int? weight)
        {
            Name = name;
            DateOfBirth = dateOfBirth;
            Nationality = nationality;
            Position = position;
            Height = height;
            Weight = weight;
        }
    }
}
