using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

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

        public string httpVersion { get; set; }
        public NetworkStream stream { get; set; }
        public TcpClient client { get; set; }
        public bool searchingBattle { get; set; }

        public User(string? username, /*string? password,*/ string token, int coins = 20, int wins = 0, int looses = 0, int elo = 100, int rating = 0, bool searchingBattle = false){
            this.username = username;
            //this.password = password;
            this.coins = coins;       //  20
            this.wins = wins;         //   0
            this.looses = looses;     //   0
            this.elo = elo;           // 100
            this.rating = rating;     //   0
            this.token = token;
            this.searchingBattle = searchingBattle;

            this.stack = new List<A_Card>();

            this.deck = new List<A_Card>(4);
        }

        public void sendServerBattleResponse(string response){
            byte[] responseBuffer = Encoding.UTF8.GetBytes($"{httpVersion} 200 OK\r\nContent-Length: {response.Length}\r\n\r\n{response}");
            stream.Write(responseBuffer, 0, responseBuffer.Length);
            stream.Close();
            client.Close();

            /*
            if(stream != null && stream.CanWrite) {
                byte[] responseBuffer = Encoding.UTF8.GetBytes($"{httpVersion} 200 OK\r\nContent-Length: {response.Length}\r\n\r\n{response}");
                stream.Write(responseBuffer, 0, responseBuffer.Length);
                stream.Close();
            } else {
                // Fehlerbehandlung, z.B. Loggen, Stream schließen, oder erneutes Öffnen des Streams
                Console.WriteLine("Stream ist nicht verfügbar oder wurde bereits geschlossen.");
            }
            */
        }

        public void closeConnectionToServer(){
            byte[] responseBuffer = Encoding.UTF8.GetBytes($"{httpVersion} 200 OK\r\nContent-Length: {"200 Stopped searching for player to battle.".Length}\r\n\r\nStopped searching for player to battle.");
            stream.Write(responseBuffer, 0, responseBuffer.Length);
            stream.Close();
        }
    }
}