using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace ZGTR_VideoCipherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            GUIController.InitializeController(this);
            GUIController.InitializeColorCompsEvents();
        }

        private void btnVideoOrigin_Click(object sender, RoutedEventArgs e)
        {
            GUIController.SetVideoEncryptIn();
        }

        private void btnSetStreamEncrypt_Click(object sender, RoutedEventArgs e)
        {
            GUIController.SetStreamToEncrypt();
        }

        private void btnVideoCiphere_Click(object sender, RoutedEventArgs e)
        {
            GUIController.SetVideoCiphered();
        }

        private void btnPlayVideo_Click(object sender, RoutedEventArgs e)
        {
            meVideo.Play();
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            meVideo.Stop();
        }

        private void btnPause_Click(object sender, RoutedEventArgs e)
        {
            meVideo.Pause();
        }

        private void btnSetVideo_Click(object sender, RoutedEventArgs e)
        {
            GUIController.SetChosenVideo();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GUIController.NeutralizeAllRectangles();
        }

        private void btnDPlayVideo_Click(object sender, RoutedEventArgs e)
        {
            meD1Video.BeginInit();
            meD1Video.Source = new Uri(GUIController.VideoOriginPath, UriKind.Relative);
            meD1Video.EndInit();

            meD2Video.BeginInit();
            meD2Video.Source = new Uri(GUIController.VideoEncryptedPath, UriKind.Relative);
            meD2Video.EndInit();

            meD1Video.Play();
            meD2Video.Play();
        }

        private void btnDPause_Click(object sender, RoutedEventArgs e)
        {
            meD1Video.Pause();
            meD2Video.Pause();
        }

        private void btnDStop_Click(object sender, RoutedEventArgs e)
        {
            meD1Video.Stop();
            meD2Video.Stop();
        }

        private void btnEncryDecryp_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                GUIController.NeutralizeVideosIO();
                GUIController.EncryptStream();
            }
            catch (Exception)
            {
                MessageBox.Show("Error Ouccured.");
            }
        }
    }
}
