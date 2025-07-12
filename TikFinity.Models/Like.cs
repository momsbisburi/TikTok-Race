using Newtonsoft.Json;
using System.Collections.Generic;

namespace TikFinity.Models
{
    public class Likes
    {
        [JsonProperty("likeCount")]
        public int LikeCount { get; set; }

        [JsonProperty("totalLikeCount")]
        public int TotalLikeCount { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("secUid")]
        public string SecUid { get; set; }

        [JsonProperty("uniqueId")]
        public string UniqueId { get; set; }

        [JsonProperty("nickname")]
        public string Nickname { get; set; }

        [JsonProperty("profilePictureUrl")]
        public string ProfilePictureUrl { get; set; }

        [JsonProperty("followRole")]
        public int FollowRole { get; set; }

        [JsonProperty("userBadges")]
        public List<UserBadge> UserBadges { get; set; }

        [JsonProperty("userDetails")]
        public UserDetails UserDetails { get; set; }

        [JsonProperty("followInfo")]
        public FollowInfo FollowInfo { get; set; }

        [JsonProperty("isModerator")]
        public bool IsModerator { get; set; }

        [JsonProperty("isNewGifter")]
        public bool IsNewGifter { get; set; }

        [JsonProperty("isSubscriber")]
        public bool IsSubscriber { get; set; }

        [JsonProperty("topGifterRank")]
        public object TopGifterRank { get; set; }

        [JsonProperty("msgId")]
        public string MsgId { get; set; }

        [JsonProperty("createTime")]
        public string CreateTime { get; set; }

        [JsonProperty("displayType")]
        public string DisplayType { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }
    }
}
