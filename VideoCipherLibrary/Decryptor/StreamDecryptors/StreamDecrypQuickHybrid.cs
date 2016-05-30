using System;
using System.Drawing;
using AForge.Video.VFW;
using VideoCipherLibrary.Decryptor.ByteDecryptorEngine;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;
using VideoCipherLibrary.Helpers;

namespace VideoCipherLibrary.Decryptor.StreamDecryptors
{
    public class StreamDecrypQuickHybrid : IDecryptable
    {
        protected AVIReader _reader;
        private int _bufferSize;
        private string _videoCipheredPath;
        public byte[] _bufferRetrieved;
        private int _currentIndexBuffer;
        private int _frameCounter;
        private bool _isFileToEncodeFinishedProcessing;
        private bool _isFinishedAll;
        public EncryptingMessage EncryptingMessage { set; get; }

        public StreamDecrypQuickHybrid(string videoCipheredPath, int bufferSize)
        {
            this._videoCipheredPath = videoCipheredPath;
            this._bufferSize = bufferSize;
            this._bufferRetrieved = new byte[this._bufferSize];
            this._currentIndexBuffer = 0;
            this._frameCounter = 0;
            this._isFileToEncodeFinishedProcessing = false;
            this._isFinishedAll = false;

            InitVideoStream();
        }

        private void InitVideoStream()
        {
            _reader = new AVIReader();
            _reader.Open(this._videoCipheredPath);
        }

        public byte[] DecryptStream(EncryptingMessage message)
        {
            this.EncryptingMessage = message;
            var _timeFromBeginning = DateTime.Now;
            int counter = 0;
            int iBufferIndexStart = 0;
            while (_reader.Position - _reader.Start < _reader.Length)
            {
                counter++;
                if (counter % this.EncryptingMessage.FramesStep == 0)
                {
                    if (!_isFileToEncodeFinishedProcessing)
                    {
                        try
                        {
                            Bitmap image = _reader.GetNextFrame();
                            DecodeBitmap(image, _bufferSize,
                                         ref _bufferRetrieved,
                                         ref _currentIndexBuffer,
                                         ref _isFileToEncodeFinishedProcessing);
                        }
                        catch (Exception)
                        {
                            _isFileToEncodeFinishedProcessing = true;
                            _isFinishedAll = true;
                            _reader.Close();
                            break;
                        }
                    }
                    else
                    {
                        _isFileToEncodeFinishedProcessing = true;
                        _isFinishedAll = true;
                        _reader.Close();
                        break;
                    }
                }
                else
                {
                    try
                    {
                        _reader.GetNextFrame();
                    }
                    catch (Exception)
                    {
                        _isFileToEncodeFinishedProcessing = true;
                        _isFinishedAll = true;
                        _reader.Close();
                        break;
                    }
                }
            }
            return this._bufferRetrieved;
        }

        private void DecodeBitmap(Bitmap bitmapIn, int maxBufferSize, 
            ref byte[] bufferRetrieved, 
            ref int iBufferPosition,
            ref bool isFileToEncodeFinishedProcessing)
        {
            FastPixelImage bitmap = new FastPixelImage(bitmapIn);
            bitmap.Lock();
            for (int i = 0; i < bitmap.Height; i+= this.EncryptingMessage.ColAreaStep)
            {
                for (int j = 0; j < bitmap.Width; j+= this.EncryptingMessage.RowAreaStep)
                {
                    Color currentColor = bitmap.GetPixel(j, i);
                    bufferRetrieved[iBufferPosition] = ByteDecryptor.DecryptByte(currentColor, this.EncryptingMessage);
                    iBufferPosition++;
                    if (iBufferPosition == maxBufferSize)
                    {
                        isFileToEncodeFinishedProcessing = true;
                        i = bitmap.Height + 1;
                        break;
                    }
                }
            }
            bitmap.Unlock(true);
        }
    }
}
