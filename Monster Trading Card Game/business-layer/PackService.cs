namespace mtcg{
    internal class PackService{

        public int cost { get; set; } = 5;
        public int cards_amount { get; set; } = 5;
        public PackService(){ }

        public string unpackPack(Random random, int userID, CardQueries cardQueries){
            string response = "";
            for(int i = 0; i < cards_amount; i++){
                int cardID = random.Next(10) + 1;
                cardQueries.addCard(cardID, userID);
                response += getCardInfo(cardID);
            }
                
            return response;
        }

        private string getCardInfo(int id){
            CardService cardService = new CardService();
            A_Card card = cardService.getCard(id);
            
            return $"You unpacked {card.name} ({card.damage},\"{card.elementType}\").\n";
        }
    }
}