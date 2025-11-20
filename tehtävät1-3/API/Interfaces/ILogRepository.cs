using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Models;

namespace API.Interfaces
{
    public interface ILogRepo
    {
        public Task<AppLog?> Create(AddLogEntryReq req);
    }
}
