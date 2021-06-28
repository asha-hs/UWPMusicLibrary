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
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPMusicLibrary
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MusicsDisplayPage : Page
    {
        private ObservableCollection<Song> MusicList = new ObservableCollection<Song>();
        public MusicsDisplayPage()
        {
            this.InitializeComponent();
            MusicList = new ObservableCollection<Song>();
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            QueryOptions queryOption = new QueryOptions
            (CommonFileQuery.OrderByTitle, new string[] { ".mp3", ".mp4", ".wma" });

            queryOption.FolderDepth = FolderDepth.Deep;

            Queue<IStorageFolder> folders = new Queue<IStorageFolder>();

            var files = await KnownFolders.MusicLibrary.CreateFileQueryWithOptions
              (queryOption).GetFilesAsync();
            //StorageFolder musicLib = KnownFolders.MusicLibrary;
            //var files = await musicLib.GetFilesAsync();
            foreach (var file in files)
            {
                var musicProperties = await file.Properties.GetMusicPropertiesAsync();
                var artist = musicProperties.Artist;
                if (artist == "")
                    artist = "Unknown";
                var album = musicProperties.Album;
                if (album == "")
                    album = "Unknown";

                var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem);
                BitmapImage thumbnailImage = new BitmapImage();
                await thumbnailImage.SetSourceAsync(thumbnail);

                MusicList.Add(new Song { FileName = file.DisplayName, Artist = artist, Album = album, FilePath = file.Path, Thumbnail = thumbnailImage });
            }
        }

        private void MusicImage_Tapped(object sender, TappedRoutedEventArgs e)
        {

        }


    }
}
