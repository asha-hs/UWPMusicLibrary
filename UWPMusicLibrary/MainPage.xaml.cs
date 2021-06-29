using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPMusicLibrary.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPMusicLibrary
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private ObservableCollection<Song> MusicList = new ObservableCollection<Song>();
        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            string context = "AllMusic";
            this.MainFrame.Navigate(typeof(AllMusicDisplayPage),context);
        }

        private void CollectionViewFlyout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CollectionNewFlyout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MusicsViewFlyOut_Click(object sender, RoutedEventArgs e)
        {
            string context = "AllMusic";
            this.MainFrame.Navigate(typeof(AllMusicDisplayPage),context);
        }

        private async void MusicsNewFluOut_Click(object sender, RoutedEventArgs e)
        {
            // allow the user to add new music
            await SongManager.AddNewMusic();

            //reload all music page to reflect the change
            this.MainFrame.Navigate(typeof(AllMusicDisplayPage));

        }

       
    }
}
