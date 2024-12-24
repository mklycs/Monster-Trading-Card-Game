using System;

namespace mtcg{
    internal class PackService{

        public int cost { get; set; } = 5;
        public int cards_amount { get; set; } = 5;
        public PackService(){ }

        public List<A_Card> unpackPack(Random random){
            List<A_Card> pack = new List<A_Card>();
            for(int i = 0; i < cards_amount; i++){
                int random_num = random.Next(10);
                if(random_num == 0) pack.Add(new Dragon());
                else if(random_num == 1) pack.Add(new FireElf());
                else if(random_num == 2) pack.Add(new Goblin());
                else if(random_num == 3) pack.Add(new Knight());
                else if(random_num == 4) pack.Add(new Kraken());
                else if(random_num == 5) pack.Add(new Orc());
                else if(random_num == 6) pack.Add(new Wizard());
                else if(random_num == 7) pack.Add(new Fireball());
                else if(random_num == 8) pack.Add(new Waterfall());
                else if(random_num == 9) pack.Add(new Gale());
                Console.WriteLine($"You unpacked {pack[pack.Count - 1].name} ({pack[pack.Count - 1].damage},\"{pack[pack.Count - 1].elementType}\").");
            }

            return pack;
        }
    }
}