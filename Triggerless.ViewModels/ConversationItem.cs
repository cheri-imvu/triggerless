using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.ViewModels
{
    public class ConversationItem : INotifyPropertyChanged

    {
        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        private string _author;

        public string Author
        {
            get { return _author; }
            set { 
                _author = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Author)));
            }
        }

        private string _quote;

        public string Quote
        {
            get { return _quote; }
            set { 
                _quote = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Quote)));
            }
        }


    }
}
