using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media.Imaging;

namespace UWPMusicLibrary.Model
{
    public static class SongManager
    {
        private static List<Song> AllMusic = new List<Song>();

        private static bool isAllMusicInitialized = false;

        public static async Task<bool> GetAllMusic(ObservableCollection<Song> allmusic)
        {
            allmusic.Clear();
            bool retval = true;
            if (!isAllMusicInitialized)
            {
                try
                {
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

                        AllMusic.Add(new Song { FileName = file.DisplayName, Artist = artist, Album = album, FilePath = file.Path, Thumbnail = thumbnailImage });

                    }
                    isAllMusicInitialized = true;
                }
                catch (Exception)
                {
                    retval = false;
                }
                
            }

            AllMusic.ForEach(item => allmusic.Add(item));
            return retval;

        }

        public static async Task AddNewMusic()
        {
            var filePicker = new FileOpenPicker();
            filePicker.ViewMode = PickerViewMode.Thumbnail;
            filePicker.SuggestedStartLocation = PickerLocationId.ComputerFolder;
            filePicker.FileTypeFilter.Add(".mp3");

            StorageFile musicFile = await filePicker.PickSingleFileAsync();

            if(musicFile != null)
            {
                var myMusicFolder = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);

                string libraryPath = myMusicFolder.SaveFolder.Path;

                //first of all we need to check if the file already exists -
                //if so replace the exisiting at the same time remove the listing from the
                //musiccollection
                StorageFolder folder = KnownFolders.MusicLibrary;
                string fileName = Path.GetFileName(musicFile.Path);

                if(await folder.TryGetItemAsync(fileName) != null)
                {
                    string path = $"{libraryPath}\\{fileName}";
                    AllMusic.RemoveAll(item => item.FilePath == path);
                }

                StorageFile file = await musicFile.CopyAsync(KnownFolders.MusicLibrary, fileName, NameCollisionOption.ReplaceExisting);

                Song music = new Song { FilePath = file.Path };

                var thumbnail = await file.GetThumbnailAsync(ThumbnailMode.SingleItem);
                BitmapImage thumbnailimage = new BitmapImage();
                await thumbnailimage.SetSourceAsync(thumbnail);
                music.Thumbnail = thumbnailimage;

                var musicProperties = await file.Properties.GetMusicPropertiesAsync();
                var artist = musicProperties.Artist;
                if (artist == "")
                    artist = "Unknown";

                var album = musicProperties.Album;
                if (album == "")
                    album = "Unknown";

                music.Artist = artist;
                music.Album = album;
                music.FileName = file.DisplayName;
                music.FilePath = file.Path;
                AllMusic.Add(music);
            }

        }
    }
}
