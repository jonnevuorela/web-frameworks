using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    // Blogeja hakiessa blogeihin liitetään tämä TagDto:n sijasta,
    // jotta ei palauteta jälleen tagiin liitettyjä blogeja turhaan.
    public class TagBriefDto
    {
        public int Id { get; set; }
        public required string TagText { get; set; }
    }
}
