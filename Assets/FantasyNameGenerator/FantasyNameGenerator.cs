using System;
using System.Collections.Generic;
using RPGKit.FantasyNameGenerator.Generators;
using System.Linq;

namespace RPGKit.FantasyNameGenerator
{
    public class FantasyNameGenerator
    {
        private readonly List<INameGenerator> _nameGenerators;

        public Gender Gender { get; set; }
        public INameGenerator PrefixGenerator { get; set; }
        public INameGenerator FirstNameGenerator { get; set; }
        public INameGenerator LastNameGenerator { get; set; }
        public INameGenerator PostfixNameGenerator { get; set; }
        public INameGenerator TownNameGenerator { get; set; }

        public FantasyName GetFantasyName()
        {
            FantasyName name = new FantasyName();

            name.Gender = Gender;

            if (PrefixGenerator != null)
                name.Prefix = PrefixGenerator.GetName();

            if (FirstNameGenerator != null)
                name.FirstName = FirstNameGenerator.GetName();

            if (LastNameGenerator != null)
                name.LastName = LastNameGenerator.GetName();

            if (PostfixNameGenerator != null)
                name.Postfix = PostfixNameGenerator.GetName();

            return name;
        }

        public FantasyNameGenerator()
        {
            _nameGenerators = new List<INameGenerator>();
        }

        public static FantasyNameGenerator GetCharacterNameGenerator(FantasyNameSettings fantasyNameSettings)
        {
            var fantasyNameGenerator = new FantasyNameGenerator();

            fantasyNameGenerator.Gender = fantasyNameSettings.Gender;

            if (fantasyNameSettings.ChosenClass != Classes.None)
            {
                INameGenerator maleNameGenerator = null;

                if (fantasyNameSettings.ChosenClass == Classes.Cleric)
                    maleNameGenerator = new MaleClericFirstNameGenerator();

                if (fantasyNameSettings.ChosenClass == Classes.Rogue)
                    maleNameGenerator = new MaleRogueFirstNameGenerator();

                if (fantasyNameSettings.ChosenClass == Classes.Warrior)
                    maleNameGenerator = new MaleWarriorFirstNameGenerator();

                if (fantasyNameSettings.ChosenClass == Classes.Wizard)
                    maleNameGenerator = new MaleWizardFirstNameGenerator();

                if (fantasyNameSettings.Gender == Gender.Male)
                {
                    fantasyNameGenerator.FirstNameGenerator = maleNameGenerator;
                }
                else
                {

                    fantasyNameGenerator.FirstNameGenerator = new FemaleWrapperNameGenerator(maleNameGenerator);
                }

                fantasyNameGenerator.LastNameGenerator = new LastNameGenerator();
            }
            else
            {
                fantasyNameGenerator.FirstNameGenerator = new RaceNameGenerator(fantasyNameSettings.ChosenRace);
                fantasyNameGenerator.LastNameGenerator = new RaceNameGenerator(fantasyNameSettings.ChosenRace);
            }

            if (fantasyNameSettings.IncludePostfix)
            {
                if (fantasyNameSettings.ChosenClass == Classes.Wizard)
                    fantasyNameGenerator.PostfixNameGenerator = new PostfixWizardGenerator();
                else if (fantasyNameSettings.ChosenRace != Race.None)
                    fantasyNameGenerator.PostfixNameGenerator = new VilePostfixGenerator();
                else
                    fantasyNameGenerator.PostfixNameGenerator = new PostfixGenerator();
            }

            return fantasyNameGenerator;
        }

        public static string GetTownName()
        {
            var name = new TownNameGenerator().GetName();
            return name.First().ToString().ToUpper() + name.Substring(1);
        }
    }
}
