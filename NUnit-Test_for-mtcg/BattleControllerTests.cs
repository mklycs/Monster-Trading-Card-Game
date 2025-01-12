using NUnit.Framework;
using Moq;
using System;
using mtcg;
using System.Drawing;

namespace mtcg.Tests{
    [TestFixture]
    public class BattleControllerTests{
        private BattleController _battleController;
        private User _mockUser = new User("testUser", null, 20, 5, 3, 50, 60); // coins, wins, looses, elo, rating

        [SetUp]
        public void Setup(){
            _battleController = new BattleController();
        }

        [Test]
        public void ProcessWin_UpdatesUserStats_Correctly(){
            // Act
            _battleController.processWin(_mockUser);

            // Assert
            Assert.AreEqual(6, _mockUser.wins); 
            Assert.AreEqual(53, _mockUser.elo); 
            Assert.AreEqual(24, _mockUser.coins); 
            Assert.AreEqual((float)6 / (6 + 3) * 100, _mockUser.rating); 
        }

        [Test]
        public void ProcessLoose_UpdatesUserStats_Correctly(){
            // Act
            _battleController.processLoose(_mockUser);

            // Assert
            Assert.AreEqual(4, _mockUser.looses); // Looses should increment by 1
            Assert.AreEqual(45, _mockUser.elo); // Elo should decrease by 5
            Assert.AreEqual((float)5 / (5 + 4) * 100, _mockUser.rating); // Rating should be recalculated
        }

        [Test]
        public void ProcessDraw_UpdatesUserStats_Correctly(){
            // Act
            _battleController.processDraw(_mockUser);

            // Assert
            Assert.AreEqual(51, _mockUser.elo); // Elo should increase by 1
            Assert.AreEqual(21, _mockUser.coins); // Coins should increase by 1
        }
    }
}