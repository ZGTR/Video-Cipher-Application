using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VideoCipherLibrary.Modes;

namespace VideoCipherLibrary.Encryptor.ByteEncryptorEngine
{
    public class EncryptingMessage
    {
        public ByteEncryptionMode Mode { get; set; }
        public TimeSpan? TimeSpan { get; set; }
        public int? FrameRate { set; get; }
        public ColorComponent? ColorComponent { get; set; }
        public int RLen { get; set; }
        public int GLen { get; set; }
        public int BLen { get; set; }
        public int FramesStep { get; set; }
        public int ColAreaStep { set; get; }
        public int RowAreaStep { set; get; }

        public EncryptingMessage(ByteEncryptionMode mode,
            TimeSpan? timeSpan,
            int? frameRate,
            ColorComponent? colorComponent,
            int rLen = 2,
            int gLen = 3,
            int bLen = 3,
            int framesStep = 1,
            int rowAreaStep = 1,
            int colAreaStep = 1)
        {
            Mode = mode;
            TimeSpan = timeSpan;
            FrameRate = frameRate;
            ColorComponent = colorComponent;
            RLen = rLen;
            GLen = gLen;
            BLen = bLen;
            FramesStep = framesStep;
            RowAreaStep = rowAreaStep;
            ColAreaStep = colAreaStep;
        }
    }
}
