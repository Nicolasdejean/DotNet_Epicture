using Epicture.Model;
using Epicture.Pages;
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
using System.Net.Http;
using System.Net.Http.Headers;
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

// Pour plus d'informations sur le modèle d'élément Page vierge, consultez la page https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Epicture
{
    /// <summary>
    /// Une page vide peut être utilisée seule ou constituer une page de destination au sein d'un frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<ClientImage> _imagelist;
        static public IOAuth2Token userToken;
        public IGalleryItem saveSlot;
        public List<IGalleryItem> _imageGallery;
        static public string _clientId = "56fc55b3d27ad8f";
        static public string _clientSecret = "a06508e384599cd02491b3d24a9a1eacaba75f64";
        public MainPage()
        {
            this.InitializeComponent();
            _imagelist = new ObservableCollection<ClientImage>();
            _imageGallery = new List<IGalleryItem>();
            DataContext = this;
            saveSlot = null;
            Loaded += MainPage_Loaded;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ConnectUser.SavedToken += ConnectUser_SavedToken;
            UploadImage.doneUploading += UploadImage_doneUploading;
            FavoriteImage.backMenu += FavoriteImage_backMenu;
            ConsultImage.backMenu += ConsultImage_backMenu;
        }

        private async void ConsultImage_backMenu(object sender, EventArgs e)
        {
            await GetGallery();
            ConsultImage.Visibility = Visibility.Collapsed;
            ImageGrid.Visibility = Visibility.Visible;
            commandBar.Visibility = Visibility.Visible;
            research.Visibility = Visibility.Visible;
            textReasearch.Visibility = Visibility.Visible;
            textFilter.Visibility = Visibility.Visible;
            filter.Visibility = Visibility.Visible;

        }

        private void FavoriteImage_backMenu(object sender, EventArgs e)
        {
            FavouriteImage.Visibility = Visibility.Collapsed;
            ImageGrid.Visibility = Visibility.Visible;
            commandBar.Visibility = Visibility.Visible;
            research.Visibility = Visibility.Visible;
            textReasearch.Visibility = Visibility.Visible;
            textFilter.Visibility = Visibility.Visible;
            filter.Visibility = Visibility.Visible;


        }

        private void UploadImage_doneUploading(object sender, EventArgs e)
        {
            ImageGrid.Visibility = Visibility.Visible;
            commandBar.Visibility = Visibility.Visible;
            research.Visibility = Visibility.Visible;
            textReasearch.Visibility = Visibility.Visible;
            textFilter.Visibility = Visibility.Visible;
            filter.Visibility = Visibility.Visible;

            UploadImage.Visibility = Visibility.Collapsed;
        }

        private async void ConnectUser_SavedToken(object sender, IOAuth2Token e)
        {
            userToken = e;
            ImageGrid.Visibility = Visibility.Visible;
            commandBar.Visibility = Visibility.Visible;
            research.Visibility = Visibility.Visible;
            textReasearch.Visibility = Visibility.Visible;
            textFilter.Visibility = Visibility.Visible;
            filter.Visibility = Visibility.Visible;
            ConnectUser.Visibility = Visibility.Collapsed;
            await GetGallery();
        }

        public async Task GetGallery()
        {
            try
            {
                FavouriteImage.main();
                var imageCount = 0;
                var client = new ImgurClient(_clientId, _clientSecret);
                var endpoint = new GalleryEndpoint(client);
                var tmpGallery = await endpoint.GetRandomGalleryAsync();
                _imageGallery = tmpGallery.ToList();

                foreach (IGalleryItem img in _imageGallery)
                {
                    var galleryAlbum = img as IGalleryAlbum;
                    if (galleryAlbum != null)
                    {
                        foreach (IImage albumImage in ((IGalleryAlbum)img).Images)
                        {
                            Debug.WriteLine("Album Image: " + albumImage.Link + "   " + albumImage.Id);
                            imageCount += 1;
                            if (albumImage.Link.Contains(".gif"))
                                continue;
                            Uri imageUri = new Uri(albumImage.Link);
                            var newImage = new ClientImage(new BitmapImage(imageUri), albumImage.Id, true);
                            _imagelist.Add(newImage);
                        }
                    }
                    else
                    {
                        var galleryImage = img as IGalleryImage;
                        imageCount += 1;
                        if (galleryImage.Link.Contains(".gif"))
                            continue;
                        Uri imageUri = new Uri(galleryImage.Link);
                        var newImage = new ClientImage(new BitmapImage(imageUri), galleryImage.Id, false);
                        _imagelist.Add(newImage);
                    }
                }
            }
            catch (ImgurException imgurEx)
            {
                Debug.Write("An error occurred getting an image from Imgur.");
                Debug.Write(imgurEx.Message);
            }
        }

        public async Task GetQueryGallery(string name)
        {
            try
            {
                FavouriteImage.main();
                var imageCount = 0;
                if (_imageGallery.Count == 0)
                {
                    var client = new ImgurClient(_clientId);
                    var endpoint = new GalleryEndpoint(client);
                    var tmpGallery = await endpoint.SearchGalleryAsync(name);
                    _imageGallery = tmpGallery.ToList();
                }

                foreach (IGalleryItem img in _imageGallery)
                {
                    var galleryAlbum = img as IGalleryAlbum;
                    if (galleryAlbum != null)
                    {
                        foreach (IImage albumImage in ((IGalleryAlbum)img).Images)
                        {
                            Debug.WriteLine("Album Image: " + albumImage.Link + "   " + albumImage.Id);
                            imageCount += 1;
                            if (albumImage.Link.Contains(".gif"))
                                continue;
                            Uri imageUri = new Uri(albumImage.Link);
                            var newImage = new ClientImage(new BitmapImage(imageUri), albumImage.Id, true);
                            _imagelist.Add(newImage);
                        }
                    }
                    else
                    {
                        var galleryImage = img as IGalleryImage;
                        imageCount += 1;
                        if (galleryImage.Link.Contains(".gif"))
                            continue;
                        Debug.WriteLine("Image: " + galleryImage.Link);
                        Uri imageUri = new Uri(galleryImage.Link);
                        var newImage = new ClientImage(new BitmapImage(imageUri), galleryImage.Id, false);
                        _imagelist.Add(newImage);
                    }
                }
            }
            catch (ImgurException imgurEx)
            {
                Debug.Write("An error occurred getting an image from Imgur.");
                Debug.Write(imgurEx.Message);
            }
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            ImageGrid.Visibility = Visibility.Collapsed;
            commandBar.Visibility = Visibility.Collapsed;
            research.Visibility = Visibility.Collapsed;
            textReasearch.Visibility = Visibility.Collapsed;
            textFilter.Visibility = Visibility.Collapsed;
            filter.Visibility = Visibility.Collapsed;
            UploadImage.Visibility = Visibility.Visible;
        }

        private async void Refresh_Click(object sender, RoutedEventArgs e)
        {
            await GetGallery();
        }

        private void Favorite_Click(object sender, RoutedEventArgs e)
        {
            ImageGrid.Visibility = Visibility.Collapsed;
            commandBar.Visibility = Visibility.Collapsed;
            research.Visibility = Visibility.Collapsed;
            textFilter.Visibility = Visibility.Collapsed;
            filter.Visibility = Visibility.Collapsed;
            textReasearch.Visibility = Visibility.Collapsed;
            FavouriteImage.Visibility = Visibility.Visible;
            FavouriteImage.main();
        }

        private void Image_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var id = (string)((Image)sender).Tag;

            foreach (ClientImage img in _imagelist)
            {
                if (id == img._id)
                {
                    ConsultImage._image = img;
                    ConsultImage._image._isFavourite = FavouriteImage.isFavourite(id);
                    ImageGrid.Visibility = Visibility.Collapsed;
                    ConsultImage.Visibility = Visibility.Visible;
                    commandBar.Visibility = Visibility.Collapsed;
                    textFilter.Visibility = Visibility.Collapsed;
                    filter.Visibility = Visibility.Collapsed;
                    research.Visibility = Visibility.Collapsed;
                    textReasearch.Visibility = Visibility.Collapsed;
                    ConsultImage.setSourceImage();
                    break;
                }
            }

        }

        private async void research_Click(object sender, RoutedEventArgs e)
        {
            var text = textReasearch.Text;
            if (text != null)
            {
                _imagelist.Clear();
                _imageGallery.Clear();
                await GetQueryGallery(text);
            }
        }

        private void filter_Click(object sender, RoutedEventArgs e)
        {
            //filter in size and type
        }
    }
}
    