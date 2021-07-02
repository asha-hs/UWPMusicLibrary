using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPMusicLibrary.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Popups;
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
    public sealed partial class AllMusicDisplayPage : Page
    {
        private ObservableCollection<Song> MusicList;
        private string context;
        public AllMusicDisplayPage()
        {
            this.InitializeComponent();
            MusicList = new ObservableCollection<Song>();
        }

        async protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            context = e.Parameter as string;
            bool loaded = false;
            if(string.IsNullOrEmpty(context) || context == "AllMusic")
            {
                TitleTextBlock.Text = "All Music";
                loaded = await SongManager.GetAllMusic(MusicList);
            }

            //if the retval is true and music collection is empty - notify the user
            if(loaded && MusicList.Count == 0)
            {
                NotifyUser("No Music found in the MusicLibrary,\n" +
                    " use the ADD option of the AllMusic Menu to add music to library");
               
            }
            else if(!loaded)
            {
                NotifyUser("Opps,Something went wrong, try again!!");
                
            }
        }

        private async void NotifyUser(string message)
        {
            var messageDialog = new MessageDialog(message);
            messageDialog.Commands.Add(new UICommand("OK"));
            await messageDialog.ShowAsync();
        }
      

        private async void AllMusicGrid_ItemClick(object sender, ItemClickEventArgs e)
        {
            var song = (Song)e.ClickedItem;
            StorageFile file = await StorageFile.GetFileFromPathAsync(song.FilePath);
            
            AppMediaElement.Source = MediaSource.CreateFromStorageFile(file);

        }
    }
}
