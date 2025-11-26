using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.CustomExceptions
{
    public class TagCreationException : Exception
    {
        public TagCreationException() { }

        public TagCreationException(string message)
            : base(message) { }
    }
}
