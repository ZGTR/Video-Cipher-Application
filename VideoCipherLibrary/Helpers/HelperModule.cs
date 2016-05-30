using System;

namespace VideoCipherLibrary.Helpers
{
    public static class HelperModule
    {
        //public static byte GetLowerBitsByte(byte byteIn, uint numOfDigits)
        //{
        //    byte byteToReturn = 0;
        //    byteToReturn = (byte)((byte)(byteIn << 8 - (int)numOfDigits) >> 8 - (int)numOfDigits);
        //    return byteToReturn;
        //}

        public static byte GetMidBitsByte(byte byteIn, int startIndex, int numOfDigits)
        {
            try
            {
                int destToEnd = 8 - (startIndex + numOfDigits);
                //byte byteToReturn = 0;
                byte byteShiftRight = (byte)(byteIn >> (destToEnd));
                byte byteShiftLeft = (byte)(byteShiftRight << (8 - numOfDigits));
                byte byteOut = (byte)(byteShiftLeft >> (8 - numOfDigits));
                return byteOut;
            }
            catch (Exception)
            {
                throw new Exception("Exceed byte boundry Index Exception.");
            }
        }

        public static byte NormalizeFromIndex(byte byteIn, int startIndex)
        {
            byte byteOut = 0;
            int dest = 8 - startIndex;
            byteOut = (byte) ((byte) (byteIn >> dest) << dest);
            return byteOut;
        }

        public static byte ApplyOR(byte byteIn, int startIndex, int numOfDigits, byte byteOther)
        {
            try
            {
                int destToEnd = 8 - (startIndex + numOfDigits);
                //byte byteToReturn = 0;
                byte byteShiftRight = (byte)(byteIn >> (destToEnd));
                byte byteShiftLeft = (byte)(byteShiftRight << (8 - numOfDigits));
                byte byteOut = (byte)((byte)(byteShiftLeft >> (8 - numOfDigits)) | byteOther);
                return byteOut;
            }
            catch (Exception)
            {
                throw new Exception("Exceed byte boundry Index Exception.");
            }
        }

        public static byte ApplyORFirst(byte byteIn, int startIndex, int numOfDigits, byte byteOther)
        {
            try
            {
                int destToEnd = 8 - (startIndex + numOfDigits);
                //byte byteToReturn = 0;
                byte byteShiftRight = (byte)(byteIn >> (destToEnd));
                byte byteShiftLeft = (byte)(byteShiftRight << (8 - numOfDigits));
                byte byteOut = (byte)(byteShiftLeft | byteOther);
                return byteOut;
            }
            catch (Exception)
            {
                throw new Exception("Exceed byte boundry Index Exception.");
            }
        }
    }
}
