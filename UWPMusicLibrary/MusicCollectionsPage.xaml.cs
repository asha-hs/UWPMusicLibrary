using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
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
    public sealed partial class MusicCollectionsPage : Page
    {
        public ObservableCollection<MusicCollection> AllMusicCollections;

        private bool isSelectButtonClicked = false;
        public MusicCollectionsPage()
        {
            this.InitializeComponent();
            AllMusicCollections = new ObservableCollection<MusicCollection>();
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            bool retval = await SongManager.GetAllMusicCollectionsAsync(AllMusicCollections);
            if(retval && AllMusicCollections.Count == 0)
            {
                //there are no musics to display 
                //prompt the user to use the menu option to add musics
                var messageDialog = new MessageDialog("No Albums to display. Use Add Option of Albums menu to add new albums");
                messageDialog.Commands.Add(new UICommand("OK"));
                await messageDialog.ShowAsync();
            }
            else if(!retval)
            {
                //something went wrong - tell the user to try again
                var messageDialog = new MessageDialog("OOPS!! Something went wrong, unable to load albums, Try Again or Contact App support");
                messageDialog.Commands.Add(new UICommand("OK"));
                await messageDialog.ShowAsync();
            }
            else
            {
                SelectCollection.Visibility = Visibility.Visible;
            }

        }
        private void SelectCollection_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void DeleteCollection_Click(object sender, RoutedEventArgs e)
        {
            //ask the user if he is sure of deleting the album
            var messageDialog = new MessageDialog("About to delete Album(s). Are you sure?");
            //create list of command and handlers
            messageDialog.Commands.Add(new UICommand("OK", new UICommandInvokedHandler(this.DeleteOK)));
            messageDialog.Commands.Add(new UICommand("Cancel", new UICommandInvokedHandler(this.DeleteCancel)));
            await messageDialog.ShowAsync();

        }

        private void DeleteCancel(IUICommand command)
        {
            throw new NotImplementedException();
        }

        private void DeleteOK(IUICommand command)
        {
            throw new NotImplementedException();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CollectionsGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if(!isSelectButtonClicked)
            {
                MusicCollection mc = CollectionsGrid.SelectedItem as MusicCollection;

                this.Frame.Navigate(typeof(AllMusicDisplayPage), mc.Name);
            }
        }
    }
}
