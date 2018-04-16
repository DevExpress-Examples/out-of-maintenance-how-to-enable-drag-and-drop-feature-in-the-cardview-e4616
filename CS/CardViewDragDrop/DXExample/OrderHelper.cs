using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DXSample {
    public class OrderHelper {
        public ObservableCollection<Order> Orders { get; private set; }

        public OrderHelper() {
            Random rnd = new Random();

            Orders = new ObservableCollection<Order>();
            for (int i = 0; i < rnd.Next(100, 200); i++)
                Orders.Add(new Order());
        }
    }

    public class Order : INotifyPropertyChanged {
        static Random rnd = new Random();

        string _Name;
        DateTime _OrderDate;
        int _Amount;
        int _Price;
        bool _IsProcessed;

        #region Properties

        public string Name {
            get { return _Name; }
            set { SetProperty(ref _Name, value); }
        }
        public DateTime OrderDate {
            get { return _OrderDate; }
            set { SetProperty(ref _OrderDate, value); }
        }
        public int Amount {
            get { return _Amount; }
            set { SetProperty(ref _Amount, value); }
        }
        public int Price {
            get { return _Price; }
            set { SetProperty(ref _Price, value); }
        }
        public bool IsProcessed {
            get { return _IsProcessed; }
            set { SetProperty(ref _IsProcessed, value); }
        }

        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        void NotifyPropertyChanged([CallerMemberName] String propertyName = "") {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "") {
            if ((storage == null && value == null) ||
                (storage != null && storage.Equals(value)))
                return false;

            storage = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }
        #endregion

        public Order() {
            Name = RandomStringHelper.GetRandomString();
            OrderDate = new DateTime(rnd.Next(1998, 2012), rnd.Next(1, 12), rnd.Next(1, 28));
            Amount = rnd.Next(-1000, 1000);
            Price = rnd.Next(0, 10000);
            IsProcessed = rnd.NextDouble() > 0.5;
        }

    }

    public class RandomStringHelper {
        static Random rnd = new Random();
        static string letters = "abcdefghijklmnopqrstuvwxyz";

        public static string GetRandomString() {
            int length = rnd.Next(6, 20);
            string retVal = ("" + letters[rnd.Next(25)]).ToUpper();

            for (int i = 0; i < length - 1; i++)
                retVal += letters[rnd.Next(25)];

            return retVal;
        }
    }
}
