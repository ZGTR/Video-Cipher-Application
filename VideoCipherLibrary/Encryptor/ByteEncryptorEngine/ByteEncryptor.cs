using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VideoCipherLibrary.Encryptor;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;
using VideoCipherLibrary.Helpers;

namespace VideoCipherLibrary.Modes
{
    public static class ByteEncryptor
    {
        public static Color EncryptByte(Color colorNative, byte byteToEncrypt, EncryptingMessage message)
        {
            switch (message.Mode)
            {
                case ByteEncryptionMode.OneColorComponent:
                    return ByteEncryptor.EncryptByte(colorNative, byteToEncrypt, (ColorComponent)message.ColorComponent);
                    break;
                case ByteEncryptionMode.MultipleColorComp:
                    return ByteEncryptor.EncryptByte(colorNative, byteToEncrypt, message.RLen, message.GLen, message.BLen);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("colorComponent");
            }
        }

        //public static Color EncryptByteOnMultipleColorComponent(Color colorNative, byte byteToEncrypt)
        //{
        //    byte newColorR = (byte)HelperModule.ApplyOR(byteToEncrypt, 6, 2, HelperModule.NormalizeFromIndex(colorNative.R, 6));
        //    byte newColorG = HelperModule.ApplyOR(byteToEncrypt, 0, 3, HelperModule.NormalizeFromIndex(colorNative.G, 5));
        //    byte newColorB = HelperModule.ApplyOR(byteToEncrypt, 3, 3, HelperModule.NormalizeFromIndex(colorNative.B, 5));
        //    Color newColor = Color.FromArgb(newColorR, newColorG, newColorB);
        //    return newColor;
        //}

        public static Color EncryptByte(Color colorNative, byte byteToEncrypt,
            int rLen,
            int gLen,
            int bLen)
        {
            byte newColorR = (byte)HelperModule.ApplyOR(byteToEncrypt, 8 - rLen, rLen,
                HelperModule.NormalizeFromIndex(colorNative.R, 8 - rLen));

            byte newColorG = HelperModule.ApplyOR(byteToEncrypt, 0, gLen,
                HelperModule.NormalizeFromIndex(colorNative.G, 8 - gLen));

            byte newColorB = HelperModule.ApplyOR(byteToEncrypt, gLen, bLen,
                HelperModule.NormalizeFromIndex(colorNative.B, 8 - bLen));

            Color newColor = Color.FromArgb(newColorR, newColorG, newColorB);
            return newColor;
        }

        public static Color EncryptByte(Color colorNative,
                            byte byteToEncrypt,
                            ColorComponent colorComponent)
        {
            switch (colorComponent)
            {
                case ColorComponent.Red:
                    return Color.FromArgb(colorNative.A,
                         byteToEncrypt,
                         colorNative.G,
                         colorNative.B);
                    break;
                case ColorComponent.Green:
                    return Color.FromArgb(colorNative.A,
                         colorNative.R,
                         byteToEncrypt,
                         colorNative.B);
                    break;
                case ColorComponent.Blue:
                    return Color.FromArgb(colorNative.A,
                         colorNative.R,
                         colorNative.G,
                         byteToEncrypt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("ColorComponent");
            }
        }
    }
}
