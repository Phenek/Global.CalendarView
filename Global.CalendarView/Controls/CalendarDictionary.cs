using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace Global.CalendarView.Controls
{
    public class CalendarDictionary<TKey, TValue> : Dictionary<TKey, TValue>, INotifyCollectionChanged,
        INotifyPropertyChanged
    {
        public CalendarDictionary()
        {
        }

        public CalendarDictionary(int capacity) : base(capacity)
        {
        }

        public CalendarDictionary(IEqualityComparer<TKey> comparer) : base(comparer)
        {
        }

        public CalendarDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary)
        {
        }

        public CalendarDictionary(int capacity, IEqualityComparer<TKey> comparer) : base(capacity, comparer)
        {
        }

        public CalendarDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer) : base(
            dictionary, comparer)
        {
        }

        public new TValue this[TKey key]
        {
            get => base[key];
            set
            {
                TValue oldValue;
                var exist = TryGetValue(key, out oldValue);
                var oldItem = new KeyValuePair<TKey, TValue>(key, oldValue);
                base[key] = value;
                var newItem = new KeyValuePair<TKey, TValue>(key, value);
                if (exist)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace,
                        newItem, oldItem, Keys.ToList().IndexOf(key)));
                }
                else
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newItem,
                        Keys.ToList().IndexOf(key)));
                    OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                }
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        public new void Add(TKey key, TValue value)
        {
            if (!ContainsKey(key))
            {
                var item = new KeyValuePair<TKey, TValue>(key, value);
                base.Add(key, value);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item,
                    Keys.ToList().IndexOf(key)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
            }
        }

        public new bool Remove(TKey key)
        {
            TValue value;
            if (TryGetValue(key, out value))
            {
                var item = new KeyValuePair<TKey, TValue>(key, base[key]);
                var result = base.Remove(key);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item,
                    Keys.ToList().IndexOf(key)));
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
                return result;
            }

            return false;
        }

        public new void Clear()
        {
            base.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(Count)));
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null) CollectionChanged(this, e);
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null) PropertyChanged(this, e);
        }

        public void ClearCollectionChangedHandler()
        {
            if (CollectionChanged == null) return;
            var ll = CollectionChanged.GetInvocationList().ToList();
            foreach (var e in ll) CollectionChanged -= (NotifyCollectionChangedEventHandler) e;

            CollectionChanged = null;
        }
    }
}