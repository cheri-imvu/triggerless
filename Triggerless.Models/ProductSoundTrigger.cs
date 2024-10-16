using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Models
{
    public class ProductSoundTrigger
    {
        public string Trigger { get; set; }
        public string Location { get; set; }
    }

    public class ProductSoundTriggerPayload
    {
        public long ProductId { get; set; }
        public string ProductName { get; set; }
        public string CreatorName { get; set; }
        public string ImageLocation { get; set; }
        public ProductSoundTrigger[] Triggers { get; set; }
    }

    public class SoundTriggerComparer : IComparer<ProductSoundTrigger>
    {
        public int Compare(ProductSoundTrigger x, ProductSoundTrigger y)
        {
            if (x == null && y == null) return 0;
            if (x.Trigger == null && y.Trigger == null) return 0;
            return x.Trigger.ToLower().CompareTo(y.Trigger.ToLower());
        }
    }

    public class ProductSoundTriggerList : List<ProductSoundTrigger>
    {
        public long ParentProductId { get; set; } = 80;
    }
}
