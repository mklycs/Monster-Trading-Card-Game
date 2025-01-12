namespace mtcg{
    internal class CardController{
        public CardController(){ }
        
        public Status buyPackage(string token, Random random){
            CardQueries cardQueries = new CardQueries();
            int userID = cardQueries.getUserID(token);
            int coins = cardQueries.getCoins(userID);
            PackService packService = new PackService();

            if(coins < packService.cost) 
                return new Status(400, "Not enough coins.");

            if(!cardQueries.setCoins(userID, coins - packService.cost)) 
                return new Status(400, "Something went wrong with coin payment.");

            string response = packService.unpackPack(random, userID, cardQueries);

            return new Status(200, $"{response}");
        }

        public bool getDeck(string token, DeckDto deckDto, User user){
            List<int> cards = new List<int>();
            cards.Add(deckDto.card1);
            cards.Add(deckDto.card2);
            cards.Add(deckDto.card3);
            cards.Add(deckDto.card4);

            foreach(var card in cards){
                if(card == 0)
                    return false;
            }

            CardQueries cardQueries = new CardQueries();
            int userID = cardQueries.getUserID(token);

            foreach(var card in cards){
                if(!cardQueries.checkifCardInStack(card, userID))
                    return false;
            }

            CardService cardService = new CardService();
            foreach(var card in cards)
                user.deck.Add(cardService.getCard(card));

            return true;
        }

        public Status offerCard(string token, int offerCardID, int requestCardID){
            CardQueries cardQueries = new CardQueries();
            int userID = cardQueries.getUserID(token);

            if(!cardQueries.checkifCardInStack(offerCardID, userID))
                return new Status(404, "The card to offer could not be found in stack.");

            if(!cardQueries.addOffer(userID, offerCardID, requestCardID))
                return new Status(400, "Something went wrong with adding offer.");

            return new Status(200, "Successfully added offer.");
        }
        
        public Status deleteOffer(string token, int tradeID){
            CardQueries cardQueries = new CardQueries();
            int userID = cardQueries.getUserID(token);
            var tradeInfo = cardQueries.getTradeInfo(tradeID);
            int offerUserID = tradeInfo.Item1;
            int offerCardID = tradeInfo.Item2;
            int requestCardID = tradeInfo.Item3;

            if(userID == 0 || offerUserID == 0 || offerCardID == 0 || requestCardID == 0)
                return new Status(404, "Could not find trade offer.");

            if(!cardQueries.deleteOffer(tradeID, userID))
                return new Status(400, "Something went wrong with deleting the offer.");

            return new Status(200, "Successfully deleted offer.");
        }

        public Status tradeCards(string token, int tradeID){
            CardQueries cardQueries = new CardQueries();

            int userID = cardQueries.getUserID(token);
            var tradeInfo = cardQueries.getTradeInfo(tradeID);
            int offerUserID = tradeInfo.Item1;
            int offerCardID = tradeInfo.Item2;
            int requestCardID = tradeInfo.Item3;

            if(userID == 0 || offerUserID == 0 || offerCardID == 0 || requestCardID == 0)
                return new Status(404, "Could not find trade offer.");

            if(userID == offerUserID)
                return new Status(400, "You cannot accept your own offer.");

            if(!cardQueries.addCard(offerCardID, userID) || !cardQueries.addCard(requestCardID, offerUserID))
                return new Status(400, "An error occured with adding traded card.");

            if(!cardQueries.removeCard(requestCardID, userID) || !cardQueries.removeCard(offerCardID, offerUserID))
                return new Status(400, "An error occured with removing traded card.");

            if(!cardQueries.deleteOffer(tradeID, offerUserID))
                return new Status(400, "An error occured with deleting the offer.");

            return new Status(200, "Successfully traded cards.");
        }

        public Status gamble(string token, Random random, int coins, int headortails) {
            CardQueries cardQueries = new CardQueries();
            int userID = cardQueries.getUserID(token);
            int userCoins = cardQueries.getCoins(userID);

            if(coins < 1)
                return new Status(400, "Invalid coins amount.");

            if(userCoins < 1)
                return new Status(400, "Not enough coins to gamble.");

            if(headortails != 0 && headortails != 1)
                return new Status(400, "Invalid bet.");

            int head = 0;
            if(headortails == 0) // make head opposite so player dont win
                head = 1;

            double result = random.NextDouble();
            if(result >= 0.7 && result <= 0.8)
                head = headortails;

            if(headortails == head){
                if(!cardQueries.setCoins(userID, userCoins + coins))
                    return new Status(400, "Something went wrong with coin transaction.");
                return new Status(200, $"You win {coins} coins!");
            }else{
                if(!cardQueries.setCoins(userID, userCoins - coins))
                    return new Status(400, "Something went wrong with coin transaction.");
                return new Status(200, $"You loose {coins} coins.");
            }
        }
    }
}