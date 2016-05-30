using System;
using System.Drawing;
using AForge.Video.DirectShow;
using VideoCipherLibrary.Decryptor;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;
using VideoCipherLibrary.Helpers;
using VideoCipherLibrary.Modes;

namespace VideoCipherLibrary.Encryptor
{
    class StreamEncrypBasicHybrid: IEncryptable
    {
        private FileVideoSource _videoSource;
        private String _videoPathToEncodeIn;
        private String _videoOutPath;
        public byte[] _buffer;
        private bool _isInitializeWriter;
        private int _frameCounter;
        private int _currentIndexBuffer;
        private VideoWriterController _writerController;
        private bool _isFileToEncodeFinishedProcessing;
        private bool _isFinishedAll;
        public EncryptingMessage EncryptingMessage { set; get; }

        public StreamEncrypBasicHybrid(string videoPathToEncodeIn, string videoPathOut,
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

        protected void InitVideoStream()
        {
            _videoSource = new FileVideoSource(_videoPathToEncodeIn);
            CreateEvents();
        }

        protected void CreateEvents()
        {
            _videoSource.NewFrame += new AForge.Video.NewFrameEventHandler(VideoSourceNewFrame);
            _videoSource.PlayingFinished += new AForge.Video.PlayingFinishedEventHandler(VideoSourcePlayingFinished);
        }

        public void EncryptStream(EncryptingMessage encryptingMessage)
        {
            this.EncryptingMessage = encryptingMessage;
            _videoSource.Start();
            while (!_isFinishedAll)
            { }
            System.Threading.Thread.Sleep(10);
        }

        void VideoSourcePlayingFinished(object sender, AForge.Video.ReasonToFinishPlaying reason)
        {
            this._isFinishedAll = true;
        }

        void VideoSourceNewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            if (_isInitializeWriter)
            {
                InitializeWriterController(eventArgs.Frame.Width, eventArgs.Frame.Height);
                _isInitializeWriter = false;
            }
            if (_frameCounter%this.EncryptingMessage.FramesStep == 0)
            {
                _frameCounter++;
                if (!_isFileToEncodeFinishedProcessing)
                {
                    Bitmap newVirtualBitmap = new Bitmap(eventArgs.Frame);
                    var bitmapCiphered = this.CipherBitmap(newVirtualBitmap,
                                                            _buffer,
                                                            ref _currentIndexBuffer,
                                                            ref _isFileToEncodeFinishedProcessing);
                    _writerController.InsertToWriter(bitmapCiphered);
                }
                else
                {
                    if (this.EncryptingMessage.TimeSpan == null)
                    {
                        _writerController.CloseWriter();
                        _isFileToEncodeFinishedProcessing = true;
                        _isFinishedAll = true;
                        _videoSource.Stop();
                    }
                    else
                    {
                        _writerController.InsertToWriter(eventArgs.Frame);
                        _frameCounter++;
                    }
                }
            }
            else
            {
                _writerController.InsertToWriter(eventArgs.Frame);
                _frameCounter++;
            }
            if (this.EncryptingMessage.TimeSpan != null)
            {
                if (_frameCounter >= ((TimeSpan)this.EncryptingMessage.TimeSpan).TotalSeconds * this.EncryptingMessage.FrameRate)
                {
                    _writerController.CloseWriter();
                    _isFileToEncodeFinishedProcessing = true;
                    _isFinishedAll = true;
                    _videoSource.Stop();
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
