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

        private static List<MusicCollection> AllMusicCollections = new List<MusicCollection>();

        private static bool isAllMusicInitialized = false;

        private static bool isAllMusicCollectionsInitialized = false;

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

        /// <summary>
        /// This method will initialize the list of music collection that have been created previously if any!!
        /// Since it picks stuff from file, we will maintain a static collection and return it everytime the user queries
        /// </summary>
        /// <param name="albums"></param>
        /// <returns></returns
        /// 

        public static async Task<bool> GetAllMusicCollectionsAsync(ObservableCollection<MusicCollection> collections)
        {
            bool retval = true;
            if(!isAllMusicCollectionsInitialized)
            {
                StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                StorageFolder collectionsFolder;
                try
                {
                    collectionsFolder = await storageFolder.CreateFolderAsync("Collections", CreationCollisionOption.OpenIfExists);

                    if(collectionsFolder != null)
                    {
                        var files = await collectionsFolder.GetFilesAsync();
                        if (files != null)
                        {
                            foreach(StorageFile file in files)
                            {
                                string collectionName = Path.GetFileNameWithoutExtension(file.Path);
                                string coverfilePath;

                                using(StreamReader sr = new StreamReader(file.Path))
                                {
                                    coverfilePath = sr.ReadLine();

                                    sr.Close();
                                }

                                //load the file with the path
                                StorageFolder musicsFolder = KnownFolders.MusicLibrary;
                                StorageFile musicFile = await musicsFolder.GetFileAsync(Path.GetFileName(coverfilePath));

                                var thumbnail = await musicFile.GetThumbnailAsync(ThumbnailMode.SingleItem);
                                BitmapImage thumbnailImage = new BitmapImage();
                                await thumbnailImage.SetSourceAsync(thumbnail);

                                MusicCollection newCollection = new MusicCollection
                                {
                                    Name = collectionName,
                                    CoverArtThumbnail = thumbnailImage,
                                    CoverPhotoFilePath = coverfilePath
                                };

                                AllMusicCollections.Add(newCollection);
                            }

                            isAllMusicCollectionsInitialized = true;
                        }else
                        {
                            retval = false;
                        }
                    }else
                    {
                        retval = false;
                    }
                } catch(Exception)
                {
                    retval = false;
                }
            }

            AllMusicCollections.ForEach(item => collections.Add(item));
            return retval;
        }
        public static bool GetAllMusicCollectionsByName(ObservableCollection<Song> musics, string collectionName)
        {
            bool retval = true;
            try
            {
                musics.Clear();
                List<Song> musicCollection = new List<Song>();

                string path = $"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Collections\\{collectionName}.txt";
                StreamReader sr = new StreamReader(path);
                string line;
                line = sr.ReadLine();
                while (line != null)
                {
                    foreach(Song song in AllMusic)
                    {
                        if(line == song.FilePath)
                        {
                            musicCollection.Add(song);
                        }
                    }

                    line = sr.ReadLine();
                }
                sr.Close();
                musicCollection.ForEach(item => musics.Add(item));
            }
            catch(Exception)
            {
                retval = false;
            }
            return retval;
        }


        public static async Task<bool> AddModifyMusicCollection(string context, MusicCollection newCollection, ObservableCollection<Song> selectedMusic)
        {
            bool retval = true;
            try
            {
                if(!string.IsNullOrEmpty(context))
                {
                    retval = DeleteMusicCollection(context);
                }
                retval = await AddNewMusicCollection(newCollection, selectedMusic);
            }
            catch(Exception)
            {
                retval = false;
            }
            return retval;
        }

        private static async Task<bool> AddNewMusicCollection(MusicCollection newCollection, ObservableCollection<Song> selectedMusic)
        {
            StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFolder collectionsFolder;
            bool retval = true;
            try
            {
                collectionsFolder = await storageFolder.CreateFolderAsync("Collections", CreationCollisionOption.OpenIfExists);
                if (String.IsNullOrEmpty(newCollection.Name))
                {
                    int count = AllMusicCollections.Count + 1;
                    newCollection.Name = $"Collection{count}";
                }
                string path = $"{collectionsFolder.Path}//{newCollection.Name}.txt";

                using(StreamWriter sw = new StreamWriter(path,false))
                {
                    //First write the path of the coverphotoimage
                    sw.WriteLine(newCollection.CoverPhotoFilePath);
                    foreach(Song sg in selectedMusic)
                    {
                        sw.WriteLine(sg.FilePath);
                    }
                    sw.Close();

                }
                AllMusicCollections.Add(newCollection);
            }
            catch (Exception)
            {
                retval = false;
            }
            return retval;
        }

        private static bool DeleteMusicCollection(string context)
        {
            bool retval = true;
            try
            {
                //First Delete the Collection.txt file
                string path = $"{Windows.Storage.ApplicationData.Current.LocalFolder.Path}\\Collections\\{context}.txt";
                File.Delete(path);
                //Also the music collection needs to be updated
                AllMusicCollections.RemoveAll(item => context == item.Name);

            }
            catch
            {
                retval = false;
            }
            return retval;
        }
    }
}
