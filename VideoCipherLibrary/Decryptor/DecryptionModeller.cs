using System.IO;
using VideoCipherLibrary.Decryptor.StreamDecryptors;
using VideoCipherLibrary.Encryptor;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;

namespace VideoCipherLibrary.Decryptor
{
    public class DecryptionModeller
    {
        public IDecryptable _decoder;
        public DecryptionModeller(string videoPathDecodedPath, int bufferSize, EncryptionMode mode)
        {
            
            switch (mode)
            {
                case EncryptionMode.BasicIF:
                    _decoder = new StreamDecrypBasicIF(videoPathDecodedPath, bufferSize);
                    break;
                case EncryptionMode.QuickFBF:
                    _decoder = new StreamDecrypBasicFBF(videoPathDecodedPath, bufferSize);
                    break;
                case EncryptionMode.BasicFBF:
                    _decoder = new StreamDecrypBasicFBF(videoPathDecodedPath, bufferSize);
                    break;
                case EncryptionMode.BasicHybrid:
                    _decoder = new StreamDecrypBasicHybrid(videoPathDecodedPath, bufferSize);
                    break;
                case EncryptionMode.QuickHybrid:
                    _decoder = new StreamDecrypQuickHybrid(videoPathDecodedPath, bufferSize);
                    break;
            }
        }
        
        public void RetieveFile(string fileRetrievedPath, EncryptingMessage message)
        {
            byte[] bufferOut = _decoder.DecryptStream(message);
            FileStream fStream = new FileStream(fileRetrievedPath, FileMode.Create);
            fStream.Write(bufferOut, 0, bufferOut.Length);
            fStream.Close();
        }
    }
}
