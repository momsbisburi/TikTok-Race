using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using GTA;

namespace TikTok_Race.Models
{
    public class BoostPlayerModel
    {
        public int Ped_ID { get; set; }
        public EventSource Source { get; set; }

        public int BoostStrength { get; set; }
        public bool Nitro { get; set; } = false;
    }

    public class CreatePlayerModel
    {
        public EventSource Source { get; set; }
        public string NickName { get; set; }
        public VehicleHash Car { get; set; }
    }

    public enum EventSource
    {
        Like,
        Gift,
        Share,
        Comment,
    }

    public class Race
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RaceID { get; set; } // Unique race identifier

        public DateTime RaceDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<PlayerWin> PlayerWins { get; set; }
    }
    public class PlayerWin
    {
        [Key]
        public int PlayerWinID { get; set; }

        [ForeignKey("Race")]
        public int RaceID { get; set; }
        public virtual Race Race { get; set; }

        public string PlayerName { get; set; }
        public int Placement { get; set; } // 1st, 2nd, 3rd
        public int Points { get; set; } // 3, 2, 1 points based on placement
    }

    public class Player
    {
        [Key]
        public int PlayerID { get; set; }
        public string uniqueId { get; set; }
        public string nickname { get; set; }
        public string ProfilePicture { get; set; }


    }
}