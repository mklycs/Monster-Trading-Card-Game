namespace mtcg{
    internal class StatsDto{
        public int? coins { get; set; }
        public int? wins { get; set; }
        public int? looses { get; set; }
        public int? elo { get; set; }
        public float? rating { get; set; }

        public StatsDto(int? coins, int? looses, int? wins, int? elo, int? rating){
            this.coins = coins;
            this.wins = wins;
            this.looses = looses;
            this.elo = elo;
            this.rating = rating;
        }
    }
}