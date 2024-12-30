using System;

namespace mtcg{
    internal class Knight : A_Card{
        public Knight(){
            this.id = 4;
            this.name = "Knight";
            this.cardType = 'm';
            this.elementType = "Normal";
            this.damage = 4;
        }
    }
}