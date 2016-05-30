using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using VideoCipherLibrary;
using VideoCipherLibrary.Decryptor;
using VideoCipherLibrary.Encryptor;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;
using VideoCipherLibrary.Modes;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TestEncDec();
            //byte b = HelperModule.GetMidBitsByte(48, 2, 4);

            Console.WriteLine("DONE all.");
        }

        private static void TestEncDec()
        {
            //EncryptionModeller encoder = new EncryptionModeller("CTR.jpg",
            //    "XNA.avi",
            //    "videoCiphered.avi",
            //    EncryptionMode.QuickFBF, 0);
            //EncryptingMessage message = new EncryptingMessage(ByteEncryptionMode.MultipleColorComp, null, 2, 3, 3);
            //encoder.CipherFile(message);
            //Console.WriteLine("DONE ciphering.");

            //DecryptionModeller decoder = new DecryptionModeller(encoder.VideoPathDecodedPath,
            //    encoder.BufferSize, EncryptionMode.QuickFBF, 0);
            //decoder.RetieveFile("tOut", message);

            EncryptingMessage message = new EncryptingMessage(ByteEncryptionMode.MultipleColorComp, null, null, null,
                8, 0, 0, 1, 1, 1);

            EncryptionModeller encoder = new EncryptionModeller("t2.mp4",
                "XNA.avi",
                "videoCiphered.avi",
                EncryptionMode.QuickHybrid);
            encoder.CipherFile(message);
            Console.WriteLine("DONE ciphering.");

            DecryptionModeller decoder = new DecryptionModeller(encoder.VideoPathDecodedPath,
                encoder.BufferSize, EncryptionMode.QuickHybrid);
            decoder.RetieveFile("tOut.jpg", message);

            //CheckBuffers(((StreamEncrypQuickFBF)(encoder._encoder))._buffer,
            // ((StreamDecrypBasicFBF)(decoder._decoder))._bufferRetrieved);


            //CheckBuffers(((StreamEncrypBasicFBF)(encoder._encoder))._buffer,
            // ((StreamDecrypBasicFBF)(decoder._decoder))._bufferRetrieved);



            Console.WriteLine("FINISHED ALL");
        }

        private static void CheckBuffers(byte[] buffer1, byte[] buffer2)
        {
            bool isOk = true;
            for (int i = 0; i < buffer1.Length; i++)
            {
                if (buffer1[i] != buffer2[i])
                {
                    isOk = false;
                    break;
                }  
            }
            if (isOk)
                Console.WriteLine("BUFFERS OK");
            else
            {
                Console.WriteLine("BUFFERS NOOOOOOOOOOOOOT OK");
            }
        }
    }
}
