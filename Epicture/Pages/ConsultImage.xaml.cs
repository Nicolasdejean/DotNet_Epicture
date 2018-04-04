using Epicture.Model;
using Imgur.API;
using Imgur.API.Authentication.Impl;
using Imgur.API.Endpoints.Impl;
using System;
using System.Collections.Generic;
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
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrôle utilisateur, consultez la page https://go.microsoft.com/fwlink/?LinkId=234236

namespace Epicture.Pages
{
    public sealed partial class ConsultImage : UserControl
    {
        static public event EventHandler backMenu;
        static public event EventHandler updateFavoriteList;
        public ClientImage _image;

        public ConsultImage()
        {
            this.InitializeComponent();
        }
        public async Task sendFavourite()
        {
            try
            {
                var client = new ImgurClient(MainPage._clientId, MainPage.userToken);
                var endpoint = new ImageEndpoint(client);
                var favorited = await endpoint.FavoriteImageAsync(_image._id);
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


        public void setSourceImage()
        {
            displayImage.Source = _image._image;
            symbolButton.Symbol = _image._isFavourite == true ? Symbol.UnFavorite : Symbol.Favorite;
        }

        private async void addFavorite_Click(object sender, RoutedEventArgs e)
        {
            symbolButton.Symbol = symbolButton.Symbol == Symbol.Favorite ? Symbol.UnFavorite : Symbol.Favorite;
            await sendFavourite();
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
