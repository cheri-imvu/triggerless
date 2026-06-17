using System.Collections.ObjectModel;

namespace ExposeOutfitsApp.Models
{
    public class Room : ObservableCollection<Product>
    {
        public Room() : base() { }

        public Room(IEnumerable<Product> products) : base(products) { }
    }
}
