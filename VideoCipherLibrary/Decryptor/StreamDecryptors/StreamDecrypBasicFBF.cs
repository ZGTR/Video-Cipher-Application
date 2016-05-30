using System.Drawing;
using AForge.Video.DirectShow;
using VideoCipherLibrary.Decryptor.ByteDecryptorEngine;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;
using VideoCipherLibrary.Helpers;

namespace VideoCipherLibrary.Decryptor.StreamDecryptors
{
    public class StreamDecrypBasicFBF : IDecryptable
    {
        private FileVideoSource _videoSource;
        private int _bufferSize;
        private string _videoCipheredPath;
        public byte[] _bufferRetrieved;
        private int _currentIndexBuffer;
        private int _frameCounter;
        private bool _isFileToEncodeFinishedProcessing;
        private bool _isFinishedAll;
        public EncryptingMessage EncryptingMessage { set; get; }

        public StreamDecrypBasicFBF(string videoCipheredPath, int bufferSize)
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
            _videoSource = new FileVideoSource(_videoCipheredPath);
            _videoSource.NewFrame += new AForge.Video.NewFrameEventHandler(videoSource_NewFrame);
            _videoSource.PlayingFinished += new AForge.Video.PlayingFinishedEventHandler(videoSource_PlayingFinished);
        }

        public byte[] DecryptStream(EncryptingMessage message)
        {
            this.EncryptingMessage = message;
            _videoSource.Start();
            while (!_isFinishedAll)
            { }
            System.Threading.Thread.Sleep(10);
            return this._bufferRetrieved;
        }

        void videoSource_PlayingFinished(object sender, AForge.Video.ReasonToFinishPlaying reason)
        {
            _isFinishedAll = true;
        }

        void videoSource_NewFrame(object sender, AForge.Video.NewFrameEventArgs eventArgs)
        {
            //if ((_frameCounter) % (_framesCountToNextInnerFrame) == 0)
            //{
                _frameCounter++;
                if (!_isFileToEncodeFinishedProcessing)
                {
                    Bitmap virtualBitmap = eventArgs.Frame;
                    DecodeBitmap(virtualBitmap, _bufferSize, 
                        ref _bufferRetrieved, 
                        ref _currentIndexBuffer,
                        ref _isFileToEncodeFinishedProcessing);
                }
                else
                {
                    _isFileToEncodeFinishedProcessing = true;
                    _isFinishedAll = true;
                    _videoSource.Stop();
                }
            //}
            //else
            //{
            //    _frameCounter++;
            //}
        }

        private void DecodeBitmap(Bitmap bitmapIn, int maxBufferSize, 
            ref byte[] bufferRetrieved, 
            ref int iBufferPosition,
            ref bool isFileToEncodeFinishedProcessing)
        {
            FastPixelImage bitmap = new FastPixelImage(bitmapIn);
            bitmap.Lock();
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
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
