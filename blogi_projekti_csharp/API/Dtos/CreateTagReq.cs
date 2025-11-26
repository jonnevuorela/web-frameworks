using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class CreateTagReq
    {
        public required string TagText { get; set; }
    }
}
