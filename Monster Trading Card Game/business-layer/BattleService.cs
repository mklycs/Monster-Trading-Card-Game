// enthält die Logik für den Kampf, die Kampfauswertung und Battle-Log

using System;

namespace mtcg{
    internal class BattleService{
        public BattleService() { }

        public int battle(User you, User opponent, Random random){
            CardService your_cardService = new CardService(you.deck, you.stack);
            CardService opps_cardService = new CardService(opponent.deck, opponent.stack);

            for(int round = 1; round < 101; round++){
                if(opponent.deck.Count < 1) return 1;
                else if(you.deck.Count < 1) return -1;

                Console.WriteLine($"Round: {round}");
                int opps_card = random.Next(opponent.deck.Count);
                int your_card = random.Next(you.deck.Count);
                Console.WriteLine($"{opponent.username}'s card: {opponent.deck[opps_card].name} ({opponent.deck[opps_card].damage}, \"{opponent.deck[opps_card].elementType}\")");
                Console.WriteLine($"Your card: {you.deck[your_card].name} ({you.deck[your_card].damage}, \"{you.deck[your_card].elementType}\")");

                int result = fight(you.deck[your_card], opponent.deck[opps_card]);
                if(result == 1){ // W
                    your_cardService.add(opponent.deck[opps_card]);
                    opps_cardService.remove(opponent.deck[opps_card]);
                }
                else if(result == -1){ // L
                    opps_cardService.add(you.deck[your_card]);
                    your_cardService.remove(you.deck[your_card]);
                }else Console.WriteLine("Draw.");

                displayDecks(you.deck, opponent.deck, opponent.username);
                Console.WriteLine();
            }

            your_cardService.emptyDeck();
            //your_cardService.initDeck(random);
            return 0;
        }

        public int fight(A_Card your_card, A_Card opps_card){
            int result = specialCases(your_card, opps_card);
            if(result == 1) return 1;
            else if(result == -1) return -1;

            float temp_yourdamage = your_card.damage;
            float temp_oppsdamage = opps_card.damage;
            if(your_card.cardType == 's' || opps_card.cardType == 's'){
                float effectiveness = compareElements(your_card.elementType, opps_card.elementType);
                if(effectiveness == 2) Console.WriteLine($"{your_card.elementType} is effective against {opps_card.elementType}, so damage is doubled.");
                else if(effectiveness == 1) Console.WriteLine($"{your_card.elementType} has no special effect against {opps_card.elementType}.");
                else if(effectiveness == 0.5F) Console.WriteLine($"{your_card.elementType} is not effective against {opps_card.elementType}, so damage is halved.");
                temp_yourdamage = your_card.damage * effectiveness;
                temp_oppsdamage = opps_card.damage * compareElements(opps_card.elementType, your_card.elementType);
            }

            if(temp_yourdamage > temp_oppsdamage){
                Console.WriteLine($"{your_card.name}'s damage is higher than {opps_card.name}. {opps_card.name} is added to your deck.");
                return 1;
            }else if(temp_yourdamage < temp_oppsdamage){
                Console.WriteLine($"{your_card.name}'s damage is lower than {opps_card.name}. You loose {your_card.name} from your deck.");
                return -1;
            }
            else return 0;
        }

        public float compareElements(string? your_elementType, string? opps_elementType){
            float effectiveness = 1;
            if(your_elementType == "Water" && opps_elementType == "Fire") effectiveness = 2;
            //else if(your_elementType == "Water" && opps_elementType == "Water") effectiveness = 1;
            else if(your_elementType == "Water" && opps_elementType == "Normal") effectiveness = 0.5F;

            else if(your_elementType == "Fire" && opps_elementType == "Normal") effectiveness = 2;
            else if(your_elementType == "Fire" && opps_elementType == "Water") effectiveness = 0.5F;

            else if(your_elementType == "Normal" && opps_elementType == "Water") effectiveness = 2;
            else if(your_elementType == "Normal" && opps_elementType == "Fire") effectiveness = 0.5F;

            return effectiveness;
        }

        public int specialCases(A_Card your_card, A_Card opps_card){
            int result = 0;
            // Dragon & FireElf
            if(your_card.name == "Dragon" && opps_card.name == "FireElf"){
                Console.WriteLine($"{opps_card.name} dodged {your_card.name}'s attack.");
                result = -1;
            }
            else if(your_card.name == "FireElf" && opps_card.name == "Dragon"){
                Console.WriteLine($"{your_card.name} dodged {opps_card.name}'s attack.");
                result = 1;
            }
            // Dragon & Goblin
            if(your_card.name == "Dragon" && opps_card.name == "Goblin"){
                Console.WriteLine($"{your_card.name} attacks the too afraid {opps_card.name}.");
                result = 1;
            }
            else if(your_card.name == "Goblin" && opps_card.name == "Dragon"){
                Console.WriteLine($"{your_card.name} is too afraid to attack {opps_card.name}.");
                result = -1;
            }
            // Knight & Water
            if(your_card.name == "Knight" && (opps_card.elementType == "Water" && opps_card.cardType == 's')){
                Console.WriteLine($"{your_card.name}'s armor is too heavy and is being drowned in {opps_card.elementType}.");
                result = -1;
            }else if((your_card.elementType == "Water" && your_card.cardType == 's') && opps_card.name == "Knight"){
                Console.WriteLine($"{opps_card.name}'s armor is too heavy and is being drowned in {your_card.elementType}.");
                result = 1;
            }
            // Wizard & Orc
            if (your_card.name == "Wizard" && opps_card.name == "Orc"){
                Console.WriteLine($"{your_card.name} controlled {opps_card.name} and {opps_card.name} is unable to damage {your_card.name}.");
                result = 1;
            }
            else if(your_card.name == "Orc" && opps_card.name == "Wizard"){
                Console.WriteLine($"{your_card.name} is controlled by {opps_card.name} and {your_card.name} is unable to damage {opps_card.name}.");
                result = -1;
            }
            // Kraken & Spells
            if(your_card.name == "Kraken" && opps_card.cardType == 's'){
                Console.WriteLine($"{your_card.name} is immune to spells.");
                result = 1;
            }else if(your_card.cardType == 's' && opps_card.name == "Kraken"){
                Console.WriteLine($"{your_card.name} is immune to spells.");
                result = -1;
            }

            if(result == -1) Console.WriteLine($"Your opponent gets your {your_card.name}.");
            else if(result == 1) Console.WriteLine($"Your opponents {opps_card.name} is being added to your deck.");

            return result;
        }

        public void displayDecks(List<A_Card> your_deck, List<A_Card> opps_deck, string? opps_username){
            Console.Write("Your deck: ");
            for(int i = 0; i < your_deck.Count; i++)
                Console.Write($"{your_deck[i].name} ");
            Console.WriteLine();

            Console.Write($"{opps_username} deck: ");
            for(int i = 0; i < opps_deck.Count; i++)
                Console.Write($"{opps_deck[i].name} ");
            Console.WriteLine();
        }
    }
}