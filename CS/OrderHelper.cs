using DevExpress.Mvvm;
using System;
using System.Collections.ObjectModel;

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

    public class Order : ViewModelBase {
        static Random rnd = new Random();

        string _Name;
        DateTime _OrderDate;
        int _Amount;
        int _Price;
        bool _IsProcessed;

        #region Properties

        public string Name {
            get { return _Name; }
            set { SetProperty(ref _Name, value, () => Name); }
        }
        public DateTime OrderDate {
            get { return _OrderDate; }
            set { SetProperty(ref _OrderDate, value, () => OrderDate); }
        }
        public int Amount {
            get { return _Amount; }
            set { SetProperty(ref _Amount, value, () => Amount); }
        }
        public int Price {
            get { return _Price; }
            set { SetProperty(ref _Price, value, () => Price); }
        }
        public bool IsProcessed {
            get { return _IsProcessed; }
            set { SetProperty(ref _IsProcessed, value, () => IsProcessed); }
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
