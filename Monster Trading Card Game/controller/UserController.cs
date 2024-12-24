// nimmt Anfragen zur Registrierung, Login und anderen Benutzeraktionen entgegen

using System.Security.Cryptography;
using System.Text;
using Npgsql;
using Npgsql.Replication;
//using Npgsql.Replication;

namespace mtcg{
    internal class UserController{
        private static readonly string secretKey = "dont implement secret key like that.";
        //private UserDto? userDto;
        public UserController(/*UserDto userDto*/){
            /*
            this.userDto = userDto;
            this.userDto.password = this.hashPassword(userDto.password);
            this.userDto.password_repeat = this.hashPassword(userDto.password_repeat);
            //*/
        }

        private bool validateCredentials(UserDto userDto){
            Validator validator = new Validator();
            string? username = validator.checkStr(3, userDto.username);
            string? password = validator.checkStr(3, userDto.password);
            if(username == null || password == null) return false;

            return true;
        }

        public Status signup(UserDto userDto){
            if(!validateCredentials(userDto)) return new Status(400, "Username or password is invalid.");
            if(userDto.password_repeat != userDto.password) return new Status(400, "Passwords dont match.");

            string token = "-"; // "user is logged out"
            userDto.password = hashPassword(userDto.password);
            UserQueries userQueries = new UserQueries();
            if (userQueries.addUser(userDto, token)) return new Status(201, "Signup succesful.");

            return new Status(403, "User with this username already exists.");
        }

        public Status login(UserDto userDto){
            if(!validateCredentials(userDto)) return new Status(400, "Username or password is invalid.");
            UserQueries userQueries = new UserQueries();
            if(userQueries.isTokenSet(userDto.username)) return new Status(400, "Already logged in.");

            string token = hashPassword(GenerateToken(userDto.username));
            userDto.password = hashPassword(userDto.password);
            if(userQueries.setToken(userDto.username, token)) return new Status(200, "Login succesful.");
            return new Status(500, "Login failed.");
        }

        public Status logout(string token){
            UserQueries userQueries = new UserQueries();
            if(!userQueries.setTokenDefault(token)) return new Status(401, "Failed to log out.");

            return new Status(200, "Logout succesful.");
        }

        public Status changeCredentials(UserDto userDto){
            if(!validateCredentials(userDto)) return new Status(400, "Username or password is invalid.");
            if(userDto.password_repeat != userDto.password) return new Status(400, "Passwords dont match.");

            userDto.password = hashPassword(userDto.password);
            UserQueries userQueries = new UserQueries();
            if(!userQueries.updateUser(userDto)) return new Status(400, "This username is already taken.");

            return new Status(200, "Successfully changed user credentials.");
        }

        public Status deleteUser(UserDto userDto){
            if(!validateCredentials(userDto)) return new Status(400, "Username or password is invalid.");
            if(userDto.password_repeat != userDto.password) return new Status(400, "Passwords dont match.");

            userDto.password = hashPassword(userDto.password);
            UserQueries userQueries = new UserQueries();
            if(!userQueries.deleteUser(userDto)) return new Status(400, "Could not delete user: User does not exist.");

            return new Status(200, $"Successfully deleted user \"{userDto.username}\".");
        }

        public string hashPassword(string? password){
            using(var sha256 = SHA256.Create()){
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        private string GenerateToken(string username){
            string sessionId = username; //generateRandomString(32);
            string timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
            string signature = CreateSignature(sessionId, timestamp);
            return $"{sessionId}.{timestamp}.{signature}";
        }

        private static string generateRandomString(int len){
            Random random = new Random();                                     // sollte nicht hier sein, aber fürs erste reicht das aus, wenns hier ist
            int num = 0;
            string random_string = "";
            char charac;
            for(int i = 0; i < len; i++){
                num = random.Next(65, 91);                                    // ASCII Kleinbuchstaben fangen bei 65 und Großbuchstaben fangen bei 97 an 
                if(random.Next(0, 2) == 1) charac = Convert.ToChar(num + 32); // für upper case
                else charac = Convert.ToChar(num);                            // für lower case
                random_string += charac;
            }
            return random_string;
        }

        private static string CreateSignature(string sessionId, string timestamp){
            string data = $"{sessionId}.{timestamp}";
            using(var HMAC = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey))){
                byte[] hash = HMAC.ComputeHash(Encoding.UTF8.GetBytes(data));
                return Convert.ToBase64String(hash);
            }
        }
        /*
        public string getToken(string username){
            UserQueries userQueries = new UserQueries();
            return userQueries.getToken(username);
        }
        //*/

        public void getUser(){
            // ...
        }
    }
}