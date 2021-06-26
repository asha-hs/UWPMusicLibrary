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
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
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
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            QueryOptions queryOption = new QueryOptions
        (CommonFileQuery.OrderByTitle, new string[] { ".mp3", ".mp4", ".wma" });

            queryOption.FolderDepth = FolderDepth.Deep;

            Queue<IStorageFolder> folders = new Queue<IStorageFolder>();

            var files = await KnownFolders.MusicLibrary.CreateFileQueryWithOptions
              (queryOption).GetFilesAsync();
            //StorageFolder musicLib = KnownFolders.MusicLibrary;
            //var files = await musicLib.GetFilesAsync();
            foreach(var file in files)
            {
                var musicProperties = await file.Properties.GetMusicPropertiesAsync();
                var artist = musicProperties.Artist;
                if (artist == "")
                    artist = "Unknown";
                var album = musicProperties.Album;
                if (album == "")
                    album = "Unknown";
                
                MusicList.Add(new Song { FileName = file.DisplayName, Artist = artist, Album = album, FilePath = file.Path });
            }
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var song = (Song)e.ClickedItem;
            MusicLibraryMediaElement.Source = new Uri(song.FilePath);
            MusicLibraryMediaElement.Play();
        }
    }
}
