using GTA;
using GTA.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using TikFinity.Models;
using TikTok_Race.Models;
using WatsonWebsocket;
using static System.Net.Mime.MediaTypeNames;
using static TikFinity.Models.Logger;

namespace TikTok_Race
{
    // Token: 0x02000004 RID: 4
    public class WSC
    {
        public const VehicleHash DefaultCar = VehicleHash.Vigero;
        public const VehicleHash Paid = VehicleHash.Adder;
        public const VehicleHash VVIPCar = VehicleHash.Adder;
        // Token: 0x0600001D RID: 29 RVA: 0x000049D4 File Offset: 0x00002BD4
        public void Start(string type)
        {
            try
            {
                Uri uri = new Uri("ws://localhost:21213");
                if (type == "first")
                {
                    uri = new Uri("ws://localhost:21213");
                }
                else
                {
                    uri = new Uri("ws://localhost:21213");
                }
                this.client = new WatsonWsClient(uri, default(Guid));
                this.client.ServerConnected += this.ServerConnected;
                this.client.ServerDisconnected += this.ServerDisconnected;
                this.client.MessageReceived += this.MessageReceived;
                this.client.Start();
               
            }
            catch (Exception ex)
            {
                this.Tass = this.Tass + ex.Message.ToString() + "\n";
            }
            try
            {
                using (StreamReader streamReader = new StreamReader("./scripts/info.json"))
                {
                    string json = streamReader.ReadToEnd();
                    this.info = JObject.Parse(json);
                }
            }
            catch (Exception ex2)
            {
                this.Tass = ex2.Message.ToString();
            }
        }

        // Token: 0x0600001E RID: 30 RVA: 0x00004B0C File Offset: 0x00002D0C
        public void SendMsg(string message)
        {
            this.client.SendAsync(message, WebSocketMessageType.Text, default(CancellationToken));
        }
        private void SaveOrUpdatePlayer(string uniqueId, string nickname, string profilePicture)
        {
            using (var dbContext = new RaceDbContext())
            {
                var existingPlayer = dbContext.Player.FirstOrDefault(p => p.uniqueId == uniqueId);

                if (existingPlayer != null)
                {
                    // Update existing player
                    existingPlayer.nickname = nickname;
                    existingPlayer.ProfilePicture = profilePicture;
                }
                else
                {
                    // Add new player
                    var newPlayer = new TikTok_Race.Models.Player
                    {
                        uniqueId = uniqueId,
                        nickname = nickname,
                        ProfilePicture = profilePicture
                    };
                    dbContext.Player.Add(newPlayer);
                }

                dbContext.SaveChanges();
            }
        }
        // Token: 0x0600001F RID: 31 RVA: 0x00004B30 File Offset: 0x00002D30
        private void MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            string text = Encoding.UTF8.GetString(args.Data.Array, 0, args.Data.Count);
           // LogMessage($"message received");
            try
            {
               
                this.is_inited_m = true;
                if (this.is_inited_m)
                {
                   // LogMessage($"initiated");
                    var eventData = JsonConvert.DeserializeObject<Event>(text);
                   // LogMessage($"eventdata deserialized");
                    //LogMessage($"eventname {eventData._Event} not null");
                    if (eventData._Event!=null)
                    {
                        string eventName = eventData._Event;
                       // LogMessage($"eventname {eventName}");
                        uint num = Helpers.ComputeStringHash(eventName);
                       
                        switch (eventName)
                        {
                            case "like":
                                if (this.eventnum > 200)
                                {
                                    this.eventnum = 0;
                                }                               
                                try
                                {

                                    var likeData = JsonConvert.DeserializeObject<Likes>(eventData.Data.ToString());
                                  
                                    if (this.Game_State_T == 3 && this.HandleUsers(likeData.UniqueId, likeData.Nickname.ToString(), likeData.UserId, likeData.ProfilePictureUrl))
                                    {
                                       // LogMessage($"Rendering name: {likeData.Nickname}");
                                        this.eventrun.Add("event" + this.eventnum.ToString(), new CreatePlayerModel
                                        {
                                            Source = EventSource.Comment,
                                            NickName = likeData.UniqueId.ToString()+"("+ likeData.Nickname.ToString() + ")",
                                            Car = DefaultCar //default car
                                        });
                                        this.eventnum++;
                                    }


                                    if (this.Game_State_T == 6 && !this.ai_mode)
                                    {
                                        int id3 = this.GetId(likeData.UserId);
                                      //  LogMessage($"id3 {id3}");
                                        if (id3 != -1)
                                        {
                                            //LogMesssage($"tried to boosted");

                                            this.eventboostplayer.Add("event" + this.eventnum.ToString(), new BoostPlayerModel
                                            {
                                                Ped_ID = id3,
                                                Source = EventSource.Like,
                                                BoostStrength = 2
                                            });
                                           // LogMessage($"car boosted");
                                            this.eventnum++;
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                   // LogMessage($"error :{ex.Message}");
                                   // LogMessage($"trace :{ex.StackTrace}");
                                }
                                break;

                            case "chat":
                                var chatData = JsonConvert.DeserializeObject<Chat>(eventData.Data.ToString());
                                if (this.HandleUsers(chatData.UniqueId, chatData.Nickname.ToString(), chatData.UserId,chatData.ProfilePictureUrl))
                                {

                                    //init join command
                                    this.eventrun.Add("event" + this.eventnum.ToString(), new CreatePlayerModel
                                    {
                                        Source = EventSource.Comment,
                                        NickName = chatData.UniqueId.ToString() + "(" + chatData.Nickname.ToString() + ")",
                                        Car = DefaultCar //default car
                                    });
                                    this.eventnum++;
                                    break;
                                }
                                break;
                            case "gift":
                                var giftData = JsonConvert.DeserializeObject<Gift>(eventData.Data.ToString());
                                //string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gifts.json");
                                var giftdetails = GetGifts();
                                var giftvalue = giftdetails.GiftList.Where(x => x.id == giftData.GiftId).FirstOrDefault().diamond_count;
                                if (this.Game_State_T == 3 && this.HandleUsers(giftData.UniqueId, giftData.Nickname, giftData.UserId, giftData.ProfilePictureUrl))
                                {
                                 //   LogMessage($"Rendering name: {giftData.Nickname}");
                                    var car = VVIPCar;
                                    if (giftData.GiftId == 8913)
                                    {
                                        car = VVIPCar;
                                    }
                                    this.eventrun.Add("event" + this.eventnum.ToString(), new CreatePlayerModel
                                    {
                                        Source = EventSource.Gift,
                                        NickName = giftData.UniqueId.ToString() + "(" + giftData.Nickname.ToString() + ")",
                                        Car = car //paidCar
                                    });
                                    this.eventnum++;
                                }
                                if (this.Game_State_T == 6 && !this.ai_mode)
                                {
                                    int id3 = this.GetId(giftData.UserId);
                                    if (id3 != -1)
                                    {
                                        //TODO : calc boost strength based on gift type
                                      
                                       // var giftvalue = giftdetails.GiftList.Where(x=>x.id == giftData.GiftId).FirstOrDefault().diamond_count;
                                      //  LogMessage($"gift {giftData.GiftId} ,value: {giftvalue}");
                                        var istrue = false;
                                        if(giftData.GiftId == 8913 || giftData.GiftId == 5827 || giftData.GiftId == 5879)
                                        {
                                            istrue= true;
                                        }
                                        this.eventboostplayer.Add("event" + this.eventnum.ToString(), new BoostPlayerModel
                                        {
                                            Ped_ID = id3,
                                            Source = EventSource.Gift,
                                            BoostStrength = giftData.RepeatCount* giftvalue , //change boost strength here
                                            Nitro = istrue
                                        });
                                        this.eventnum++;
                                    }
                                }
                                break;
                           
                            default:
                                break;
                        }
                    }
                    else
                    {
                       // LogMessage($"event name null");
                    }//
                }
            }
            catch (Exception ex)
            {
              //  LogMessage($"error :{ex.Message}");
              //  LogMessage($"trace :{ex.StackTrace}");
            }
        }

        // Token: 0x06000020 RID: 32 RVA: 0x000052FC File Offset: 0x000034FC
        private string GetEvent(int giftid)
        {
            foreach (JToken jtoken in ((IEnumerable<JToken>)this.main_info.GetValue("events")))
            {
                JObject jobject = JObject.Parse(jtoken.ToString());
                if ((int)jobject.GetValue("giftid") == giftid)
                {
                    return jobject.GetValue("event").ToString();
                }
            }
            return null;
        }

        //TODO : WTF IS THIS??
        public void RunEvent(string ename, string name = "", bool fe = false, bool je = false)
        {
          
            this.Tass = "Invalid event!";
        }

        // Token: 0x06000022 RID: 34 RVA: 0x00005503 File Offset: 0x00003703
        private void ServerConnected(object sender, EventArgs args)
        {
            this.ConnectedB = true;
            Console.WriteLine("Server connected");
        }

        // Token: 0x06000023 RID: 35 RVA: 0x00005516 File Offset: 0x00003716
        private void ServerDisconnected(object sender, EventArgs args)
        {
            this.ConnectedB = false;
            Console.WriteLine("Server disconnected");
        }

        // Token: 0x06000024 RID: 36 RVA: 0x0000552C File Offset: 0x0000372C
        public void Drawm(int padd)
        {
            try
            {
                this.DrawText("Likes: " + this.total_likes.ToString(), 0f, Color.Red, padd);
                this.DrawText("Last message: " + this.Tass, 0f, Color.Orange, padd + 1);
            }
            catch (Exception)
            {
            }
        }

        public void ResetPlayers()
        {
            for (int i = 0; i < this.usernames_.Length; i++)
            {
                this.usernames_[i] = null;
                this.real_nicknames[i] = null;
                this.uniqueids[i] = null;
            }
            this.player_id = 0; // Reset player counter
        }

        // Token: 0x06000025 RID: 37 RVA: 0x00005598 File Offset: 0x00003798
        public void DrawText(string text, float x, Color color, int padd)
        {
            PointF pointF = new PointF(x, (float)(padd * 10));
            new TextElement(text, pointF, 1f)
            {
                Color = color,
                Scale = 0.3f
            }.Draw();
        }

        // Token: 0x06000026 RID: 38 RVA: 0x000055D8 File Offset: 0x000037D8
        public bool HandleUsers(string uniqueid, string real_nickname, string username, string profilePicture)
        {
            if (this.Game_State_T != 3)
            {
                return false;
            }
            bool flag = false;
            for (int i = 0; i < this.usernames_.Length; i++)
            {
                if (this.usernames_[i] != null && this.usernames_[i] == username)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                SaveOrUpdatePlayer(uniqueid, real_nickname, profilePicture);
                this.usernames_[this.player_id] = username;
                this.real_nicknames[this.player_id] = username;
                this.uniqueids[this.player_id] = uniqueid;
                this.player_id++;
                return true;
            }
            return false;
        }

        // Token: 0x06000027 RID: 39 RVA: 0x00005664 File Offset: 0x00003864
        public int GetId(string username)
        {
            int result = -1;
            for (int i = 0; i < this.usernames_.Length; i++)
            {
                if (this.usernames_[i] != null && this.usernames_[i] == username)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        // Token: 0x06000028 RID: 40 RVA: 0x000056A8 File Offset: 0x000038A8
        public void SendWin(int id, string token)
        {
            ResetPlayers();
        }

        // Token: 0x0400004A RID: 74
        private WatsonWsClient client;

        // Token: 0x0400004B RID: 75
        private string Tass = "";

        // Token: 0x0400004C RID: 76
        public bool ConnectedB;

        // Token: 0x0400004D RID: 77
        private JObject info;

        // Token: 0x0400004E RID: 78
        public JObject main_info;

        /// <summary>
        /// events to setup the race
        /// </summary>
        public Dictionary<string, CreatePlayerModel> eventrun = new Dictionary<string, CreatePlayerModel>();

        /// <summary>
        /// events to boost the player
        /// </summary>
        public Dictionary<string, BoostPlayerModel> eventboostplayer = new Dictionary<string, BoostPlayerModel>();

        // Token: 0x04000051 RID: 81
        public Dictionary<string, Event> eventjoinrun = new Dictionary<string, Event>();

        // Token: 0x04000052 RID: 82
        public Dictionary<string, Event> eventpetrun = new Dictionary<string, Event>();

        // Token: 0x04000053 RID: 83
        public Dictionary<string, Event> eventpets = new Dictionary<string, Event>();

        // Token: 0x04000054 RID: 84
        public Dictionary<string, Event> eventuser = new Dictionary<string, Event>();

        // Token: 0x04000055 RID: 85
        private int eventnum;
       
      

        // Token: 0x04000056 RID: 86
        public int total_likes;

        // Token: 0x04000057 RID: 87
        public int total_likesto = 500;

        // Token: 0x04000058 RID: 88
        public int total_likes2;

        // Token: 0x04000059 RID: 89
        public int total_likesto2 = 500;

        // Token: 0x0400005A RID: 90
        public int disable_time = 10;

        // Token: 0x0400005B RID: 91
        public string follow_e = "none";

        // Token: 0x0400005C RID: 92
        public string share_e = "none";

        // Token: 0x0400005D RID: 93
        public string join_e = "none";

        // Token: 0x0400005E RID: 94
        public string like_e = "none";

        // Token: 0x0400005F RID: 95
        public string like_e2 = "none";

        // Token: 0x04000060 RID: 96
        private int usernn;

        // Token: 0x04000061 RID: 97
        public bool is_inited_m;

        // Token: 0x04000062 RID: 98
        public JObject follow_users = JObject.Parse("{}");

        // Token: 0x04000063 RID: 99
        public string ws_err_msg = "";

        // Token: 0x04000064 RID: 100
        public string lic_plate = "";

        // Token: 0x04000065 RID: 101
        public bool joinlock;

        // Token: 0x04000066 RID: 102
        public bool join_like;

        // Token: 0x04000067 RID: 103
        public string[] usernames_ = new string[256];

        // Token: 0x04000068 RID: 104
        public string[] real_nicknames = new string[256];

        // Token: 0x04000069 RID: 105
        public string[] uniqueids = new string[256];

        // Token: 0x0400006A RID: 106
        public int player_id;

        /// <summary>
        /// 3 = JOIN ALLOWED
        /// 6 = RACE STARTED
        /// </summary>
        public int Game_State_T;

        // Token: 0x0400006C RID: 108
        public bool ai_mode;

        // Token: 0x0400006D RID: 109
        public bool switchx;

        // Token: 0x0400006E RID: 110
        public bool switchok;

        // Token: 0x0400006F RID: 111
        public bool switchtype = true;
    }
}
