using System.IO;
using VideoCipherLibrary.Encryptor;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;

namespace VideoCipherLibrary.Decryptor
{
    public class EncryptionModeller
    {
        private byte[] _bufferReader;
        public IEncryptable _encoder;
        
        public int BufferSize
        {
            get { return this._bufferReader.Length; }
        }

        public int FramesCountToNextInnerFrame { get; private set; }

        public string VideoPathDecodedPath { get; private set; }


        public EncryptionModeller(string filePathToCipher, 
            string videoPathToEncode, 
            string videoDecodedPath,
            EncryptionMode mode)
        {
            InitFileStream(filePathToCipher);
            VideoPathDecodedPath = videoDecodedPath;
            switch (mode)
            {
                case EncryptionMode.BasicIF:
                    _encoder = new StreamEncrypBasicIF(videoPathToEncode,
                        videoDecodedPath, _bufferReader);
                    break;
                case EncryptionMode.BasicFBF:
                    _encoder = new StreamEncrypBasicFBF(videoPathToEncode,
                        videoDecodedPath, _bufferReader);
                    break;
                case EncryptionMode.QuickFBF:
                    _encoder = new StreamEncrypQuickFBF(videoPathToEncode,
                        videoDecodedPath, _bufferReader);
                    break;
                case EncryptionMode.BasicHybrid:
                    _encoder = new StreamEncrypBasicHybrid(videoPathToEncode,
                        videoDecodedPath, _bufferReader);
                    break;
                case EncryptionMode.QuickHybrid:
                    _encoder = new StreamEncrypQuickHybrid(videoPathToEncode,
                        videoDecodedPath, _bufferReader);
                    break;
            }
        }

        public void CipherFile(EncryptingMessage message)
        {
            _encoder.EncryptStream(message);
        }

        private void InitFileStream(string filePathToCipher)
        {
            FileStream fileStream = File.Open(filePathToCipher, FileMode.Open);
            _bufferReader = new byte[fileStream.Length];
            fileStream.Read(_bufferReader, 0, (int)fileStream.Length);
            fileStream.Close();
        }
    }
}
