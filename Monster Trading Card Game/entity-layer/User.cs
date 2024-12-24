using System;

namespace mtcg{
    public class User{
        public string? username { get; set; }
        //public string? password { get; set; }
        public int coins { get; set; }    // = 20;
        public int wins { get; set; }     // = 0;
        public int looses { get; set; }   // = 0;
        public int elo { get; set; }      // = 100;
        public float rating { get; set; } // = 0;
        public List<A_Card> stack { get; set; } = new List<A_Card>();
        public List<A_Card> deck { get; set; } = new List<A_Card>(4);
        public string token { get; set; }

        public User(string? username, /*string? password,*/ string token, int coins = 20, int wins = 0, int looses = 0, int elo = 100, int rating = 0){
            this.username = username;
            //this.password = password;
            this.coins = coins;       //  20
            this.wins = wins;         //   0
            this.looses = looses;     //   0
            this.elo = elo;           // 100
            this.rating = rating;     //   0
            this.token = token;

            this.stack = new List<A_Card>();
            for(int i = 0; i < 2; i++){
                this.stack.Add(new Dragon());
                this.stack.Add(new FireElf());
                this.stack.Add(new Goblin());
                this.stack.Add(new Knight());
                this.stack.Add(new Kraken());
                this.stack.Add(new Orc());
                this.stack.Add(new Wizard());
                this.stack.Add(new Fireball());
                this.stack.Add(new Waterfall());
                this.stack.Add(new Gale());
            }

            this.deck = new List<A_Card>(4);
        }
    }
}