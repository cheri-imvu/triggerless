using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Models
{
    // ReSharper disable once ClassNeverInstantiated.Local
    public class ProductList
    {
        // ReSharper disable once UnusedAutoPropertyAccessor.Local
        public ProductItem[] productArray { get; set; }
    }

    // ReSharper disable once ClassNeverInstantiated.Local
    public class ProductItem
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Local
        public string url { get; set; }
        public string name { get; set; }
        public string original_dimensions { get; set; }
        public string[] tags { get; set; }
        public byte[] content { get; set; }
        public long length { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Local
    }

}
