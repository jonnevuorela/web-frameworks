using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Dtos
{
    // Tageja hakiessa tageihin liitetään tämä kokonaisen BlogDto:n sijasta,
    // jotta ei palauteta jälleen blogiin liitettyjä tageja ja aiheuteta ikilooppia.
    public class BlogBriefDto
    {
        public int Id { get; set; }
        public required string Title { get; set; }
    }
}
