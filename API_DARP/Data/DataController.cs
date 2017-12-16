
using API.DARP.Data.Writers;
using LGC.DARP.Data.Readers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API.DARP.Data
{
    public class DataController
    {
        public ReaderController Readers { get; } = new ReaderController();

        public WriterController Writers { get; set; } = new WriterController();
    }
}
