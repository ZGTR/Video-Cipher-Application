using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;
using VideoCipherLibrary.Modes;

namespace VideoCipherLibrary.Decryptor
{
    public interface IDecryptable
    {
        //new byte[] DecryptStream(ByteEncryptor byteEncryptor);
        //new byte DecryptColor(Color color);
        new byte[] DecryptStream(EncryptingMessage message);
    }
}
