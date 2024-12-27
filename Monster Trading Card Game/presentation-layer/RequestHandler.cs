using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Web;
using System.Xml.Linq;

namespace mtcg{
    public class RequestHandler{
        private static RequestHandler? instance = null;
        private Random random = new Random();
        private List<User> battleQueue = new List<User>();
        private RequestHandler(){ }

        public static RequestHandler getInstance(){
            if(instance == null) 
                instance = new RequestHandler();

            return instance;
        }
        public string HandleRequest(string request, string httpMethod, string? jsonBody){
            Status response = new Status();

            if(httpMethod == "GET") 
                return HandleGet(jsonBody, request, response);

            if(httpMethod == "POST") 
                return HandlePost(jsonBody, request, response);

            if(httpMethod == "DELETE") 
                return HandleDelete(jsonBody, request, response);

            if(httpMethod == "PUT") 
                return HandlePut(jsonBody, request, response);

            if(httpMethod == "PATCH") 
                return HandlePatch(jsonBody, request, response);

            return "0:Unknown request.";
        }

        private string HandleGet(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody)) 
                return "400:No JSON body provided.";

            if(request == "showScoreboard"){
                // ...
            }

            return $"{response.statusCode}:{response.message}";
        }
        private string HandlePost(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody)) 
                return "400:No JSON body provided.";

            if(request == "signup") 
                response = this.signup(jsonBody);

            if(request == "buyPackage")
                response = this.buyPackage(jsonBody);

            if(request == "battle"){
                response = this.battle(jsonBody);
            }else if(request == "stopBattlesearch")
                response = this.stopBattlesearch(jsonBody);
            
            if(request == "offerCard")
                response = this.offerCard(jsonBody);

            return $"{response.statusCode}:{response.message}";
        }

        private string HandleDelete(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody)) 
                return "400:No JSON body provided.";

            if(request == "deleteUser")
                response = this.deleteUser(jsonBody);

            if(request == "deleteOffer")
                response = this.deleteOffer(jsonBody);

            return $"{response.statusCode}:{response.message}";
        }

        private string HandlePut(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody)) 
                return "400:No JSON body provided.";

            // if(request == "foo") 
            // ...

            return $"{response.statusCode}:{response.message}";
        }

        private string HandlePatch(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody)) 
                return "400:No JSON body provided.";

            if(request == "changeCredentials"){
                response = this.changeCredentials(jsonBody);
            }else if(request == "logout"){
                response = this.logout(jsonBody);
            }else if(request == "login")
                response = this.login(jsonBody);

            if(request == "tradeCards")
                response = this.tradeCards(jsonBody);

            return $"{response.statusCode}:{response.message}";
        }

        private Status signup(string jsonBody){
            var userDto = JsonSerializer.Deserialize<UserDto>(jsonBody);
            UserController userController = new UserController();
            return userController.signup(userDto);
        }

        private Status logout(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");
            if(authToken == "-" || authToken.Length != 64)
                return new Status(401, "You need to be logged in.");

            UserController userController = new UserController();
            return userController.logout(authToken);
        }

        private Status login(string jsonBody){
            var userDto = JsonSerializer.Deserialize<UserDto>(jsonBody);
            UserController userController = new UserController();
            return userController.login(userDto);
        }

        private Status deleteUser(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");
            if(authToken == "-" || authToken.Length != 64)
                return new Status(401, "You need to be logged in.");

            var userDto = JsonSerializer.Deserialize<UserDto>(jsonBody);
            UserController userController = new UserController();
            return userController.deleteUser(userDto);
        }

        private Status changeCredentials(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");
            if(authToken == "-" || authToken.Length != 64)
                return new Status(401, "You need to be logged in.");

            var userDto = JsonSerializer.Deserialize<UserDto>(jsonBody);
            UserController userController = new UserController();
            return userController.changeCredentials(userDto);
        }

        private Status buyPackage(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");
            if(authToken == "-" || authToken.Length != 64)
                return new Status(401, "You need to be logged in.");

            CardController cardController = new CardController();
            return cardController.buyPackage(authToken, random);
        }

        private Status battle(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");
            if(authToken == "-" || authToken.Length != 64)
                return new Status(401, "You need to be logged in.");

            if(isinQueue(authToken))
                return new Status(400, "You are already in battlequeue.");

            UserController userController = new UserController();
            User player1 = userController.getUser(authToken);
            player1.searchingBattle = true;
            battleQueue.Add(player1);
            // Console.WriteLine($"Player \"{player1.username}\" has been added to battlequeue.");

            User player2 = findOpponent(player1);
            if(player2 != null){
                // Gegner gefunden - starte den Kampf
                string response = ""; //StartBattle(player1, player2); <>/<>/<>/<>/<>/<>/<>/<>/<>/<>/<>/<>/<>
                return new Status(200, $"{response}"); 
            }

            return new Status(404, "Could not find worthy opponent.");
        }

        private Status offerCard(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");
            if(authToken == "-" || authToken.Length != 64)
                return new Status(401, "You need to be logged in.");

            int offerCardID = int.Parse(getElementFromJson(jsonBody, "offerCardID"));
            int requestCardID = int.Parse(getElementFromJson(jsonBody, "requestCardID"));
            CardController cardController = new CardController();
            return cardController.offerCard(authToken, offerCardID, requestCardID);
        }
        
        private Status deleteOffer(string jsonBody) {
            string authToken = getElementFromJson(jsonBody, "authToken");
            if(authToken == "-" || authToken.Length != 64)
                return new Status(401, "You need to be logged in.");

            int tradeID = int.Parse(getElementFromJson(jsonBody, "tradeID"));
            CardController cardController = new CardController();
            return cardController.deleteOffer(authToken, tradeID);
        }

        private Status tradeCards(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");
            if(authToken == "-" || authToken.Length != 64)
                return new Status(401, "You need to be logged in.");

            int tradeID = int.Parse(getElementFromJson(jsonBody, "tradeID"));
            CardController cardController = new CardController();
            return cardController.tradeCards(authToken, tradeID);
        }

        private User? findOpponent(User player1){
            foreach(var player2 in battleQueue){
                if(player2.token != player1.token && player2.searchingBattle){
                    battleQueue.Remove(player1);
                    battleQueue.Remove(player2);
                    return player2;
                }
            }
            return null;
        }

        private Status stopBattlesearch(string jsonBody){
            string authToken = getElementFromJson(jsonBody, "authToken");
            if(authToken == "-" || authToken.Length != 64)
                return new Status(401, "You need to be logged in.");

            if(!isinQueue(authToken))
                return new Status(400, "Not in battlequeue.");

            removePlayerFromBattlequeue(authToken);
            return new Status(200, "Stopped searching for player to battle.");
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

        private bool isinQueue(string token){
            for(int i = 0; i < battleQueue.Count; i++){
                if(token == battleQueue[i].token && battleQueue[i].searchingBattle == true)
                    return true;
            }
            return false;
        }

        private void removePlayerFromBattlequeue(string token){
            for(int i = 0; i < battleQueue.Count; i++) {
                if(token == battleQueue[i].token && battleQueue[i].searchingBattle == true) {
                    battleQueue.Remove(battleQueue[i]);
                    break;
                }
            }
        }
    }
}