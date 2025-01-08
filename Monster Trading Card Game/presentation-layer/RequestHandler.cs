using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Web;

namespace mtcg{
    public class RequestHandler{
        private Random random = new Random();
        private List<User> battleQueue = new List<User>();
        private static readonly object battleQueueLock = new object();

        private static RequestHandler? instance = null;
        private RequestHandler(){ }

        public static RequestHandler getInstance(){
            if(instance == null)
                instance = new RequestHandler();

            return instance;
        }

        public string handleResponseStatus(string statusCode){
            if(statusCode == "100") return "Continue";
            else if(statusCode == "101") return "Switching Protocols";
            else if(statusCode == "200") return "OK";
            else if(statusCode == "201") return "Created";
            else if(statusCode == "202") return "Accepted";
            else if(statusCode == "204") return "No Content";
            else if(statusCode == "301") return "Moved Permanently";
            else if(statusCode == "302") return "Found";
            else if(statusCode == "304") return "No Modified";
            else if(statusCode == "400") return "Bad Request";
            else if(statusCode == "401") return "Unauthorized";
            else if(statusCode == "403") return "Forbidden";
            else if(statusCode == "404") return "Not Found";
            else if(statusCode == "500") return "Internal Server Error";
            else if(statusCode == "502") return "Bad Gateway";
            else if(statusCode == "503") return "Service Unavailable";
            else return "Unknown Request";
        }

        public void handleRequest(string request, string httpMethod, string? jsonBody, string httpVersion, TcpClient client, NetworkStream? stream){
            Status status = new Status();
            string response = "0~Unknown request.";

            if(httpMethod == "GET")
                response = handleGet(request, status);

            if(httpMethod == "POST"){
                response = handlePost(jsonBody, request, status, httpVersion, client, stream);
                if(response == null)
                    return;
            }

            if(httpMethod == "DELETE")
                response = handleDelete(jsonBody, request, status);

            if(httpMethod == "PUT")
                response = handlePut(jsonBody, request, status);

            if(httpMethod == "PATCH")
                response = handlePatch(jsonBody, request, status);

            byte[] responseBuffer = Encoding.UTF8.GetBytes($"{httpVersion} {response.Split('~')[0]} {handleResponseStatus(response.Split('~')[0])}\r\nContent-Length: {response.Length}\r\n\r\n{response.Split('~')[1]}");
            Console.WriteLine(responseBuffer.Length);
            stream.Write(responseBuffer, 0, responseBuffer.Length); // Startet bei der Position 0 des Buffers und sendet die volle Länge des Buffers.
            stream.Close();
            client.Close();
        }

        private string handleGet(string request, Status response){
            if(request == "showScoreboard")
                response = this.showScoreboard();

            return $"{response.statusCode}~{response.message}";
        }
        private string handlePost(string? jsonBody, string request, Status response, string httpVersion, TcpClient client, NetworkStream? stream){
            if(string.IsNullOrWhiteSpace(jsonBody))
                return "400~No JSON body provided.";

            if(request == "signup")
                response = this.signup(jsonBody);

            if(request == "buyPackage")
                response = this.buyPackage(jsonBody);

            if(request == "gamble")
                response = this.gamble(jsonBody);

            if(request == "battle"){
                response = this.battle(jsonBody, httpVersion, client, stream);
                if(response == null)
                    return null;
            }else if(request == "stopBattlesearch"){
                response = this.stopBattlesearch(jsonBody);
                if(response == null)
                    return null;
            }

            if(request == "offerCard")
                response = this.offerCard(jsonBody);

            return $"{response.statusCode}~{response.message}";
        }

        private string handleDelete(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody))
                return "400~No JSON body provided.";

            if(request == "deleteUser")
                response = this.deleteUser(jsonBody);

            if(request == "deleteOffer")
                response = this.deleteOffer(jsonBody);

            return $"{response.statusCode}~{response.message}";
        }

        private string handlePut(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody))
                return "400~No JSON body provided.";

            // if(request == "foo") 
            // ...

            return $"{response.statusCode}~{response.message}";
        }

        private string handlePatch(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody))
                return "400~No JSON body provided.";

            if(request == "changeCredentials"){
                response = this.changeCredentials(jsonBody);
            }else if(request == "logout"){
                response = this.logout(jsonBody);
            }else if(request == "login")
                response = this.login(jsonBody);

            if(request == "tradeCards")
                response = this.tradeCards(jsonBody);

            return $"{response.statusCode}~{response.message}";
        }

        private Status signup(string jsonBody){
            var userDto = JsonSerializer.Deserialize<UserDto>(jsonBody);
            UserController userController = new UserController();
            return userController.signup(userDto);
        }

        private Status logout(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");
            UserController userController = new UserController();
            if(!userController.checkifLoggedIn(authToken))
                return new Status(401, "You need to be logged in.");

            return userController.logout(authToken);
        }

        private Status login(string jsonBody){
            var userDto = JsonSerializer.Deserialize<UserDto>(jsonBody);
            UserController userController = new UserController();
            return userController.login(userDto);
        }

        private Status deleteUser(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");
            UserController userController = new UserController();
            if(!userController.checkifLoggedIn(authToken))
                return new Status(401, "You need to be logged in.");

            var userDto = JsonSerializer.Deserialize<UserDto>(jsonBody);
            return userController.deleteUser(userDto);
        }

        private Status changeCredentials(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");
            UserController userController = new UserController();
            if(!userController.checkifLoggedIn(authToken))
                return new Status(401, "You need to be logged in.");

            var userDto = JsonSerializer.Deserialize<UserDto>(jsonBody);
            return userController.changeCredentials(userDto);
        }

        private Status buyPackage(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");

            {
                UserController userController = new UserController();
                if(!userController.checkifLoggedIn(authToken))
                    return new Status(401, "You need to be logged in.");
            }

            CardController cardController = new CardController();
            return cardController.buyPackage(authToken, random);
        }

        private Status offerCard(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");

            {
                UserController userController = new UserController();
                if(!userController.checkifLoggedIn(authToken))
                    return new Status(401, "You need to be logged in.");
            }

            int offerCardID = int.Parse(getElementFromJson(jsonBody, "offerCardID"));
            int requestCardID = int.Parse(getElementFromJson(jsonBody, "requestCardID"));
            CardController cardController = new CardController();
            return cardController.offerCard(authToken, offerCardID, requestCardID);
        }

        private Status deleteOffer(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");

            {
                UserController userController = new UserController();
                if(!userController.checkifLoggedIn(authToken))
                    return new Status(401, "You need to be logged in.");
            }

            int tradeID = int.Parse(getElementFromJson(jsonBody, "tradeID"));
            CardController cardController = new CardController();
            return cardController.deleteOffer(authToken, tradeID);
        }

        private Status tradeCards(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");

            {
                UserController userController = new UserController();
                if(!userController.checkifLoggedIn(authToken))
                    return new Status(401, "You need to be logged in.");
            }

            int tradeID = int.Parse(getElementFromJson(jsonBody, "tradeID"));
            CardController cardController = new CardController();
            return cardController.tradeCards(authToken, tradeID);
        }

        private Status gamble(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");

            {
                UserController userController = new UserController();
                if(!userController.checkifLoggedIn(authToken))
                    return new Status(401, "You need to be logged in.");
            }

            int coins = int.Parse(getElementFromJson(jsonBody, "coins"));
            int headortails = int.Parse(getElementFromJson(jsonBody, "headortails"));

            CardController cardController = new CardController();
            return cardController.gamble(authToken, random, coins, headortails);
        }

        private Status battle(string jsonBody, string httpVersion, TcpClient client, NetworkStream? stream){
            string authToken = getElementFromJson(jsonBody, "authToken");
            UserController userController = new UserController();
            if(!userController.checkifLoggedIn(authToken))
                return new Status(401, "You need to be logged in.");

            if(isinQueue(authToken))
                return new Status(400, "You are already in battlequeue.");

            User player1 = userController.getUser(authToken);
            DeckDto deckDto = JsonSerializer.Deserialize<DeckDto>(jsonBody);
            CardController cardcontroller = new CardController();

            if(!cardcontroller.getDeck(authToken, deckDto, player1))
                return new Status(400, "Please define a deck before battling.");

            player1.searchingBattle = true;
            player1.client = client;
            player1.stream = stream;
            player1.httpVersion = httpVersion;

            lock(battleQueueLock){
                battleQueue.Add(player1);
            }

            Console.WriteLine($"Added player \"{player1.username}\" to battlequeue.");

            User player2 = findOpponent(player1);
            if(player2 != null){
                BattleController battleController = new BattleController();
                battleController.battle(player1, player2, random);
                return null;
            }
            string response = "200~Still searching...";
            byte[] responseBuffer = Encoding.UTF8.GetBytes($"{httpVersion} {"200"} {"OK"}\r\nContent-Length: {"Still searching...".Length}\r\n\r\n{"Still searching..."}");
            stream.Write(responseBuffer, 0, responseBuffer.Length);
            return null; /* new Status(202, "Still searching..."); */
        }

        private User? findOpponent(User player1){
            lock(battleQueueLock){
                foreach(var player2 in battleQueue){
                    if(player2.token != player1.token && player2.searchingBattle){
                        battleQueue.Remove(player1);
                        battleQueue.Remove(player2);
                        return player2;
                    }
                }
            }

            return null;
        }

        private Status stopBattlesearch(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");

            {
                UserController userController = new UserController();
                if(!userController.checkifLoggedIn(authToken))
                    return new Status(401, "You need to be logged in.");
            }

            if(!isinQueue(authToken))
                return new Status(400, "Not in battlequeue.");

            removePlayerFromBattlequeue(authToken);
            return new Status(200, "Stopped searching for player to battle.");
        }

        private bool isinQueue(string token){
            for(int i = 0; i < battleQueue.Count; i++){
                if(token == battleQueue[i].token && battleQueue[i].searchingBattle == true)
                    return true;
            }
            return false;
        }

        private void removePlayerFromBattlequeue(string token){
            lock(battleQueueLock){
                for(int i = 0; i < battleQueue.Count; i++){
                    if(token == battleQueue[i].token && battleQueue[i].searchingBattle == true){
                        battleQueue[i].closeConnectionToServer();
                        battleQueue.Remove(battleQueue[i]);
                        break;
                    }
                }
            }
        }

        private Status showScoreboard(){
            UserController userController = new UserController();
            string scoreboard = userController.showScoreboard();
            return new Status(200, scoreboard);
        }

        private string? ExtractQueryParam(string request, string param){
            var uri = new Uri("http://localhost:8080" + request);
            var query = HttpUtility.ParseQueryString(uri.Query);
            return query.Get(param);
        }

        private string getElementFromJson(string jsonBody, string element){
            string str = "-";
            using(JsonDocument document = JsonDocument.Parse(jsonBody)){
                JsonElement root = document.RootElement;
                str = root.GetProperty($"{element}").GetString();
            }
            return str;
        }
    }
}