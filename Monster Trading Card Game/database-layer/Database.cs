using System;
using System.Data;
using Npgsql;

namespace mtcg{
    internal class Database{
        protected Database(){ }

        public NpgsqlConnection getConn(){
            return new NpgsqlConnection("Host=localhost;Username=mtcg_admin;Password=admin;Database=mtcg_database");
        }

        public void CloseConnection(NpgsqlConnection conn){
            if(conn != null && conn.State == System.Data.ConnectionState.Open){
                conn.Close();
                // Console.WriteLine("Connection closed.");
            }
        }
        //*/
    }
}