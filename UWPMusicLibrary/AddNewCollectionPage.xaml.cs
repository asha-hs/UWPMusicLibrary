using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UWPMusicLibrary.Model;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPMusicLibrary
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNewCollectionPage : Page
    {
        // collection of all music
        public ObservableCollection<Song> AllMusic;

        // Collection of music selected by user
        public ObservableCollection<Song> SelectedMusic;

        private string context;
        public AddNewCollectionPage()
        {
            this.InitializeComponent();
            AllMusic = new ObservableCollection<Song>();
            SelectedMusic = new ObservableCollection<Song>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //load collection of music from SongManager
            await SongManager.GetAllMusic(AllMusic);

            context = e.Parameter as string;
            bool retval = true;
            if(!string.IsNullOrEmpty(context))
            {
                CollectionName.Text = context;
                retval = SongManager.GetAllMusicCollectionsByName(SelectedMusic, context);
                if(retval)
                {
                    List<Song> selmusics = new List<Song>();
                    foreach(Song so in SelectedMusic)
                    {
                        selmusics.Add(so);
                    }
                    MusicSelectedGrid.ItemsSource = AllMusic;
                    foreach(Song s in selmusics)
                    {
                        MusicSelectedGrid.SelectedItems.Add(s);
                    }
                }else
                {
                    var messageDialog = new MessageDialog("OOPS Something went wrong, Try Again or Contact App Support");
                    messageDialog.Commands.Add(new UICommand("OK"));
                    await messageDialog.ShowAsync();
                    this.Frame.Navigate(typeof(MusicCollectionsPage));
                }

            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Song coverArt = MusicSelectedGrid.SelectedItem as Song;

            if(coverArt == null)
            {
                coverArt = SelectedMusic[0];
            }
            string albumName = CollectionName.Text;
            MusicCollection newCollection = new MusicCollection
            {
                CoverArtThumbnail = coverArt.Thumbnail,
                Name = albumName,
                CoverPhotoFilePath = coverArt.FilePath

            };
            bool retval = true;
            retval = await SongManager.AddModifyMusicCollection(context, newCollection, SelectedMusic);

            if(!retval)
            {
                //display error message to user and go back to music collections page
                var messageDialog = new MessageDialog("OOPS Something went wrong, Try Again or Contact App Support");
                messageDialog.Commands.Add(new UICommand("OK"));
                await messageDialog.ShowAsync();
            }
            this.Frame.Navigate(typeof(MusicCollectionsPage));
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MusicSelectionGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SaveButton.Visibility = Visibility.Visible;
            CollectionName.Visibility = Visibility.Visible;
            ChooseMusicMsg.Visibility = Visibility.Visible;
            MsgTextBlock.Visibility = Visibility.Visible;
            var musics = MusicSelectionGrid.SelectedItems.ToList();
            SelectedMusic.Clear();
            foreach(Song s in musics) { SelectedMusic.Add(s); }
        }

        private void SelectedMusic_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //just show the flyout
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }
    }
}
