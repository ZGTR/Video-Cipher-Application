using System;
using System.Drawing;
using AForge.Video.DirectShow;
using VideoCipherLibrary.Decryptor;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;
using VideoCipherLibrary.Helpers;
using VideoCipherLibrary.Modes;

namespace VideoCipherLibrary.Encryptor
{
    class StreamEncrypBasicIF : IEncryptable
    {
        protected FileVideoSource _videoSource;
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

        public StreamEncrypBasicIF(string videoPathToEncodeIn, string videoPathOut,
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
            _videoSource.PlayingFinished += new AForge.Video.PlayingFinishedEventHandler(videoSource_PlayingFinished);
        }

        public void EncryptStream(EncryptingMessage encryptingMessage) 
        {
            this.EncryptingMessage = encryptingMessage;
            _videoSource.Start();
            while (!_isFinishedAll)
            { }
            System.Threading.Thread.Sleep(10);
        }

        void videoSource_PlayingFinished(object sender, AForge.Video.ReasonToFinishPlaying reason)
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
            if (_frameCounter % this.EncryptingMessage.FramesStep == 0)
            {
                _frameCounter++;
                if (!_isFileToEncodeFinishedProcessing)
                {
                    Bitmap newVirtualBitmap = new Bitmap(eventArgs.Frame.Width, eventArgs.Frame.Height);
                    var bitmapCiphered =  this.CipherBitmap(newVirtualBitmap,
                        _buffer,
                        ref _currentIndexBuffer,
                        ref _isFileToEncodeFinishedProcessing);
                    _writerController.InsertToWriter(bitmapCiphered);
                }
                else
                {
                    _writerController.CloseWriter();
                    _isFileToEncodeFinishedProcessing = true;
                    _isFinishedAll = true;
                    _videoSource.Stop();
                }
            }
            else
            {
                _writerController.InsertToWriter(eventArgs.Frame);
                _frameCounter++;
            }
        }

        public  Bitmap CipherBitmap(Bitmap bitmap,
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

                    Color currentColor = bitmapCiphered.GetPixel(j, i);
                    // newByteR = buffer[iBufferIndexStart];
                    //Color newEncodedColor = Color.FromArgb(currentColor.A, newByteR,
                    //                                        currentColor.G, currentColor.B);
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
