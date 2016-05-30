using System;
using System.Drawing;
using VideoCipherLibrary.Encryptor;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;
using VideoCipherLibrary.Helpers;
using VideoCipherLibrary.Modes;

namespace VideoCipherLibrary.Decryptor.ByteDecryptorEngine
{
    public class ByteDecryptor
    {
        public static byte DecryptByte(Color colorNative, EncryptingMessage message)
        {
            switch (message.Mode)
            {
                case ByteEncryptionMode.OneColorComponent:
                    return ByteDecryptor.DecryptColor(colorNative, (ColorComponent)message.ColorComponent);
                    break;
                case ByteEncryptionMode.MultipleColorComp:
                    return ByteDecryptor.DecryptColor(colorNative, message.RLen, message.GLen, message.BLen);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Not Found Mode Exception");
            }
        }

        //public static byte DecryptColorOnMultipleColorComponent(Color color)
        //{
        //    byte byteOut = 0;
        //    byte colorR = (byte)HelperModule.GetMidBitsByte(color.R, 6, 2);
        //    byte colorG = (byte)(color.G << 5);
        //    byte colorB = (byte)((byte)(color.B << 5) >> 3);
        //    byteOut = (byte)(colorR | colorG | colorB);
        //    return byteOut;
        //}

        public static byte DecryptColor(Color color, int rLen, int gLen, int bLen)
        {
            byte byteOut = 0;
            byte colorR = (byte)HelperModule.GetMidBitsByte(color.R, 8 - rLen, rLen);
            byte colorG = (byte)(color.G << 8 - gLen);
            byte colorB = (byte)((byte)(color.B << 8 - (8 - bLen)) >> gLen);
            byteOut = (byte)(colorR | colorG | colorB);
            return byteOut;
        }

        public static byte DecryptColor(Color color, ColorComponent colorComp)
        {
            switch (colorComp)
            {
                case ColorComponent.Red:
                    return color.R;
                    break;
                case ColorComponent.Green:
                    return color.G;
                    break;
                case ColorComponent.Blue:
                    return color.B;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Color Component");
            }
        }
    }
}
