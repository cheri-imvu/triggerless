using System;
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
}
