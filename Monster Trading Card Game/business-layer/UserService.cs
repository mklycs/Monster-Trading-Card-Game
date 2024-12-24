// Schnittstelle für Benutzer-bezogene Operationen, wie Starten eines Kampfes, Kaufen von Karten

using System;

namespace mtcg{
    internal class UserService{
        //public User you;
        //public Validator validator;
        //private static UserService? instance = null;
        private UserService(Random random){
            /*
            this.validator = new Validator();
            this.you = login();
            while(true){
                int input = this.validator.checkInt(0, 6, "Choose an option:\n0. Exit\n1. Define your deck\n2. Battle against other players\n3. Show score-board\n4. Trade cards\n5. Buy a package\n6. Change Login credentials\n> ");
                if(input == 0) return;
                else if(input == 1) defineDeck(validator);
                else if(input == 2) battle(random);
                else if(input == 3) showScoreboard();
                else if(input == 4) tradeCards();
                else if(input == 5) buyPackage(random);
                else if(input == 6) changeCredentials();
            }
            //*/
        }
        /*
        public static UserService getInstance(Random random){
            if(instance == null) instance = new UserService(random);
            return instance;
        }
        //*/

        void defineDeck(){
            /*
            CardService cardService = new CardService(you.deck, you.stack);
            cardService.defineDeck(validator);
            //*/
        }

        void battle(Random random){
            /*
            if(you.deck.Count < 1){
                Console.WriteLine("You have no cards in your deck.");
                return;
            }
            User opponent = login(); // einfach so fürs erste
            CardService opps_cardService = new CardService(opponent.deck, opponent.stack);
            opps_cardService.initDeck(random);
            BattleController battleController = new BattleController(you, opponent);
            battleController.battle(random);
            */
        }

        void showScoreboard(){
            /*
            Console.WriteLine($"RANK   USERNAME   WINS   LOOSES   RATIO   RATING");
            for(int i = 0; i < 10; i++){
                // Datensätze aus der DB bekommen
                // Datensätze sortieren
                // nur die besten 10 anzeigen
            }
            Console.WriteLine($"{0} {you.username} {you.wins} {you.looses} {you.rating} {you.elo}");
            */
        }
        void tradeCards(){

        }

        void buyPackage(Random random){
            /*
            CardController cardController = new CardController();
            you.coins += cardController.buyPackage(you.coins, you.stack, random);
            //*/
        }
    }
}