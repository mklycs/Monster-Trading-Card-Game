﻿using System.Security.Cryptography;
using System.Text;

namespace mtcg{
    internal class UserController{
        private static readonly string secretKey = "dont implement secret key like that.";
        public UserController(){}

        private bool validateCredentials(UserDto userDto){
            Validator validator = new Validator();
            string? username = validator.checkStr(3, userDto.username);
            string? password = validator.checkStr(3, userDto.password);

            if(username == null || password == null) 
                return false;

            return true;
        }

        public Status signup(UserDto userDto){
            if(!validateCredentials(userDto)) 
                return new Status(400, "Username or password is invalid.");

            if(userDto.password_repeat != userDto.password) 
                return new Status(400, "Passwords dont match.");

            string token = "-"; // "user is logged out"
            userDto.password = hashPassword(userDto.password);
            UserQueries userQueries = new UserQueries();

            if (userQueries.addUser(userDto, token)) 
                return new Status(201, "Signup succesful.");

            return new Status(403, "User with this username already exists.");
        }

        public Status login(UserDto userDto){
            if(!validateCredentials(userDto)) 
                return new Status(400, "Username or password is invalid.");

            UserQueries userQueries = new UserQueries();

            if(userQueries.isTokenSet(userDto.username)) 
                return new Status(400, "Already logged in.");

            string token = hashPassword(GenerateToken(userDto.username));
            userDto.password = hashPassword(userDto.password);

            if(userQueries.setToken(userDto.username, token)) 
                return new Status(200, "Login succesful.");

            return new Status(400, "Login failed.");
        }

        public Status logout(string token){
            UserQueries userQueries = new UserQueries();

            if(!userQueries.setTokenDefault(token)) 
                return new Status(401, "Failed to log out.");

            return new Status(200, "Logout succesful.");
        }

        public Status changeCredentials(UserDto userDto){
            if(!validateCredentials(userDto)) 
                return new Status(400, "Username or password is invalid.");

            if(userDto.password_repeat != userDto.password) 
                return new Status(400, "Passwords dont match.");

            userDto.password = hashPassword(userDto.password);
            UserQueries userQueries = new UserQueries();

            if(!userQueries.updateUser(userDto)) 
                return new Status(400, "This username is already taken.");

            return new Status(200, "Successfully changed user credentials.");
        }

        public Status deleteUser(UserDto userDto){
            if(!validateCredentials(userDto)) 
                return new Status(400, "Username or password is invalid.");

            if(userDto.password_repeat != userDto.password) 
                return new Status(400, "Passwords dont match.");

            userDto.password = hashPassword(userDto.password);
            UserQueries userQueries = new UserQueries();

            if(!userQueries.deleteUser(userDto)) 
                return new Status(400, "Could not delete user.");

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
            Random random = new Random();
            int num = 0;
            string random_string = "";
            char charac;
            for(int i = 0; i < len; i++){
                num = random.Next(65, 91);
                if(random.Next(0, 2) == 1) 
                    charac = Convert.ToChar(num + 32);
                else 
                    charac = Convert.ToChar(num);
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

        public User getUser(string token){
            UserQueries userQueries = new UserQueries();

            var userInfo = userQueries.getUser(token);
            string username = userInfo.Item1;
            int coins = userInfo.Item2;
            int wins = userInfo.Item3;
            int looses = userInfo.Item4;
            int elo = userInfo.Item5;
            float rating = userInfo.Item6;

            return new User(username, token, coins, wins, looses, elo, rating);
        }

        public bool checkifLoggedIn(string token){
            if(token == "-" || token.Length != 64)
                return false;

            UserQueries userQueries = new UserQueries();
            if(!userQueries.isloggedin(token))
                return false;

            return true;
        }

        public string showScoreboard(){
            UserQueries userQueries = new UserQueries();
            var tupleList = userQueries.getBestPlayers();
            string scoreboard = "";

            for(int i = 0; i < tupleList.Count; i++){
                scoreboard += $"{i + 1}. ";
                scoreboard += tupleList[i].Item1 + ": ";
                scoreboard += $"{tupleList[i].Item2} wins ";
                scoreboard += $"{tupleList[i].Item3} looses ";
                scoreboard += $"{tupleList[i].Item4} elo ";
                scoreboard += $"{tupleList[i].Item5}% winratio\n";
            }

            return scoreboard;
        }
    }
}