using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Win32;
using VideoCipherLibrary.Decryptor;
using VideoCipherLibrary.Encryptor;
using VideoCipherLibrary.Encryptor.ByteEncryptorEngine;
using VideoCipherLibrary.Modes;

namespace ZGTR_VideoCipherApp
{
    class GUIController
    {
        public static string VideoOriginPath;
        public static string VideoEncryptedPath;
        public static string StreamToEncryptPath;
        public static string StreamOutFilePath = "StreamOutFile";
        public static MainWindow GUIMainWindow;

        public static void InitializeController(MainWindow mainWindow)
        {
            GUIMainWindow = mainWindow;
            //GUIMainWindow.meD2Video.BeginInit();
            //GUIMainWindow.meD2Video.Source = new Uri(VideoEncryptedPath, UriKind.Relative);
            //GUIMainWindow.meD2Video.EndInit();
        }

        public static void NeutralizeVideosIO()
        {
            GUIMainWindow.meVideo.Stop();
            GUIMainWindow.meD1Video.Stop();
            GUIMainWindow.meD2Video.Stop();

            GUIMainWindow.meD1Video.BeginInit();
            GUIMainWindow.meD1Video.Source = null;
            GUIMainWindow.meD1Video.EndInit();

            GUIMainWindow.meD2Video.BeginInit();
            GUIMainWindow.meD2Video.Source = null;
            GUIMainWindow.meD2Video.EndInit();

            GUIMainWindow.meVideo.BeginInit();
            GUIMainWindow.meVideo.Source = null;
            GUIMainWindow.meVideo.EndInit();
        }


        public static void SetVideoEncryptIn()
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                if ((bool)op.ShowDialog())
                {
                    VideoOriginPath = op.FileName;
                    GUIMainWindow.meD1Video.BeginInit();
                    GUIMainWindow.meD1Video.Source = new Uri(VideoOriginPath);
                    GUIMainWindow.meD1Video.EndInit();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please, choose appropriate video.");
            }
        }

        public static void SetStreamToEncrypt()
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                if ((bool)op.ShowDialog())
                {
                    StreamToEncryptPath = op.FileName;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please, choose appropriate stream.");
            }
        }

        public static void SetVideoCiphered()
        {
            try
            {
                OpenFileDialog op = new OpenFileDialog();
                if ((bool)op.ShowDialog())
                {
                    VideoEncryptedPath = op.FileName;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Please, choose appropriate video.");
            }
        }

        public static void SetChosenVideo()
        {
            try
            {
                string videoPath = GetVideoPathAccordingToUser();
                GUIMainWindow.meVideo.BeginInit();
                if (videoPath == VideoEncryptedPath)
                    GUIMainWindow.meVideo.Source = new Uri(videoPath, UriKind.Absolute);
                else
                    GUIMainWindow.meVideo.Source = new Uri(videoPath, UriKind.Absolute);
                GUIMainWindow.meVideo.EndInit();
            }
            catch (Exception)
            {
                MessageBox.Show("Can't play the specified file.");
            }
        }

        private static string GetVideoPathAccordingToUser()
        {
            string path = String.Empty;
            try
            {
                switch (GUIMainWindow.cBoxVideo.SelectedIndex)
                {
                    case -1:
                        path = GUIController.VideoOriginPath;
                        break;
                    case 0:
                        path = GUIController.VideoOriginPath;
                        break;
                    case 1:
                        path = GUIController.VideoEncryptedPath;
                        break;
                }
                return path;
            }
            catch (Exception)
            {
                path = GUIController.VideoOriginPath;
                return path;
            }
        }

        public static void InitializeColorCompsEvents()
        {
            foreach (var rect in GUIMainWindow.spRed.Children)
            {
                ((Rectangle)rect).MouseDown += new System.Windows.Input.MouseButtonEventHandler(Rectangle_MouseDown);
            }
            foreach (var rect in GUIMainWindow.spGreen.Children)
            {
                ((Rectangle)rect).MouseDown += new System.Windows.Input.MouseButtonEventHandler(Rectangle_MouseDown);
            }
            foreach (var rect in GUIMainWindow.spBlue.Children)
            {
                ((Rectangle)rect).MouseDown += new System.Windows.Input.MouseButtonEventHandler(Rectangle_MouseDown);
            }
        }

        static void Rectangle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            int countOfColorRects = GetAllColoDarkRedRectsCount()
                                    + GetAllColoDarkGreenRectsCount()
                                    + GetAllColoDarkBlueRectsCount();
            if (countOfColorRects < 8)
            {
                Rectangle sendRec = sender as Rectangle;
                StackPanel spParent = sendRec.Parent as StackPanel;
                if (IsAllRightRectAreColoDarkRed(sendRec, spParent))
                {
                    if (spParent == GUIMainWindow.spRed)
                        sendRec.Fill = new SolidColorBrush(Colors.DarkRed);
                    else if (spParent == GUIMainWindow.spGreen)
                        sendRec.Fill = new SolidColorBrush(Colors.Green);
                    else if (spParent == GUIMainWindow.spBlue)
                        sendRec.Fill = new SolidColorBrush(Colors.Blue);
                }
            }
            else
            {
                MessageBox.Show("You reached the maximum of 8 bits.");
            }
        }

        private static int GetAllColoDarkRedRectsCount()
        {
            int count = 0;
            for (int i = 0; i < GUIMainWindow.spRed.Children.Count; i++)
            {
                if (((GUIMainWindow.spRed.Children[i] as Rectangle).Fill as SolidColorBrush).Color == Colors.DarkRed)
                {
                    count++;
                }
            }
            return count;
        }

        private static int GetAllColoDarkGreenRectsCount()
        {
            int count = 0;
            for (int i = 0; i < GUIMainWindow.spGreen.Children.Count; i++)
            {
                if (((GUIMainWindow.spGreen.Children[i] as Rectangle).Fill as SolidColorBrush).Color == Colors.Green)
                {
                    count++;
                }
            }
            return count;
        }

        private static int GetAllColoDarkBlueRectsCount()
        {
            int count = 0;
            for (int i = 0; i < GUIMainWindow.spBlue.Children.Count; i++)
            {
                if (((GUIMainWindow.spBlue.Children[i] as Rectangle).Fill as SolidColorBrush).Color == Colors.Blue)
                {
                    count++;
                }
            }
            return count;
        }

        private static bool IsAllRightRectAreColoDarkRed(Rectangle sendRec, StackPanel spParent)
        {
            int indexOfRect = spParent.Children.IndexOf(sendRec);
            bool isAllOk = true;
            if (spParent == GUIMainWindow.spRed)
                isAllOk = CheckIsAllRightRectAreColoDarkRed(indexOfRect, spParent, Colors.DarkRed);
            else if (spParent == GUIMainWindow.spGreen)
                isAllOk = CheckIsAllRightRectAreColoDarkRed(indexOfRect, spParent, Colors.Green);
            else
                isAllOk = CheckIsAllRightRectAreColoDarkRed(indexOfRect, spParent, Colors.Blue);
            return isAllOk;
        }

        private static bool CheckIsAllRightRectAreColoDarkRed(int indexOfRect, StackPanel spParent, Color colorIn)
        {
            bool isAllOk = true;
            for (int i = indexOfRect + 1; i < spParent.Children.Count; i++)
            {
                if (!(((spParent.Children[i] as Rectangle).Fill as SolidColorBrush).Color == colorIn))
                {
                    isAllOk = false;
                    break;
                }
            }
            return isAllOk;
        }

        public static void NeutralizeAllRectangles()
        {
            foreach (var rect in GUIMainWindow.spRed.Children)
            {
                ((Rectangle) rect).Fill = new SolidColorBrush(Colors.White);
            }
            foreach (var rect in GUIMainWindow.spGreen.Children)
            {
                ((Rectangle)rect).Fill = new SolidColorBrush(Colors.White);
            }
            foreach (var rect in GUIMainWindow.spBlue.Children)
            {
                ((Rectangle)rect).Fill = new SolidColorBrush(Colors.White);
            }
        }

        public static void EncryptStream()
        {
            EncryptionMode encrypMode = GetChosenEncrypMode();
            EncryptionMode decrypMode = GetChosenDecrypMode();
            EncryptingMessage message = InitializeEncrypMessage(encrypMode, decrypMode);
            //Encryption
            EncryptionModeller encoder = new EncryptionModeller(StreamToEncryptPath,
                                                                VideoOriginPath,
                                                                VideoEncryptedPath,
                                                                encrypMode);
            DateTime t1 = DateTime.Now;
            encoder.CipherFile(message);
            GUIMainWindow.tbEncrypTime.Text = (DateTime.Now - t1).TotalSeconds.ToString() + " sec";

            //Decryption
            DecryptionModeller decoder = new DecryptionModeller(encoder.VideoPathDecodedPath,
                                                                encoder.BufferSize,
                                                                decrypMode);
            DateTime t2 = DateTime.Now;
            decoder.RetieveFile(StreamOutFilePath, message);
            GUIMainWindow.tbDecrypTime.Text = (DateTime.Now -  t2).TotalSeconds.ToString() + " sec";
        }

        private static EncryptingMessage InitializeEncrypMessage(EncryptionMode encrypMode,
            EncryptionMode decrypMode)
        {
            EncryptingMessage message = null;

            // Params
            bool isTimeSpan = (bool)GUIMainWindow.cbIsTimeSpan.IsChecked;
            TimeSpan timeSpan = TimeSpan.FromSeconds(GetInt(GUIMainWindow.tbTimeSpan));
            int frameRate = GetInt(GUIMainWindow.tbFrameRate);
            int frameStep = GetInt(GUIMainWindow.tbFrameStep);
            int rowStep = GetInt(GUIMainWindow.tbRowStep);
            int colStep = GetInt(GUIMainWindow.tbColStep);

            // Byte Encryp
            bool isMultColorComp = (bool)GUIMainWindow.rbMultipleColorComp.IsChecked;
            bool isOneColorComp = (bool)GUIMainWindow.rbOneColorComp.IsChecked;
            int rLen = GetColorDistributionCount(GUIMainWindow.spRed);
            int gLen = GetColorDistributionCount(GUIMainWindow.spGreen);
            int bLen = GetColorDistributionCount(GUIMainWindow.spBlue);
            ColorComponent colorComp = GetOneColorComp();
            
            // Message according to mode
            if (encrypMode == EncryptionMode.BasicIF || encrypMode == EncryptionMode.BasicFBF || encrypMode == EncryptionMode.QuickFBF)
            {
                if (isMultColorComp)
                    message = new EncryptingMessage(ByteEncryptionMode.MultipleColorComp,
                                                    null,
                                                    null,
                                                    null,
                                                    rLen, gLen, bLen, frameStep);
                else
                {
                    message = new EncryptingMessage(ByteEncryptionMode.OneColorComponent,
                                                    null,
                                                    null,
                                                    colorComp,
                                                    1, 1, 1, frameStep);
                }
            }
            else
            {
                if (encrypMode == EncryptionMode.BasicHybrid || encrypMode == EncryptionMode.QuickHybrid)
                {
                    if (isMultColorComp)
                    {
                        if (isTimeSpan)
                        {
                            message = new EncryptingMessage(ByteEncryptionMode.MultipleColorComp,
                                                            timeSpan,
                                                            frameRate,
                                                            null,
                                                            rLen, gLen, bLen, frameStep, rowStep, colStep);
                        }
                        else
                        {
                            message = new EncryptingMessage(ByteEncryptionMode.MultipleColorComp,
                                                            null, 
                                                            null,
                                                            null,
                                                            rLen, gLen, bLen, frameStep, rowStep, colStep);
                        }
                    }
                    else
                    {
                        if (isTimeSpan)
                        {
                            message = new EncryptingMessage(ByteEncryptionMode.OneColorComponent,
                                                            timeSpan,
                                                            frameRate,
                                                            colorComp,
                                                            1, 1, 1, frameStep, rowStep, colStep);
                        }
                        else
                        {
                            message = new EncryptingMessage(ByteEncryptionMode.OneColorComponent,
                                                            null,
                                                            null,
                                                            colorComp,
                                                            1, 1, 1, frameStep, rowStep, colStep);
                        }
                    }
                }
            }
            return message;
        }

        private static int GetColorDistributionCount(StackPanel stackPanel)
        {
            int counter = 0;
            for (int i = 0; i < stackPanel.Children.Count; i++)
            {
                if (((stackPanel.Children[i] as Rectangle).Fill as SolidColorBrush).Color != Colors.White)
                {
                    counter++;
                }
            }
            return counter;
        }

        private static ColorComponent GetOneColorComp()
        {
            ColorComponent colorComponent = ColorComponent.Blue;
            if ((bool) GUIMainWindow.rbRed.IsChecked)
            {
                colorComponent = ColorComponent.Red;
            }
            else
            {
                if ((bool)GUIMainWindow.rbGreen.IsChecked)
                {
                    colorComponent = ColorComponent.Green;
                }
                else
                {
                    colorComponent = ColorComponent.Blue;
                }
            }
            return colorComponent;
        }

        private static int GetInt(TextBox textBox)
        {
            return Int32.Parse(textBox.Text);
        }

        private static EncryptionMode GetChosenEncrypMode()
        {
            EncryptionMode mode = EncryptionMode.QuickHybrid;
            if ((bool)GUIMainWindow.rbBasicIF.IsChecked)
            {
                mode = EncryptionMode.BasicIF;
            }
            else
            {
                if ((bool)GUIMainWindow.rbBasicFBF.IsChecked)
                {
                    mode = EncryptionMode.BasicFBF;
                }
                else
                {
                    if ((bool)GUIMainWindow.rbQuickFBF.IsChecked)
                    {
                        mode = EncryptionMode.QuickFBF;
                    }
                    else
                    {
                        if ((bool)GUIMainWindow.rbBasicHybrid.IsChecked)
                        {
                            mode = EncryptionMode.BasicHybrid;
                        }
                        else
                        {
                            mode = EncryptionMode.QuickHybrid;
                        }
                    }
                }
            }
            return mode;
        }

        private static EncryptionMode GetChosenDecrypMode()
        {
            EncryptionMode mode = EncryptionMode.QuickHybrid;
            if ((bool)GUIMainWindow.rbDBasicIF.IsChecked)
            {
                mode = EncryptionMode.BasicIF;
            }
            else
            {
                if ((bool)GUIMainWindow.rbDBasicFBF.IsChecked)
                {
                    mode = EncryptionMode.BasicFBF;
                }
                else
                {
                    if ((bool)GUIMainWindow.rbDQuickHybrid.IsChecked)
                    {
                        mode = EncryptionMode.QuickHybrid;
                    }
                    else
                    {
                        mode = EncryptionMode.BasicHybrid;
                    }
                }
            }
            return mode;
        }
    }
}
