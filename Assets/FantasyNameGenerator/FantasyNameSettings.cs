using RPGKit.FantasyNameGenerator.Generators;

namespace RPGKit.FantasyNameGenerator
{
    public class FantasyNameSettings
    {
        public Classes ChosenClass { get; set; }
        public Race ChosenRace { get; set; }
        public bool IncludePostfix { get; set; }
        public Gender Gender { get; set; }

        public FantasyNameSettings(Classes chosenclass = Classes.Warrior,
         Race race = Race.None, bool includePostfix = false, Gender gender = Gender.Male)
        {
            ChosenClass = chosenclass;
            ChosenRace = race;
            IncludePostfix = includePostfix;
            Gender = gender;
        }

    }
}