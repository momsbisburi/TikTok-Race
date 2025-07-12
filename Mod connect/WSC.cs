using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using GTA.UI;
using Newtonsoft.Json.Linq;
using WatsonWebsocket;
using static System.Net.Mime.MediaTypeNames;

namespace TikTok_Race
{
	// Token: 0x02000004 RID: 4
	public class WSC
	{
		// Token: 0x0600001D RID: 29 RVA: 0x000049D4 File Offset: 0x00002BD4
		public void Start(string type)
		{
			try
			{

				CreateLogFileIfNeeded();
				this.DrawText("Test: " + this.total_likes.ToString(), 0f, Color.Red, 1);
				Uri uri = new Uri("wss://keepmy.live:2096");
				if (type == "first")
				{
					//uri = new Uri("wss://keepmy.live:2096");
					uri = new Uri("wss://keepmy.live:2096");
				}
				else
				{
					uri = new Uri("wss://keepmy.live:2096");

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

        private void CreateLogFileIfNeeded()
        {
            string directoryPath = "./scripts"; // Define the folder path (relative or absolute)
            string filePath = Path.Combine(directoryPath, "debug_log.txt");
            string filePath3 = Path.Combine(directoryPath, "array.txt");
            string filePath2 = Path.Combine(directoryPath, "count.txt");

            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            // If the log file doesn't exist, create it
            if (!File.Exists(filePath))
            {
                // Optionally, add an initial log message when the server starts
                File.AppendAllText(filePath, $"{DateTime.Now}: Server started and log file created.\n");
                File.AppendAllText(filePath3, $"{DateTime.Now}: Server started and log file created.\n");
                File.AppendAllText(filePath2, $"{DateTime.Now}: Server started and log file created.\n");
            }
        }

        // Token: 0x0600001E RID: 30 RVA: 0x00004B0C File Offset: 0x00002D0C
        public void SendMsg(string message)
		{
			this.client.SendAsync(message, WebSocketMessageType.Text, default(CancellationToken));
		}

        private void LogMessage(string message)
        {
            string directoryPath = "./scripts"; // Define the folder path (relative or absolute)
            string filePath = Path.Combine(directoryPath, "debug_log.txt");
            string filePath3 = Path.Combine(directoryPath, "array.txt");
            string filePath2 = Path.Combine(directoryPath, "count.txt");
            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            // Append the message to the file (or create the file if it doesn't exist)
            File.AppendAllText(filePath, $"{DateTime.Now}: {message}\n");
           
        }
        private void LogMessage2(string message)
        {
            string directoryPath = "./scripts"; // Define the folder path (relative or absolute)
           
            string filePath3 = Path.Combine(directoryPath, "array.txt");
           
            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            // Append the message to the file (or create the file if it doesn't exist)
           
            File.AppendAllText(filePath3, $"{DateTime.Now}: {message}\n");
            
        }
        private void LogMessage3(string message)
        {
            string directoryPath = "./scripts"; // Define the folder path (relative or absolute)
           
            
            string filePath2 = Path.Combine(directoryPath, "count.txt");
            // Ensure the directory exists
            Directory.CreateDirectory(directoryPath);

            // Append the message to the file (or create the file if it doesn't exist)
           
            File.AppendAllText(filePath2, $"{DateTime.Now}: {message}\n");
        }

        // Token: 0x0600001F RID: 31 RVA: 0x00004B30 File Offset: 0x00002D30
        private void MessageReceived(object sender, MessageReceivedEventArgs args)
		{

            string text = Encoding.UTF8.GetString(args.Data.Array, 0, args.Data.Count);

            // Step 2: Log the received message for debugging
            //LogMessage($"Received raw message: {text}");

			// Step 3: Parse the raw JSON message
			//JObject jsonMessage = JObject.Parse(text);
			//string messageType = jsonMessage["type"]?.ToString();
			//string uniqueId = jsonMessage["uniqueId"]?.ToString();
			//string rNickname = jsonMessage["r_nickname"]?.ToString();
			//string userId = jsonMessage["userId"]?.ToString();
			//string nickname = jsonMessage["nickname"]?.ToString();
			string nic2 =  ",";

			//if (messageType == "join")
			//{
   //             if (this.Game_State_T == 3 && this.HandleUsers(uniqueId, rNickname, userId))
   //             {
   //                 this.eventrun.Add("event" + this.eventnum.ToString(), nic2);

   //                 this.eventnum++;
   //                 //LogMessage(jsonMessage);
   //             }
   //         }
   //         if (messageType == "comment")
   //         {
   //             if (this.Game_State_T == 6 && !this.ai_mode)
   //             {
   //                 int id3 = this.GetId(uniqueId.ToString());
   //                 nic2 = id3.ToString() + ",comment";
   //                 if (id3 != -1)
   //                 {
   //                     this.eventboostplayer.Add("event" + this.eventnum.ToString(), id3.ToString() ?? "");
   //                     this.eventnum++;
   //                 }
   //             }
   //         }

    //        else if (messageType == "vipjoin")
    //        {
    ////            if (this.Game_State_T == 3 && this.HandleUsers(uniqueId, rNickname, userId))
    ////            {
    ////                 nic2 = nickname + ",batmobile";
    ////                this.eventrun.Add("event" + this.eventnum.ToString(), nic2);

    ////                this.eventnum++;
				////	//LogMessage(jsonMessage);
				////}
				////else  if (this.Game_State_T == 6 && !this.ai_mode)
    ////                {
    ////                    int id3 = this.GetId(uniqueId.ToString());
    ////                    nic2 = id3.ToString() + ",gift";
    ////                if (id3 != -1)
    ////                    {
    ////                        this.eventboostplayer.Add("event" + this.eventnum.ToString(), id3.ToString() ?? "");
    ////                        this.eventnum++;
    ////                    }
    ////                }
                
    //        }
     //       else if (messageType == "tap")
     //       {
     //           if (this.Game_State_T == 3 && this.HandleUsers(uniqueId, rNickname, userId))
     //           {
     //               nic2 = nickname + ",";
     //               this.eventrun.Add("event" + this.eventnum.ToString(), nic2);

     //               this.eventnum++;
     //               //LogMessage(jsonMessage);
     //           }
     //           else if (this.Game_State_T == 6 && !this.ai_mode)
     //           {
     //               int id3 = this.GetId(uniqueId.ToString());
					//nic2 = id3.ToString() + ",tap";
     //               if (id3 != -1)
     //               {
     //                   this.eventboostplayer.Add("event" + this.eventnum.ToString(), id3.ToString() ?? "");
     //                   this.eventnum++;
     //               }
     //           }

     //       }

            try
			{
				if (text.StartsWith("infr22"))
				{
					text = text.Replace("infr22", "");
					JObject jobject = JObject.Parse(text.ToString());
					this.main_info = jobject;
					this.total_likesto = (int)jobject.GetValue("likes");
					this.total_likesto2 = (int)jobject.GetValue("likes2");
					this.disable_time = (int)jobject.GetValue("disable_controls");
					this.follow_e = jobject.GetValue("follow_event").ToString();
					this.share_e = jobject.GetValue("share_event").ToString();
					this.join_e = jobject.GetValue("join_event").ToString();
					this.like_e = jobject.GetValue("like_event").ToString();
					this.like_e2 = jobject.GetValue("like_event2").ToString();
					this.is_inited_m = true;
				}
				else if (text.StartsWith("bllx1"))
				{
					text = text.Replace("bllx1", "");
					this.ws_err_msg = text;
				}
				else if (text.StartsWith("switch"))
				{
					this.switchx = true;
				}
				else if (text.StartsWith("okswitch"))
				{
					this.switchok = true;
					this.is_inited_m = true;
				}
				else if (text.StartsWith("testme2"))
				{
					this.switchok = true;
				}
				else if (this.is_inited_m)
				{
					JObject jobject2 = JObject.Parse(text);
					jobject2.Values();
					if (jobject2.ContainsKey("type"))
					{
						string text2 = jobject2.GetValue("type").ToString();
						uint num = Helpers.ComputeStringHash(text2);
						if (num <= 904404814U)
						{
							if (num != 199741238U)
							{
								if (num != 619841764U)
								{
									if (num == 904404814U)
									{
										if (!(text2 == "subcom"))
										{
										}
									}
								}
								else if (text2 == "message")
								{
									if (jobject2.GetValue("comment").ToString().StartsWith("!join") && this.HandleUsers(jobject2.GetValue("uniqueId").ToString(), jobject2.GetValue("r_nickname").ToString(), jobject2.GetValue("userId").ToString()))
									{
                                        nic2 = jobject2.GetValue("nickname").ToString() + ",";
                                        this.eventrun.Add("event" + this.eventnum.ToString(), nic2);
										this.eventnum++;
									}
									
								}
							}
							else if (text2 == "like")
							{
								if (this.eventnum > 200)
								{
									this.eventnum = 0;
								}
								if (this.Game_State_T == 3 && this.HandleUsers(jobject2.GetValue("uniqueId").ToString(), jobject2.GetValue("r_nickname").ToString(), jobject2.GetValue("userId").ToString()))
								{
                                    nic2 = jobject2.GetValue("nickname").ToString() + ",";
                                    this.eventrun.Add("event" + this.eventnum.ToString(), nic2);
									this.eventnum++;
								}
								if (this.Game_State_T == 6 && !this.ai_mode)
								{
									int id2 = this.GetId(jobject2.GetValue("uniqueId").ToString());
									if (id2 != -1)
									{
                                        nic2 = id2.ToString() + ",tap";
                                        this.eventboostplayer.Add("event" + this.eventnum.ToString(), nic2 ?? "");
										this.eventnum++;
									}
								}
							}
						}
						else if (num <= 2322801903U)
						{
							if (num != 1533824880U)
							{
								if (num == 2322801903U)
								{
									if (text2 == "gift")
									{
										if (this.Game_State_T == 3 && this.HandleUsers(jobject2.GetValue("uniqueId").ToString(), jobject2.GetValue("r_nickname").ToString(), jobject2.GetValue("userId").ToString()))
										{
											nic2 = jobject2.GetValue("nickname").ToString() + ",batmobile";
											this.eventrun.Add("event" + this.eventnum.ToString(), nic2);
											this.eventnum++;
										}
										if (this.Game_State_T == 6 && !this.ai_mode)
										{
											int id3 = this.GetId(jobject2.GetValue("uniqueId").ToString());
											if (id3 != -1)
											{
                                                nic2 = id3.ToString() + ",gift";
                                                this.eventboostplayer.Add("event" + this.eventnum.ToString(), nic2 ?? "");
												this.eventnum++;
											}
										}
									}
								}
							}
							else if (text2 == "follow")
							{
								if (this.Game_State_T == 3 && this.HandleUsers(jobject2.GetValue("uniqueId").ToString(), jobject2.GetValue("r_nickname").ToString(), jobject2.GetValue("userId").ToString()))
								{
                                    nic2 = jobject2.GetValue("nickname").ToString() + ",";
                                    this.eventrun.Add("event" + this.eventnum.ToString(), jobject2.GetValue("nickname").ToString());
									this.eventnum++;
								}
							}
						}
						else if (num != 2848586808U)
						{
							if (num == 3374496889U)
							{
								if (!(text2 == "join"))
								{
								}
							}
						}
						else if (text2 == "share")
						{
							if (this.HandleUsers(jobject2.GetValue("uniqueId").ToString(), jobject2.GetValue("r_nickname").ToString(), jobject2.GetValue("userId").ToString()))
							{
                                nic2 = jobject2.GetValue("nickname").ToString() + ",batmobile";
                                this.eventrun.Add("event" + this.eventnum.ToString(), nic2);
								this.eventnum++;
							}
							if (this.eventnum > 200)
							{
								this.eventnum = 0;
							}
							if (this.Game_State_T == 6 && !this.ai_mode)
							{
								int id4 = this.GetId(jobject2.GetValue("uniqueId").ToString());
								if (id4 != -1)
								{
                                    nic2 = id4.ToString() + ",share";
                                    this.eventboostplayer.Add("event" + this.eventnum.ToString(), nic2 ?? "");
									this.eventnum++;
								}
							}
						}
					}
				}
			}
			catch (Exception)
			{
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

		// Token: 0x06000021 RID: 33 RVA: 0x00005380 File Offset: 0x00003580
		public void RunEvent(string ename, string name = "", bool fe = false, bool je = false)
		{
			if (ename != null)
			{
				if (je && ename == "pedattacker")
				{
					this.eventjoinrun.Add("event" + this.eventnum.ToString(), ename);
				}
				else if (!fe && !(ename == "pedattacker"))
				{
					if (ename == "spawnpet")
					{
						this.eventpetrun.Add("event" + this.eventnum.ToString(), ename);
						this.eventpets.Add("event" + this.eventnum.ToString(), name);
					}
					else if (ename == "changelicense")
					{
						this.eventrun.Add("event" + this.eventnum.ToString(), ename);
						this.lic_plate = name;
					}
					else
					{
						this.eventrun.Add("event" + this.eventnum.ToString(), ename);
					}
				}
				if (ename == "pedattacker")
				{
					this.eventuser.Add("event" + this.eventnum.ToString(), name);
				}
				if (this.eventnum > 200)
				{
					this.eventnum = 0;
				}
				this.eventnum++;
				return;
			}
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

		// Token: 0x06000025 RID: 37 RVA: 0x00005598 File Offset: 0x00003798
		public void DrawText(string text, float x, Color color, int padd)
		{
			PointF position = new PointF(x, (float)(padd * 10));
			new TextElement(text, position, 1f)
			{
				Color = color,
				Scale = 0.3f
			}.Draw();
		}

		// Token: 0x06000026 RID: 38 RVA: 0x000055D8 File Offset: 0x000037D8
		public bool HandleUsers(string username, string real_nickname, string uniqueid)
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
				this.usernames_[this.player_id] = username;
				this.real_nicknames[this.player_id] = real_nickname;
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
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://keepmy.live/api/v1/lb/win");
			httpWebRequest.ContentType = "application/json";
			httpWebRequest.Method = "POST";
			string text = this.real_nicknames[id];
			string text2 = this.usernames_[id];
			string text3 = "0";
			string text4 = this.uniqueids[id];
			using (StreamWriter streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
			{
				string value = string.Concat(new string[]
				{
					"{\"token\":\"",
					token,
					"\",\"username\":\"",
					text2,
					"\",\"nickname\":\"",
					text,
					"\",\"host_id\":\"",
					text3,
					"\",\"racer_id\":\"",
					text4,
					"\"}"
				});
				streamWriter.Write(value);
			}
			using (StreamReader streamReader = new StreamReader(((HttpWebResponse)httpWebRequest.GetResponse()).GetResponseStream()))
			{
				string tass = streamReader.ReadToEnd();
				this.Tass = tass;
			}
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

		// Token: 0x0400004F RID: 79
		public JObject eventrun = JObject.Parse("{}");

		// Token: 0x04000050 RID: 80
		public JObject eventboostplayer = JObject.Parse("{}");

		// Token: 0x04000051 RID: 81
		public JObject eventjoinrun = JObject.Parse("{}");

		// Token: 0x04000052 RID: 82
		public JObject eventpetrun = JObject.Parse("{}");

		// Token: 0x04000053 RID: 83
		public JObject eventpets = JObject.Parse("{}");

		// Token: 0x04000054 RID: 84
		public JObject eventuser = JObject.Parse("{}");

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

		// Token: 0x0400006B RID: 107
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
