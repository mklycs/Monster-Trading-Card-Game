using System;

namespace mtcg{
    internal class Fireball : A_Card{
        public Fireball(){
            this.name = "Fireball";
            this.cardType = 's';
            this.elementType = "Fire";
            this.damage = 6;
        }
    }
}