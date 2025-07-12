using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikFinity.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ColorImage
    {
        public string avg_color { get; set; }
        public int height { get; set; }
        public int image_type { get; set; }
        public bool is_animated { get; set; }
        public string open_web_url { get; set; }
        public string uri { get; set; }
        public List<string> url_list { get; set; }
        public int width { get; set; }
    }

    public class ColorInfo
    {
        public int color_effect_id { get; set; }
        public int color_id { get; set; }
        public ColorImage color_image { get; set; }
        public string color_name { get; set; }
        public List<string> color_values { get; set; }
        public GiftImage gift_image { get; set; }
        public bool is_default { get; set; }
    }

    public class DefaultFormat
    {
        public bool bold { get; set; }
        public string color { get; set; }
        public int font_size { get; set; }
        public bool italic { get; set; }
        public int italic_angle { get; set; }
        public bool use_heigh_light_color { get; set; }
        public bool use_remote_clor { get; set; }
        public int weight { get; set; }
    }

    public class DisplayText
    {
        public DefaultFormat default_format { get; set; }
        public string default_pattern { get; set; }
        public string key { get; set; }
        public List<object> pieces { get; set; }
    }

    public class GiftImage
    {
        public string avg_color { get; set; }
        public int height { get; set; }
        public int image_type { get; set; }
        public bool is_animated { get; set; }
        public string open_web_url { get; set; }
        public string uri { get; set; }
        public List<string> url_list { get; set; }
        public int width { get; set; }
    }

    public class GiftLabelIcon
    {
        public string avg_color { get; set; }
        public int height { get; set; }
        public int image_type { get; set; }
        public bool is_animated { get; set; }
        public string open_web_url { get; set; }
        public string uri { get; set; }
        public List<string> url_list { get; set; }
        public int width { get; set; }
    }

    public class GiftList
    {
        public bool can_put_in_gift_box { get; set; }
        public List<ColorInfo> color_infos { get; set; }
        public bool combo { get; set; }
        public string describe { get; set; }
        public int diamond_count { get; set; }
        public int duration { get; set; }
        public bool for_linkmic { get; set; }
        public GiftLabelIcon gift_label_icon { get; set; }
        public string gift_rank_recommend_info { get; set; }
        public int gift_sub_type { get; set; }
        public List<int> gift_vertical_scenarios { get; set; }
        public string gold_effect { get; set; }
        public Icon icon { get; set; }
        public int id { get; set; }
        public Image image { get; set; }
        public bool is_box_gift { get; set; }
        public bool is_broadcast_gift { get; set; }
        public bool is_displayed_on_panel { get; set; }
        public bool is_effect_befview { get; set; }
        public bool is_random_gift { get; set; }
        public LockInfo lock_info { get; set; }
        public string name { get; set; }
        public int primary_effect_id { get; set; }
        public TrackerParams tracker_params { get; set; }
        public int type { get; set; }
        public GiftPanelBanner gift_panel_banner { get; set; }
        public PreviewImage preview_image { get; set; }
    }

    public class GiftPanelBanner
    {
        public string banner_lynx_url { get; set; }
        public int banner_priority { get; set; }
        public List<object> bg_color_values { get; set; }
        public string deprecated { get; set; }
        public DisplayText display_text { get; set; }
        public LeftIcon left_icon { get; set; }
        public string schema_url { get; set; }
    }

    public class Icon
    {
        public string avg_color { get; set; }
        public int height { get; set; }
        public int image_type { get; set; }
        public bool is_animated { get; set; }
        public string open_web_url { get; set; }
        public string uri { get; set; }
        public List<string> url_list { get; set; }
        public int width { get; set; }
    }

    public class Image
    {
        public string avg_color { get; set; }
        public int height { get; set; }
        public int image_type { get; set; }
        public bool is_animated { get; set; }
        public string open_web_url { get; set; }
        public string uri { get; set; }
        public List<string> url_list { get; set; }
        public int width { get; set; }
    }

    public class LeftIcon
    {
        public string avg_color { get; set; }
        public int height { get; set; }
        public int image_type { get; set; }
        public bool is_animated { get; set; }
        public string open_web_url { get; set; }
        public string uri { get; set; }
        public List<string> url_list { get; set; }
        public int width { get; set; }
    }

    public class LockInfo
    {
        public int gift_level { get; set; }
        public bool @lock { get; set; }
        public int lock_type { get; set; }
    }

    public class PreviewImage
    {
        public string avg_color { get; set; }
        public int height { get; set; }
        public int image_type { get; set; }
        public bool is_animated { get; set; }
        public string open_web_url { get; set; }
        public string uri { get; set; }
        public List<string> url_list { get; set; }
        public int width { get; set; }
    }

    public class Gifts
    {
        public List<GiftList> GiftList { get; set; }
    }

    public class TrackerParams
    {
        public string gift_property { get; set; }
    }


}
