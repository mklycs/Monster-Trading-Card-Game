using System;
using System.Drawing;
using System.Security.Cryptography.X509Certificates;
using Npgsql;

namespace mtcg{
    internal class CardQueries : Database{
        private NpgsqlConnection? conn = null;
        public CardQueries(){
            this.conn = this.getConn();
            conn.Open();
        }

        public int getCoins(string token) {
            int coins = 0;
            using(var command = new NpgsqlCommand($"SELECT coins FROM \"STATS\" stats JOIN \"USERS\" users on stats.id = users.id WHERE token = @token;", conn)){
                command.Parameters.AddWithValue("token", $"{token}");
                using(var reader = command.ExecuteReader()) {
                    while(reader.Read()) {
                        coins = reader.GetInt32(0);
                    }
                }
            }
            return coins;
        }

        public bool setCoins(string token, int coins){
            using(var command = new NpgsqlCommand($"UPDATE \"STATS\" SET coins = @coins WHERE id = (SELECT id FROM \"USERS\" WHERE token = @token);", conn)){
                command.Parameters.AddWithValue("coins", coins);
                command.Parameters.AddWithValue("token", token);
                command.ExecuteNonQuery();
            }
            return true;
        }

        public bool addCard(int cardID, string token){
            using(var command = new NpgsqlCommand($"INSERT INTO \"STACKS\" (userid, cardid) VALUES ((SELECT id FROM \"USERS\" WHERE token = @token), @cardid);", conn)){
                command.Parameters.AddWithValue("token", token);
                command.Parameters.AddWithValue("cardid", cardID);
                command.ExecuteNonQuery();
            }
            return true;
        }

        private int selectUserID(string? username){
            int userID = -1;
            using(var command = new NpgsqlCommand($"SELECT id FROM \"USERS\" WHERE username = @username;", conn)){
                command.Parameters.AddWithValue("username", $"{username}");
                using(var reader = command.ExecuteReader()){
                    while(reader.Read()){
                        userID = reader.GetInt32(0);
                    }
                }
            }
            return userID;
        }
    }
}