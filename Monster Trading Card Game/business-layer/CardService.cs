// Schnittstelle für Kartenoperationen, wie das Verwalten von Decks, Hinzufügen/Entfernen von Karten

using System;

namespace mtcg{
    internal class CardService{
        public List<A_Card> deck;
        public List<A_Card> stack;
        public CardService(List<A_Card> deck, List<A_Card> stack){
            this.deck = deck;
            this.stack = stack;
        }

        public void add(A_Card card){
            stack.Add(card);          // "Objekte werden erst nach ihrem Lebenszyklus (Scope) freigegeben"
            deck.Add(card);           // "Solange ein Objekt referenziert wird, bleibt es im Speicher"
        }

        public void remove(A_Card card){
            stack.Remove(card);          // löscht die Karte aus dem Stack, die Referenz ist aber noch im Deck enthalten
            deck.Remove(card);           // löscht die Karte komplett und es gibt gar keine Referenzen
        }

        public void emptyDeck(){
            deck.Clear();
        }

        public void initDeck(Random random){
            for(int i = 0; i < 4; i++){
                int random_card = random.Next(this.stack.Count);
                this.deck.Add(this.stack[random_card]);          // erstellt kein neues Objekt sondern Referenz
            }
        }

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
    }
}