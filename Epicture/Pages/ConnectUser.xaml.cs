using Epicture.Model;
using Imgur.API.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class ConnectUser : UserControl
    {
        public event EventHandler<IOAuth2Token> SavedToken;
        public Auth _auth;
        public ConnectUser()
        {
            if (_auth == null)
                _auth = new Auth();
            this.InitializeComponent();
        }

        private async void GetPin_Click(object sender, RoutedEventArgs e)
        {
            await _auth.AuthorizationRequest();
            GetPin.Visibility = Visibility.Collapsed;
            PinPage.Visibility = Visibility.Visible;
        }

        private async void AcceptPin_Click(object sender, RoutedEventArgs e)
        {
            var token = await _auth.ResponseCode(PinNumber.Text);
            if (token != null)
                sendToken(token);
        }

        private void sendToken(IOAuth2Token tok)
        {
            SavedToken?.Invoke(null, tok);
        }
    }
}
