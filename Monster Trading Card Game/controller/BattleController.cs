// startet Kämpfe zwischen Benutzern und gibt die Ergebnisse zurück

using System;

namespace mtcg{
    internal class BattleController{
        public BattleController(){}

        public void battle(User player1, User player2, Random random){
            string response = $"{player1.username} vs {player2.username}\n";
            BattleService battleService = new BattleService();
            response += battleService.displayDecks(player1.deck, player2.deck, player1.username, player2.username);

            int result = battleService.battle(player1, player2, random, ref response);
            if(result == 1){
                player1.wins += 1;
                player1.elo += 3;
                player1.coins += 4;
                response += "You win.\n";
            }else if(result == -1){
                player1.looses += 1;
                player1.elo -= 5;
                response += "You loose.\n";
            }else if(result == 0){
                player1.elo += 1;
                player1.coins += 1;
                response += "Draw.\n";
            }
            //*/
        }
    }
}