using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IoTMcuEdit
{
    public class LcdObj : INotifyPropertyChanged
    {
        private string _IP;
        private int _Port;
        private int _X;
        private int _Y;
        private string _Name;

        public string IP
        {
            set
            {
                UpdateProperty(ref _IP, value);
            }
            get
            {
                return _IP;
            }
        }
        public int Port
        {
            set
            {
                UpdateProperty(ref _Port, value);
            }
            get
            {
                return _Port;
            }
        }
        public int X
        {
            set
            {
                UpdateProperty(ref _X, value);
            }
            get
            {
                return _X;
            }
        }
        public int Y
        {
            set
            {
                UpdateProperty(ref _Y, value);
            }
            get
            {
                return _Y;
            }
        }
        public string Name
        {
            set
            {
                UpdateProperty(ref _Name, value);
            }
            get
            {
                return _Name;
            }
        }

        private void UpdateProperty<T>(ref T properValue, T newValue, [CallerMemberName] string propertyName = "")
        {
            if (Equals(properValue, newValue))
            {
                return;
            }
            properValue = newValue;

            OnPropertyChanged(propertyName);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
