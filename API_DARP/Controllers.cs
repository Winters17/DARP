
using API.DARP.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP
{
    public class Controllers
    {

        public static Controllers Instance { get; } = new Controllers();

        public class SpecificController
        {
            public DataController Data { get; } = new DataController();
        }

        public SpecificController Specific { get; } = new SpecificController();
    }
}
