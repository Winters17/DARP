using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data.Writers
{
    public class WriterController
    {
        public DARPWriter DARPWriter { get; } = new DARPWriter();
    }
}
