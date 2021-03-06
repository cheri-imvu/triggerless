using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Triggerless.Models
{
    public class ImvuProduct: ImvuApiResult
    {

        [JsonProperty(PropertyName = "product_id")]
        public long Id { get; set; }

        [JsonProperty(PropertyName = "product_name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "creator_cid")]
        public long CreatorId { get; set; }

        [JsonProperty(PropertyName = "creator_name")]
        public string CreatorName { get; set; }

        public long? ParentId { get; set; } // need to look up in "relations" section

        [JsonProperty(PropertyName = "rating")]
        public string Rating { get; set; }

        [JsonProperty(PropertyName = "product_price")]
        public int Price { get; set; }

        [JsonProperty(PropertyName = "discount_price")]
        public int DiscountPrice { get; set; }

        [JsonProperty(PropertyName = "product_page")]
        public string ProductPage { get; set; }

        [JsonProperty(PropertyName = "creator_page")]
        public string CreatorPage { get; set; }

        [JsonProperty(PropertyName = "is_bundle")]
        public bool IsBundle { get; set; }

        [JsonProperty(PropertyName = "profit")]
        public int Profit { get; set; }

        [JsonProperty(PropertyName = "allows_derivation")]
        public int AllowsDerivation { get; set; }

        [JsonProperty(PropertyName = "allows_third_party_bundles")]
        public int AllowsThirdPartyBundle { get; set; }

        [JsonProperty(PropertyName = "product_image")]
        public string ProductImage { get; set; }

        [JsonProperty(PropertyName = "image_file_name")]
        public string ImageFilename { get; set; }

        [JsonProperty(PropertyName = "category_path")]
        public ImvuProductCategory[] CategoryPath { get; set; }

        [JsonProperty(PropertyName = "gender")]
        public string Gender { get; set; }

        [JsonProperty(PropertyName = "categories")]
        public string[] Categories { get; set; }

        [JsonProperty(PropertyName = "look_url")]
        public string LookURL { get; set; }

        [JsonProperty(PropertyName = "is")]
        public string[] Is { get; set; }

        [JsonProperty(PropertyName = "compatible_body_patterns")]
        public int[] CompatibleBodyPatterns { get; set; }

        [JsonProperty(PropertyName = "is_bundlable")]
        public int IsBundlable { get; set; }

        [JsonProperty(PropertyName = "is_visible")]
        public bool IsVisible { get; set; }

        [JsonProperty(PropertyName = "is_wearable_in_pure")]
        public bool IsWearableInPure { get; set; }

        [JsonProperty(PropertyName = "node_id")]
        public string NodeId { get; set; }

        [JsonProperty(PropertyName = "is_purchasable")]
        public bool IsPurchasable { get; set; }

        [JsonProperty(PropertyName = "asset_url")] //not seen for clothing
        public string AssetURL { get; set; }

        [JsonProperty(PropertyName = "default_orientation")]
        public string DefaultOrientation { get; set; }

        [JsonProperty(PropertyName = "tags")]
        public string[] Tags { get; set; }

        [JsonProperty(PropertyName = "preview_image_supports_no_redirect")]
        public bool PreviewImageSupportsNoRedirect { get; set; }

        [JsonProperty(PropertyName = "preview_image")]
        public string PreviewImage { get; set; }

    }

    public class ImvuProductCategory
    {

        [JsonProperty(PropertyName = "id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }

    public class ImvuProductRelations
    {
        [JsonProperty(PropertyName = "creator")]
        public string Creator { get; set; }
        [JsonProperty(PropertyName = "uml_products")]
        public string UmlProducts { get; set; }
        [JsonProperty(PropertyName = "ancestor_products")]
        public string AncestorProducts { get; set; }
        [JsonProperty(PropertyName = "parent")]
        public string Parent { get; set; }
        [JsonProperty(PropertyName = "viewer_inventory")]
        public string ViewerInventory { get; set; }
    }

    public class ImvuProductList
    {
        public ImvuProduct[] Products { get; set; }
    }

}
