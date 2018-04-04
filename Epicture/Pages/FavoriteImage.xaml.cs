using Epicture.Model;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace Epicture.Pages
{
    public sealed partial class FavoriteImage : UserControl
    {
        static public event EventHandler backMenu;
        public ObservableCollection<ClientImage> _favouriteList;
        public FavoriteImage()
        {
            this.InitializeComponent();
            _favouriteList = new ObservableCollection<ClientImage>();
            DataContext = this;
        }

        public async void main()
        {
            _favouriteList.Clear();
            await getFavorites();
        }

        public  bool isFavourite(string id)
        {            
            foreach (ClientImage img in _favouriteList)
            {
                if (id == img._id)
                    return true;
            }
            return false;
        }
        public async Task getFavorites()
        {
            try
            {
                var client = new ImgurClient(MainPage._clientId, MainPage.userToken);
                var endpoint = new AccountEndpoint(client);
                var favourites = await endpoint.GetAccountFavoritesAsync();

                foreach (IGalleryItem img in favourites)
                {
                    var galleryAlbum = img as IGalleryAlbum;
                    if (galleryAlbum != null)
                    {
                        foreach (IImage albumImage in galleryAlbum.Images)
                        {
                            Debug.WriteLine("Album Image: " + albumImage.Link + "   " + albumImage.Id);
                            Uri imageUri = new Uri(albumImage.Link);
                            var newImage = new ClientImage(new BitmapImage(imageUri), albumImage.Id, true);
                            _favouriteList.Add(newImage);
                        }
                    } 
                    else
                    {
                        var galleryImage = img as IGalleryImage;
                        Debug.WriteLine("Image: " + galleryImage.Link);
                        Uri imageUri = new Uri(galleryImage.Link);
                        var newImage = new ClientImage(new BitmapImage(imageUri), galleryImage.Id, false);
                        _favouriteList.Add(newImage);
                    }
                }
            }
            catch (ImgurException imgurEx)
            {
                Debug.Write("An error occurred getting an image from Imgur.");
                Debug.Write(imgurEx.Message);
            }
            catch (System.ArgumentNullException e)
            {
                Debug.WriteLine("An error occurred getting an image from Imgur." + e.Message);
            }
        }
        public async Task unFavourite(string link)
        {
            try
            {
                var client = new ImgurClient(MainPage._clientId, MainPage.userToken);
                var endpoint = new ImageEndpoint(client);
                var favorited = await endpoint.FavoriteImageAsync(link);
            }
            catch (ImgurException imgurEx)
            {
                Debug.Write("An error occurred getting an image from Imgur.");
                Debug.Write(imgurEx.Message);
            }
            catch (System.ArgumentNullException e)
            {
                Debug.WriteLine("An error occurred getting an image from Imgur." + e.Message);
            }
        }


        private async void unFavourite_Click(object sender, RoutedEventArgs e)
        {
            var obj = sender as Button;
            var id = obj.Tag as String;
            foreach (ClientImage img in _favouriteList)
            {
                if (id == img._id)
                {
                    await unFavourite(id);
                    break;
                }
            }
            _favouriteList.Clear();
            await getFavorites();
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            sendBack();
        }

        public void sendBack()
        {
            backMenu?.Invoke(null, null);

        }
}
}
