// startet Kämpfe zwischen Benutzern und gibt die Ergebnisse zurück

using System;

namespace mtcg{
    internal class BattleController{
        public User you;
        public User opponent;
        public BattleController(User you, User opponent){
            this.you = you;
            this.opponent = opponent;
        }
        public void battle(Random random){
            Console.WriteLine($"{you.username} vs {opponent.username}");
            BattleService battleService = new BattleService();
            battleService.displayDecks(you.deck, opponent.deck, opponent.username);

            int result = battleService.battle(you, opponent, random);
            if(result == 1){
                you.wins += 1;
                you.elo += 3;
                you.coins += 4;
                Console.WriteLine("You win.");
            }else if(result == -1){
                you.looses += 1;
                you.elo -= 5;
                Console.WriteLine("You loose.");
            }else if(result == 0){
                you.elo += 1;
                you.coins += 1;
                Console.WriteLine("Draw.");
            }
        }
    }
}