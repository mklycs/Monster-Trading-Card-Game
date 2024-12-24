// verwaltet Anfragen zum Erwerb von Karten oder zum Verwalten des Decks
/*
using System;
using System.Reflection;
*/
using mtcg;

namespace mtcg{
    internal class CardController{
        public CardController(){ }
        /*
            public void tradeCards(){

            }
        */

        public Status buyPackage(string token, Random random){
            CardQueries cardQueries = new CardQueries();
            int coins = cardQueries.getCoins(token);
            PackService packService = new PackService();

            if(coins < packService.cost) return new Status(400, "Not enough coins.");

            List<A_Card> pack = packService.unpackPack(random);
            for(int i = 0; i < pack.Count; i++)
                cardQueries.addCard(pack[i].id, token);

            if(!cardQueries.setCoins(token, coins - packService.cost)) return new Status(400, "Something went wrong with coin payment.");

            return new Status(200, "Succesfully bought a package.");
        }
    }
}