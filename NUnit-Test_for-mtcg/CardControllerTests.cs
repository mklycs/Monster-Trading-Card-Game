using Moq;
using NUnit.Framework;
using mtcg;
using System;

namespace mtcg.Tests{
    [TestFixture]
    public class CardControllerTests{
        private CardController _cardController;

        [SetUp]
        public void Setup(){
            _cardController = new CardController();
        }

        [Test]
        public void BuyPackage_ValidPurchase_ReturnsSuccess(){
            // Arrange
            var token = "8d8b2466a924b733ab8a0510c79575a4ab5e4f83410d6a7108d056303882bf4e"; // token anpassen sonst funktioniert nicht, wenn der user nicht logged in ist
            var random = new Random();

            // Act
            var result = _cardController.buyPackage(token, random);

            // Assert
            Assert.AreNotEqual("Not enough coins.", result.message);  
            Assert.AreNotEqual("You need to be logged in.", result.message);
            Assert.AreNotEqual("Something went wrong with coin payment.", result.message);
            Assert.AreEqual(200, result.statusCode);
        }

        [Test]
        public void BuyPackage_NotEnoughCoins_ReturnsBadRequest(){
            // Arrange
            var token = "3371b25c92e6b4f08527db067c3912a88445b5da43015108841a721566c14406"; // token von irgendeinem nehmen, der keine coins hat
            var random = new Random();

            // Act
            var result = _cardController.buyPackage(token, random);

            // Assert
            Assert.AreEqual(400, result.statusCode);
            Assert.AreEqual("Not enough coins.", result.message);
        }

        [Test]
        public void OfferCard_ValidOffer_ReturnsSuccess(){
            // Arrange
            var token = "8d8b2466a924b733ab8a0510c79575a4ab5e4f83410d6a7108d056303882bf4e"; // token anpassen sonst funktioniert nicht 
            var offerCardID = 1;
            var requestCardID = 2;

            // Act
            var result = _cardController.offerCard(token, offerCardID, requestCardID);

            // Assert
            Assert.AreEqual(200, result.statusCode);
            Assert.AreEqual("Successfully added offer.", result.message);
        }

        [Test]
        public void OfferCard_CardNotInStack_ReturnsNotFound(){
            // Arrange
            var token = "8d8b2466a924b733ab8a0510c79575a4ab5e4f83410d6a7108d056303882bf4e"; // token anpassen sonst funktioniert nicht 
            var offerCardID = 91919191;
            var requestCardID = 2;

            // Act
            var result = _cardController.offerCard(token, offerCardID, requestCardID);

            // Assert
            Assert.AreEqual(404, result.statusCode);
            Assert.AreEqual("The card to offer could not be found in stack.", result.message);
        }

        [Test]
        public void DeleteOffer_ValidTrade_ReturnsSuccess(){
            // Arrange
            var token = "8d8b2466a924b733ab8a0510c79575a4ab5e4f83410d6a7108d056303882bf4e";
            int tradeID = 2;  // Trade-ID, die es geben sollte

            // Act
            var result = _cardController.deleteOffer(token, tradeID);

            // Assert
            Assert.AreEqual(200, result.statusCode);
            Assert.AreEqual("Successfully deleted offer.", result.message);
        }

        [Test]
        public void DeleteOffer_OfferNotFound_ReturnsNotFound(){
            // Arrange
            var token = "8d8b2466a924b733ab8a0510c79575a4ab5e4f83410d6a7108d056303882bf4e";
            int tradeID = 9999;  // Ein Trade, der nicht existiert

            // Act
            var result = _cardController.deleteOffer(token, tradeID);

            // Assert
            Assert.AreEqual(404, result.statusCode);
            Assert.AreEqual("Could not find trade offer.", result.message);
        }

        [Test]
        public void TradeCards_ValidTrade_ReturnsSuccess(){
            // Arrange
            var token = "8d8b2466a924b733ab8a0510c79575a4ab5e4f83410d6a7108d056303882bf4e";
            int tradeID = 1;  // eine Trade-ID, die es geben sollte

            // Act
            var result = _cardController.tradeCards(token, tradeID);

            // Assert
            Assert.AreEqual(200, result.statusCode);
            Assert.AreEqual("Successfully traded cards.", result.message);
        }

        [Test]
        public void TradeCards_TradeNotFound_ReturnsNotFound(){
            // Arrange
            var token = "8d8b2466a924b733ab8a0510c79575a4ab5e4f83410d6a7108d056303882bf4e";
            int tradeID = 9999;  // eine Trade-ID, die es nicht geben sollte

            // Act
            var result = _cardController.tradeCards(token, tradeID);

            // Assert
            Assert.AreEqual(404, result.statusCode);
            Assert.AreEqual("Could not find trade offer.", result.message);
        }

        [Test]
        public void TradeCards_CannotTradeOwnCards_ReturnsBadRequest(){
            // Arrange
            var token = "8d8b2466a924b733ab8a0510c79575a4ab5e4f83410d6a7108d056303882bf4e";
            int tradeID = 2;  // sollte eine gültige Trade-ID sein, wo der Benutzer  auch der Anbieter ist

            // Act
            var result = _cardController.tradeCards(token, tradeID);

            // Assert
            Assert.AreEqual(400, result.statusCode);
            Assert.AreEqual("You cannot accept your own offer.", result.message);
        }

        [Test]
        public void Gamble_ValidBet_LoosesCoins_ReturnsSuccess(){ // Test kann manchmal, weil der user vllt gamble gewinnt
            // Arrange
            var token = "8d8b2466a924b733ab8a0510c79575a4ab5e4f83410d6a7108d056303882bf4e";
            var random = new Random();
            int coinsToGamble = 20;
            int headOrTails = 0;  // Kopf (1) oder Zahl (0)

            // Act
            var result = _cardController.gamble(token, random, coinsToGamble, headOrTails);

            // Assert
            Assert.AreEqual(200, result.statusCode);
            Assert.IsTrue(result.message.Contains("You loose"));
        }

        [Test]
        public void Gamble_NotEnoughCoins_ReturnsBadRequest(){
            // Arrange
            var token = "8d8b2466a924b733ab8a0510c79575a4ab5e4f83410d6a7108d056303882bf4e";
            var random = new Random();
            int coinsToGamble = -1234; // man soll ja nur Gewinn machen wollen ;)
            int headOrTails = 0;

            // Act
            var result = _cardController.gamble(token, random, coinsToGamble, headOrTails);

            // Assert
            Assert.AreEqual(400, result.statusCode);
            Assert.AreEqual("Invalid coins amount.", result.message);
        }

        [Test]
        public void Gamble_InvalidBet_ReturnsBadRequest(){
            // Arrange
            var token = "8d8b2466a924b733ab8a0510c79575a4ab5e4f83410d6a7108d056303882bf4e";
            var random = new Random();
            int coinsToGamble = 50;
            int headOrTails = 3; // ungültiger Wert

            // Act
            var result = _cardController.gamble(token, random, coinsToGamble, headOrTails);

            // Assert
            Assert.AreEqual(400, result.statusCode);
            Assert.AreEqual("Invalid bet.", result.message);
        }
    }
}
