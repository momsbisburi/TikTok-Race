using GTA;
using GTA.Math;
using GTA.Native;
using GTA.UI;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using TikFinity.Models;
using TikTok_Race.Models;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Header;

namespace TikTok_Race
{
    // Token: 0x02000003 RID: 3
    public class Main : Script
    {
        
        private GTA.UI.Font RenderFont = GTA.UI.Font.ChaletLondon;
        private int lastCheckTime = 0;
        // Token: 0x06000003 RID:s 3 RVA: 0x00002068 File Offset: 0x00000268
        public Main()
        {
            base.Tick += this.onTick;
            base.KeyUp += this.onKeyUp;
            base.KeyDown += this.onKeyDown;
        }

        private void SpectateNextPlayer()
        {
            List<Ped> sortedPeds = new List<Ped>();

            // Collect alive players who haven't finished the race
            for (int i = 0; i < this.peds.Length; i++)
            {
                if (this.peds[i] != null && this.peds[i].IsAlive && !HasFinishedRace(i))
                {
                    sortedPeds.Add(this.peds[i]);
                }
            }

            // Sort by distance to finish line (closest first)
            sortedPeds.Sort((a, b) => GetPathDistanceToFinish(a).CompareTo(GetPathDistanceToFinish(b)));

            if (sortedPeds.Count == 0)
            {
                World.RenderingCamera = null; // No players left, return control
                this.autoSpectate = false;
                return;
            }

            // Ensure index doesn't go out of range
            this.spectateIndex = this.spectateIndex % Math.Min(sortedPeds.Count, 5); // Limit to top 5
            Ped target = sortedPeds[this.spectateIndex];

            if (target != null && target.CurrentVehicle != null)
            {
                Game.Player.Character.SetIntoVehicle(target.CurrentVehicle, VehicleSeat.Passenger);
            }

            this.spectateIndex++; // Move to the next player for the next switch
        }
        private void DrawRaceTimer(int timeLeft)
        {
            string timerText = $"Race starts in: {timeLeft}s";

            try
            {
                PointF position = new PointF(640f, 670f); // Bottom center of the screen
                TextElement timerElement = new TextElement(timerText, position, 0.6f, Color.Red, GTA.UI.Font.ChaletLondon, Alignment.Center, false, true);
                timerElement.Draw();
            }
            catch (Exception) { }
        }

        private void DrawEndRaceTimer(int timeLeft)
        {
            string timerText = $"Race ends in: {timeLeft}s";

            try
            {
                PointF position = new PointF(640f, 670f); // Bottom center of the screen
                TextElement timerElement = new TextElement(timerText, position, 0.6f, Color.Red, GTA.UI.Font.ChaletLondon, Alignment.Center, false, true);
                timerElement.Draw();
            }
            catch (Exception) { }
        }
        // Token: 0x06000004 RID: 4 RVA: 0x00002248 File Offset: 0x00000448


       
        private Dictionary<int, int> stoppedTime = new Dictionary<int, int>();
        private void BoostAllCars()
        {
            for (int i = 0; i < this.peds.Length; i++)
            {
                if (this.peds[i] != null && this.peds[i].CurrentVehicle != null)
                {
                    BoostPlayer(i, 30, false);
                }
            }
        }

        private void onTick(object sender, EventArgs e)
        {

            if (!this.Initialization)
            {
                this.race_point = World.WaypointPosition;
               
                this.Initialization = true;
                this.race_point = World.WaypointPosition;
                this.KillAllInit();
                this.cam = World.CreateCamera(this.camera_pos, new Vector3(1719.525f, 3257.532f, 40f), 40f);
                this.cam.PointAt(this.camera_look_at);
            }

            //restart race
            if (this.raceRestartPending && Game.GameTime >= this.nextRaceTime)
            {
                this.raceRestartPending = false; // Reset flag
                RestartRace();
            }

            //if (this.Game_State == 6 && Game.GameTime >= this.raceStartTime + 10000 && Game.GameTime >= lastCheckTime + 20000)
            //{
            //    lastCheckTime = Game.GameTime; // Update last check time
            //   // CheckIfCarsAreMoving();
            //}

            //starts countdown
            //if (this.Game_State == 3)
            //{
            //    int remainingTime = Math.Max(0, (this.raceStartTime - Game.GameTime) / 1000);

            //    // Draw UI timer on the bottom of the screen
            //    DrawRaceTimer(remainingTime);
            //    Logger.LogMessage("Race starting at: " + this.race_point.ToString());
            //    // If the timer reaches 0, start the race
            //    if (remainingTime <= 0)
            //    {
            //        StartCountdown();
            //    }
            //}

            if (this.Game_State == 3)
            {
                // Ensure at least 2 players before starting the timer
                if (this.player_number >= 2)
                {
                    // Set race start time if not set already
                    if (this.raceStartTime == 0)
                    {
                        this.raceStartTime = Game.GameTime + 180000; // 50 seconds countdown
                    }

                    int remainingTime = Math.Max(0, (this.raceStartTime - Game.GameTime) / 1000);

                    // Draw UI timer on the bottom of the screen
                    DrawRaceTimer(remainingTime);
                    //Logger.LogMessage("Race starting at: " + this.race_point.ToString());

                    // If the timer reaches 0, start the race
                    if (remainingTime <= 0)
                    {
                        StartCountdown();
                    }
                }
                else
                {
                    // Reset race start time if not enough players
                    this.raceStartTime = 0;
                    DrawWaitingMessage();
                }
            }

            if (this.autoSpectate && Game.GameTime >= this.nextSwitchTime && this.Game_State == 6)
            {
                this.nextSwitchTime = Game.GameTime + 5000; // Set next switch time
                SpectateNextPlayer();
            }
            World.Weather = Weather.Clear;
            World.CurrentTimeOfDay = new TimeSpan(12, 0, 0);

            if (this.Game_State == 2)
            {
                this.race_point = World.WaypointPosition;
            }
            try
            {
                this.DrawText("Muzzy Tiktok Race " + this.Version, 0f, Color.Blue, 0, 0.1f, false, 0);
            }
            catch (Exception)
            {
            }
            this.DeletePopulation();
            this.DebugInfo();
            this.DrawRaceMenu();
            this.RenderNames();
            this.RacingCam();
            this.CCD();
            if (this.Game_State == 7)
            {
                Game.Player.Character.IsInvincible = true;
                Game.Player.CanControlCharacter = false;
                Hud.IsRadarVisible = false;
                Game.Player.Character.Position = new Vector3(1717.06f, 3287.031f, 41.16869f);
                World.RenderingCamera = this.cam;
                this.is_racing = false;
                this.racing_cam = false;
                this.KillAllInit();
                this.Game_State = 8;
                this.winners_list = true;
                this.race_menu = true;
            }
           
            if (this.Game_State == 7 && !this.winners_list)
            {
                this.winners_list = true; // Prevent multiple saves

                int raceID = StartNewRace(); // Get the race ID
                SaveRaceResults(raceID, this.place_1, this.place_2, this.place_3);

                CurrentRaceID = -1; // Reset for the next race
            }
        }

        private void DrawWaitingMessage()
        {
            try
            {
                PointF position = new PointF(640f, 670f); // Bottom center of the screen
                TextElement waitingText = new TextElement("Waiting for players to join...", position, 0.6f, Color.White, GTA.UI.Font.ChaletLondon, Alignment.Center, false, true);
                waitingText.Draw();
            }
            catch (Exception) { }
        }

        private void ForceFinishRace()
        {
           // GTA.UI.Screen.ShowSubtitle("lets go!", 5000);
            try
            {
                for (int i = 0; i < this.peds.Length; i++)
                {
                    if (this.peds[i] != null)
                    {
                        this.player_position[i] = ((int)World.GetDistance(this.peds[i].Position, this.race_point)).ToString() + "|" + this.players_names[i];
                    }
                    if (this.peds[i] != null)
                    {
                        if (this.place == 0)
                        {
                            this.place_1 = this.players_names[i];
                            this.peds[i].Kill();
                            this.SendWin(i);
                        }
                        if (this.place == 1)
                        {
                            this.place_2 = this.players_names[i];
                            this.peds[i].Kill();
                            if (this.player_number == 2)
                            {
                                this.is_racing = false;
                                this.Game_State = 7;
                            }
                        }
                        if (this.place >= 2)
                        {
                            this.place_3 = this.players_names[i];
                            this.is_racing = false;
                            this.peds[i].Kill();
                            this.Game_State = 7;
                            if (this.Game_State == 7 && !this.winners_list)
                            {
                                this.winners_list = true; // Prevent multiple saves
                            }
                        }

                        this.place++;
                    }
                    this.Game_State = 7;
                }
            }
            catch
            {
               
            }
           
            // Assign the remaining player to the last position


            //this.peds[lastPlayer].Kill(); // Stop the player
           
                this.is_racing = false;
                this.Game_State = 7; // Mark race as finished

                // Save results
                int raceID = StartNewRace();
                SaveRaceResults(raceID, this.place_1, this.place_2, string.IsNullOrEmpty(this.place_3) ? "N/A" : this.place_3);

                this.nextRaceTime = Game.GameTime + 30000; // 30 seconds delay before next race
                this.raceRestartPending = true;
                CurrentRaceID = -1; // Reset for the next race
            
        }

        private void RestartRace()
        {

            // Reuse the last race waypoint
            if (World.WaypointPosition == Vector3.Zero)
            {

                this.race_point = new Vector3(-1571.672f, -916.4675f, 16.19775f); // Updated default race point
                GTA.UI.Screen.ShowSubtitle("⚠ No waypoint set. Using default race start location.", 5000);

            }
            else
            {
                this.race_point = World.WaypointPosition;
            }
            KillAllInit(); // Delete old vehicles

            // Reset race properties
            this.Game_State = 2;
            this.player_number = 0;
            this.place = 0;
            this.place_1 = "";
            this.place_2 = "";
            this.place_3 = "";

            // Reset player names and positions
            for (int i = 0; i < this.players_names.Length; i++)
            {
                this.players_names[i] = "";
                this.peds[i] = null; // Clear ped references
                this.vehicles[i] = null; // Clear vehicle references
            }

            //GTA.Script.Wait(10000);
            this.winners_list = false;

            this.websocket.ResetPlayers();

            // Reset spawn positioning
            this.Players_X = 0f;
            this.Players_Y = 0f;
            this.Players_r_n = 1;
            this.Players_r_nr = 0;

            this.DrawText("Next race starting!", 0f, Color.Yellow, 5, 0.5f, true, 0);

            // Move to queue state
            this.Game_State = 3;
            this.queue_list = true;
            this.raceStartTime = Game.GameTime + 180000;
            //this.raceStartTime = Game.GameTime + 50000;
            this.DrawQueueList();
        }

        private void StartCountdown()
        {
            if (this.player_number < 2) return; // Prevent starting with less than 2 players

            this.countdown_ = true;
            this.countdown = Game.GameTime + this.countdown_time * 1000;
            this.Game_State = 5; // Move to countdown state
        }

        // Token: 0x06000005 RID: 5 RVA: 0x00002424 File Offset: 0x00000624
        private void onKeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad9 && this.debug_lock)
            {
                this.debug_lock = false;
            }
            if (e.KeyCode == Keys.NumPad7 || (e.KeyCode == Keys.NumPad8 && this.debug_lock))
            {
                this.debug_lock = false;
            }
            if ((e.KeyCode == Keys.U || e.KeyCode == Keys.H || e.KeyCode == Keys.J || e.KeyCode == Keys.Left || e.KeyCode == Keys.Right) && this.hold_f)
            {
                this.hold_f = false;
            }
            if (e.KeyCode == Keys.O && this.Game_State == 6) // Only when race is ongoing
            {
                this.autoSpectate = !this.autoSpectate; // Toggle auto-spectate
                if (this.autoSpectate)
                {
                    this.spectateIndex = 0; // Start from the first player
                    this.nextSwitchTime = Game.GameTime + 10000; // Set next switch time (10s delay)
                    SpectateNextPlayer();
                }
                else
                {
                    World.RenderingCamera = null; // Return control to player
                }
            }
        }

        // Token: 0x06000006 RID: 6 RVA: 0x000024B0 File Offset: 0x000006B0
        private void onKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.NumPad9 && !this.debug_lock)
            {
                this.debug_lock = true;
                this.is_debug = !this.is_debug;
            }
            if (e.KeyCode == Keys.NumPad7 && !this.debug_lock)
            {
                this.debug_lock = true;
                this.peds_en = !this.peds_en;
            }
            if (e.KeyCode == Keys.NumPad8 && !this.debug_lock)
            {
                this.debug_lock = true;
                this.ai_mode = !this.ai_mode;
            }
            if (e.KeyCode == Keys.L && this.Game_State == 6)
            {
                this.name_f = Game.GetUserInput("");
            }
            if (e.KeyCode == Keys.U && !this.hold_f)
            {
                this.hold_f = true;
                this.race_menu = !this.race_menu;
            }
            if (e.KeyCode == Keys.H && !this.hold_f && this.race_menu && !this.queue_list)
            {
                this.hold_f = true;
                this.ButtonControlH();
            }
            if (e.KeyCode == Keys.J && !this.hold_f && (this.Game_State == 3 || this.Game_State == 8))
            {
                this.hold_f = true;
                if (this.Game_State == 3)
                {
                    this.queue_list = !this.queue_list;
                }
                else if (this.Game_State == 8)
                {
                    this.winners_list = !this.winners_list;
                }
            }
            if (e.KeyCode == Keys.Left && !this.hold_f && this.Game_State == 6)
            {
                this.hold_f = true;
                if (this.c_camera <= 0)
                {
                    if (this.vehicles[this.player_number - 1] != null)
                    {
                        Game.Player.Character.SetIntoVehicle(this.vehicles[this.player_number - 1], VehicleSeat.Passenger);
                    }
                    this.c_camera = this.player_number - 1;
                }
                else
                {
                    if (this.vehicles[this.c_camera - 1] != null)
                    {
                        Game.Player.Character.SetIntoVehicle(this.vehicles[this.c_camera - 1], VehicleSeat.Passenger);
                    }
                    this.c_camera--;
                }
            }
            if (e.KeyCode == Keys.Right && !this.hold_f && this.Game_State == 6)
            {
                this.hold_f = true;
                if (this.c_camera >= this.player_number - 1)
                {
                    if (this.vehicles[0] != null)
                    {
                        Game.Player.Character.SetIntoVehicle(this.vehicles[0], VehicleSeat.Passenger);
                    }
                    this.c_camera = 0;
                    return;
                }
                if (this.vehicles[this.c_camera + 1] != null)
                {
                    Game.Player.Character.SetIntoVehicle(this.vehicles[this.c_camera + 1], VehicleSeat.Passenger);
                }
                this.c_camera++;
            }
        }

        // Token: 0x06000007 RID: 7 RVA: 0x0000277C File Offset: 0x0000097C
        public void DrawText(string text, float x, Color color, int padd, float size = 0f, bool wd = false, int adds = 0)
        {
            try
            {
                PointF position = new PointF(x, (float)(padd * 10));
                TextElement textElement = new TextElement(text, position, 1f)
                {
                    Color = color,
                    Scale = 0.3f + size
                };
                if (wd)
                {
                    textElement.Font = GTA.UI.Font.Pricedown;
                    textElement.Color = Color.White;
                    textElement.Outline = true;
                    textElement.Position = new PointF(640f - textElement.Width / 2f, 660f - (float)adds);
                }
                textElement.Draw();
            }
            catch (Exception)
            {
            }
        }

        // Token: 0x06000008 RID: 8 RVA: 0x00002818 File Offset: 0x00000A18
        public void DrawRaceMenuItem(string text, float x, float y, int padd, Color color, float size, bool bold = false, bool qlist = false, bool alignh = false, bool cent = false)
        {
            try
            {
               // string utf8Text = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(text.Normalize(NormalizationForm.FormC)));
               // Logger.LogMessage($"Rendering Name in Main.cs: {text}");

                PointF position = new PointF(x, (float)(padd * 10));
                TextElement textElement = new TextElement(ExtractOutsideName(text), position, size)
                {
                    Color = color,
                    Shadow = bold,
                    Font= RenderFont

                };
                if (qlist)
                {
                    textElement.Position = new PointF(x, 190f + y + (float)(padd * 10));
                }
                else
                {
                    textElement.Position = new PointF(1280f - x, 190f + y + (float)(padd * 10));
                }
                if (cent)
                {
                    textElement.Position = new PointF(x, y);
                }
                if (alignh)
                {
                    textElement.Alignment = Alignment.Center;
                }
                textElement.Draw();
            }
            catch (Exception)
            {
            }
        }

        // Token: 0x06000009 RID: 9 RVA: 0x000028C4 File Offset: 0x00000AC4
        public void DrawRaceMenuItemLB(string text, float x, float y, int padd, Color color, float size, bool bold = false, bool qlist = false, bool alignh = false, bool cent = false)
        {
            try
            {
                // Convert text to UTF-8 for special characters and emojis
              //  Logger.LogMessage($"Rendering Name in Main.cs: {text}");
                string utf8Text = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(text.Normalize(NormalizationForm.FormC)));

                PointF position = new PointF(x, (float)(padd * 10));
                TextElement textElement = new TextElement(utf8Text, position, size)
                {
                    Color = color,
                    Shadow = bold,
                    Font = RenderFont,  // Use a font that supports emojis!
                };

                if (qlist)
                {
                    textElement.Position = new PointF(x, 190f + y + (float)(padd * 10));
                }
                else
                {
                    textElement.Position = new PointF(x, 190f + y + (float)(padd * 10));
                }

                if (cent)
                {
                    textElement.Position = new PointF(x, y);
                }
                if (alignh)
                {
                    textElement.Alignment = Alignment.Center;
                }

                textElement.Draw();
            }
            catch (Exception ex)
            {
              //  LogMessage($"Error in DrawRaceMenuItemLB: {ex.Message}");
            }
        }

        // Token: 0x0600000A RID: 10 RVA: 0x0000296C File Offset: 0x00000B6C
        public void DrawRaceBase()
        {
            this.DrawRaceMenuItem("TikTok Race Menu", 266f, 0f, 0, Color.OrangeRed, 0.7f, false, false, false, false);
            try
            {
                int num = 250;
                int num2 = 140;
                new ContainerElement(new PointF(1280f - (float)num - 20f, 190f), new SizeF((float)num, (float)num2), Color.FromArgb(150, Color.Black))
                {
                    Enabled = true
                }.Draw();
            }
            catch (Exception)
            {
            }
        }

        // Token: 0x0600000B RID: 11 RVA: 0x00002A00 File Offset: 0x00000C00
        public void DrawRaceBaseLB()
        {
            this.DrawRaceMenuItemLB("Leaderboard", 60f, -75f, 0, Color.Cyan, 0.7f, false, false, false, false);
            try
            {
                int num = 250;
                int num2 = 250;
                new ContainerElement(new PointF(20f, 120f), new SizeF((float)num, (float)num2), Color.FromArgb(150, Color.Black))
                {
                    Enabled = true
                }.Draw();
            }
            catch (Exception)
            {
            }
        }

        private bool HasFinishedRace(int playerIndex)
        {
            return this.peds[playerIndex] != null && World.GetDistance(this.peds[playerIndex].Position, this.race_point) < 10f;
        }

        private float GetPathDistanceToFinish(Ped player)
        {
            if (player == null) return float.MaxValue;

            // Find nearest road position
            Vector3 nearestRoad = World.GetNextPositionOnStreet(player.Position);

            // If player is far from the road, add a penalty
            float offRoadPenalty = World.GetDistance(player.Position, nearestRoad) * 2;

            // Get distance from nearest road to finish
            float roadDistance = World.GetDistance(nearestRoad, this.race_point);

            return roadDistance + offRoadPenalty;
        }

        // Token: 0x0600000C RID: 12 RVA: 0x00002A8C File Offset: 0x00000C8C
        public void DrawRaceMenu()
        {
            int num = 0;
            if (this.race_menu)
            {
                num = 5;
                switch (this.Game_State)
                {
                    case 0:
                        this.DrawRaceBase();
                        this.DrawRaceMenuItem("[H] - Setup race!", 265f, 0f, num++, Color.White, 0.4f, false, false, false, false);
                        goto IL_608;
                    case 1:
                    case 5:
                    case 7:
                        goto IL_608;
                    case 2:
                        this.DrawRaceBase();
                        if (this.race_point.X == 0f && this.race_point.Y == 0f)
                        {
                            this.DrawRaceMenuItem("PLEASE SET WAYPOINT!", 265f, 0f, num++, Color.Turquoise, 0.5f, false, false, false, false);
                            num += 2;
                        }
                        this.DrawRaceMenuItem("[H] - Open Queue!", 265f, 0f, num++, Color.White, 0.4f, false, false, false, false);
                        goto IL_608;
                    case 3:
                        if (this.queue_list)
                        {
                            try
                            {
                                int num2 = 190;
                                int num3 = 40;
                                new ContainerElement(new PointF(1280f - (float)num2 - 20f, 360f - (float)(num3 / 2) - 300f), new SizeF((float)num2, (float)num3), Color.FromArgb(150, Color.Black))
                                {
                                    Enabled = true
                                }.Draw();
                            }
                            catch (Exception)
                            {
                            }
                            this.DrawRaceMenuItem("[J] - Close Queue List!", 200f, -140f, 0, Color.White, 0.4f, true, false, false, false);
                            this.DrawQueueList();
                            goto IL_608;
                        }
                        this.DrawRaceBase();
                        this.DrawRaceMenuItem("[H] - End Queue!", 265f, 0f, num++, Color.White, 0.4f, false, false, false, false);
                        num += 2;
                        this.DrawRaceMenuItem("[J] - Open Queue List!", 265f, 0f, num++, Color.White, 0.4f, false, false, false, false);
                        goto IL_608;
                    case 4:
                        this.DrawRaceBase();
                        this.DrawRaceMenuItem("[H] - Start countdown!", 265f, 0f, num++, Color.White, 0.4f, false, false, false, false);
                        goto IL_608;
                    case 6:
                        {
                            this.DrawRaceBase();
                            this.DrawRaceMenuItem("Change camera (" + (this.c_camera + 1).ToString() + "):", 265f, -20f, num, Color.White, 0.4f, false, false, false, false);
                            this.DrawRaceMenuItem("[<] - Previous player!", 265f, 0f, num++, Color.White, 0.4f, false, false, false, false);
                            num += 2;
                            this.DrawRaceMenuItem("[>] - Next player!", 265f, 0f, num++, Color.White, 0.4f, false, false, false, false);
                            num += 2;
                            this.DrawRaceMenuItem("[L] - Search player!", 265f, 0f, num++, Color.White, 0.4f, false, false, false, false);
                            this.DrawRaceBaseLB();
                            List<string> list = new List<string>();

                            for (int i = 0; i < this.player_position.Length; i++)
                            {
                                // Check if the player exists, is alive, and has NOT finished the race
                                if (this.peds[i] != null && this.peds[i].IsAlive && !HasFinishedRace(i))
                                {
                                    float distanceToFinish = GetPathDistanceToFinish(this.peds[i]);
                                    list.Add(distanceToFinish.ToString("F1") + "|" + this.players_names[i]);
                                }
                            }

                            // Sort by distance (closest to finish first)
                            list.Sort((a, b) => float.Parse(a.Split('|')[0]).CompareTo(float.Parse(b.Split('|')[0])));
                            list = list.Take(10).ToList();
                            int num4 = 1;
                            foreach (string text in list)
                            {
                                string[] array = text.Split('|');
                                if (array.Length > 1)
                                {
                                    this.DrawRaceMenuItemLB($"{num4}. {ExtractOutsideName(array[1])} ({array[0]}m)", 25f, -120f, num++, Color.White, 0.5f, false, false, false, false);
                                    num++;
                                }
                                num4++;
                            }
                            break;
                        }
                    case 8:
                        break;
                    default:
                        goto IL_608;
                }
                if (this.winners_list)
                {
                    try
                    {
                        int num5 = 190;
                        int num6 = 40;
                        new ContainerElement(new PointF(1280f - (float)num5 - 20f, 360f - (float)(num6 / 2) - 300f), new SizeF((float)num5, (float)num6), Color.FromArgb(150, Color.Black))
                        {
                            Enabled = true
                        }.Draw();
                    }
                    catch (Exception)
                    {
                    }
                    this.DrawRaceMenuItem("[J] - Close Winners List!", 200f, -140f, 0, Color.White, 0.4f, true, false, false, false);
                    this.DrawWinnerList();
                }
                else
                {
                    this.DrawRaceBase();
                    this.DrawRaceMenuItem("[H] - Setup Race!", 265f, 0f, num++, Color.White, 0.4f, false, false, false, false);
                    num += 2;
                    this.DrawRaceMenuItem("[J] - Open Winners List!", 265f, 0f, num++, Color.White, 0.4f, false, false, false, false);
                }
            }
            else
            {
                try
                {
                    int num7 = 190;
                    int num8 = 40;
                    new ContainerElement(new PointF(1280f - (float)num7 - 20f, 360f - (float)(num8 / 2) - 300f), new SizeF((float)num7, (float)num8), Color.FromArgb(150, Color.Black))
                    {
                        Enabled = true
                    }.Draw();
                }
                catch (Exception)
                {
                }
                this.DrawRaceMenuItem("[U] - Show Race Menu!", 200f, -140f, num++, Color.White, 0.4f, true, false, false, false);
                if (this.Game_State == 6)
                {
                    num = 9;
                    this.DrawRaceBaseLB();
                    List<string> list2 = new List<string>();
                    for (int j = 0; j < 10; j++)
                    {
                        if (this.player_position[j] != null)
                        {
                            list2.Add(this.player_position[j]);
                        }
                    }
                    list2.Sort();
                    int num9 = 1;
                    foreach (string text2 in list2)
                    {
                        string[] array2 = text2.Split(new char[]
                        {
                            '|'
                        });
                        if (array2.Length != 0)
                        {
                            this.DrawRaceMenuItemLB(num9.ToString() + ". " + array2[1], 25f, -120f, num++, Color.White, 0.5f, false, false, false, false);
                            num++;
                        }
                        num9++;
                    }
                }
            }
        IL_608:
            if (this.countdown_)
            {
                TimeSpan timeSpan = TimeSpan.FromSeconds((double)((this.countdown - Game.GameTime) / 1000));
                this.sound_play_id_s = timeSpan.Seconds;
                if (timeSpan.Seconds < 1)
                {
                    if (this.sound_play_id != this.sound_play_id_s)
                    {
                        Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "Beep_Green", "DLC_HEIST_HACKING_SNAKE_SOUNDS", 1);
                        this.sound_play_id = this.sound_play_id_s;
                    }
                    this.DrawRaceMenuItem("GO!!!", 640f, 360f, 0, Color.Green, 1.5f, true, false, true, true);
                }
                else if (timeSpan.Seconds <= 3)
                {
                    if (this.sound_play_id != this.sound_play_id_s)
                    {
                        Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "3_2_1", "HUD_MINI_GAME_SOUNDSET", 1);
                        this.sound_play_id = this.sound_play_id_s;
                    }
                    this.DrawRaceMenuItem(timeSpan.Seconds.ToString() ?? "", 640f, 360f, 0, Color.Red, 1.5f, true, false, true, true);
                }
                else
                {
                    if (this.sound_play_id != this.sound_play_id_s)
                    {
                        Function.Call(Hash.PLAY_SOUND_FRONTEND, -1, "3_2_1", "HUD_MINI_GAME_SOUNDSET", 1);
                        this.sound_play_id = this.sound_play_id_s;
                    }
                    this.DrawRaceMenuItem(timeSpan.Seconds.ToString() ?? "", 640f, 360f, 0, Color.White, 1.5f, true, false, true, true);
                }
                if (timeSpan.Seconds < 1)
                {
                    Hud.IsRadarVisible = true;
                    this.countdown_ = false;
                    this.is_racing = true;
                    this.racing_cam = true;
                    this.Game_State = 6;
                    Game.Player.CanControlCharacter = true;
                    this.StartDriving();
                }
            }
        }

        // Token: 0x0600000E RID: 14 RVA: 0x0000336C File Offset: 0x0000156C
        private void DeletePopulation()
        {
            if (this.peds_en)
            {
                foreach (Ped ped in World.GetAllPeds(Array.Empty<Model>()))
                {
                    if (!ped.IsPlayer && ped.MaxHealth != 123)
                    {
                        try
                        {
                            ped.Delete();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                foreach (Vehicle vehicle in World.GetAllVehicles(Array.Empty<Model>()))
                {
                    if (vehicle != Game.Player.Character.LastVehicle && vehicle.MaxHealth != 123)
                    {
                        try
                        {
                            vehicle.Delete();
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }
        }

        // Token: 0x0600000F RID: 15 RVA: 0x00003428 File Offset: 0x00001628
        private void StartDriving()
        {
            Ped[] nearbyPeds = World.GetNearbyPeds(new Vector3(1722f, 3239f, 40.7287f), 500f, Array.Empty<Model>());
            int num = 0;
            Vector3[] array = new Vector3[]
            {
                World.WaypointPosition,
                World.WaypointPosition
            };
            foreach (Ped ped in nearbyPeds)
            {
                if (ped != null && ped.MaxHealth == 123)
                {
                    try
                    {
                        if (ped != null)
                        {
                            ped.MaxDrivingSpeed = 50f;
                            // Lower driving ability to encourage risky behavior
                           
                            if (!this.ai_mode)
                            {
                                if (num % 2 == 0)
                                {
                                    Function.Call(Hash.TASK_VEHICLE_DRIVE_TO_COORD, ped, ped.LastVehicle, this.race_point.X, this.race_point.Y, this.race_point.Z, 0f, 262196, 10f);
                                    this.players_ai[num] = 393624;
                                }
                                else
                                {
                                    Function.Call(Hash.TASK_VEHICLE_DRIVE_TO_COORD, ped, ped.LastVehicle, this.race_point.X, this.race_point.Y, this.race_point.Z, 0f, 262196, 10f);
                                    this.players_ai[num] = 262196;
                                }
                                num++;
                            }
                            else
                            {
                                Function.Call(Hash.TASK_VEHICLE_DRIVE_TO_COORD, ped, ped.LastVehicle, this.race_point.X, this.race_point.Y, this.race_point.Z, 300f, 262196, 10f);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            foreach (Vehicle vehicle in World.GetAllVehicles(Array.Empty<Model>()))
            {
                if (vehicle.MaxHealth == 123)
                {
                    try
                    {
                        vehicle.IsEngineRunning = true;
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        // Token: 0x06000010 RID: 16 RVA: 0x000036AC File Offset: 0x000018AC
        public void DebugInfo()
        {
            int num = 10;
            if (this.websocket != null)
            {
                if (!this.websocket.ConnectedB)
                {
                    this.DrawTextI("Not connected...", 0f, Color.Red, 0, 0f);
                    if (this.websocket.ws_err_msg.Length > 1)
                    {
                        this.DrawTextI("Error: " + this.websocket.ws_err_msg, 0f, Color.Red, 4, 0f);
                    }
                }
            }
            else
            {
                this.DrawTextI("Not connected...", 0f, Color.Red, 0, 0f);
            }
            if (this.is_debug)
            {
                try
                {
                    this.DrawText("Debug info: ", 0f, Color.Orange, num++, 0f, false, 0);
                    this.DrawText("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~", 0f, Color.Orange, num++, 0f, false, 0);
                    if (this.websocket != null)
                    {
                        if (this.websocket.ConnectedB)
                        {
                            this.DrawText("Connected", 0f, Color.Green, num++, 0f, false, 0);
                        }
                        else
                        {
                            this.DrawText("Not connected", 0f, Color.Red, num++, 0f, false, 0);
                        }
                    }
                    else
                    {
                        this.DrawText("Not connected", 0f, Color.Red, num++, 0f, false, 0);
                    }
                    if (this.peds_en)
                    {
                        this.DrawText("Disable peds: " + this.peds_en.ToString(), 0f, Color.Green, num++, 0f, false, 0);
                    }
                    else
                    {
                        this.DrawText("Disable peds: " + this.peds_en.ToString(), 0f, Color.Red, num++, 0f, false, 0);
                    }
                    if (this.ai_mode)
                    {
                        this.DrawText("Enable AI: " + this.ai_mode.ToString(), 0f, Color.Red, num++, 0f, false, 0);
                    }
                    else
                    {
                        this.DrawText("Enable AI: " + this.ai_mode.ToString(), 0f, Color.Green, num++, 0f, false, 0);
                    }
                    num++;
                    this.DrawText("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~", 0f, Color.Orange, num++, 0f, false, 0);
                }
                catch (Exception)
                {
                }
            }
        }

        // Token: 0x06000011 RID: 17 RVA: 0x00003930 File Offset: 0x00001B30
        public void CreatePlayer(string name, VehicleHash cartype)
        {

            if (this.player_number >= this.max_players || this.Game_State != 3)
            {
                return;
            }
            Vector3 position = new Vector3(1722f, 3239f, 40.7287f) + new Vector3(this.Players_X, this.Players_Y, 0f);
            Vehicle vehicle = null;
            if (cartype == null)
            {
                vehicle = World.CreateVehicle(WSC.DefaultCar, position, 286.7277f);
            }
            else
            {
                vehicle = World.CreateVehicle(cartype, position, 286.7277f);
            }

            if (this.Players_r_n >= 7)
            {
                this.Players_r_nr++;
                this.Players_Y = -4.4f * (float)this.Players_r_nr;
                this.Players_X -= 10f;
                this.Players_r_n = 0;
            }
            else
            {
                this.Players_Y += 5f;
                this.Players_X -= 1f;
            }
            this.Players_r_n++;
            Ped ped = World.CreateRandomPed(new Vector3(1732f, 3204f, 43f));
            vehicle.AddBlip();
            vehicle.MaxHealth = 123;
            vehicle.IsInvincible = true;
            ped.CanFlyThroughWindscreen = false;
            ped.CanBeDraggedOutOfVehicle = false;
            ped.MaxHealth = 123;
            ped.CanBeShotInVehicle = false;
            ped.IsInvincible = true;
            ped.StaysInVehicleWhenJacked = true;
            ped.Task.ClearAllImmediately();
            ped.Task.Wait(-1);
            ped.AlwaysKeepTask = true;
            this.peds.SetValue(ped, this.player_number);
            this.vehicles.SetValue(vehicle, this.player_number);
            this.players_speed.SetValue(0, this.player_number);
            this.players_names[this.player_number] = name;


            this.player_number++;
            ped.SetIntoVehicle(vehicle, VehicleSeat.Driver);
        }

        // Token: 0x06000012 RID: 18 RVA: 0x00003AEC File Offset: 0x00001CEC
        public void KillAllInit()
        {
            foreach (Ped ped in World.GetAllPeds(Array.Empty<Model>()))
            {
                if (ped.MaxHealth == 123)
                {
                    try
                    {
                        ped.Delete();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            foreach (Vehicle vehicle in World.GetAllVehicles(Array.Empty<Model>()))
            {
                if (vehicle.MaxHealth == 123)
                {
                    try
                    {
                        vehicle.Delete();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        // Token: 0x06000013 RID: 19 RVA: 0x00003B7C File Offset: 0x00001D7C
        public void SendWin(int id)
        {
            this.websocket.SendWin(id, this.info_t);
         //   Game.FrameCount = 0;
            // Mark restart pending

        }

       

        // Token: 0x06000014 RID: 20 RVA: 0x00003B90 File Offset: 0x00001D90
        public void RenderNames()
        {
            try
            {
                if (this.Game_State == 6)
                {
                    for (int i = 0; i < this.peds.Length; i++)
                    {
                        if (this.peds[i] != null && this.peds[i].CurrentVehicle != null)
                        {
                            this.player_position[i] = ((int)World.GetDistance(this.peds[i].Position, this.race_point)).ToString() + "|" + this.players_names[i];
                        }
                        if (this.peds[i] != null && World.GetDistance(this.peds[i].Position, this.race_point) < 50f && this.peds[i].IsAlive)
                        {
                            if (this.place == 0)
                            {
                                this.place_1 = this.players_names[i];
                                this.peds[i].Kill();
                                this.SendWin(i);
                            }
                            if (this.place == 1)
                            {
                                this.place_2 = this.players_names[i];
                                this.peds[i].Kill();
                                if (this.player_number == 2)
                                {
                                    this.is_racing = false;
                                    this.Game_State = 7;
                                    int raceID = StartNewRace(); // Get the race ID
                                    SaveRaceResults(raceID, this.place_1, this.place_2, this.place_3);
                                    this.nextRaceTime = Game.GameTime + 30000; // 10 seconds delay
                                    DrawRestartCountdown();
                                    this.raceRestartPending = true;
                                    CurrentRaceID = -1;
                                }
                            }
                            if (this.place >= 2)
                            {
                                this.place_3 = this.players_names[i];
                                this.is_racing = false;
                                this.peds[i].Kill();
                                this.Game_State = 7;
                                if (this.Game_State == 7 && !this.winners_list)
                                {
                                    this.winners_list = true; // Prevent multiple saves

                                    int raceID = StartNewRace(); // Get the race ID
                                    SaveRaceResults(raceID, this.place_1, this.place_2, this.place_3);
                                    this.nextRaceTime = Game.GameTime + 30000; // 10 seconds delay
                                    DrawRestartCountdown();
                                    this.raceRestartPending = true;
                                    CurrentRaceID = -1; // Reset for the next race
                                }
                            }
                           
                            this.place++;
                        }
                        if (this.peds[i] != null && World.GetDistance(this.peds[i].Position, Game.Player.Character.Position) < 50f && this.peds[i].IsOnScreen)
                        {
                            PointF position = GTA.UI.Screen.WorldToScreen(this.peds[i].Position, false);
                            string utf8Text = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(this.players_names[i]));
                            new TextElement(ExtractOutsideName(utf8Text), position, 0.5f, Color.White, GTA.UI.Font.Pricedown, Alignment.Center, false, true).Draw();
                          //  Logger.LogMessage($"Rendering Name in Main.cs: {this.players_names[i]}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        // Token: 0x06000015 RID: 21 RVA: 0x00003DAC File Offset: 0x00001FAC
        public void ButtonControlH()
        {
            switch (this.Game_State)
            {
                case 0:
                    World.RemoveWaypoint();
                    Game.Player.Character.IsInvincible = true;
                    Game.Player.CanControlCharacter = false;
                    Hud.IsRadarVisible = false;
                    Game.Player.Character.Position = new Vector3(1717.06f, 3287.031f, 41.16869f);
                    World.RenderingCamera = this.cam;
                    this.player_number = 0;
                    this.place = 0;
                    this.websocket.player_id = 0;
                    for (int i = 0; i < this.players_names.Length; i++)
                    {
                        this.players_names[i] = "";
                    }
                    for (int j = 0; j < this.websocket.usernames_.Length; j++)
                    {
                        this.websocket.usernames_[j] = "";
                    }
                    this.Game_State = 2;
                    return;
                case 1:
                case 5:
                case 6:
                case 7:
                    break;
                case 2:
                    if (this.race_point.X == 0f && this.race_point.Y == 0f)
                    {
                        return;
                    }
                    this.Players_r_nr = 0;
                    this.Players_r_n = 1;
                    this.Players_X = 0f;
                    this.Players_Y = 0f;
                    this.queue_list = true;
                    this.Game_State = 3;
                    return;
                case 3:
                    if (this.player_number < 2)
                    {
                        return;
                    }
                    this.Game_State = 4;
                    return;
                case 4:
                    this.countdown_ = true;
                    this.countdown = Game.GameTime + this.countdown_time * 1000;
                    this.Game_State = 5;
                    return;
                case 8:
                    Game.Player.Character.IsInvincible = true;
                    Game.Player.CanControlCharacter = false;
                    Hud.IsRadarVisible = false;
                    Game.Player.Character.Position = new Vector3(1717.06f, 3287.031f, 41.16869f);
                    World.RenderingCamera = this.cam;
                    this.player_number = 0;
                    this.place = 0;
                    for (int k = 0; k < this.players_names.Length; k++)
                    {
                        this.players_names[k] = "";
                    }
                    for (int l = 0; l < this.websocket.usernames_.Length; l++)
                    {
                        this.websocket.usernames_[l] = "";
                    }
                    this.Game_State = 2;
                    break;
                default:
                    return;
            }
        }

        // Token: 0x06000016 RID: 22 RVA: 0x00003FF0 File Offset: 0x000021F0
        public void DrawQueueList()
        {
            int num = 900;
            int num2 = 600;
            try
            {
                new ContainerElement(new PointF(640f - (float)(num / 2), 360f - (float)(num2 / 2)), new SizeF((float)num, (float)num2), Color.FromArgb(150, Color.Black))
                {
                    Enabled = true
                }.Draw();
            }
            catch (Exception)
            {
            }
            this.DrawRaceMenuItem(string.Concat(new string[]
            {
                "Joined: ",
                this.player_number.ToString(),
                "/",
                this.max_players.ToString(),
                " | Write !join in chat to enter the race"
            }), 640f, -130f, 0, Color.White, 0.8f, true, true, true, false);
            this.DrawRaceMenuItem("___________________________", 640f, -120f, 0, Color.White, 0.8f, true, true, true, false);
            this.DrawRaceMenuItem("Players List", 640f, -80f, 0, Color.Blue, 0.7f, true, true, true, false);
            int num3 = 230;
            int num4 = 35;
            int num5 = num3;
            int num6 = num4;
            int num7 = 0;
            int num8 = 0;
            try
            {
                for (int i = 0; i < this.players_names.Length; i++)
                {
                    if (this.players_names[i].Length > 0)
                    {
                        string text = this.players_names[i];
                        int num9 = 13;
                        if (text.Length > num9)
                        {
                            text = text.Substring(0, Math.Min(text.Length, num9));
                        }
                        this.DrawRaceMenuItem((i + 1).ToString() + ". " + text, (float)(num5 + num7 * 170), (float)(-1 * num6 + num8 * 25), 0, Color.White, 0.4f, true, true, false, false);
                        if (num8 >= 19)
                        {
                            num8 = 0;
                            num7++;
                        }
                        else
                        {
                            num8++;
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        // Token: 0x06000017 RID: 23 RVA: 0x000041E8 File Offset: 0x000023E8
        public void RacingCam()
        {
            if (this.racing_cam)
            {
                Game.DisableControlThisFrame(GTA.Control.VehicleExit);
                if (!Game.Player.Character.IsInVehicle() && this.vehicles[0] != null)
                {
                    Game.Player.Character.SetIntoVehicle(this.vehicles[0], VehicleSeat.Passenger);
                    World.RenderingCamera = null;
                }
            }
        }

        public static int CurrentRaceID = -1;
        public int StartNewRace()
        {
            if (CurrentRaceID != -1)
            {
                return CurrentRaceID; // Use the existing race ID if the race is ongoing
            }

            using (var context = new RaceDbContext())
            {
                var race = new Race();
                context.Races.Add(race);
                context.SaveChanges();
                CurrentRaceID = race.RaceID; // Store the new race ID
                return race.RaceID;
            }
        }
        public void SaveRaceResults(int raceID, string first, string second, string third)
        {
            try
            {
                if (raceID == -1 || (string.IsNullOrEmpty(first)))
                    return; // Prevent saving empty race results

                using (var context = new RaceDbContext())
                {
                    if (!string.IsNullOrEmpty(first))
                    {
                        context.PlayerWins.Add(new PlayerWin { RaceID = raceID, PlayerName = ExtractName(first), Placement = 1, Points = 3 });
                    }
                    if (!string.IsNullOrEmpty(second))
                    {
                        context.PlayerWins.Add(new PlayerWin { RaceID = raceID, PlayerName = ExtractName(second), Placement = 2, Points = 2 });
                    }
                    if (!string.IsNullOrEmpty(third))
                    {
                        context.PlayerWins.Add(new PlayerWin { RaceID = raceID, PlayerName = ExtractName(third), Placement = 3, Points = 1 });
                    }
                    context.SaveChanges();
                }
            }
            catch
            {

            }
          
        }
        // Token: 0x06000018 RID: 24 RVA: 0x00004244 File Offset: 0x00002444
        public void DrawWinnerList()
        {
            int num = 400;
            int num2 = 450;
            try
            {
                new ContainerElement(new PointF(640f - (float)(num / 2), 360f - (float)(num2 / 2)), new SizeF((float)num, (float)num2), Color.FromArgb(150, Color.Black))
                {
                    Enabled = true
                }.Draw();
            }
            catch (Exception)
            {
            }
            int num3 = -100;
            this.DrawRaceMenuItem("Leaderboard", 640f, (float)(-130 - num3), 0, Color.White, 0.8f, true, true, true, false);
            this.DrawRaceMenuItem("____________", 640f, (float)(-120 - num3), 0, Color.White, 0.8f, true, true, true, false);
            this.DrawRaceMenuItem("1st " + this.place_1, 640f, (float)(-90 - num3 + 40), 0, Color.White, 0.9f, true, true, true, false);
            this.DrawRaceMenuItem("2nd " + this.place_2, 640f, (float)(-30 - num3 + 40), 0, Color.White, 0.6f, true, true, true, false);
            if (this.player_number < 2)
            {
                return;
            }
            this.DrawRaceMenuItem("3rd " + this.place_3, 640f, (float)(20 - num3 + 40), 0, Color.White, 0.4f, true, true, true, false);
          

            //File.AppendAllText("results.csv", $"{place_1},1");
            //File.AppendAllText("results.csv", $"{place_2},2");
            //File.AppendAllText("results.csv", $"{place_3},3");
        }

        // Token: 0x06000019 RID: 25 RVA: 0x000043A4 File Offset: 0x000025A4
        public void CCD()
        {
            if (this.loadeddata)
            {
                if (!this.done)
                {
                    this.websocket = new WSC();
                    this.websocket.Start("first");
                    this.done = true;
                }
                if (!this.switchconn && this.switchx)
                {
                    this.ws_total_likes = this.websocket.total_likes;
                    this.ws_total_likesto = this.websocket.total_likesto;
                    this.ws_total_likes2 = this.websocket.total_likes2;
                    this.ws_total_likesto2 = this.websocket.total_likesto2;
                    this.ws_disable_time = this.websocket.disable_time;
                    this.ws_follow_e = this.websocket.follow_e;
                    this.ws_share_e = this.websocket.share_e;
                    this.ws_join_e = this.websocket.join_e;
                    this.ws_like_e = this.websocket.like_e;
                    this.ws_like_e2 = this.websocket.like_e2;
                    this.ws_main_info = this.websocket.main_info;
                    this.websocket = new WSC();
                    this.websocket.Start("second");
                    this.switchconn = true;
                }
                if (!this.initmsg && this.info_t != "nothing")
                {
                    this.initmsg = true;
                    this.websocket.SendMsg("inflex331@" + this.info_t);
                }
                if (!this.initmsg2 && !this.websocket.switchok && this.switchconn)
                {
                    this.initmsg2 = true;
                    this.websocket.total_likes = this.ws_total_likes;
                    this.websocket.total_likesto = this.ws_total_likesto;
                    this.websocket.total_likes2 = this.ws_total_likes2;
                    this.websocket.total_likesto2 = this.ws_total_likesto2;
                    this.websocket.disable_time = this.ws_disable_time;
                    this.websocket.follow_e = this.ws_follow_e;
                    this.websocket.share_e = this.ws_share_e;
                    this.websocket.join_e = this.ws_join_e;
                    this.websocket.like_e = this.ws_like_e;
                    this.websocket.like_e2 = this.ws_like_e2;
                    this.websocket.main_info = this.ws_main_info;
                    this.websocket.SendMsg("me");
                }
                if (this.websocket.switchx)
                {
                    this.switchx = true;
                }
                if (this.websocket.eventrun != null)
                {
                    this.websocket.Game_State_T = this.Game_State;
                    this.websocket.ai_mode = this.ai_mode;
                    try
                    {
                        var eventrun = this.websocket.eventrun;
                        if (eventrun.Count > 0)
                        {
                            var enumerable = eventrun.First();

                            if (Game.Player.Character.IsDead)
                            {
                                return;
                            }
                            eventrun.Remove(enumerable.Key);
                            this.CreatePlayer(enumerable.Value.NickName, enumerable.Value.Car);
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
                if (this.websocket.eventboostplayer == null)
                {
                    return;
                }
                try
                {
                    var eventboostplayer = this.websocket.eventboostplayer;
                    if (eventboostplayer.Count > 0)
                    {
                        var enumerable2 = eventboostplayer.First();

                        if (!Game.Player.Character.IsDead)
                        {
                            foreach (var boost in websocket.eventboostplayer.Values)
                            {
                                BoostPlayer(boost.Ped_ID, boost.BoostStrength, boost.Nitro);
                            }
                            websocket.eventboostplayer.Clear();
                            eventboostplayer.Remove(enumerable2.Key);
                        }
                    }
                    return;
                }
                catch (Exception)
                {
                    return;
                }
            }
            if (!this.blockl)
            {
                try
                {
                    using (StreamReader streamReader = new StreamReader("./scripts/info.json"))
                    {
                        JObject jobject = JObject.Parse(streamReader.ReadToEnd());
                        this.info_t = jobject.GetValue("info").ToString();
                        if (this.info_t != null)
                        {
                            this.loadeddata = true;
                        }
                        this.blockl = true;
                    }
                }
                catch (Exception ex)
                {
                    this.infox = "Failed to load data! " + ex.Message.ToString();
                }
            }
        }

        // Token: 0x0600001A RID: 26 RVA: 0x00004834 File Offset: 0x00002A34
        public void DrawTextI(string text, float x, Color color, int padd, float size = 0f)
        {
            try
            {
                PointF position = new PointF(0f, 0f);
                TextElement textElement = new TextElement(text, position, 1f)
                {
                    Color = color,
                    Scale = 1f + size,
                    Font = RenderFont,
                    Outline = true
                };
                textElement.Position = new PointF(640f - textElement.Width / 2f, (float)(20 + padd * 10));
                textElement.Draw();
            }
            catch (Exception)
            {
            }
        }

        // Token: 0x0600001B RID: 27 RVA: 0x000048C4 File Offset: 0x00002AC4
        public void BoostPlayer(int id, int speed, bool nitro)
        {
            try
            {
                if (this.peds[id] != null)
                {

                    Ped ped = this.peds[id];
                    this.players_speed[id] += speed;
                    int num = this.players_speed[id];
                    Function.Call(Hash.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE, ped, ped.LastVehicle, this.race_point.X, this.race_point.Y, this.race_point.Z, (float)num, 262196, 10f);
                    if (nitro)
                    {
                        ApplyNitroForce(ped.LastVehicle, 100, id);

                    }
                }
            }
            catch (Exception e)
            {
            }
        }


        public string ExtractName(string fullName)
        {
            Match match = Regex.Match(fullName, @"\((.*?)\)");
            return match.Success ? match.Groups[1].Value : fullName; // Return extracted name or full name if not found
        }
        public string ExtractOutsideName(string fullName)
        {
            return Regex.Replace(fullName, @"\s*\(.*?\)", "").Trim();
        }

        private void ApplyNitroForce(Vehicle vehicle, int speed, int id)
        {
            float boostStrength = speed; // Adjust boost strength
            Vector3 forwardForce = vehicle.ForwardVector * boostStrength;
            vehicle.ApplyForce(forwardForce);
        }

        private void StartRestartCountdown()
        {
            restartCountdownActive = true;
            restartCountdownEndTime = nextRaceTime; // Use nextRaceTime directly
        }
        private void DrawRestartCountdown()
        {
            if (restartCountdownActive)
            {
                int remainingTime = Math.Max(0, (restartCountdownEndTime - Game.GameTime) / 1000);
                string countdownText = $"Restarting in: {remainingTime}s";

                try
                {
                    PointF position = new PointF(640f, 670f); // Bottom center of the screen
                    TextElement countdownElement = new TextElement(countdownText, position, 0.8f, Color.Yellow, GTA.UI.Font.ChaletLondon, Alignment.Center, false, true);
                    countdownElement.Draw();
                }
                catch (Exception) { }

                if (remainingTime <= 0)
                {
                    restartCountdownActive = false;
                    RestartRace();
                }
            }
        }

        // Token: 0x0600001C RID: 28 RVA: 0x00004988 File Offset: 0x00002B88
        public int GetName(string name)
        {
            int result = -1;
            for (int i = 0; i < this.players_names.Length; i++)
            {
                string value = name.ToLower();
                if (this.players_names[i].ToLower().Contains(value) && this.c_camera != i)
                {
                    result = i;
                    break;
                }
            }
            return result;
        }

        private int nextRaceTime = 0;
        private bool raceRestartPending = false;
        private bool restartCountdownActive = false;
        private int restartCountdownEndTime = 0;
        // Token: 0x04000002 RID: 2
        public Camera cam;

        // Token: 0x04000003 RID: 3
        public bool Initialization;

        // Token: 0x04000004 RID: 4
        public string Version = "v0.8";

        // Token: 0x04000005 RID: 5
        public Ped[] peds = new Ped[256];

        // Token: 0x04000006 RID: 6
        public int[] players_speed = new int[256];

        // Token: 0x04000007 RID: 7
        public int[] players_ai = new int[256];

        // Token: 0x04000008 RID: 8
        public Vehicle[] vehicles = new Vehicle[256];

        // Token: 0x04000009 RID: 9
        public string[] players_names = new string[256];

        // Token: 0x0400000A RID: 10
        public string[] player_position = new string[256];

        // Token: 0x0400000B RID: 11
        public string name_f = "undefined";

        // Token: 0x0400000C RID: 12
        public string name_fs = "";

        // Token: 0x0400000D RID: 13
        public int player_number;

        // Token: 0x0400000E RID: 14
        public int max_players = 100;

        // Token: 0x0400000F RID: 15
        public int Players_r_n = 1;

        // Token: 0x04000010 RID: 16
        public int Players_r_nr;

        // Token: 0x04000011 RID: 17
        public float Players_Y;

        // Token: 0x04000012 RID: 18
        public float Players_X;

        // Token: 0x04000013 RID: 19
        public const int IDLE = 0;

        // Token: 0x04000014 RID: 20
        public const int SETUP_DONE = 2;

        // Token: 0x04000015 RID: 21
        public const int QUEUE = 3;

        // Token: 0x04000016 RID: 22
        public const int READY = 4;

        // Token: 0x04000017 RID: 23
        public const int COUNTDOWN = 5;

        // Token: 0x04000018 RID: 24
        public const int RACE = 6;

        // Token: 0x04000019 RID: 25
        public const int WIN = 7;

        // Token: 0x0400001A RID: 26
        public const int WINNERS = 8;

        // Token: 0x0400001B RID: 27
        public const int CLEAN = 9;

        // Token: 0x0400001C RID: 28
        public bool debug_lock;

        // Token: 0x0400001D RID: 29
        public bool is_debug;

        // Token: 0x0400001E RID: 30
        public bool peds_en = true;

        // Token: 0x0400001F RID: 31
        public bool race_menu;

        // Token: 0x04000020 RID: 32
        public bool hold_f;

        // Token: 0x04000021 RID: 33
        public bool queue_list;

        // Token: 0x04000022 RID: 34
        public bool winners_list;

        // Token: 0x04000023 RID: 35
        public int Game_State;

        // Token: 0x04000024 RID: 36
        public int countdown = 10;

        // Token: 0x04000025 RID: 37
        public bool countdown_;

        // Token: 0x04000026 RID: 38
        public bool is_racing;

        // Token: 0x04000027 RID: 39
        public bool racing_cam;

        // Token: 0x04000028 RID: 40
        public int c_camera;

        // Token: 0x04000029 RID: 41
        public int place = 1;

        // Token: 0x0400002A RID: 42
        public string place_1 = "";

        // Token: 0x0400002B RID: 43
        public string place_2 = "";

        // Token: 0x0400002C RID: 44
        public string place_3 = "";

        // Token: 0x0400002D RID: 45
        public bool SentT;

        // Token: 0x0400002E RID: 46
        public WSC websocket;

        // Token: 0x0400002F RID: 47
        private bool done;

        // Token: 0x04000030 RID: 48
        private bool initmsg;

        // Token: 0x04000031 RID: 49
        private bool loadeddata;

        // Token: 0x04000032 RID: 50
        private bool blockl;

        // Token: 0x04000033 RID: 51
        public string infox = "";

        // Token: 0x04000034 RID: 52
        public string info_t = "";

        // Token: 0x04000035 RID: 53
        public int sound_play_id;

        // Token: 0x04000036 RID: 54
        public int sound_play_id_s = 1;

        // Token: 0x04000037 RID: 55
        public int countdown_time = 11;

        // Token: 0x04000038 RID: 56
        public bool ai_mode;

        // Token: 0x04000039 RID: 57
        public Vector3 race_point = new Vector3(0f, 0f, 0f);

        // Token: 0x0400003A RID: 58
        public Vector3 camera_look_at = new Vector3(1621f, 3228f, 40.41968f);

        // Token: 0x0400003B RID: 59
        public Vector3 camera_pos = new Vector3(1768f, 3266f, 60f);

        // Token: 0x0400003C RID: 60
        private bool switchx;

        // Token: 0x0400003D RID: 61
        private bool switchconn;

        // Token: 0x0400003E RID: 62
        public int ws_total_likes;

        // Token: 0x0400003F RID: 63
        public int ws_total_likesto = 500;

        // Token: 0x04000040 RID: 64
        public int ws_total_likes2;

        // Token: 0x04000041 RID: 65
        public int ws_total_likesto2 = 500;

        // Token: 0x04000042 RID: 66
        public int ws_disable_time = 10;

        // Token: 0x04000043 RID: 67
        public string ws_follow_e = "none";

        // Token: 0x04000044 RID: 68
        public string ws_share_e = "none";

        // Token: 0x04000045 RID: 69
        public string ws_join_e = "none";

        // Token: 0x04000046 RID: 70
        public string ws_like_e = "none";

        // Token: 0x04000047 RID: 71
        public string ws_like_e2 = "none";

        // Token: 0x04000048 RID: 72
        public JObject ws_main_info;

        // Token: 0x04000049 RID: 73
        private bool initmsg2;

        private bool autoSpectate = false;
        private int spectateIndex = 0;
        private int nextSwitchTime = 0;
        private int raceStartTime = Game.GameTime + 300000;
        private bool raceTimeoutActive = false;
        private int raceTimeoutEnd = 0;
    }
}
