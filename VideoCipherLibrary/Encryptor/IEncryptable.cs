using System.Drawing;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;
using VideoCipherLibrary.Modes;

namespace VideoCipherLibrary.Encryptor
{
    public interface IEncryptable
    {
        //new Color EncryptByte(Color colorNative, byte byteToEncrypt);
        new void EncryptStream(EncryptingMessage encryptingMessage);
    }
}
