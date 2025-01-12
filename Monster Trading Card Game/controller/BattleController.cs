namespace mtcg{
    internal class BattleController{
        public BattleController(){}

        public void battle(User player1, User player2, Random random){
            string response = $"{player1.username} vs {player2.username}\n";
            BattleService battleService = new BattleService();
            response += battleService.displayDecks(player1.deck, player2.deck, player1.username, player2.username);

            CardService cardService = new CardService();
            cardService.removeDeckFromStack(player1.token, player1.deck);
            cardService.removeDeckFromStack(player2.token, player2.deck);

            int result = battleService.battle(player1, player2, random, ref response, cardService);
            if(result == 1){ // player1 wins & player2 looses
                processWin(player1);
                processLoose(player2);
                response += $"{player1.username} wins.\n";
            }else if(result == -1){ // player2 wins & player1 looses
                processWin(player2);
                processLoose(player1);
                response += $"{player2.username} wins.\n";
            }else if(result == 0){ // Draw.
                processDraw(player1);
                processDraw(player2);
                response += "Draw.\n";
            }

            cardService.addDeckToStack(player1.token, player1.deck);
            cardService.addDeckToStack(player2.token, player2.deck);

            updatePlayerStats(player1);
            updatePlayerStats(player2);

            player1.sendServerBattleResponse(response);
            player2.sendServerBattleResponse(response);
        }

        internal void processWin(User player){
            player.wins += 1;
            player.elo += 3;
            player.coins += 4;
            player.rating = (((float)player.wins / (player.wins + player.looses)) * 100);
        }

        internal void processLoose(User player){
            player.looses += 1;
            player.elo -= 5;
            player.rating = (((float)player.wins / (player.wins + player.looses)) * 100);
        }

        internal void processDraw(User player){
            player.elo += 1;
            player.coins += 1;
        }

        private void updatePlayerStats(User user){
            UserQueries userQueries = new UserQueries();
            userQueries.updateStats(user);
        }
    }
}