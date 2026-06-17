using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ExposeOutfitsApp.Models
{
    public class State : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        // =========================
        // Room
        // =========================

        private Room? _room;
        public Room? Room
        {
            get => _room;
            set
            {
                if (_room != value)
                {
                    _room = value;
                    OnPropertyChanged();
                }
            }
        }

        // =========================
        // Users
        // =========================

        public ObservableCollection<User> Users { get; } = new();

        // =========================
        // Progress Maximum
        // =========================

        private int _progressMaximum;
        public int ProgressMaximum
        {
            get => _progressMaximum;
            set
            {
                if (_progressMaximum != value)
                {
                    _progressMaximum = value;
                    OnPropertyChanged();
                }
            }
        }

        // =========================
        // Progress Current Value
        // =========================

        private int _progressValue;
        public int ProgressValue
        {
            get => _progressValue;
            set
            {
                if (_progressValue != value)
                {
                    _progressValue = value;
                    OnPropertyChanged();
                }
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
