using Newtonsoft.Json;
using System.Collections.Generic;

namespace TikFinity.Models
{
    public class UserBadge
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("displayType")]
        public int? DisplayType { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public class UserDetails
    {
        [JsonProperty("createTime")]
        public string CreateTime { get; set; }

        [JsonProperty("bioDescription")]
        public string BioDescription { get; set; }

        [JsonProperty("profilePictureUrls")]
        public List<string> ProfilePictureUrls { get; set; }
    }

    public class FollowInfo
    {
        [JsonProperty("followingCount")]
        public int FollowingCount { get; set; }

        [JsonProperty("followerCount")]
        public int FollowerCount { get; set; }

        [JsonProperty("followStatus")]
        public int FollowStatus { get; set; }

        [JsonProperty("pushStatus")]
        public int PushStatus { get; set; }
    }

    public class MonitorExtra
    {
        [JsonProperty("anchor_id")]
        public long AnchorId { get; set; }

        [JsonProperty("from_idc")]
        public string FromIdc { get; set; }

        [JsonProperty("from_user_id")]
        public long FromUserId { get; set; }

        [JsonProperty("gift_id")]
        public int GiftId { get; set; }

        [JsonProperty("gift_type")]
        public int GiftType { get; set; }

        [JsonProperty("log_id")]
        public string LogId { get; set; }

        [JsonProperty("msg_id")]
        public long MsgId { get; set; }

        [JsonProperty("repeat_count")]
        public int RepeatCount { get; set; }

        [JsonProperty("repeat_end")]
        public int RepeatEnd { get; set; }

        [JsonProperty("room_id")]
        public long RoomId { get; set; }

        [JsonProperty("send_gift_profit_core_start_ms")]
        public int SendGiftProfitCoreStartMs { get; set; }

        [JsonProperty("send_gift_send_message_success_ms")]
        public long SendGiftSendMessageSuccessMs { get; set; }

        [JsonProperty("to_user_id")]
        public long ToUserId { get; set; }
    }

}
