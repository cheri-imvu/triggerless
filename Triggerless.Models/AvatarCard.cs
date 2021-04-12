using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace Triggerless.Models
{
    public class AvatarCard
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string AviPicUrl { get; set; }
        public DateTime Registered { get; set; }
        public DateTime? LastLogin { get; set; }
        public string Interests { get; set; }
        public string RelationshipStatus { get; set; }
        public string Orientation { get; set; }
        public string LookingFor { get; set; }
        public string Gender { get; set; }
        public int? Age { get; set; }
        public string Tagline { get; set; }
        public string Location { get; set; }
        public int CountryCode { get; set; }
        public string LocationState { get; set; }
        public bool Online { get; set; }
        public string Availability { get; set; }
        public int BadgeCount { get; set; }
        public int BadgeLevel { get; set; }
        public List<Badge> BadgeLayout { get; set; }
        public string BadgeAreaHtml { get; set; }
        public bool? ShowAP { get; set; }
        public bool? ShowVIP { get; set; }
        public bool ShowBadgeCount { get; set; }
        public bool? ShowFlagIcon { get; set; }
        public bool? ShowFlagAv { get; set; }
        public bool? ShowMessage { get; set; }
        public int AvPicDefault { get; set; }
        public bool ShowBlock { get; set; }
        public int? WelcomeModeratorScore { get; set; }
        public int IsWelcomeModerator { get; set; }
        public bool? IsCreator { get; set; }
        public List<PublicRoom> PublicRooms { get; set; }
        public int VisibleAlbums { get; set; }
        public int? ShowMarriage { get; set; }
        public long? MarriedToPartnerCid { get; set; }
        public string MarriedToPartnerAvName { get; set; }
        public string MarriedToPartnerUrl { get; set; }
        public int? ImvuLevel { get; set; }
        public int? WallpaperId { get; set; }
        public string Status { get; set; }

        public static bool TryParse(string json, out AvatarCard result)
        {
            JObject jObject;
            try
            {
                jObject = JObject.Parse(json);
            } catch(Exception)
            {
                result = null;
                return false;
            }

            result = new AvatarCard();

            try
            {
                result.CustomerId = jObject["cid"].Value<int>();
                result.Name = jObject["avname"].Value<string>();
                result.Url = jObject["url"].Value<string>();
                result.AviPicUrl = jObject["avpic_url"].Value<string>();
                result.Registered = DateTime.Parse(jObject["registered"].Value<string>());
                string dateStr = jObject["last_login"].Value<string>();
                if (dateStr != "00/00/00") result.LastLogin = DateTime.Parse(dateStr);
                result.Interests = jObject["interests"]["full_text_string"]["raw_tag"].Value<string>();
                result.RelationshipStatus = jObject["dating"]["relationship_status"].Value<string>();
                result.Orientation = jObject["dating"]["orientation"].Value<string>();
                result.LookingFor = jObject["dating"]["looking_for"].Value<string>();
                result.Gender = jObject["gender"].Value<string>();
                string ageStr = jObject["age"].Value<string>();
                if (int.TryParse(ageStr, out var age)) result.Age = age;
                result.Tagline = jObject["tagline"].Value<string>();
                result.Location = jObject["location"].Value<string>();
                result.CountryCode = jObject["country_code"].Value<int>();
                result.LocationState = jObject["location_state"].Value<string>();
                result.Online = jObject["online"].Value<bool>();
                result.Availability = jObject["availability"].Value<string>();
                result.BadgeCount = jObject["badge_count"].Value<int>();
                result.BadgeLevel = jObject["badge_level"].Value<int>();
                result.BadgeAreaHtml = jObject["badge_area_html"].Value<string>();

                result.BadgeLayout = jObject["badge_layout"].Select(badgeToken => {
                    var token = badgeToken.First;
                    return new Badge {
                        CreatorId = token["creator_id"].Value<int>(),
                        CreatorBadgeIndex = token["creator_badge_index"].Value<int>(),
                        Name = token["name"].Value<string>(),
                        Description = token["description"].Value<string>(),
                        ImageUrl = token["image_url"].Value<string>(),
                        ImageWidth = token["image_width"].Value<int>(),
                        ImageHeight = token["image_height"].Value<int>(),
                        ImageMogileKey = token["image_mogilekey"]?.Value<string>(),
                        BadgeId = token["badgeid"].Value<string>(),
                        ReviewStatus = token["review_status"].Value<string>(),
                        FlaggerId = token["flagger_id"]?.Value<string>(),
                        FlagTime = token["flag_time"]?.Value<string>(),
                        XLoc = token["xloc"].Value<int>(),
                        YLoc = token["yloc"].Value<int>(),
                    };
                }).ToList();

                //minimum set
                result.ShowBadgeCount = jObject["show_badgecount"].Value<bool>();
                result.ShowFlagIcon = jObject["show_flag_icon"]?.Value<bool>();
                result.ShowFlagAv = jObject["show_flag_av"]?.Value<bool>();
                result.ShowMessage = jObject["show_message"]?.Value<bool>();
                result.AvPicDefault = jObject["avpic_default"].Value<int>();
                result.ShowBlock = jObject["show_block"].Value<bool>();
                result.WelcomeModeratorScore = jObject["welcome_moderator_score"].Value<int>();
                result.IsWelcomeModerator = jObject["is_welcome_moderator"].Value<int>();
                result.VisibleAlbums = jObject["visible_albums"].Value<int>();
                result.Status = jObject["status"].Value<string>();

                result.PublicRooms = jObject["public_rooms"].Children().Select(prToken => new PublicRoom
                    {
                        isAP = prToken["is_ap"].Value<bool>(),
                        isVIP = prToken["is_vip"].Value<bool>(),
                        Name = prToken["name"].Value<string>(),
                        RoomInstanceId = prToken["room_instance_id"].Value<string>()
                    }).ToList();

                //additional properties
                result.IsCreator = jObject["is_creator"]?.Value<bool>();
                result.ShowMarriage = jObject["show_marriage"]?.Value<int>();
                result.MarriedToPartnerCid = jObject["married_to_partner_cid"]?.Value<long>();
                result.MarriedToPartnerAvName = jObject["married_to_partner_avname"]?.Value<string>();
                result.MarriedToPartnerUrl = jObject["married_to_partner_url"]?.Value<string>();
                result.ImvuLevel = jObject["imvu_level"]?.Value<int>();
                result.WallpaperId = jObject["wallpaper_id"].Value<int>();

                return true;

            }
            catch (Exception)
            {
                result = null;
                return false;
            }

        }

    }

    public class PublicRoom
    {
        public string RoomInstanceId { get; set; }
        public string Name { get; set; }
        public bool isAP { get; set; }
        public bool isVIP { get; set; }

    }

    public class Badge
    {
        public string BadgeId { get; set; }
        public int CreatorId { get; set; }
        public int CreatorBadgeIndex { get; set; }
        public string Name { get; set; }
        public string ImageMogileKey { get; set; }
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        public string Description { get; set; }
        public bool AutoGrant { get; set; }
        public string BadgeType { get; set; }
        public string ReviewStatus { get; set; }
        public string FlaggerId { get; set; }
        public string FlagTime { get; set; }
        public string ImageUrl { get; set; }
        public int XLoc { get; set; }
        public int YLoc { get; set; }
    }
}
