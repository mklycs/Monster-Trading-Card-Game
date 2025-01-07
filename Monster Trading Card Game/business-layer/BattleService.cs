namespace mtcg{
    internal class BattleService{
        public BattleService() { }
        
        public int battle(User player1, User player2, Random random, ref string response, CardService cardService){
            for(int round = 1; round < 101; round++){
                if(player2.deck.Count < 1){
                    return 1;
                }else if(player1.deck.Count < 1) 
                    return -1;

                response += $"Round: {round}\n";
                int player2_card = random.Next(player2.deck.Count);
                int player1_card = random.Next(player1.deck.Count);
                response += $"{player2.username}'s card: {player2.deck[player2_card].name} ({player2.deck[player2_card].damage}, \"{player2.deck[player2_card].elementType}\")\n";
                response += $"{player1.username}'s card: {player1.deck[player1_card].name} ({player1.deck[player1_card].damage}, \"{player1.deck[player1_card].elementType}\")\n";

                int result = fight(player1.username, player1.deck[player1_card], player2.username, player2.deck[player2_card], ref response);
                if(result == 1){ // Player1 wins & Player2 looses
                    player1.deck.Add(player2.deck[player2_card]);
                    player2.deck.Remove(player2.deck[player2_card]);
                }else if(result == -1){ // Player1 looses & Player2 wins
                    player2.deck.Add(player1.deck[player1_card]);
                    player1.deck.Remove(player1.deck[player1_card]);
                }else // Draw.
                    response += "Draw.\n";

                response += '\n' + displayDecks(player1.deck, player2.deck, player1.username, player2.username);
            }

            return 0;
        }

        private int fight(string player1_name, A_Card player1_card, string player2_name, A_Card player2_card, ref string response){
            int result = specialCases(player1_name, player1_card, player2_name, player2_card, ref response);
            if(result == 1){
                return 1;
            }else if(result == -1)
                return -1;

            float temp_player1damage = player1_card.damage;
            float temp_player2damage = player2_card.damage;

            if(player1_card.cardType == 's' || player2_card.cardType == 's'){
                float effectiveness = compareElements(player1_card.elementType, player2_card.elementType);

                if(effectiveness == 2) {
                    response += $"{player1_card.elementType} is effective against {player2_card.elementType}, so damage is doubled.\n";
                }else if(effectiveness == 1) {
                    response += $"{player1_card.elementType} has no special effect against {player2_card.elementType}.\n";
                }else if(effectiveness == 0.5F)
                    response += $"{player1_card.elementType} is not effective against {player2_card.elementType}, so damage is halved.\n";

                temp_player1damage = player1_card.damage * effectiveness;
                temp_player2damage = player2_card.damage * compareElements(player2_card.elementType, player1_card.elementType);
            }

            if(temp_player1damage > temp_player2damage){
                response += $"{player1_card.name}'s damage is higher than {player2_card.name}. {player2_card.name} is being removed from {player2_name}'s deck and added to {player1_name}'s deck.\n"; 
                return 1;
            }else if(temp_player1damage < temp_player2damage){
                response += $"{player2_card.name}'s damage is higher than {player1_card.name}. {player1_card.name} is being removed from {player1_name}'s deck and added to {player2_name}'s deck.\n";
                return -1;
            }else return 0;
        }

        private float compareElements(string? player1_elementType, string? player2_elementType){
            float effectiveness = 1;

            if(player1_elementType == "Water" && player2_elementType == "Fire"){
                effectiveness = 2;
            }else if(player1_elementType == "Water" && player2_elementType == "Normal"){
                effectiveness = 0.5F;
            }else if(player1_elementType == "Fire" && player2_elementType == "Normal"){
                effectiveness = 2;
            }else if(player1_elementType == "Fire" && player2_elementType == "Water"){
                effectiveness = 0.5F;
            }else if(player1_elementType == "Normal" && player2_elementType == "Water"){
                effectiveness = 2;
            }else if(player1_elementType == "Normal" && player2_elementType == "Fire")
                effectiveness = 0.5F;

            return effectiveness;
        }

        private int specialCases(string player1_name, A_Card player1_card, string player2_name, A_Card player2_card, ref string response){
            int result = 0;
            // Dragon & FireElf
            if(player1_card.name == "Dragon" && player2_card.name == "FireElf"){
                response += $"{player2_card.name} dodged {player1_card.name}'s attack.\n";
                result = -1;
            }else if(player1_card.name == "FireElf" && player2_card.name == "Dragon"){
                response +=  $"{player1_card.name} dodged {player2_card.name}'s attack.\n";
                result = 1;
            }
            // Dragon & Goblin
            if(player1_card.name == "Dragon" && player2_card.name == "Goblin"){
                response += $"{player1_card.name} attacks the too afraid {player2_card.name}.\n";
                result = 1;
            }else if(player1_card.name == "Goblin" && player2_card.name == "Dragon"){
                response += $"{player1_card.name} is too afraid to attack {player2_card.name}.\n";
                result = -1;
            }
            // Knight & Water
            if(player1_card.name == "Knight" && (player2_card.elementType == "Water" && player2_card.cardType == 's')){
                response += $"{player1_card.name}'s armor is too heavy and is being drowned in {player2_card.elementType}.\n";
                result = -1;
            }else if((player1_card.elementType == "Water" && player1_card.cardType == 's') && player2_card.name == "Knight"){
                response += $"{player2_card.name}'s armor is too heavy and is being drowned in {player1_card.elementType}.\n";
                result = 1;
            }
            // Wizard & Orc
            if(player1_card.name == "Wizard" && player2_card.name == "Orc"){
                response += $"{player1_card.name} controlled {player2_card.name} and {player2_card.name} is unable to damage {player1_card.name}.\n";
                result = 1;
            }else if(player1_card.name == "Orc" && player2_card.name == "Wizard"){
                response += $"{player1_card.name} is controlled by {player2_card.name} and {player1_card.name} is unable to damage {player2_card.name}.\n";
                result = -1;
            }
            // Kraken & Spells
            if(player1_card.name == "Kraken" && player2_card.cardType == 's'){
                response += $"{player1_card.name} is immune to spells.\n";
                result = 1;
            }else if(player1_card.cardType == 's' && player2_card.name == "Kraken"){
                response += $"{player1_card.name} is immune to spells.\n";
                result = -1;
            }

            if(result == -1){
                response += $"{player1_name}'s {player1_card.name} is being added to {player2_name}'s deck.\n";
            }else if(result == 1)
                response += $"{player2_name}'s {player2_card.name} is being added to {player1_name}'s deck.\n";

            return result;
        }

        public string displayDecks(List<A_Card> player1_deck, List<A_Card> player2_deck, string player1_username, string player2_username){
            string text = "";
            text += $"{player1_username}'s deck: \n";
            foreach(var card in player1_deck)
                text += $"{card.name}\n";
            text += '\n';

            text += $"{player2_username}'s deck: \n";
            foreach(var card in player2_deck)
                text += $"{card.name}\n";
            text += '\n';

            return text;
        }
    }
}