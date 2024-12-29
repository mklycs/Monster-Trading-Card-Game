﻿// enthält die Logik für den Kampf, die Kampfauswertung und Battle-Log

using System;

namespace mtcg{
    internal class BattleService{
        public BattleService() { }
        
        public int battle(User player1, User player2, Random random, ref string response){
            CardService player1_cardService = new CardService();
            CardService player2_cardService = new CardService(); // ???

            for(int round = 1; round < 101; round++) {
                if(player2.deck.Count < 1){
                    return 1;
                }else if(player1.deck.Count < 1) 
                    return -1;

                response += $"Round: {round}\n";
                int opps_card = random.Next(player2.deck.Count);
                int your_card = random.Next(player1.deck.Count);
                response += $"{player2.username}'s card: {player2.deck[opps_card].name} ({player2.deck[opps_card].damage}, \"{player2.deck[opps_card].elementType}\")\n";
                response += $"{player1.username}'s card: {player1.deck[your_card].name} ({player1.deck[your_card].damage}, \"{player1.deck[your_card].elementType}\")\n";

                int result = fight(player1.deck[your_card], player2.deck[opps_card], ref response);
                if(result == 1){ // W
                    //player1_cardService.add(player2.deck[opps_card]);
                    //player2_cardService.remove(player2.deck[opps_card]);
                }else if(result == -1){ // L
                    //player2_cardService.add(player1.deck[your_card]);
                    //player1_cardService.remove(player1.deck[your_card]);
                }else
                    response += "Draw.\n";

                response += displayDecks(player1.deck, player2.deck, player1.username, player2.username);
                response += '\n';
            }

            return 0;
        }

        public int fight(A_Card player1_card, A_Card player2_card, ref string response) {
            int result = specialCases(player1_card, player2_card, ref response);
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
                response += $"{player1_card.name}'s damage is higher than {player2_card.name}. {player2_card.name} is added to your deck.\n";
                return 1;
            }else if(temp_player1damage < temp_player2damage){
                response += $"{player1_card.name}'s damage is lower than {player2_card.name}. You loose {player1_card.name} from your deck.\n";
                return -1;
            }else return 0;
        }

        public float compareElements(string? player1_elementType, string? player2_elementType){
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

        public int specialCases(A_Card player1_card, A_Card player2_card, ref string response){
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
                response += $"Your opponent gets your {player1_card.name}.\n";
            }else if(result == 1)
                response += $"Your opponents {player2_card.name} is being added to your deck.\n";

            return result;
        }
        //*/

        public string displayDecks(List<A_Card> player1_deck, List<A_Card> player2_deck, string player1_username, string player2_username){
            string text = "";
            text += $"{player1_username} deck: \n";
            for(int i = 0; i < player1_deck.Count; i++)
                text += $"{player1_deck[i].name}\n";
            text += '\n';

            text += $"{player2_username} deck: \n";
            for(int i = 0; i < player2_deck.Count; i++)
                text += $"{player2_deck[i].name}\n";
            text += '\n';

            return text;
        }
    }
}