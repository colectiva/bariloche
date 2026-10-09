using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace La_Colectiva.Models.Api
{
    public interface IPaginator : INotifyPropertyChanged
    {
        int Page { get; set; }
        int Size { get; set; }
        int TotalPages { get; }
        int TotalElements { get; }
        bool CanGoLast { get; }
        bool CanGoFirst { get; }
        bool HasNext { get; }
        bool HasPrevious { get; }
        Task NextPage();
        Task PreviousPage();
        Task LastPage();
        Task FirstPage();
    }
    public class Paginator<T> : IPaginator
    {

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public enum PaginatorPolicy
        {
            Paginate, Append
        }

        public PaginatorPolicy Policy { get; set; }

        private int _page = 1;
        public int Page {
            get => _page;
            set {
                if (_page != value)
                {
                    _page = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasNext));
                    OnPropertyChanged(nameof(HasPrevious));
                    OnPropertyChanged(nameof(CanGoFirst));
                    OnPropertyChanged(nameof(CanGoLast));
                }
            }
        }

        private int _size = 5;
        public int Size {
            get => _size;
            set {
                if (_size != value)
                {
                    _size = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _totalPages = 0;
        public int TotalPages {
            get => _totalPages;
            private set {
                if (_totalPages != value)
                {
                    _totalPages = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(HasNext));
                    OnPropertyChanged(nameof(CanGoFirst));
                    OnPropertyChanged(nameof(CanGoLast));
                }
            }
        }

        private int _totalElements = 0;
        public int TotalElements {
            get => _totalElements;
            private set {
                if (_totalElements != value)
                {
                    _totalElements = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool HasNext => Page < TotalPages;
        public bool HasPrevious => Page > 1;

        public Visibility ShowNextButton => HasNext ? Visibility.Visible : Visibility.Collapsed;

        public bool CanGoFirst => Page != 1;
        public bool CanGoLast => Page < TotalPages;

        private Func<int, int, Task<Page<T>>> _loader;

        public ObservableCollection<T> Items { get; } = new ObservableCollection<T>();

        public Paginator() { }

        public void Add(IEnumerable<T> items)
        {
            if (Policy == PaginatorPolicy.Paginate) Items.Clear();
            foreach (var item in items)
            {
                Items.Add(item);
            }
        }

        public void Clear()
        {
            Items.Clear();
        }

        public Paginator<T> Loader(Func<int, int, Task<Page<T>>> loader)
        {
            _loader = loader;
            return this;
        }

        public Paginator<T> PageSize(int size)
        {
            _size = size;
            return this;
        }

        public Paginator<T> Append()
        {
            this.Policy = PaginatorPolicy.Append;
            return this;
        }

        public async Task<Paginator<T>> Load()
        {
            return await Go(Page);
        }

        public async Task<Paginator<T>> Go(int page)
        {
            if (_loader == null || page < 1 || (TotalPages > 0 && page > TotalPages))
                return this;
            try
            {
                var data = await this._loader.Invoke(page - 1, Size);
                if (data.Pageable != null)
                {
                    Add(data.Content);
                    this.Page = data.Pageable.Number + 1;
                    this.TotalPages = data.Pageable.TotalPages;
                    this.TotalElements = data.Pageable.TotalElements;
                }
            }
            catch (NullReferenceException e)
            {

            }

            return this;
        }

        public async Task<Paginator<T>> Next()
        {
            if (!HasNext) return this;
            return await Go(Page + 1);
        }

        public async Task<Paginator<T>> Previous()
        {
            if (!HasPrevious) return this;
            return await Go(Page - 1);
        }

        public async Task<Paginator<T>> First()
        {
            return await Go(1);
        }

        public async Task<Paginator<T>> Last()
        {
            return await Go(TotalPages);
        }


        public async Task NextPage()
        {
            await Next();
        }
        public async Task PreviousPage()
        {
            await Previous();
        }
        public async Task LastPage()
        {
            await Last();
        }
        public async Task FirstPage()
        {
            await First();
        }

    }
}
