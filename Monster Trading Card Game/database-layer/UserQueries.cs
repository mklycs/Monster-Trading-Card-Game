using Npgsql;

namespace mtcg{
    internal class UserQueries : Database{
        private NpgsqlConnection? conn = null;
        public UserQueries(){
            this.conn = this.getConn();
            conn.Open();
        }

        private int getUserID(string? username){
            int userID = -1;
            using(var command = new NpgsqlCommand($"SELECT id FROM \"USERS\" WHERE username = @username;", conn)){
                command.Parameters.AddWithValue("username", $"{username}");
                using(var reader = command.ExecuteReader()){
                    while(reader.Read())
                        userID = reader.GetInt32(0);
                }
            }
            return userID;
        }

        private bool comparePasswords(string? username, string? paswd){
            string password = "";
            using(var command = new NpgsqlCommand($"SELECT password FROM \"USERS\" WHERE username = @username;", conn)){
                command.Parameters.AddWithValue("username", $"{username}");
                using(var reader = command.ExecuteReader()){
                    while(reader.Read())
                        password = reader.GetString(0);
                }
            }

            if(paswd != password) 
                return false;

            return true;
        }

        public bool addUser(UserDto user, string token){
            int userID = getUserID(user.username);
            if(userID != -1) 
                return false;

            using(var command = new NpgsqlCommand($"INSERT INTO \"STATS\" (coins, wins, looses, elo, rating) VALUES (20, 0, 0, 100, 0.00);", conn))
                command.ExecuteNonQuery();

            using(var command = new NpgsqlCommand($"INSERT INTO \"USERS\" (username, password, token) VALUES (@username, @password, @token);", conn)){
                command.Parameters.AddWithValue("username", $"{user.username}"); 
                command.Parameters.AddWithValue("password", $"{user.password}");
                command.Parameters.AddWithValue("token", $"{token}");
                command.ExecuteNonQuery();
            }

            userID = getUserID(user.username);
            for(int i = 1; i <= 10; i++){
                using(var command = new NpgsqlCommand($"INSERT INTO \"STACKS\" (userid, cardid) VALUES (@userid, @cardid);", conn)){
                    command.Parameters.AddWithValue("userid", userID);
                    command.Parameters.AddWithValue("cardid", i);
                    command.ExecuteNonQuery();
                }
                
                using(var command = new NpgsqlCommand($"INSERT INTO \"STACKS\" (userid, cardid) VALUES (@userid, @cardid);", conn)){
                    command.Parameters.AddWithValue("userid", userID);
                    command.Parameters.AddWithValue("cardid", i);
                    command.ExecuteNonQuery();
                }
            }
            return true;
        }

        public bool updateUser(UserDto user){
            int userID = getUserID(user.new_username);
            if(userID != -1) 
                return false;

            userID = getUserID(user.username);

            using(var command = new NpgsqlCommand($"UPDATE \"USERS\" SET username = @username, password=@password WHERE id=@id;", conn)){
                command.Parameters.AddWithValue("username", $"{user.new_username}");
                command.Parameters.AddWithValue("password", $"{user.password}");
                command.Parameters.AddWithValue("id", userID);
                command.ExecuteNonQuery();
            }
            return true;
        }

        public bool deleteUser(UserDto user){
            int userID = getUserID(user.username);
            if(userID == -1) 
                return false;

            if(!comparePasswords(user.username, user.password))
                return false;

            using(var command = new NpgsqlCommand($"DELETE FROM \"USERS\" WHERE id = {userID};", conn)){
                int rowsAffected = command.ExecuteNonQuery();
                if(rowsAffected == 0)
                    return false;
            }

            using(var command = new NpgsqlCommand($"DELETE FROM \"STATS\" WHERE id = {userID};", conn)){ // weil scheiss delete on cascade nicht funktioniert
                int rowsAffected = command.ExecuteNonQuery();
                if(rowsAffected == 0)
                    return false;
            }

            using(var command = new NpgsqlCommand($"DELETE FROM \"STACKS\" WHERE userid = {userID};", conn)){
                int rowsAffected = command.ExecuteNonQuery();
                if(rowsAffected == 0)
                    return false;
            }

            return true;
        }

        public bool setTokenDefault(string token){
            using(var command = new NpgsqlCommand($"UPDATE \"USERS\" SET token = @token_new WHERE token=@token_old;", conn)){
                command.Parameters.AddWithValue("token_new", "-");
                command.Parameters.AddWithValue("token_old", $"{token}");
                int rowsAffected = command.ExecuteNonQuery();

                if(rowsAffected == 0)
                    return false;
            }

            return true;
        }

        public bool setToken(string username, string token){
            using(var command = new NpgsqlCommand($"UPDATE \"USERS\" SET token=@token WHERE username=@username;", conn)){
                command.Parameters.AddWithValue("token", $"{token}");
                command.Parameters.AddWithValue("username", $"{username}");
                int rowsAffected = command.ExecuteNonQuery();

                if(rowsAffected == 0)
                    return false;
            }

            return true;
        }

        public bool isTokenSet(string username){
            using(var command = new NpgsqlCommand("SELECT CASE WHEN token = '-' THEN FALSE ELSE TRUE END FROM \"USERS\" WHERE username=@username;", conn)){
                command.Parameters.AddWithValue("username", $"{username}");
                var result = command.ExecuteScalar();

                if(result != null && result is bool) 
                    return (bool)result;
            }
            return false;
        }

        public bool isloggedin(string token){
            int count = 0;
            using(var command = new NpgsqlCommand("SELECT COUNT(id) FROM \"USERS\" WHERE token = @token;", conn)){
                command.Parameters.AddWithValue("token", $"{token}");
                using(var reader = command.ExecuteReader()) {
                    while(reader.Read())
                        count = reader.GetInt32(0);
                }
            }

            if(count < 1)
                return false;

            return true;
        }

        public (string, int, int, int, int, float, int) getUser(string token){
            int coins = 0, wins = 0, looses = 0, elo = 0, cardid = 0;
            string username = "";
            float rating = 0;
            using(var command = new NpgsqlCommand($"SELECT username, coins, wins, looses, elo, rating, cardid FROM \"USERS\" users JOIN \"STATS\" stats on users.statsid = stats.id JOIN \"STACKS\" stack on users.id = stack.userid WHERE token = @token;", conn)) {
                command.Parameters.AddWithValue("token", $"{token}");
                using(var reader = command.ExecuteReader()){
                    while(reader.Read()){
                        username = reader.GetString(0);
                        coins = reader.GetInt32(1);
                        wins = reader.GetInt32(2);
                        looses = reader.GetInt32(3);
                        elo = reader.GetInt32(4);
                        rating = reader.GetFloat(5);
                        cardid = reader.GetInt32(6);
                    }
                }
            }

            return (username, coins, wins, looses, elo, rating, cardid);
        }

        public bool updateStats(User user){
            int userID = getUserID(user.username);
            using(var command = new NpgsqlCommand($"UPDATE \"STATS\" SET coins=@coins, wins = @wins, looses = @looses, elo = @elo, rating = @rating WHERE id=@userID;", conn)){
                command.Parameters.AddWithValue("coins", user.coins);
                command.Parameters.AddWithValue("wins", user.wins);
                command.Parameters.AddWithValue("looses", user.looses);
                command.Parameters.AddWithValue("elo", user.elo);
                command.Parameters.AddWithValue("rating", user.rating);
                command.Parameters.AddWithValue("userID", userID);
                int rowsAffected = command.ExecuteNonQuery();

                if(rowsAffected == 0)
                    return false;
            }

            return true;
        }

        public List<(string, int, int, int, float)> getBestPlayers(){
            var tupleList = new List<(string, int, int, int, float)>{};
            using(var command = new NpgsqlCommand("SELECT username, wins, looses, elo, rating FROM \"USERS\" users \r\nJOIN \"STATS\" stats ON users.id = stats.id\r\nORDER BY elo DESC LIMIT 5;", conn)){
                using(var reader = command.ExecuteReader()){
                    while(reader.Read()){
                        string username = reader.GetString(0);
                        int wins = reader.GetInt32(1);
                        int looses = reader.GetInt32(2);
                        int elo = reader.GetInt32(3);
                        float rating = reader.GetFloat(4);
                        tupleList.Add((username, wins, looses, elo, rating));
                    }
                }
            }
            return tupleList;
        }
    }
}