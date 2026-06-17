using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExposeOutfitsApp.Models
{
    public class Product : INotifyPropertyChanged
    {
        public long Id { get; set; }

        private string? _name;
        public string? Name
        {
            get => _name;
            set { _name = value; OnPropertyChanged(); }
        }

        private string? _imageUrl;
        public string? ImageUrl
        {
            get => _imageUrl;
            set { _imageUrl = value; OnPropertyChanged(); }
        }

        private string? _creatorName;
        public string? CreatorName
        {
            get => _creatorName;
            set { _creatorName = value; OnPropertyChanged(); }
        }

        private long _creatorId;
        public long CreatorId
        {
            get => _creatorId;
            set { _creatorId = value; OnPropertyChanged(); }
        }

        public bool IsHidden =>
            string.IsNullOrWhiteSpace(Name) ||
            string.IsNullOrWhiteSpace(ImageUrl);

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
