using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using Imgur.API.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace Epicture.Pages
{
    public sealed partial class UploadImage : UserControl
    {
        public string _clientId = "56fc55b3d27ad8f";
        public string _clientSecret = "a06508e384599cd02491b3d24a9a1eacaba75f64";
        public event EventHandler doneUploading;


        public UploadImage()
        {
            this.InitializeComponent();
        }

        public async Task uploadImage(IOAuth2Token token, Stream location)
        {
            try
            {
                var client = new ImgurClient(_clientId, _clientSecret, token);
                var endpoint = new ImageEndpoint(client);
                IImage image;
                image = await endpoint.UploadImageStreamAsync(location);
                Debug.Write("Image uploaded. Image Url: " + image.Link);
                sendToken();
            }
            catch (ImgurException imgurEx)
            {
                Debug.Write("An error occurred uploading an image to Imgur.");
                Debug.Write(imgurEx.Message);
            }
            catch (System.ArgumentNullException e)
            {
                Debug.Write("An error occurred uploading an image to Imgur.");
                Debug.Write(e.Message);
            }
        }
        public async Task uploadURLImage(IOAuth2Token token, string url)
        {
            try
            {
                var client = new ImgurClient(_clientId, _clientSecret, token);
                var endpoint = new ImageEndpoint(client);
                var image = await endpoint.UploadImageUrlAsync(url);
                Debug.Write("Image uploaded. Image Url: " + image.Link);
                sendToken();
            }
            catch (ImgurException imgurEx)
            {
                Debug.Write("An error occurred uploading an image to Imgur.");
                Debug.Write(imgurEx.Message);
            }
            catch (System.ArgumentNullException e)
            {
                Debug.Write("An error occurred uploading an image to Imgur.");
                Debug.Write(e.Message);
            }
        }

        public async Task<Stream> fileToStream()
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.Thumbnail;
            openPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            Windows.Storage.StorageFile file = await openPicker.PickSingleFileAsync();

            if (file != null)
            {
                var fileStream = await file.OpenAsync(FileAccessMode.Read);
                return fileStream.AsStream();
            }
            return null;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            sendToken();
        }

        private async void UploadDevice_Click(object sender, RoutedEventArgs e)
        {
            var image = await fileToStream();
            if (image == null)
                return;
            await uploadImage(MainPage.userToken, image);
        }

        private async void UploadURL_Click(object sender, RoutedEventArgs e)
        {
            var image = UploadLocation.Text;
            UploadLocation.Text = string.Empty;
            await uploadURLImage(MainPage.userToken, image);
        }

        private void sendToken()
        {
            Debug.WriteLine("sending Token");
            doneUploading?.Invoke(null, null);
        }
    }
}
