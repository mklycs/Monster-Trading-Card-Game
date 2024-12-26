using System;

namespace mtcg{
    internal class PackService{

        public int cost { get; set; } = 5;
        public int cards_amount { get; set; } = 5;
        public PackService(){ }

        public string unpackPack(Random random, string token, CardQueries cardQueries){
            string response = "";
            for(int i = 0; i < cards_amount; i++){
                int random_num = random.Next(10) + 1;
                cardQueries.addCard(random_num, token);
                response += getCardInfo(random_num);
            }
                
            return response;
        }

        private string getCardInfo(int index){
            A_Card card = null;
            if(index == 1) card = new Dragon();
            else if(index == 2) card = new FireElf();
            else if(index == 3) card = new Goblin();
            else if(index == 4) card = new Knight();
            else if(index == 5) card = new Kraken();
            else if(index == 6) card = new Orc();
            else if(index == 7) card = new Wizard();
            else if(index == 8) card = new Fireball();
            else if(index == 9) card = new Waterfall();
            else if(index == 10) card = new Gale();
            
            return $"You unpacked {card.name} ({card.damage},\"{card.elementType}\").\n";
        }
    }
}