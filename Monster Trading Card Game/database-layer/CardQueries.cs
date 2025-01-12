using System.Security.Principal;
using Npgsql;

namespace mtcg{
    internal class CardQueries : Database{
        private NpgsqlConnection? conn = null;
        public CardQueries(){
            this.conn = this.getConn();
            conn.Open();
        }

        public CardQueries(NpgsqlConnection conn){
            this.conn = conn;

            if(this.conn.State != System.Data.ConnectionState.Open)
                conn.Open();
        }

        public int getUserID(string token){
            int userID = 0;
            using(var command = new NpgsqlCommand("SELECT id FROM \"USERS\" WHERE token = @token;", conn)){
                command.Parameters.AddWithValue("token", $"{token}");
                using(var reader = command.ExecuteReader()){
                    while(reader.Read())
                        userID = reader.GetInt32(0);
                }
            }
            return userID;
        }

        public int getCoins(int userID){
            int coins = 0;
            using(var command = new NpgsqlCommand("SELECT coins FROM \"STATS\" WHERE id = @userid;", conn)){
                command.Parameters.AddWithValue("userid", userID);
                using(var reader = command.ExecuteReader()){
                    while(reader.Read())
                        coins = reader.GetInt32(0);
                }
            }
            return coins;
        }

        public bool setCoins(int userID, int coins){
            using(var command = new NpgsqlCommand("UPDATE \"STATS\" SET coins = @coins WHERE id = @userid;", conn)){
                command.Parameters.AddWithValue("coins", coins);
                command.Parameters.AddWithValue("userid", userID);
                int rowsAffected = command.ExecuteNonQuery();

                if(rowsAffected == 0)
                    return false;
            }
            return true;
        }

        public bool addCard(int cardID, int userID){
            using(var command = new NpgsqlCommand("INSERT INTO \"STACKS\" (userid, cardid) VALUES (@userid, @cardid);", conn)){
                command.Parameters.AddWithValue("userid", userID);
                command.Parameters.AddWithValue("cardid", cardID);
                int rowsAffected = command.ExecuteNonQuery();

                if(rowsAffected == 0)
                    return false;
            }
            return true;
        }

        public bool removeCard(int cardID, int userID){
            int stackID = -1;
            using(var command = new NpgsqlCommand("SELECT id FROM \"STACKS\" WHERE userid = @userid AND cardid = @cardid;", conn)) {
                command.Parameters.AddWithValue("userid", userID);
                command.Parameters.AddWithValue("cardid", cardID);
                using(var reader = command.ExecuteReader()){
                    while(reader.Read())
                        stackID = reader.GetInt32(0);
                }
            }

            using(var command = new NpgsqlCommand("DELETE FROM \"STACKS\" WHERE id = @id;", conn)) {
                command.Parameters.AddWithValue("id", stackID);
                int rowsAffected = command.ExecuteNonQuery();

                if(rowsAffected == 0)
                    return false;
            }
            return true;
        }

        public bool checkifCardInStack(int cardID, int userID){
            int stackCount = 0;
            using(var command = new NpgsqlCommand("SELECT COUNT(cardid) FROM \"STACKS\" WHERE userid = @userID AND cardid = @cardID;", conn)){
                command.Parameters.AddWithValue("userID", userID);
                command.Parameters.AddWithValue("cardID", cardID);
                using(var reader = command.ExecuteReader()){
                    while(reader.Read())
                        stackCount = reader.GetInt32(0);
                }
            }

            if(stackCount < 1)
                return false;

            return true;
        }

        public bool addOffer(int userID, int offerCardID, int requestCardID){
            try{
                using(var command = new NpgsqlCommand("INSERT INTO \"TRADEOFFERS\" (userid, offercardid, requestcardid) VALUES (@userID, @offerCardID, @requestCardID);", conn)){
                    command.Parameters.AddWithValue("userID", userID);
                    command.Parameters.AddWithValue("offerCardID", offerCardID);
                    command.Parameters.AddWithValue("requestCardID", requestCardID);
                    int rowsAffected = command.ExecuteNonQuery();
                    
                    if(rowsAffected == 0)
                        return false;
                }
            }catch(Exception exception){
                return false;
            }
            
            return true;
        }

        public bool deleteOffer(int tradeID, int userID){
            using(var command = new NpgsqlCommand("DELETE FROM \"TRADEOFFERS\" WHERE id = @tradeID AND userid = @userID;", conn)){
                command.Parameters.AddWithValue("tradeID", tradeID);
                command.Parameters.AddWithValue("userID", userID);
                int rowsAffected = command.ExecuteNonQuery();

                if(rowsAffected == 0)
                    return false;
            }
            return true;
        }

        public (int, int, int) getTradeInfo(int tradeID){
            int userID = 0, offercardID = 0, requestcardID = 0;
            using(var command = new NpgsqlCommand("SELECT userid, offercardid, requestcardid FROM \"TRADEOFFERS\" WHERE id = @tradeID;", conn)){
                command.Parameters.AddWithValue("tradeID", tradeID);
                using(var reader = command.ExecuteReader()){
                    while(reader.Read()){
                        userID = reader.GetInt32(0);
                        offercardID = reader.GetInt32(1);
                        requestcardID = reader.GetInt32(2);
                    }
                }
            }
            
            return (userID, offercardID, requestcardID);
        }
    }
}