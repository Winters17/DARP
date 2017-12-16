using API.DARP.Data.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LGC.DARP.Data.Readers
{
    public class ReaderController
    {
        public DARPReader DARPReader { get; } = new DARPReader();

    }
}
