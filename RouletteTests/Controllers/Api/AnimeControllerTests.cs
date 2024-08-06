using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roulette.Controllers.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Roulette.Services;
using ShikimoriSharp.Classes;
using ShikimoriSharp.Settings;

namespace Roulette.Controllers.Api.Tests
{
    [TestClass()]
    public class AnimeControllerTests
    {
        private ShikimoriApiConnectorService _mockApiConnectorService;
        private ShikiDataService _mockShikiDataService;
        private AnimeController _controller;

        [TestInitialize]
        public void Setup()
        {
            // Настройка
            _mockApiConnectorService = new <ShikimoriApiConnectorService>();
            _mockShikiDataService = new Mock<ShikiDataService>();
            _controller = new AnimeController(_mockApiConnectorService.Object, _mockShikiDataService.Object);
        }

        [TestMethod()]
        public void GetGenresTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetAnimesTest()
        {

            var expectedAnimes = new Anime[] { }; // Предполагаем, что Anime - это класс, представляющий аниме
            _mockApiConnectorService.Setup(service => service.GetAnimes(It.IsAny<AnimeRequestSettings>()))
                .ReturnsAsync(expectedAnimes);

            // Вызов метода
            var result =  _controller.GetAnimes();

            // Проверка
            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = result as OkObjectResult;
            Assert.IsNotNull(okResult);
            Assert.AreEqual(200, okResult.StatusCode);
            Assert.AreEqual(expectedAnimes, okResult.Value);
        }
    }
}