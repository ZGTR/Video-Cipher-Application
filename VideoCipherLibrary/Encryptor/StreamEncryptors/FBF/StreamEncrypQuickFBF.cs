using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AForge.Video.DirectShow;
//using AForge.Video.DirectShow.Internals;
using AForge.Video.VFW;
using VideoCipherLibrary.Decryptor;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;
using VideoCipherLibrary.Helpers;
using VideoCipherLibrary.Modes;

namespace VideoCipherLibrary.Encryptor
{
    public class StreamEncrypQuickFBF : IEncryptable
    {
        //protected FileVideoSource _videoSource;
        protected AVIReader _reader;
        protected String _videoPathToEncodeIn;
        protected String _videoOutPath;
        public byte[] _buffer;
        protected bool _isInitializeWriter;
        protected int _frameCounter;
        protected int _currentIndexBuffer;
        protected VideoWriterController _writerController;
        protected bool _isFileToEncodeFinishedProcessing;
        protected bool _isFinishedAll;
        public EncryptingMessage EncryptingMessage { set; get; }

        public StreamEncrypQuickFBF(string videoPathToEncodeIn, string videoPathOut,
            byte[] buffer)
        {
            this._buffer = buffer;
            this._videoPathToEncodeIn = videoPathToEncodeIn;
            this._videoOutPath = videoPathOut;

            this._currentIndexBuffer = 0;
            this._isFileToEncodeFinishedProcessing = false;
            this._isInitializeWriter = true;

            InitVideoStream();
        }

        private void InitVideoStream()
        {
            _reader = new AVIReader();
            _reader.Open(this._videoPathToEncodeIn);
            Bitmap imageWH = _reader.GetNextFrame();
            this.InitializeWriterController(imageWH.Width, imageWH.Height);

            _reader = new AVIReader();
            _reader.Open(this._videoPathToEncodeIn);
        }


        public void EncryptStream(EncryptingMessage encryptingMessage)
        {
            this.EncryptingMessage = encryptingMessage;
            int counter = 0;
            int iBufferIndexStart = 0;
            while (_reader.Position - _reader.Start < _reader.Length)
            {
                counter++;
                if (!_isFileToEncodeFinishedProcessing)
                {
                    try
                    {
                        Bitmap image = _reader.GetNextFrame();
                        image = this.CipherBitmap(image, this._buffer, ref iBufferIndexStart,
                                                  ref this._isFileToEncodeFinishedProcessing);
                        _writerController.InsertToWriter(image);
                    }
                    catch (Exception)
                    {
                        _writerController.CloseWriter();
                        _isFileToEncodeFinishedProcessing = true;
                        _isFinishedAll = true;
                        _reader.Close();
                        break;
                    }
                }
                else
                {
                    _writerController.CloseWriter();
                    _isFileToEncodeFinishedProcessing = true;
                    _isFinishedAll = true;
                    break;
                }
            }
            try
            {
                _reader.Close();
            }
            catch(Exception)
            {
            }
        }

        public Bitmap CipherBitmap(Bitmap bitmap,
                               byte[] buffer,
                               ref int iBufferIndexStart,
                               ref bool isFileToEncodeFinishedProcessing)
        {
            FastPixelImage bitmapCiphered = new FastPixelImage(bitmap);
            bitmapCiphered.Lock();
            for (int i = 0; i < bitmapCiphered.Height; i++)
            {
                for (int j = 0; j < bitmapCiphered.Width; j++)
                {
                    if (iBufferIndexStart == 82202)
                    {
                        int iss = 0;
                    }
                    Color currentColor = bitmapCiphered.GetPixel(j, i);
                    Color newEncodedColor = ByteEncryptor.EncryptByte(currentColor, buffer[iBufferIndexStart],
                        this.EncryptingMessage);
                    bitmapCiphered.SetPixel(j, i, newEncodedColor);
                    iBufferIndexStart++;
                    if (iBufferIndexStart == buffer.Length)
                    {
                        i = bitmapCiphered.Height + 1;
                        isFileToEncodeFinishedProcessing = true;
                        break;
                    }
                }
            }
            bitmapCiphered.Unlock(true);
            return bitmapCiphered.Bitmap;
        }

        protected void InitializeWriterController(int width, int height)
        {
            this._writerController = new VideoWriterController(_videoOutPath, width, height);
        }
    }
}
