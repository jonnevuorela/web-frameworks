using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.CustomExceptions;
using API.Dtos;
using API.Interfaces;
using API.Models;

namespace APITests.Fakes
{
    public class FakeBlogService : IBlogService
    {
        private readonly IEnumerable<Blog> _blogs;
        private readonly bool _shouldThrowGeneric;

        private readonly bool _shouldthrowNotFound;

        // Tehdään servicen constructorista sellainen, että voimme välittää datan ja exceptionin tarpeen sen kautta
        // näin voimme käyttää sama FakeBlogService-luokkaa useissa eri testeissä
        public FakeBlogService(
            IEnumerable<Blog> blogs,
            bool shouldThrow = false,
            bool shouldthrowNotFound = false
        )
        {
            _blogs = blogs;
            _shouldThrowGeneric = shouldThrow;
            _shouldthrowNotFound = shouldthrowNotFound;
        }

        public Task<Blog> GetById(int id)
        {
            if (_shouldThrowGeneric)
            {
                throw new Exception("Generic error");
            }
            else if (_shouldthrowNotFound)
            {
                throw new NotFoundException($"Blog with id of: {id} not found");
            }

            return Task.FromResult(
                new Blog
                {
                    Id = id,
                    Title = "Blog title",
                    Content = "Blog contnet",
                }
            );
        }

        public Task<IEnumerable<Blog>> GetAll()
        {
            if (_shouldThrowGeneric)
            {
                throw new Exception("Simulated service error.");
            }
            return Task.FromResult(_blogs);
        }

        // Jätetään nämä metodit toistaiseksi implementoimatta, koska testaamma GetAll-metodia

        public Task<Blog> Create(CreateBlogReq requestData, int loggedInUser) =>
            throw new NotImplementedException();

        public Task<Blog> Edit(int id, UpdateBlogReq requestData) =>
            throw new NotImplementedException();

        public Task Delete(int id) => throw new NotImplementedException();
    }
}
