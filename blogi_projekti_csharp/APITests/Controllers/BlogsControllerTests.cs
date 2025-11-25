using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers;
using API.Dtos;
using API.Interfaces;
using API.Models;
using APITests.Fakes;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APITests.Controllers
{
    public class BlogsControllerTests
    {
        private readonly IMapper _mapper;

        public BlogsControllerTests()
        {
            // Luodaan controlleria varten oma mapper.
            // Program.cs-filussa tehtiin mapper näin: builder.Services.AddAutoMapper(typeof(Program));
            // yo. koodi luo autom. kaikista Profile-luokista mapit
            // mutta luodaan testiä varten se itse näin:
            var mapperConfig = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Blog, BlogDto>();
            });
            _mapper = mapperConfig.CreateMapper();
        }

        // Statuskoodi 200 OK
        [Fact]
        public async Task GetAll_UsesFakeService_ReturnsOkWithListOfBlogDtos()
        {
            // Testitapauksissa käytetään ilmaisua AAA

            // Arrange = Järjestetään testille oikeat puitteet
            var blogsToReturn = new List<Blog>
            {
                new Blog
                {
                    Id = 1,
                    Title = "Fake Blog 1",
                    Content = "Sisältö 1",
                },
                new Blog
                {
                    Id = 2,
                    Title = "Fake Blog 2",
                    Content = "Sisältö 2",
                },
            };

            // 1. Luodaan instanssi fakeservicestä, joka äsken luotiin
            IBlogService fakeService = new FakeBlogService(blogsToReturn);

            // 2. luodaan controllerista instanssi fakeservicellä ja oikealla mapperilla
            var controller = new BlogsController(fakeService, _mapper);

            // Act = Toimitaan / Suoritetaan testattava metodi
            var result = await controller.GetAll();

            // Assert = Varmistetaan tulos
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var blogDtos = Assert.IsAssignableFrom<IEnumerable<BlogDto>>(okResult.Value);
            Assert.Equal(blogsToReturn.Count, blogDtos.Count());
            Assert.Equal("Fake Blog 1", blogDtos.First().Title);
        }

        // Koska GetAll-metodissa on poikkeuksen käsittely, pitää myös se haara (branch) testata
        // Heitämme tästä test casesta tarkoituksella poikkeuksen

        [Fact]
        public async Task GetAll_UsesFakeService_ThrowsException_ReturnsProblemDetails()
        {
            // Arrange
            var expectedMessage = "Simulated service error.";

            // 1. Luodaan instanssi fakeservicestä, joka äsken luotiin
            IBlogService fakeService = new FakeBlogService([], shouldThrow: true);

            // 2. luodaan controllerista instanssi fakeservicellä ja oikealla mapperilla
            var controller = new BlogsController(fakeService, _mapper);

            // Act
            var result = await controller.GetAll();

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);

            var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
            Assert.Equal(expectedMessage, problemDetails.Detail);
        }

        [Fact]
        public async Task GetById_UsesFakeService_500()
        {
            IBlogService fakeService = new FakeBlogService([], true, false);
            var ctrl = new BlogsController(fakeService, _mapper);

            var result = await ctrl.GetById(1);

            // Assert
            var problemResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, problemResult.StatusCode);

            var problemDetails = Assert.IsType<ProblemDetails>(problemResult.Value);
            Assert.Equal("Generic error", problemDetails.Detail);
        }

        [Fact]
        public async Task GetById_UsesFakeService_404()
        {
            IBlogService fakeService = new FakeBlogService([], false, true);
            var ctrl = new BlogsController(fakeService, _mapper);

            var result = await ctrl.GetById(1);

            // Assert
            var problemResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, problemResult.StatusCode);
        }

        [Fact]
        public async Task GetById_UsesFakeService_200()
        {
            IBlogService fakeService = new FakeBlogService([], false, false);
            var ctrl = new BlogsController(fakeService, _mapper);

            var result = await ctrl.GetById(1);

            // Assert
            var okresult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okresult.StatusCode);

            var blogDto = Assert.IsAssignableFrom<BlogDto>(okresult.Value);

            Assert.Equal("Blog title", blogDto.Title);
            Assert.Equal(1, blogDto.Id);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(3)]
        public async Task GetById_Theory_UsesFakeService_200(int id)
        {
            IBlogService fakeService = new FakeBlogService([], false, false);
            var ctrl = new BlogsController(fakeService, _mapper);

            var result = await ctrl.GetById(id);

            // Assert
            var okresult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okresult.StatusCode);

            var blogDto = Assert.IsAssignableFrom<BlogDto>(okresult.Value);

            Assert.Equal("Blog title", blogDto.Title);
            Assert.Equal(id, blogDto.Id);
        }
    }
}
