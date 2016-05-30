using System;
using System.Drawing;
using AForge.Video.DirectShow;
using AForge.Video.VFW;
using VideoCipherLibrary.Decryptor;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;
using VideoCipherLibrary.Helpers;
using VideoCipherLibrary.Modes;

namespace VideoCipherLibrary.Encryptor
{
    class StreamEncrypQuickHybrid: IEncryptable
    {
        protected AVIReader _reader;
        private String _videoPathToEncodeIn;
        private String _videoOutPath;
        public byte[] _buffer;
        private bool _isInitializeWriter;
        private int _currentIndexBuffer;
        private VideoWriterController _writerController;
        private bool _isFileToEncodeFinishedProcessing;
        private bool _isFinishedAll;
        public EncryptingMessage EncryptingMessage { set; get; }

        public StreamEncrypQuickHybrid(string videoPathToEncodeIn, string videoPathOut,
            byte[] buffer)
        {
            this._buffer = buffer;
            this._videoPathToEncodeIn = videoPathToEncodeIn;
            this._videoOutPath = videoPathOut;
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
                if (counter % this.EncryptingMessage.FramesStep == 0)
                {
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
                        if (this.EncryptingMessage.TimeSpan == null)
                        {
                            _writerController.CloseWriter();
                            _isFileToEncodeFinishedProcessing = true;
                            _isFinishedAll = true;
                            _reader.Close();
                            break;
                        }
                        else
                        {
                            _writerController.InsertToWriter(_reader.GetNextFrame());
                        }
                    }
                }
                else
                {
                    _writerController.InsertToWriter(_reader.GetNextFrame());
                }
                if (this.EncryptingMessage.TimeSpan != null)
                {
                    if (counter >= ((TimeSpan)this.EncryptingMessage.TimeSpan).TotalSeconds * this.EncryptingMessage.FrameRate)
                    {
                        _writerController.CloseWriter();
                        _isFileToEncodeFinishedProcessing = true;
                        _isFinishedAll = true;
                        _reader.Close();
                        break;
                    }
                }
            }
        }

        public Bitmap CipherBitmap(Bitmap bitmap,
                               byte[] buffer,
                               ref int iBufferIndexStart,
                               ref bool isFileToEncodeFinishedProcessing)
        {
            FastPixelImage bitmapCiphered = new FastPixelImage(bitmap);
            bitmapCiphered.Lock();
            for (int i = 0; i < bitmapCiphered.Height; i+= this.EncryptingMessage.ColAreaStep)
            {
                for (int j = 0; j < bitmapCiphered.Width; j += this.EncryptingMessage.RowAreaStep)
                {
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
