// verwaltet Anfragen zum Erwerb von Karten oder zum Verwalten des Decks

using System;

namespace mtcg{
    internal class CardController{
        public CardController(){ }
        
        public Status buyPackage(string token, Random random) {
            CardQueries cardQueries = new CardQueries();
            int coins = cardQueries.getCoins(token);
            PackService packService = new PackService();

            if(coins < packService.cost) 
                return new Status(400, "Not enough coins.");

            if(!cardQueries.setCoins(token, coins - packService.cost)) 
                return new Status(400, "Something went wrong with coin payment.");

            string response = packService.unpackPack(random, token, cardQueries);

            return new Status(200, $"{response}");
        }

        public void defineDeck(){
            CardService cardService = new CardService(null, null);
        }

        public void tradeCards(){

        }
    }
}