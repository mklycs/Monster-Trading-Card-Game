using System;

namespace mtcg{
    internal class DeckDto{
        public int card1 { get; set; }
        public int card2 { get; set; }
        public int card3 { get; set; }
        public int card4 { get; set; }

        public DeckDto(int card1 = 0, int card2 = 0, int card3 = 0, int card4 = 0){
            this.card1 = card1;
            this.card2 = card2;
            this.card3 = card3;
            this.card4 = card4;
        }
    }
}