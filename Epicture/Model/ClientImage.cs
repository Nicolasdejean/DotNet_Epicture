using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Epicture.Model
{
    public class ClientImage : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public ClientImage(BitmapImage Image, string ID = "", bool IsAlbum = false)
        {
            _image = Image;
            _id = ID;
            _isAlbum = IsAlbum;
        }

        private BitmapImage image;
        public BitmapImage _image
        {
            get { return image; }
            set
            {
                image = value;
                NotifyPropertyChanged();
            }
        }

        private string id;
        public string _id
        {
            get { return id; }
            set
            {
                id = value;
                NotifyPropertyChanged();
            }
        }

        private bool isAlbum;
        public bool _isAlbum
        {
            get { return isAlbum; }
            set
            {
                isAlbum = value;
                NotifyPropertyChanged();
            }
        }

        private bool isFavourite;
        public bool _isFavourite
        {
            get { return isFavourite; }
            set
            {
                isFavourite = value;
                NotifyPropertyChanged();
            }
        }

    }
}
