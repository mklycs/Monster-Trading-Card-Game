namespace mtcg{
    internal class CardService{
        public CardService(){}

        public A_Card getCard(int id){
            if(id == 1) return new Dragon();
            else if(id == 2) return new FireElf();
            else if(id == 3) return new Goblin();
            else if(id == 4) return new Knight();
            else if(id == 5) return new Kraken();
            else if(id == 6) return new Orc();
            else if(id == 7) return new Wizard();
            else if(id == 8) return new Fireball();
            else if(id == 9) return new Waterfall();
            else if(id == 10) return new Gale();

            return null;
        }

        public void removeDeckFromStack(string token, List<A_Card> deck){
            CardQueries cardQueries = new CardQueries();

            int player1userID = cardQueries.getUserID(token);
            foreach(var card in deck)
                cardQueries.removeCard(card.id, player1userID);
        }

        public void addDeckToStack(string token, List<A_Card> deck){
            CardQueries cardQueries = new CardQueries();

            int player1userID = cardQueries.getUserID(token);
            foreach(var card in deck)
                cardQueries.addCard(card.id, player1userID);
        }
    }
}