using System;
using System.Security.Cryptography.X509Certificates;
using Npgsql;

namespace mtcg{
    internal class UserQueries : Database{
        private NpgsqlConnection? conn = null;
        public UserQueries(){
            this.conn = this.getConn();
            conn.Open();
        }

        private int selectUserID(string? username){
            int userID = -1;
            using(var command = new NpgsqlCommand($"SELECT id FROM \"USERS\" WHERE username = @username;", conn)){
                command.Parameters.AddWithValue("username", $"{username}");
                using(var reader = command.ExecuteReader()){
                    while (reader.Read()){
                        userID = reader.GetInt32(0);
                    }
                }
            }
            return userID;
        }

        private bool comparePasswords(string? username, string? paswd){
            string password = "";
            using(var command = new NpgsqlCommand($"SELECT password FROM \"USERS\" WHERE username = @username;", conn)){
                command.Parameters.AddWithValue("username", $"{username}");
                using(var reader = command.ExecuteReader()){
                    while(reader.Read()){
                        password = reader.GetString(0);
                    }
                }
            }

            if(paswd != password) return false;

            return true;
        }

        public bool addUser(UserDto user, string token){
            int userID = selectUserID(user.username);
            if(userID != -1) return false; // wurde ein user gefunden/exisitiert wird kein neuer user hinzugefügt
            using(var command = new NpgsqlCommand($"INSERT INTO \"STATS\" (coins, wins, looses, elo, rating) VALUES (20, 0, 0, 100, 0.00);", conn))
                command.ExecuteNonQuery();

            using(var command = new NpgsqlCommand($"INSERT INTO \"USERS\" (username, password, token) VALUES (@username, @password, @token);", conn)){ //"INSERT INTO \"USERS\" (username, password, statsid, stackid, deckid, admin) VALUES (@username, @password, @statsid, @stackid, @deckid, 0);"
                command.Parameters.AddWithValue("username", $"{user.username}"); // das @ ist jetzt hier nicht notwendig, weil Npgsql das autmoatisch handelt :)
                command.Parameters.AddWithValue("password", $"{user.password}");
                command.Parameters.AddWithValue("token", $"{token}");
                command.ExecuteNonQuery();
            }

            userID = selectUserID(user.username);
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
            return true; // "user wurde erfolgreich hinzugefuegt"
        }

        public bool updateUser(UserDto user){
            int userID = selectUserID(user.new_username);
            if(userID != -1) return false;

            userID = selectUserID(user.username);

            using(var command = new NpgsqlCommand($"UPDATE \"USERS\" SET username = @username, password=@password WHERE id=@id;", conn)){
                command.Parameters.AddWithValue("username", $"{user.new_username}");
                command.Parameters.AddWithValue("password", $"{user.password}");
                command.Parameters.AddWithValue("id", userID);
                command.ExecuteNonQuery();
            }
            return true; // "user wurde erfolgreich geupdated"
        }

        public bool deleteUser(UserDto user){
            int userID = selectUserID(user.username);
            if(userID == -1) return false; // "user wurde nicht gefunden"

            if(!comparePasswords(user.username, user.password)) return false;

            using(var command = new NpgsqlCommand($"DELETE FROM \"USERS\" WHERE id = {userID};", conn))
                command.ExecuteNonQuery();

            using(var command = new NpgsqlCommand($"DELETE FROM \"STATS\" WHERE id = {userID};", conn)) // weil scheiss delete on cascade nicht funktioniert
                command.ExecuteNonQuery();

            using(var command = new NpgsqlCommand($"DELETE FROM \"STACKS\" WHERE userid = {userID};", conn))
                command.ExecuteNonQuery();

            return true; // "user wurde erfolgreich geloescht"
        }

        public bool setTokenDefault(string token){
            using(var command = new NpgsqlCommand($"UPDATE \"USERS\" SET token = @token_new WHERE token=@token_old;", conn)){
                command.Parameters.AddWithValue("token_new", "-");
                command.Parameters.AddWithValue("token_old", $"{token}");
                command.ExecuteNonQuery();
            }

            return true;
        }
        public bool setToken(string username, string token){
            using(var command = new NpgsqlCommand($"UPDATE \"USERS\" SET token=@token WHERE username=@username;", conn)){
                command.Parameters.AddWithValue("token", $"{token}");
                command.Parameters.AddWithValue("username", $"{username}");
                command.ExecuteNonQuery();
            }
            return true;
        }

        public string getToken(string username){
            string token = "";
            using(var command = new NpgsqlCommand($"SELECT token FROM \"USERS\" WHERE username = @username;", conn)) {
                command.Parameters.AddWithValue("username", $"{username}");
                using(var reader = command.ExecuteReader()) {
                    while(reader.Read()) {
                        token = reader.GetString(0);
                    }
                }
            }

            return token;
        }

        public bool isTokenSet(string username){
            using(var command = new NpgsqlCommand("SELECT CASE WHEN token = '-' THEN FALSE ELSE TRUE END FROM \"USERS\" WHERE username=@username;", conn)){
                command.Parameters.AddWithValue("username", $"{username}");
                var result = command.ExecuteScalar();
                if(result != null && result is bool) return (bool)result;
            }
            return false;
        }
    }
}