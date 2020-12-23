using SIKOSI.Exchange.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SIKOSI.Exchange.Model
{
    public class ExFile : IFile
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte[] Content { get; set; }

        public long Size { get; set; }

        public DateTime LastModified { get; set; }

        public string Type { get; set; }
    }
}
