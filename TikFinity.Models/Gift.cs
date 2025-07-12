using Newtonsoft.Json;
using System.Collections.Generic;

namespace TikFinity.Models
{
    public class GiftDetails
    {
        [JsonProperty("gift_id")]
        public int GiftId { get; set; }

        [JsonProperty("repeat_count")]
        public int RepeatCount { get; set; }

        [JsonProperty("repeat_end")]
        public int RepeatEnd { get; set; }

        [JsonProperty("gift_type")]
        public int GiftType { get; set; }
    }
    public class Gift
    {
        [JsonProperty("giftId")]
        public int GiftId { get; set; }

        [JsonProperty("repeatCount")]
        public int RepeatCount { get; set; }

        [JsonProperty("repeatEnd")]
        public bool RepeatEnd { get; set; }

        [JsonProperty("groupId")]
        public string GroupId { get; set; }

        [JsonProperty("monitorExtra")]
        public MonitorExtra MonitorExtra { get; set; }

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
        public List<object> UserBadges { get; set; }

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

        [JsonProperty("gift")]
        public GiftDetails GiftDetails { get; set; }

        [JsonProperty("describe")]
        public string Describe { get; set; }

        [JsonProperty("giftType")]
        public int GiftType { get; set; }

        [JsonProperty("diamondCount")]
        public int DiamondCount { get; set; }

        [JsonProperty("giftName")]
        public string GiftName { get; set; }

        [JsonProperty("giftPictureUrl")]
        public string GiftPictureUrl { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("receiverUserId")]
        public string ReceiverUserId { get; set; }
    }
}
