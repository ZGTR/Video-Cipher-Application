using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using AForge.Video.VFW;

namespace VideoCipherLibrary
{
    public class AviVideoReader
    {
        public AviVideoReader(string videoPath)
        {
            // instantiate AVI reader
            AVIReader reader = new AVIReader();
            // open video file
            reader.Open(videoPath);
            // read the video file
            int counter = 0;
            while (reader.Position - reader.Start < reader.Length)
            {
                counter++;
                // get next frame
                Bitmap image = reader.GetNextFrame();
                image.Save("image" + counter + ".jpg");
                // .. process the frame somehow or display it
            }
            reader.Close();
        }
    }
}
