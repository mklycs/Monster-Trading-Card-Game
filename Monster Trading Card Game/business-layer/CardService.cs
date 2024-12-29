// Schnittstelle für Kartenoperationen, wie das Verwalten von Decks, Hinzufügen/Entfernen von Karten

using System;
using System.Reflection;

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
        public void add(){
        
        }

        public void remove(){

        }

        /*
        public void defineDeck(Validator validator){
            deck.Clear();
            Console.WriteLine("Choose 4 cards from your stack to use in your deck in battles.");
            for(int i = 0; i < stack.Count; i++)
                Console.WriteLine($"{i + 1}. {stack[i].name}");

            for (int i = 0; i < 4; i++){
                Console.WriteLine($"{i} cards are currently in your deck.");
                while(true){
                    int choice = validator.checkInt(1, stack.Count, "");
                    int count = 0;
                    for(int ii = 0; ii < deck.Count; ii++){
                        if(stack[choice - 1].name == deck[ii].name)
                            count++;
                    }

                    if(count < 2){
                        deck.Add(stack[choice - 1]);
                        Console.WriteLine($"{stack[choice - 1].name} has been added to your deck.");
                        break;
                    }else Console.WriteLine($"{stack[choice - 1].name} appears already twice in your deck.");
                }
            }
        }
        */
    }
}