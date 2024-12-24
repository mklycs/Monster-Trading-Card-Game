using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using System.Xml.Linq;

namespace mtcg{
    public class RequestHandler{
        private static RequestHandler? instance = null;
        private Random random = new Random();
        //private List<User> users = new List<User>();
        private RequestHandler(){ }

        public static RequestHandler getInstance(){
            if(instance == null) instance = new RequestHandler();
            return instance;
        }
        public string HandleRequest(string request, string httpMethod, string? jsonBody){
            Status response = new Status();

            if(httpMethod == "GET") return HandleGet(jsonBody, request, response);// "function to show scoreboard."; // <- Note to myself
            if(httpMethod == "POST") return HandlePost(jsonBody, request, response);
            if(httpMethod == "DELETE") return HandleDelete(jsonBody, request, response);
            if(httpMethod == "PUT") return HandlePut(jsonBody, request, response);
            if(httpMethod == "PATCH") return HandlePatch(jsonBody, request, response);

            return "0:Unknown request.";
        }

        private string HandleGet(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody)) return "400:No JSON body provided.";

            if(request == "showScoreboard"){
                // ...
            }

            return $"{response.statusCode}:{response.message}";
        }
        private string HandlePost(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody)) return "400:No JSON body provided.";

            if(request == "signup"){
                var userDto = JsonSerializer.Deserialize<UserDto>(jsonBody);
                UserController userController = new UserController();
                response = userController.signup(userDto);
            }

            if(request == "buyPackage"){
                string authToken = getAuthToken(jsonBody);
                CardController cardController = new CardController();
                response = cardController.buyPackage(authToken, random);
            }

            if(request == "battle"){
                // Token von Request extrahieren
                // Anhand des Tokens den richtigen User von der User-Liste bekommen

                // ... 
            }

            return $"{response.statusCode}:{response.message}";
        }

        private string HandleDelete(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody)) return "400:No JSON body provided.";

            var userDto = JsonSerializer.Deserialize<UserDto>(jsonBody);
            UserController userController = new UserController();

            if(request == "deleteUser") response = userController.deleteUser(userDto);

            return $"{response.statusCode}:{response.message}";
        }

        private string HandlePut(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody)) return "400:No JSON body provided.";

            // ...

            return $"{response.statusCode}:{response.message}";
        }

        private string HandlePatch(string? jsonBody, string request, Status response){
            if(string.IsNullOrWhiteSpace(jsonBody)) return "400:No JSON body provided.";

            if(request == "changeCredentials"){
                var userDto = JsonSerializer.Deserialize<UserDto>(jsonBody);
                UserController userController = new UserController();
                response = userController.changeCredentials(userDto);
            }else if(request == "changeStats"){
                string authToken = getAuthToken(jsonBody);
                // ... 

            }else if(request == "logout"){
                string authToken = getAuthToken(jsonBody);
                UserController userController = new UserController();
                response = userController.logout(authToken);
            }else if(request == "login"){
                var userDto = JsonSerializer.Deserialize<UserDto>(jsonBody);
                UserController userController = new UserController();
                response = userController.login(userDto);
            }

            return $"{response.statusCode}:{response.message}";
        }

        // Hilfsfunktion zur Extraktion von Query-Parametern aus der URL
        private string? ExtractQueryParam(string request, string param){
            var uri = new Uri("http://localhost:8080" + request);
            var query = HttpUtility.ParseQueryString(uri.Query);
            return query.Get(param);
        }

        private string getAuthToken(string jsonBody){
            string authToken = "";
            using(JsonDocument document = JsonDocument.Parse(jsonBody)){ // ich erstelle fix nicht noch eine dto nur für 1 string
                JsonElement root = document.RootElement;
                authToken = root.GetProperty("authToken").GetString();
            }
            // Console.WriteLine($"authToken: {authToken}");
            return authToken;
        }
        private void getUserFromList(){
            // ...
        }
    }
}