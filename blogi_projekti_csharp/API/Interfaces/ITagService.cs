using API.Dtos;
using API.Models;

namespace API.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<Tag>> GetAll();

        Task<Tag> GetById(int id);

        Task<Tag> Create(CreateTagReq requestData, int? ownerId);

        Task<Tag> Edit(int id, UpdateTagReq requestData);

        Task Delete(int id);
    }
}
