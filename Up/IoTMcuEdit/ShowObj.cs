using Lib;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace IoTMcuEdit
{
    public class ShowObj : INotifyPropertyChanged
    {
        private string _Name = "Show1";
        private int _X = 0;
        private int _Y = 0;
        private int _Time = 10000;
        private int _Size = 16;
        private FontSelfColor _Color = FontSelfColor.RED;
        private string _FontType;
        private string _Text = "测试";
        private int _Index = 0;

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
        public int Time
        {
            set
            {
                UpdateProperty(ref _Time, value);
            }
            get
            {
                return _Time;
            }
        }
        public int Size
        {
            set
            {
                UpdateProperty(ref _Size, value);
            }
            get
            {
                return _Size;
            }
        }
        public FontSelfColor Color
        {
            set
            {
                UpdateProperty(ref _Color, value);
            }
            get
            {
                return _Color;
            }
        }
        public string FontType
        {
            set
            {
                UpdateProperty(ref _FontType, value);
            }
            get
            {
                return _FontType;
            }
        }
        public string Text
        {
            set
            {
                UpdateProperty(ref _Text, value);
            }
            get
            {
                return _Text;
            }
        }
        public int Index
        {
            set
            {
                UpdateProperty(ref _Index, value);
            }
            get
            {
                return _Index;
            }
        }

        public void Bind(ShowObj obj)
        {
            obj.Color = Color;
            obj.FontType = FontType;
            obj.Name = Name;
            obj.X = X;
            obj.Y = Y;
            obj.Time = Time;
            obj.Text = Text;
            obj.Index = Index;
            obj.Size = Size;
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
