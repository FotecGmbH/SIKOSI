using System;
using System.Collections.Generic;
using System.Text;

namespace SIKOSI.Exchange.Model
{
    public class EncryptionContainer<TData>
    {
        public TData Data { get; set; }

        public byte[] SenderPublicKey { get; set; }
    }
}
