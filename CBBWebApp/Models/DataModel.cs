using CBBWebApp.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CBBWebApp.Models;

public class DataModel
{

    public class CollegeModel
    {
        public int Rank { get; set; }
        public string Name { get; set; }
        public string Conference { get; set; }
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public decimal ADJOE { get; set; } //higher is better
        public decimal ADJDE { get; set; } //lower is better
        public decimal BARTHAG { get; set; } //fraction, higher is better
        public decimal EFG_O { get; set; } //offensive field goal percentage, higher is better (in 00.0 form)
        public decimal EFG_D { get; set; } //defensive field goal percentage allowed, lower is better (in 0.00 form)
        public decimal TOR_O { get; set; } //offensive turnover percentage, lower is better (in 00.0 form)
        public decimal TOR_D { get; set; } //defensive turnover percentage, higher is better (in 00.0 form)
        public decimal ORB { get; set; } //offensive rebounds
        public decimal DRB { get; set; } //defensive rebounds
        public decimal FTR_O { get; set; } //free throw rate (in 0.00 form)
        public decimal FTR_D { get; set; } //free throw rate allowed (in 0.00 form)
        public decimal PT2_O { get; set; } //offensive two point shooting percentage (in 0.00 form)
        public decimal PT2_D { get; set; } //defensive two point shooting percentage allowed (in 0.00 form)
        public decimal PT3_O { get; set; } //offensive three point shooting percentage (in 0.00 form)
        public decimal PT3_D { get; set; } //defensive three point shooting percentage allowed (in 0.00 form)
        public decimal ADJ_T { get; set; } //an estimate of tempo (posessions per 40 minutes)
        public decimal WAB { get; set; } //wins above the bubble (can be negative)
        public decimal FTP { get; set; } //freethrow percentage
        
        public List<SelectListItem> TeamNames { get; set; }
        public string SelectedTeam1 { get; set; }
        public string SelectedTeam2 { get; set; }

        public CollegeModel()
        {
            this.Rank = 0;
            this.Name = "";
            this.Conference = "";
            this.GamesPlayed = 0;
            this.Wins = 0;
            this.Losses = 0;
            this.ADJOE = 0;
            this.ADJDE = 0;
            this.BARTHAG = 0;
            this.EFG_O = 0;
            this.EFG_D = 0;
            this.FTP = 0;
        }

    }

    public class PosessionResult
    {
        public int PointsScored { get; set; }
        public int SecondsUsed { get; set; }
        public bool OffenseKeepsPosession { get; set; }
        public bool DefensiveFoul { get; set; }
        public bool OffensiveFoul { get; set; }
        public PosessionResult()
        {
            PointsScored = 0;
            SecondsUsed = 0;
            OffenseKeepsPosession = false;
            DefensiveFoul = false;
            OffensiveFoul = false;
        }
    }

    public class MatchupResult
    {
        public string AwayTeam { get; set; }
        public string HomeTeam { get; set; }
        public int HomeTeamScore { get; set; }
        public int AwayTeamScore { get; set; }
        public string Winner { get; set; }
        public string Loser { get; set; }
        public int WinnerScore { get; set; }
        public int LoserScore { get; set; }
        public int Overtimes { get; set; }

        public MatchupResult()
        {
            this.HomeTeam = "";
            this.AwayTeam = "";
            this.HomeTeamScore = 0;
            this.AwayTeamScore = 0;
            this.Winner = "";
            this.Loser = "";
            this.WinnerScore = 0;
            this.LoserScore = 0;
            this.Overtimes = 0;
        }


    }
}