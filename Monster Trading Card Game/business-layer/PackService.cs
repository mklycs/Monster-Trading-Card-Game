using System;

namespace mtcg{
    internal class PackService{

        public int cost { get; set; } = 5;
        public int cards_amount { get; set; } = 5;
        public PackService() { }

        public void unpackPack(List<A_Card> stack, Random random, int cards_amount){
            for(int i = 0; i < cards_amount; i++){
                int random_num = random.Next(10); // die 10 sind statisch, aber ich weiß jzt auch nd, wie man programmieren soll, dass automtaisch hochgezählt wird, wenn eine neue Karte hinzugefügt wird
                if (random_num == 0) stack.Add(new Dragon());
                else if (random_num == 1) stack.Add(new FireElf());
                else if (random_num == 2) stack.Add(new Goblin());
                else if (random_num == 3) stack.Add(new Knight());
                else if (random_num == 4) stack.Add(new Kraken());
                else if (random_num == 5) stack.Add(new Orc());
                else if (random_num == 6) stack.Add(new Wizard());
                else if (random_num == 7) stack.Add(new Fireball());
                else if (random_num == 8) stack.Add(new Waterfall());
                else if (random_num == 9) stack.Add(new Gale());
                Console.WriteLine($"You unpacked {stack[stack.Count - 1].name} ({stack[stack.Count - 1].damage},\"{stack[stack.Count - 1].elementType}\").");
            }
        }
    }
}