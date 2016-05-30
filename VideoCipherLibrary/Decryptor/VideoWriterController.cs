using System.Drawing;
using AForge.Video.FFMPEG;

namespace VideoCipherLibrary.Decryptor
{
    public class VideoWriterController
    {
        private VideoFileWriter _videoWriter;
        private int _imageWidth;
        private int _imageHeight;

        public VideoWriterController(string videoOutPath, int imageWidth, int imageHeight)
        {
            this._imageWidth = imageWidth;
            this._imageHeight = imageHeight;
            Init(videoOutPath, this._imageWidth, this._imageHeight);
        }

        public void Init(string videoOutPath, int width, int height)
        {
            _videoWriter = new VideoFileWriter();
            _videoWriter.Open(videoOutPath, width, height, 25, VideoCodec.Raw);
        }

        public void InsertToWriter(Bitmap bitmap)
        {
            _videoWriter.WriteVideoFrame(bitmap);
        }

        public void CloseWriter()
        {
            _videoWriter.Close();
        }
    }
}
