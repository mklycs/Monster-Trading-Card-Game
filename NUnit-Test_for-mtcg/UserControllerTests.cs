using Moq;
using NUnit.Framework;
using mtcg.Tests;
using System;

namespace mtcg.Tests{
    [TestFixture]
    public class UserControllerTests{
        private UserController _userController;

        [SetUp]
        public void Setup(){
            _userController = new UserController();
        }

        [Test]
        public void Signup_ValidCredentials_ReturnsSuccess(){
            // Arrange
            var userDto = new UserDto("validUser", "ValidPassword123", null, "ValidPassword123");

            // Act
            var result = _userController.signup(userDto);

            // Assert
            Assert.AreEqual(201, result.statusCode);
            Assert.AreEqual("Signup succesful.", result.message);
        }

        [Test]
        public void Signup_InvalidCredentials_ReturnsBadRequest(){
            // Arrange
            var userDto = new UserDto("us", "pw", null, "pw");

            // Act
            var result = _userController.signup(userDto);

            // Assert
            Assert.AreEqual(400, result.statusCode);
            Assert.AreEqual("Username or password is invalid.", result.message);
        }

        [Test]
        public void Signup_PasswordsDoNotMatch_ReturnsBadRequest(){
            // Arrange
            var userDto = new UserDto("validUser", "Password123", null, "DifferentPassword123");

            // Act
            var result = _userController.signup(userDto);

            // Assert
            Assert.AreEqual(400, result.statusCode);
            Assert.AreEqual("Passwords dont match.", result.message);
        }

        [Test]
        public void Login_ValidCredentials_ReturnsSuccess(){
            // Arrange
            var userDto = new UserDto("validUser", "ValidPassword123");
            
            // Act
            var result = _userController.login(userDto);

            // Assert
            Assert.AreEqual(200, result.statusCode);
            Assert.AreEqual("Login succesful.", result.message);
        }

        [Test]
        public void Login_InvalidCredentials_ReturnsBadRequest(){
            // Arrange
            var userDto = new UserDto("invalidUser", "invalidPassword");

            // Act
            var result = _userController.login(userDto);

            // Assert
            Assert.AreEqual(400, result.statusCode);
            Assert.AreEqual("Login failed.", result.message);
        }

        [Test]
        public void Logout_ValidToken_ReturnsSuccess(){
            // Arrange
            string token = "71bae710636fdf2c42750622f2bd30e2bb6a25065910b4a1480b900d52972687"; // sollte der passende token zum eingeloggten User sein

            // Act
            var result = _userController.logout(token);

            // Assert
            Assert.AreEqual(200, result.statusCode);
            Assert.AreEqual("Logout succesful.", result.message);
        }

        [Test]
        public void Logout_InvalidToken_ReturnsUnauthorized(){
            // Arrange
            string token = "invalidToken";

            // Act
            var result = _userController.logout(token);

            // Assert
            Assert.AreEqual(401, result.statusCode);
            Assert.AreEqual("Failed to log out.", result.message);
        }

        [Test]
        public void DeleteUser_Success_ReturnsSuccess(){
            // Arrange
            string username = "validUser";
            var userDto = new UserDto(username, "ValidPassword123", null, "ValidPassword123");

            // Act
            var result = _userController.deleteUser(userDto);

            // Assert
            Assert.AreEqual(200, result.statusCode);
            Assert.AreEqual($"Successfully deleted user \"{username}\".", result.message);
        }

        [Test]
        public void DeleteUser_InvalidCredentials_ReturnsBadRequest(){
            // Arrange
            var userDto = new UserDto("us", "pw", null, "pw");

            // Act
            var result = _userController.deleteUser(userDto);

            // Assert
            Assert.AreEqual(400, result.statusCode);
            Assert.AreEqual("Username or password is invalid.", result.message);
        }
    }
}
