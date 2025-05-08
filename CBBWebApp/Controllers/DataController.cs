using CBBWebApp.Models;
using HtmlAgilityPack;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CBBWebApp.Controllers;
 public enum GameSpeed
{
    Instant = 0,
    Fast = 1,
    Medium = 2,
    Slow = 3
}

public class DataController : Controller
{
    public static int adjuster2PT { get; set; }
    public static int adjuster3PT { get; set; }
    public static int adjusterTurnover { get; set; }
    public static int adjusterFoul { get; set; }
    public static int adjusterHomeTeam { get; set; }
    public static List<DataModel.CollegeModel> GlobalCollegeData { get; set; }
    
    public static int team1Score = 0;
    public static int team2Score = 0;
    public static int team1Fouls = 0;
    public static int team2Fouls = 0;
    public static int timeLeft = 2400;
    public static bool team1HasBall = true;
    
    
    
    private readonly ILogger<DataController> _logger;

    public DataController(ILogger<DataController> logger)
    {
        _logger = logger;
    }

    public IActionResult Data()
    {
        var teamData = ScrapeLive2025Data().Result;
        Console.WriteLine(teamData[0]);

        var model = new DataModel.CollegeModel();
        model.TeamNames = new List<SelectListItem>();

        foreach (var team in teamData)
        {
            model.TeamNames.Add(new SelectListItem{ Text = team.Name, Value = team.Name});
        }

        return View(model);
    }

    public IActionResult Result()
    {
        return View(ViewBag.MatchupDetails);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    
    [HttpPost]
    public IActionResult RunLiveMatchup(DataModel.CollegeModel model)
    {
        var team1 = model.SelectedTeam1;
        var team2 = model.SelectedTeam2;
        GlobalCollegeData = ScrapeLive2025Data().Result;
        RunMatchup(team1, team2, adjuster2PT, adjuster3PT, adjusterTurnover, adjusterFoul, GameSpeed.Fast, "2025",
            false);
        return RedirectToAction("Result");
    }
    
    public static void RunProgram()
    {
    //     Console.WriteLine("Loading Program Data...");
    //     //initialize the global college data
    //     GlobalCollegeData = ScrapeLive2025Data().Result;
    //     Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //     Console.WriteLine("-College basketball simulator for 2023-2024 season.");
    //     Console.WriteLine("-Work in progress, some features are not yet implemented: ");
    //     Console.WriteLine("-Overtime: not yet implemented, the first team entered in the matchup is current just given the win if the game ends in a tie.");
    //     Console.WriteLine("-Offensive rebounding on free throws: currently the defense always gets the rebound off a missed free throw.");
    //     Console.WriteLine("-Free throw percentages: currently only Big 12 and march madness teams have free throw percentages in the .csv file.  All other teams use a default value.");
    //     Console.WriteLine("-End game situations: teams do not handle end game situations effeciently (fouling if losing, shooting the correct shot based on the score, etc.). ");
    //     Console.WriteLine("the only current team strategy is a higher chance of shooting a 3 pointer vs a 2 pointer if the team is losing. ");
    //     Console.WriteLine("in the future I'm thinking about adding a coach/gameplan that can be linked to the team, so each posession the game status can be passed to the coach and the desired game plan can be returned. ");
    //     Console.WriteLine("");
    //     Console.WriteLine("-Before running a simulation, be sure to set the correct path to the cbb24.csv file relative to your machine.  That path can be found in DataController.cs. ");
    //     Console.WriteLine("-The team name you enter in a individual game simulation must be an exact match of the name in the .csv file.  Open the .csv to see the full list of names. ");
    //     Console.WriteLine("for quick reference, a list of every march madness team name can be found in DataController.cs. ");
    //     Console.WriteLine("-V1.2 New features: ");
    //     Console.WriteLine("Live 2025 data scraped from the web in real time.");
    //     Console.WriteLine("Multi-matchup mode: pick two teams and have them play each other multiple times, and average the results.");
    //     Console.WriteLine("Big 12 simulation mode: multi-matchup every game this Big 12 season (through week 3)");
    //     Console.WriteLine("-V1.3 New Features: ");
    //     Console.WriteLine("In-depth statistics for Big 12 Simulations. ");
    //     Console.WriteLine("Adjuster added to boost home team statistics. ");
    //     Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //     Console.WriteLine("");
    //
    //     
    //
    //     while (true)
    //     {
    //         int adjuster2pt = -10;
    //         int adjuster3pt = -10;
    //         int adjusterTurnover = 0;
    //         int adjusterFoul = 0;
    //         bool loop1 = true;
    //         int mode = 0;
    //         while (loop1)
    //         {
    //             string input1;
    //             Console.WriteLine("Enter:");
    //             Console.WriteLine("'g' to simulate an individual game");
    //             Console.WriteLine("'b' to simulate the march madness bracket");
    //             Console.WriteLine("'n' to simulate a game with live 2025 data");
    //             Console.WriteLine("'m' to simulate multi matchup between 2 teams (2025)");
    //             Console.WriteLine("'s' to simulate the Big 12 Season so far (2025)");
    //             Console.WriteLine("'stats' to calculate prediction statistics");
    //             Console.WriteLine("'q' to Quit");
    //             input1 = Console.ReadLine();
    //             if(input1 == "g")
    //             {
    //                 loop1 = false;
    //                 mode = 1;
    //             }
    //             else if(input1 == "b")
    //             {
    //                 loop1 = false;
    //                 mode = 0;
    //             }
    //             else if(input1 == "n")
    //             {
    //                 loop1 = false;
    //                 mode = 2;
    //                 
    //             }
    //             else if(input1 == "m")
    //             {
    //                 loop1 = false;
    //                 mode = 3;
    //             }
    //             else if(input1 == "s")
    //             {
    //                 loop1 = false;
    //                 mode = 4;
    //             }
    //             else if(input1 == "stats")
    //             {
    //                 loop1 = false;
    //                 mode = 5;
    //             }
    //             else if (input1 == "q")
    //             {
    //                 Environment.Exit(0);
    //             }
    //             else
    //             {
    //                 Console.WriteLine("Invalid input, please try again. ");
    //             }
    //         }
    //
    //         if(mode == 0)
    //         {
    //             //tournament
    //             Console.WriteLine("");
    //             Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //             Console.WriteLine("March Madness Bracket Simulator");
    //             Console.WriteLine("This will simulate the entire bracket, one round at a time.");
    //             Console.WriteLine("Correct picks will have an X by their name in the parentheses.");
    //             Console.WriteLine("The score of each round will be displayed at the end of the round, ");
    //             Console.WriteLine("and the final score of the bracket will be displayed at the end.");
    //             Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //             Console.WriteLine("");
    //             bool loop2 = true;
    //             while (loop2)
    //             {
    //                 string input2;
    //                 Console.WriteLine("Enter 'go' to start the simulation: ");
    //                 input2 = Console.ReadLine();
    //                 if( input2 == "go")
    //                 {
    //                     loop2 = false;
    //                 }
    //                 else
    //                 {
    //                     Console.WriteLine("Invalid input, please try again. ");
    //                 }
    //             }
    //             DataController.SimulateMarchMadness(adjuster2pt, adjuster3pt, adjusterTurnover, adjusterFoul);
    //         }
    //         else if(mode == 1)
    //         {
    //             //individual game
    //             Console.WriteLine("");
    //             Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //             Console.WriteLine("Individual Game Simulator");
    //             Console.WriteLine("A timestamp will be displayed for each action of the game.");
    //             Console.WriteLine("The timestamp consists of the time left in the half, the game score, ");
    //             Console.WriteLine("the fouls for each team (in parentheses), and a description of the action.");
    //             Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //             Console.WriteLine("");
    //             string team1;
    //             string team2;
    //             Console.WriteLine("Enter Away Team Name: ");
    //             team1 = Console.ReadLine();
    //             Console.WriteLine("Enter Home Team Name: ");
    //             team2 = Console.ReadLine();
    //             bool loop3 = true;
    //             GameSpeed simSpeed = GameSpeed.Instant;
    //             while (loop3)
    //             {
    //                 string input3;
    //                 Console.WriteLine("Enter simulation speed: 's' for slow, 'm' for medium, 'f' for fast: ");
    //                 input3 = Console.ReadLine();
    //                 if (input3 == "s")
    //                 {
    //                     loop3 = false;
    //                     simSpeed = GameSpeed.Slow;
    //                 }
    //                 else if (input3 == "m")
    //                 {
    //                     loop3 = false;
    //                     simSpeed = GameSpeed.Medium;
    //                 }
    //                 else if(input3 == "f")
    //                 {
    //                     loop3 = false;
    //                     simSpeed = GameSpeed.Fast;
    //                 }
    //                 else
    //                 {
    //                     Console.WriteLine("Invalid input, please try again. ");
    //                 }
    //             }
    //             RunMatchup(team1, team2, adjuster2pt, adjuster3pt, adjusterTurnover, adjusterFoul, simSpeed, "2024", false);
    //         }
    //         else if(mode == 2)
    //         {
    //             //2025 matchup mode
    //             Console.WriteLine("");
    //             Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //             Console.WriteLine("2025 Mode");
    //             Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //             Console.WriteLine("");
    //             string team1;
    //             string team2;
    //             Console.WriteLine("Enter Away Team Name: ");
    //             team1 = Console.ReadLine();
    //             Console.WriteLine("Enter Home Team Name: ");
    //             team2 = Console.ReadLine();
    //             bool loop3 = true;
    //             GameSpeed simSpeed = GameSpeed.Instant;
    //             while (loop3)
    //             {
    //                 string input3;
    //                 Console.WriteLine("Enter simulation speed: 's' for slow, 'm' for medium, 'f' for fast: ");
    //                 input3 = Console.ReadLine();
    //                 if (input3 == "s")
    //                 {
    //                     loop3 = false;
    //                     simSpeed = GameSpeed.Slow;
    //                 }
    //                 else if (input3 == "m")
    //                 {
    //                     loop3 = false;
    //                     simSpeed = GameSpeed.Medium;
    //                 }
    //                 else if (input3 == "f")
    //                 {
    //                     loop3 = false;
    //                     simSpeed = GameSpeed.Fast;
    //                 }
    //                 else
    //                 {
    //                     Console.WriteLine("Invalid input, please try again. ");
    //                 }
    //             }
    //             RunMatchup(team1, team2, adjuster2pt, adjuster3pt, adjusterTurnover, adjusterFoul, simSpeed, "2025", true);
    //         }
    //         else if(mode == 3)
    //         {
    //             //2025 multi matchup mode
    //             Console.WriteLine("");
    //             Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //             Console.WriteLine("2025 Multi Matchup Mode");
    //             Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //             Console.WriteLine("");
    //             string team1;
    //             string team2;
    //             Console.WriteLine("Enter Away Team Name: ");
    //             team1 = Console.ReadLine();
    //             Console.WriteLine("Enter Home Team Name: ");
    //             team2 = Console.ReadLine();
    //             bool loop3 = true;
    //             int times = 0;
    //             while (loop3)
    //             {
    //                 string input3;
    //                 Console.WriteLine("Enter number of times to run matchup: ");
    //                 input3 = Console.ReadLine();
    //                 try
    //                 {
    //                     times = Convert.ToInt32(input3);
    //                     loop3 = false;
    //                 }
    //                 catch
    //                 {
    //
    //                 }
    //             }
    //             RunMultiMatchup(team1, team2, times);
    //         }
    //         else if (mode == 4)
    //         {
    //             //2025 big 12 season mode
    //             Console.WriteLine("");
    //             Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //             Console.WriteLine("2025 Big 12 Season Mode");
    //             Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //             Console.WriteLine("");
    //             bool loop3 = true;
    //             int times = 0;
    //             while (loop3)
    //             {
    //                 string input3;
    //                 Console.WriteLine("Enter number of times to run each matchup: ");
    //                 input3 = Console.ReadLine();
    //                 try
    //                 {
    //                     times = Convert.ToInt32(input3);
    //                     loop3 = false;
    //                 }
    //                 catch
    //                 {
    //
    //                 }
    //             }
    //             RunBig12Season(times, adjuster2pt, adjuster3pt, adjusterTurnover, adjusterFoul);
    //         }
    //         else if(mode == 5)
    //         {
    //             //stats calculation mode
    //             Console.WriteLine("");
    //             Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //             Console.WriteLine("Stats Calculation Mode");
    //             Console.WriteLine("--------------------------------------------------------------------------------------------------------------");
    //             Console.WriteLine("");
    //
    //             int r1;
    //             int r2;
    //             int r3;
    //             int r4;
    //             int r5;
    //             int r6;
    //             int r7;
    //             int r8;
    //             int r9;
    //             int r10;
    //             int r11;
    //             int r12;
    //             int r13;
    //             int r14;
    //             int r15;
    //             int r16;
    //             int p1;
    //             int p2;
    //             int p3;
    //             int p4;
    //             int p5;
    //             int p6;
    //             int p7;
    //             int p8;
    //             int p9;
    //             int p10;
    //             int p11;
    //             int p12;
    //             int p13;
    //             int p14;
    //             int p15;
    //             int p16;
    //
    //             Console.WriteLine("Prediction Score 1?:");
    //             p1 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 2?:");
    //             p2 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 3?:");
    //             p3 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 4?:");
    //             p4 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 5?:");
    //             p5 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 6?:");
    //             p6 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 7?:");
    //             p7 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 8?:");
    //             p8 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 9?:");
    //             p9 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 10?:");
    //             p10 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 11?:");
    //             p11 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 12?:");
    //             p12 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 13?:");
    //             p13 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 14?:");
    //             p14 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 15?:");
    //             p15 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Prediction Score 16?:");
    //             p16 = Convert.ToInt32(Console.ReadLine());
    //
    //             Console.WriteLine("Real Score 1?:");
    //             r1 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 2?:");
    //             r2 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 3?:");
    //             r3 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 4?:");
    //             r4 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 5?:");
    //             r5 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 6?:");
    //             r6 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 7?:");
    //             r7 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 8?:");
    //             r8 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 9?:");
    //             r9 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 10?:");
    //             r10 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 11?:");
    //             r11 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 12?:");
    //             r12 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 13?:");
    //             r13 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 14?:");
    //             r14 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 15?:");
    //             r15 = Convert.ToInt32(Console.ReadLine());
    //             Console.WriteLine("Real Score 16?:");
    //             r16 = Convert.ToInt32(Console.ReadLine());
    //
    //             int t1 = p1 - r1;
    //             int t2 = p2 - r2;
    //             int t3 = p3 - r3;
    //             int t4 = p4 - r4;
    //             int t5 = p5 - r5;
    //             int t6 = p6 - r6;
    //             int t7 = p7 - r7;
    //             int t8 = p8 - r8;
    //             int t9 = p9 - r9;
    //             int t10 = p10 - r10;
    //             int t11 = p11 - r11;
    //             int t12 = p12 - r12;
    //             int t13 = p13 - r13;
    //             int t14 = p14 - r14;
    //             int t15 = p15 - r15;
    //             int t16 = p16 - r16;
    //
    //             int plusMinusNet = t1 + t2 + t3 + t4 + t5 + t6 + t7 + t8 + t9 + t10 + t11 + t12 + t13 + t14 + t15 + t16;
    //             int plusMinusTotal = Math.Abs(t1) + Math.Abs(t2) + Math.Abs(t3) + Math.Abs(t4) + Math.Abs(t5) + Math.Abs(t6) + Math.Abs(t7) + Math.Abs(t8) + Math.Abs(t9) + Math.Abs(t10) + Math.Abs(t11) + Math.Abs(t12) + Math.Abs(t13) + Math.Abs(t14) + Math.Abs(t15) + Math.Abs(t16);
    //             string plusMinusNetAvg = Math.Round(Decimal.Divide(plusMinusNet, 16), 2).ToString();
    //             string plusMinusTotalAvg = Math.Round(Decimal.Divide(plusMinusTotal, 16), 2).ToString();
    //
    //             int m1 = Math.Abs((p1 - p2) - (r1 - r2));
    //             int m2 = Math.Abs((p3 - p4) - (r3 - r4));
    //             int m3 = Math.Abs((p5 - p6) - (r5 - r6));
    //             int m4 = Math.Abs((p7 - p7) - (r7 - r8));
    //             int m5 = Math.Abs((p9 - p10) - (r9 - r10));
    //             int m6 = Math.Abs((p11 - p12) - (r11 - r12));
    //             int m7 = Math.Abs((p13 - p14) - (r13 - r14));
    //             int m8 = Math.Abs((p15 - p16) - (r15 - r16));
    //
    //             int margOfVic = m1 + m2 + m3 + m4 + m5 + m6 + m7 + m8;
    //             string margOfVicAvg = Math.Round(Decimal.Divide(margOfVic, 8), 2).ToString();
    //
    //             Console.WriteLine("-------------------------------------------------");
    //             Console.WriteLine("Statistic Results: ");
    //             Console.WriteLine("Margin of Victory Average: " + margOfVicAvg);
    //             Console.WriteLine("Net +/- Average: " + plusMinusNetAvg);
    //             Console.WriteLine("Total +/- Average: " + plusMinusTotalAvg);
    //             Console.WriteLine("-------------------------------------------------");
    //         }
    //     }
    //     
    //     
    }

    public DataModel.MatchupResult RunMatchup(string team1, string team2, int a2pt, int a3pt, int aturn, int afoul, GameSpeed simSpeed, string year, bool neutralGame)
    {

        //set the adjusters
        adjuster2PT = a2pt;
        adjuster3PT = a3pt;
        adjusterTurnover = aturn;
        adjusterFoul = afoul;
        adjusterHomeTeam = 3;
        //no home team advantage if neutral game
        if(neutralGame == true)
        {
            adjusterHomeTeam = 0;
        }

        int waitTime = 0;
        if(simSpeed == GameSpeed.Fast)
        {
            waitTime = 1;
        }
        else if(simSpeed == GameSpeed.Medium)
        {
            waitTime = 500;
        }
        else if(simSpeed == GameSpeed.Slow)
        {
            waitTime = 2000;
        }
    
        team1Score = 0;
        team2Score = 0;
        team1Fouls = 0;
        team2Fouls = 0;
        timeLeft = 2400;
        team1HasBall = true;
        List<DataModel.CollegeModel> data = new List<DataModel.CollegeModel>();
        data = GlobalCollegeData;
        
        DataModel.CollegeModel team1Model = data.Where(x => x.Name == team1).FirstOrDefault();
        DataModel.CollegeModel team2Model = data.Where(x => x.Name == team2).FirstOrDefault();
        if (team1Model == null || team2Model == null)
        {
            Console.WriteLine("Invalid team name.");
            return new DataModel.MatchupResult();
        }
        
        UpdateFreeThrow(team1Model);
        UpdateFreeThrow(team2Model);

        //print pregame stats
        if(waitTime > 0)
        {
            int team1Adjuster = (int)Math.Round((team1Model.ADJOE + team2Model.ADJDE) / 2);
            team1Adjuster -= 100;
            int team2Adjuster = (int)Math.Round((team2Model.ADJOE + team1Model.ADJDE) / 2);
            team2Adjuster -= 100;
            int team1_3pt = (int)Math.Round((team1Model.PT3_O + team2Model.PT3_D) / 2);
            team1_3pt += team1Adjuster;
            team1_3pt += adjuster3PT;
            int team1_2pt = (int)Math.Round((team1Model.PT2_O + team2Model.PT2_D) / 2);
            team1_2pt += team1Adjuster;
            team1_2pt += adjuster2PT;
            int team2_3pt = (int)Math.Round((team2Model.PT3_O + team1Model.PT3_D) / 2);
            team2_3pt += team2Adjuster;
            team2_3pt += adjuster3PT;
            team2_3pt += adjusterHomeTeam;
            int team2_2pt = (int)Math.Round((team2Model.PT2_O + team1Model.PT2_D) / 2);
            team2_2pt += team2Adjuster;
            team2_2pt += adjuster2PT;
            team2_2pt += adjusterHomeTeam;
            int team1_defReb = (int)Math.Round(team1Model.DRB);
            team1_defReb += team1Adjuster;
            int team1_offReb = (int)Math.Round(team1Model.ORB);
            team1_offReb += team1Adjuster;
            int team2_defReb = (int)Math.Round(team2Model.DRB);
            team2_defReb += team2Adjuster;
            int team2_offReb = (int)Math.Round(team2Model.ORB);
            team2_offReb += team2Adjuster;
            int team1_turn = (int)Math.Round((team1Model.TOR_O + team2Model.TOR_D) / 2);
            team1_turn += adjusterTurnover;
            int team2_turn = (int)Math.Round((team2Model.TOR_O + team1Model.TOR_D) / 2);
            team2_turn += adjusterTurnover;

            int team1_ftp = (int)Math.Round(team1Model.FTP);
            int team2_ftp = (int)Math.Round(team2Model.FTP);

            string team1String = team1 + ":";
            string team2String = team2 + ":";
            int team1Padding = 20 - team1.Length;
            for (int i = 0; i < team1Padding; i++)
            {
                team1String += " ";
            }
            int team2padding = 20 - team2.Length;
            for (int i = 0; i < team2padding; i++)
            {
                team2String += " ";
            }

            team1String += "2pt: " + team1_2pt + "%, 3pt: " + team1_3pt + "%, FT: " + team1_ftp + "%, Def Reb: " + team1_defReb + "%, Off Reb: " + team1_offReb + "%, Turnovers: " + team1_turn + "%";
            team2String += "2pt: " + team2_2pt + "%, 3pt: " + team2_3pt + "%, FT: " + team2_ftp + "%, Def Reb: " + team2_defReb + "%, Off Reb: " + team2_offReb + "%, Turnovers: " + team2_turn + "%";
            Console.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
            Console.WriteLine("Pregame Stats: ");
            Console.WriteLine("(These are the team stats for this game.  Stats are automatically adjusted per game for each team based on the relative strength of the other team.  The home team also gets a stat boost.)");
            Console.WriteLine(team1String);
            Console.WriteLine(team2String);
            Console.WriteLine(
                "@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@");
        }
        
        if(waitTime > 0)
        {
            System.Threading.Thread.Sleep(waitTime * 10);
        }
        




        //run first half
        while (timeLeft > 1200)
        {
            bool shortPosession = false;
            if (team1HasBall == true)
            {
                DataModel.PosessionResult res = RunPoesssion(team1Model, team2Model, team1Score, team2Score, timeLeft, team1HasBall, shortPosession, team2Fouls, team1Fouls, waitTime);
                UdpateScore(res, shortPosession);
            }
            else
            {
                DataModel.PosessionResult res = RunPoesssion(team2Model, team1Model, team2Score, team1Score, timeLeft, team1HasBall, shortPosession, team1Fouls, team2Fouls, waitTime);
                UdpateScore(res, shortPosession);
            }
            //wait a bit
            if(waitTime > 0)
            {
                System.Threading.Thread.Sleep(waitTime);
            }
            

        }

        //print halftime score
        if(waitTime > 0)
        {
            Console.WriteLine("#########################################");
            Console.WriteLine("HALF TIME SCORE: " + team1 + " " + team1Score.ToString() + "-" + team2Score.ToString() + " " + team2);
            Console.WriteLine("#########################################");
            if(waitTime > 0)
            {
                System.Threading.Thread.Sleep(waitTime * 5);
            }
            
        }
        

        //reset fouls
        team1Fouls = 0;
        team2Fouls = 0;
        
        //set time left to 20 min
        timeLeft = 1200;

        //run second half minus final 2 minutes
        while (timeLeft > 120)
        {
            int lead = Math.Abs(team1Score - team2Score);
            bool shortPosession = false;
            if (team1HasBall == true)
            {
                DataModel.PosessionResult res = RunPoesssion(team1Model, team2Model, team1Score, team2Score, timeLeft, team1HasBall, shortPosession, team2Fouls, team1Fouls, waitTime);
                UdpateScore(res, shortPosession);
            }
            else
            {
                DataModel.PosessionResult res = RunPoesssion(team2Model, team1Model, team2Score, team1Score, timeLeft, team1HasBall, shortPosession, team1Fouls, team2Fouls, waitTime);
                UdpateScore(res, shortPosession);
            }
            //wait a bit
            if(waitTime > 0)
            {
                System.Threading.Thread.Sleep(waitTime);
            }
            

        }
        
        // run final 2 minutes
        while (timeLeft > 0)
        {
            
            bool shortPosession = false;
            int lead = Math.Abs(team1Score - team2Score);
            if (timeLeft < 30 && lead > 6)
            {
                break;
            }                 
            if (team1HasBall)
            {
                 DataModel.PosessionResult res = RunLateGamePossession(team1Model, team2Model, team1Score, team2Score, timeLeft, team1HasBall, shortPosession, team2Fouls, team1Fouls, waitTime);
                 if (res == null)
                 {
                     break;
                 }
                 UdpateScore(res, shortPosession );
             }
             else
             {
                 DataModel.PosessionResult res = RunLateGamePossession(team2Model, team1Model, team2Score, team1Score, timeLeft, team1HasBall, shortPosession, team1Fouls, team2Fouls, waitTime);
                 if (res == null)
                 {
                     break;
                 }
                 UdpateScore(res, shortPosession);
             }
            //wait a bit
            if(waitTime > 0)
            {
                System.Threading.Thread.Sleep(waitTime);
            }
            
        }

        int overtimeCounter = 0;
        //check for overtime
        if(team1Score == team2Score)
        {
            //overtime
            while(team1Score == team2Score)
            {
                timeLeft = 300;
                overtimeCounter++;
                if(overtimeCounter == 1)
                {
                    if (waitTime > 0)
                    {
                        Console.WriteLine("#########################################");
                        Console.WriteLine("Final After Regulation: " + team1 + " " + team1Score.ToString() + "-" + team2Score.ToString() + " " + team2);
                        Console.WriteLine("#########################################");
                        if(waitTime > 0)
                        {
                            System.Threading.Thread.Sleep(waitTime * 5);
                        }
                        
                    }
                }
                else
                {
                    if (waitTime > 0)
                    {
                        Console.WriteLine("#########################################");
                        Console.WriteLine("Final After Overtime " + (overtimeCounter - 1).ToString() + ": " + team1 + " " + team1Score.ToString() + "-" + team2Score.ToString() + " " + team2);
                        Console.WriteLine("#########################################");
                        if(waitTime > 0)
                        {
                            System.Threading.Thread.Sleep(waitTime * 5);
                        }
                        
                    }
                }
                while (timeLeft > 120)
                {
                    bool shortPosession = false;
                    if (team1HasBall == true)
                    {
                        DataModel.PosessionResult res = RunPoesssion(team1Model, team2Model, team1Score, team2Score, timeLeft, team1HasBall, shortPosession, team2Fouls, team1Fouls, waitTime);
                        UdpateScore(res, shortPosession);
                    }
                    else
                    {
                        DataModel.PosessionResult res = RunPoesssion(team2Model, team1Model, team2Score, team1Score, timeLeft, team1HasBall, shortPosession, team1Fouls, team2Fouls, waitTime);
                        UdpateScore(res, shortPosession);
                    }
                    //wait a bit
                    if(waitTime > 0)
                    {
                        System.Threading.Thread.Sleep(waitTime);
                    }
                }
                while (timeLeft > 0)
                {
                    bool shortPosession = false;
                    int lead = Math.Abs(team1Score - team2Score);
                    if (timeLeft < 30 && lead > 6)
                    {
                        break;
                    }
                    if (team1HasBall == true)
                    {
                        DataModel.PosessionResult res = RunLateGamePossession(team1Model, team2Model, team1Score, team2Score, timeLeft, team1HasBall, shortPosession, team2Fouls, team1Fouls, waitTime);
                        if (res == null)
                        {
                            break;
                        }
                        UdpateScore(res, shortPosession);
                    }
                    else
                    {
                        DataModel.PosessionResult res = RunLateGamePossession(team1Model, team2Model, team1Score, team2Score, timeLeft, team1HasBall, shortPosession, team2Fouls, team1Fouls, waitTime);
                        if (res == null)
                        {
                            break;
                        }
                        UdpateScore(res, shortPosession);
                    }
                    //wait a bit
                    if(waitTime > 0)
                    {
                        System.Threading.Thread.Sleep(waitTime);
                    }
                }
            }
        }


        //print final score
        if(waitTime > 0)
        {
            string overtimeString = "";
            if(overtimeCounter > 0)
            {
                if(overtimeCounter == 1)
                {
                    overtimeString = "(OT) ";
                }
                else
                {
                    overtimeString = "(" + overtimeCounter.ToString() + "OT)";
                }
            }
            Console.WriteLine("#########################################");
            Console.WriteLine("FINAL SCORE: " + overtimeString + team1 + " " + team1Score.ToString() + "-" + team2Score.ToString() + " " + team2);
            Console.WriteLine("#########################################");
            if(waitTime > 0)
            {
                System.Threading.Thread.Sleep(waitTime * 5);
            }  
        }

        DataModel.MatchupResult finalRes = new DataModel.MatchupResult();
        finalRes.AwayTeam = team1;
        finalRes.AwayTeamScore = team1Score;
        finalRes.HomeTeam = team2;
        finalRes.HomeTeamScore = team2Score;
        if(team1Score > team2Score)
        {
            finalRes.Winner = team1;
            finalRes.WinnerScore = team1Score;
            finalRes.Loser = team2;
            finalRes.LoserScore = team2Score;
        }
        else
        {
            finalRes.Winner = team2;
            finalRes.WinnerScore = team2Score;
            finalRes.Loser = team1;
            finalRes.LoserScore = team1Score;
        }
        finalRes.Overtimes = overtimeCounter;
        return finalRes;
        
    }

    public static DataModel.PosessionResult RunPoesssion(DataModel.CollegeModel team1, DataModel.CollegeModel team2, int team1Score, int team2Score, int secsLeft, bool team1HasBall, bool shortPosession, int defensiveFouls, int offensiveFouls, int waitTime)
    {
        //first init the result model
        DataModel.PosessionResult res = new DataModel.PosessionResult();
        //init the random
        Random r = new Random();

        //------------------------------------------------------------ determine how much time has gone off the clock --------------------------------------------------------------------
        //team 1 is on offense, so get their tempo as integer, going to be between 60 and 75
        int tempo = (int)Math.Round(team1.ADJ_T);
        //convert number to 0-15
        tempo = tempo - 60;
        if(tempo < 0)
        {
            tempo = 0;
        }

        //shortest posession should be like 5 seconds, so random should start at 20
        int randForTempo = r.Next(1, tempo + 1);
        int randForTime = r.Next(0, 3);
        int timeTaken = 0;
        if (shortPosession == false)
        {
            timeTaken = 20 - randForTime - randForTempo;
        }
        else
        {
            timeTaken = 10 - randForTime - randForTempo;
            if(timeTaken < 1)
            {
                timeTaken = 1;
            }

        }



        res.SecondsUsed = timeTaken;
        string timestamp = GetTimeStamp(secsLeft - timeTaken);

        //------------------------------------------------------------- determine if a turnover happened ---------------------------------------------------------------------------------
        //chance of turnover = average of offense turnover % - defense turnover %, divide it by 3 to get a slightly lower turnover rate for testing
        decimal chanceOfTurnover = ((team1.TOR_O + team2.TOR_D) / 3);
        decimal randForTurnover = r.Next(1, 101);
        if (chanceOfTurnover > randForTurnover)
        {
            //a turnover happened.
            res.PointsScored = 0;
            res.OffenseKeepsPosession = false;

            //determine if the turnover was an offensive foul
            //determine the odds by half of the defensive team's offensive freethrow attempt rate
            int offFoulChance = (int)Math.Round(team2.FTR_O / 2);
            int randForOffFoul = r.Next(1, 101);
            if (offFoulChance >= randForOffFoul)
            {
                res.OffensiveFoul = true;
                if(waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls + 1, defensiveFouls) + "---" + team1.Name + " committed an offensive foul.");
                }
                
            }
            else
            {
                res.OffensiveFoul = false;
                if(waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " turned the ball over.");
                }
                
            }



            return res;
        }

        //------------------------------------------------------------- no turnover or offensive foul, now what? --------------------------------------------------------------------------------------------------

        //------------------------------------------------------------- see if a dead ball defensive foul occurs --------------------------------------------------------------------------------------------------

        //the chance of a deadball foul is going to be half of the average of the FT attempt rates.
        int chanceOfDeadballFoul = (int)Math.Round((team1.FTR_O + team2.FTR_D) / 4);
        chanceOfDeadballFoul += adjusterFoul;
        int randForDeadballFoul = r.Next(1, 101);
        if (chanceOfDeadballFoul >= randForDeadballFoul)
        {
            //a deadball defensive foul happened.
            res.DefensiveFoul = true;
            //check if freethrows will happen
            if ((defensiveFouls + 1) > 6)
            {
                //freethrows will happen
                res.OffenseKeepsPosession = false;
                //check if it's double bonus
                int freethrowChance = (int)Math.Round(team1.FTP);
                if ((defensiveFouls + 1) > 9)
                {
                    //double bonus, two shots
                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on the floor, two free throws coming.");
                    }
                    res.PointsScored = 0;
                    int randFt1 = r.Next(1, 101);
                    if (freethrowChance >= randFt1)
                    {
                        //made the first one
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the first free throw.");
                            Thread.Sleep(waitTime);
                        }
                    }
                    else
                    {
                        //missed the first one
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the first free throw.");
                        }
                    }
                    int randFt2 = r.Next(1, 101);
                    if (freethrowChance >= randFt2)
                    {
                        //made the second one
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the second free throw.");
                        }
                    }
                    else
                    {
                        //missed the second one
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the second free throw.");
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                        }
                        
                    }
                    return res;
                }
                else
                {
                    //one and one
                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on the floor, one and one coming.");
                    }
                    
                    res.PointsScored = 0;
                    int randFt1 = r.Next(1, 101);
                    if (freethrowChance >= randFt1)
                    {
                        //made the first one
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the first free throw.");
                        }
                        
                        int randFt2 = r.Next(1, 101);
                        if (freethrowChance <= randFt2)
                        {
                            //made the second one
                            res.PointsScored++;
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the second free throw.");
                            }
                            
                            return res;
                        }
                        else
                        {
                            //missed the second one
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the second free throw.");
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                            }
                            
                            return res;
                        }

                    }
                    else
                    {
                        //missed the first one
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the first free throw.");
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                        }
                        
                        return res;
                    }
                }
            }

            //if we got to here, no freethrows happened.
            if(waitTime > 0)
            {
                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on the floor.");
            }

            res.PointsScored = 0;
            res.OffenseKeepsPosession = true;
            return res;
        }



        //determine if the shot is a 2 or 3
        bool is3Pointer = false;
        //im gonna say if a team is within 5 either way, 35% chance, winning by more than 5, 30% chance, losing by more than 5, 45% chance.

        int randForShotType = r.Next(1, 101);
        if (team1Score - team2Score > 5)
        {
            //winning by more than 5
            if (randForShotType > 30)
            {
                is3Pointer = false;
            }
            else
            {
                is3Pointer = true;
            }
        }
        else if (team2Score - team1Score > 5)
        {
            //losing by more than 5
            if (randForShotType > 40)
            {
                is3Pointer = false;
            }
            else
            {
                is3Pointer = true;
            }
        }
        else
        {
            //within 5 either way
            if (randForShotType > 35)
            {
                is3Pointer = false;
            }
            else
            {
                is3Pointer = true;
            }
        }

        //we now know what kind of shot is being taken, so split based on that
        //first calculate overall adjustment.  This takes into account how good the team is
        int adjuster = (int)Math.Round((team1.ADJOE + team2.ADJDE) / 2);
        int adjusterDef = (int)Math.Round((team2.ADJOE + team1.ADJDE) / 2);
        adjuster = adjuster - 100;
        adjusterDef = adjusterDef - 100;

        if (is3Pointer == true)
        {
            //3 pointer attempted
            int percentChance = (int)Math.Round((team1.PT3_O + team2.PT3_D) / 2);
            percentChance = percentChance + adjuster;
            //give home team a boost
            if(team1HasBall == false)
            {
                percentChance += adjusterHomeTeam;
            }
            percentChance += adjuster3PT;
            int randForShot = r.Next(1, 101);
            if (randForShot <= percentChance)
            {
                //he made that shit
                if(waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + 3, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " made 3 point basket.");
                }
                
                res.PointsScored = 3;
                res.OffenseKeepsPosession = false;
                return res;
            }
            else
            {
                //he missed that shit
                //but was he fouled? 
                //percent of fouled on a 3 point shot will be 1/4 average of FT attempt rates
                int chanceOf3ptFoul = (int)Math.Round((team1.FTR_O + team2.FTR_D) / 8);
                chanceOf3ptFoul += adjusterFoul;
                int randFor3ptFoul = r.Next(1, 101);
                if (chanceOf3ptFoul >= randFor3ptFoul)
                {
                    //fouled on a 3
                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on a 3 point basket.");
                    }
                    
                    res.PointsScored = 0;
                    res.OffenseKeepsPosession = false;
                    res.DefensiveFoul = true;
                    int freethrowChance = (int)Math.Round(team1.FTP);
                    int randFt1 = r.Next(1, 101);
                    int randFt2 = r.Next(1, 101);
                    int randFt3 = r.Next(1, 101);
                    if (freethrowChance >= randFt1)
                    {
                        //made the first
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the first free throw.");
                        }
                       
                    }
                    else
                    {
                        //missed the first
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the first free throw.");
                        }
                       
                    }
                    if (freethrowChance >= randFt2)
                    {
                        //made the second
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the second free throw.");
                        }
                       
                    }
                    else
                    {
                        //missed the second
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the second free throw.");
                        }
                      
                    }
                    if (freethrowChance >= randFt3)
                    {
                        //made the third
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the third free throw.");
                        }
                       
                    }
                    else
                    {
                        //missed the third
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the third free throw.");
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                        }
                        
                    }
                    return res;
                }

                if(waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " missed 3 point basket.");
                }
                
                //give them a chance to def rebound
                int defReboundChance = (int)Math.Round(team2.DRB);
                defReboundChance += adjusterDef;
                int randForDefRebound = r.Next(1, 101);
                if (randForDefRebound <= defReboundChance)
                {
                    //got the defensive rebound
                    if(waitTime > 0)
                    {
                        Thread.Sleep(waitTime);
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team2.Name + " got the defensive rebound.");
                    }
                   
                    res.PointsScored = 0;
                    res.OffenseKeepsPosession = false;
                    return res;
                }
                else
                {
                    //didnt get the defensive rebound, chance to get the offensive rebound
                    int offReboundChance = (int)Math.Round(team1.ORB);
                    offReboundChance += adjuster;
                    int randForOffRebound = r.Next(1, 101);
                    if (randForOffRebound <= offReboundChance)
                    {
                        //got the offensive rebound
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " got the offensive rebound.");
                        }
                       
                        res.PointsScored = 0;
                        res.OffenseKeepsPosession = true;
                        return res;
                    }
                    else
                    {
                        //didn't get the offensive rebound
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team2.Name + " got the defensive rebound.");
                        }
                       
                        res.PointsScored = 0;
                        res.OffenseKeepsPosession = false;
                        return res;
                    }
                }
            }
        }
        else
        {
            //2 pointer attempted
            int percentChance = (int)Math.Round((team1.PT2_O + team2.PT2_D) / 2);
            percentChance = percentChance + adjuster;
            percentChance += adjuster2PT;
            //give home team a boost
            if (team1HasBall == false)
            {
                percentChance += adjusterHomeTeam;
            }
            int randForShot = r.Next(1, 101);
            if (randForShot <= percentChance)
            {
                //he made that shit
                if(waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + 2, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " made 2 point basket.");
                }
               
                res.PointsScored = 2;
                res.OffenseKeepsPosession = false;
                return res;
            }
            else
            {
                //he missed that shit
                //but was he fouled?
                //chance of FT attemp rate average divided by 2
                int chanceOf2ptFoul = (int)Math.Round((team1.FTR_O + team2.FTR_D) / 4);
                chanceOf2ptFoul += adjusterFoul;
                int randFor2ptFoul = r.Next(1, 101);
                if (chanceOf2ptFoul >= randFor2ptFoul)
                {
                    //fouled on a 2
                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on a 2 point basket.");
                    }
                    
                    res.PointsScored = 0;
                    res.OffenseKeepsPosession = false;
                    res.DefensiveFoul = true;
                    int freethrowChance = (int)Math.Round(team1.FTP);
                    int randFt1 = r.Next(1, 101);
                    int randFt2 = r.Next(1, 101);
                    if (freethrowChance >= randFt1)
                    {
                        //made the first
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the first free throw.");
                        }
                        
                    }
                    else
                    {
                        //missed the first
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the first free throw.");
                        }
                       
                    }
                    if (freethrowChance >= randFt2)
                    {
                        //made the second
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the second free throw.");
                        }
                       
                    }
                    else
                    {
                        //missed the second
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the second free throw.");
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                        }
                       
                    }
                    return res;
                }

                if(waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " missed 2 point basket.");
                }
               
                //give them a chance to def rebound
                int defReboundChance = (int)Math.Round(team2.DRB);
                defReboundChance += adjusterDef;
                int randForDefRebound = r.Next(1, 101);
                if (randForDefRebound <= defReboundChance)
                {
                    //got the defensive rebound
                    if(waitTime > 0)
                    {
                        Thread.Sleep(waitTime);
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team2.Name + " got the defensive rebound.");
                    }
                   
                    res.PointsScored = 0;
                    res.OffenseKeepsPosession = false;
                    return res;
                }
                else
                {
                    //didnt get the defensive rebound, chance to get the offensive rebound
                    int offReboundChance = (int)Math.Round(team1.ORB);
                    offReboundChance += adjuster;
                    int randForOffRebound = r.Next(1, 101);
                    if (randForOffRebound >= offReboundChance)
                    {
                        //got the offensive rebound
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " got the offensive rebound.");
                        }
                        
                        res.PointsScored = 0;
                        res.OffenseKeepsPosession = true;
                        return res;
                    }
                    else
                    {
                        //didn't get the offensive rebound
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team2.Name + " got the defensive rebound.");
                        }
                        
                        res.PointsScored = 0;
                        res.OffenseKeepsPosession = false;
                        return res;
                    }
                }
            }
        }

    }

    public static void UdpateScore(DataModel.PosessionResult res, bool shortPosession)
    {
        if (team1HasBall)
        {
            team1Score += res.PointsScored;
            timeLeft -= res.SecondsUsed;
            if (res.OffenseKeepsPosession == false)
            {
                team1HasBall = !team1HasBall;
                shortPosession = false;
            }
            else
            {
                shortPosession = true;
            }

            if (res.OffensiveFoul)
            {
                team1Fouls++;
            }
            if (res.DefensiveFoul)
            {
                team2Fouls++;
            }
        }
        else
        {
            team2Score += res.PointsScored;
            timeLeft -= res.SecondsUsed;
            if (res.OffenseKeepsPosession == false)
            {
                team1HasBall = !team1HasBall;
                shortPosession = false;
            }
            else
            {
                shortPosession = true;
            }

            if (res.OffensiveFoul)
            {
                team2Fouls++;
            }

            if (res.DefensiveFoul)
            {
                team1Fouls++;
            }
        }
    } 

    public static DataModel.PosessionResult RunLateGamePossession(DataModel.CollegeModel team1, DataModel.CollegeModel team2, int team1Score, int team2Score, int secsLeft, bool team1HasBall, bool shortPosession, int defensiveFouls, int offensiveFouls, int waitTime)
    {
        //first init the result model
        DataModel.PosessionResult res = new DataModel.PosessionResult();
        //init the random
        Random r = new Random();
        
        int lead = team1Score - team2Score;
        int timeTaken = 0;
        int chanceOfDeadballFoul = (int)Math.Round((team1.FTR_O + team2.FTR_D) / 4);
        chanceOfDeadballFoul += adjusterFoul;
        int randForDeadballFoul = r.Next(1, 100);
        bool is3Pointer = false;
        int randForShotType = r.Next(1, 101);
        int threeTendency = 35;
        
        if (secsLeft > 90)
        {
            if (lead > 12)
            {
                // team 1 runs out clock
                if (shortPosession == false)
                {
                    timeTaken = r.Next(20, 30);
                }
                else
                {
                    timeTaken = r.Next(10, 20);
                }
            }
            else if (lead > 9)
            {
                // team 2 fouls
                timeTaken = r.Next(1, 5);
                chanceOfDeadballFoul = 100;
            }
            else if (lead > 3)
            {
                // team 1 runs out clock
                if (shortPosession == false)
                {
                    timeTaken = r.Next(20, 30);
                }
                else
                {
                    timeTaken = r.Next(10, 20);
                }
            }
            else if (lead == 0)
            {
                // team 1 runs normal possession
                timeTaken = r.Next(5, 30);
            }
            else if (lead < -6)
            {
                // team 1 gets a quick shot
                timeTaken = r.Next(5, 15);
                threeTendency = 50;
            }
            else
            {
                // team 1 runs normal possession
                timeTaken = r.Next(5, 30);
            }
        }
        else if (secsLeft > 60)
        {
            if (lead > 9)
            {
                // team 1 runs out clock
                if (shortPosession == false)
                {
                    timeTaken = r.Next(20, 30);
                }
                else
                {
                    timeTaken = r.Next(10, 20);
                }
            }
            else if (lead > 6)
            {
                // team 2 fouls
                timeTaken = r.Next(1, 5);
                chanceOfDeadballFoul = 100;
            }
            else if (lead > 0)
            {
                // team 1 runs out clock
                if (shortPosession == false)
                {
                    timeTaken = r.Next(20, 30);
                }
                else
                {
                    timeTaken = r.Next(10, 20);
                }
            }
            else if (lead == 0)
            {
                // team 1 runs normal possession
                timeTaken = r.Next(10, 30);
            }
            else if (lead < -3)
            {
                // team 1 gets a quick shot
                timeTaken = r.Next(5, 15);
                threeTendency = 50;
            }
            else
            {
                // team 1 runs normal possession
                timeTaken = r.Next(5, 30);
            }
        }
        else if (secsLeft > 30)
        {
            if (lead > 9)
            {
                // team 1 runs out clock
                if (shortPosession == false)
                {
                    timeTaken = r.Next(20, 30);
                }
                else
                {
                    timeTaken = r.Next(10, 20);
                }
            }
            else if (lead > 3)
            {
                // team 2 fouls
                timeTaken = r.Next(1, 5);
                chanceOfDeadballFoul = 100;
            }
            else if (lead > 0)
            {
                // team 1 runs out clock
                if (shortPosession == false)
                {
                    timeTaken = r.Next(20, 30);
                }
                else
                {
                    timeTaken = r.Next(10, 20);
                }
            }
            else if (lead == 0)
            {
                // team 1 runs normal possession
                r.Next(15, 30);
            }
            else if (lead < -3)
            {
                // team 1 gets a quick shot
                timeTaken = r.Next(5, 15);
                threeTendency = 50;
            }
            else
            {
                // team 1 runs normal possession
                timeTaken = r.Next(5, 30);
            }
        }
        else
        {
            if (lead > 6)
            {
                // team 1 runs out clock
                if (shortPosession == false)
                {
                    timeTaken = secsLeft;
                }
                else
                {
                    timeTaken = r.Next(10, 20);
                }
            }
            else if (lead > 0)
            {
                // team 2 fouls
                timeTaken = r.Next(1, 5);
                chanceOfDeadballFoul = 100;
            }
            else if (lead == 0)
            {
               // team 1 runs normal possession 
               timeTaken = r.Next(1, secsLeft);
            }
            else if (lead < -3)
            {
                // team 1 gets a quick shot
                timeTaken = r.Next(5, 15);
                threeTendency = 75;
            }
            else
            {
                // team 1 runs normal possession
                timeTaken = r.Next(1, secsLeft);
            }
        }
        
        res.SecondsUsed = timeTaken;

        if (secsLeft - timeTaken < 0)
        {
            return null;
        }
        
        string timestamp = GetTimeStamp(secsLeft - timeTaken);
        
        //------------------------------------------------------------- determine if a turnover happened ---------------------------------------------------------------------------------
        //chance of turnover = average of offense turnover % - defense turnover %, divide it by 3 to get a slightly lower turnover rate for testing
        decimal chanceOfTurnover = ((team1.TOR_O + team2.TOR_D) / 3);
        decimal randForTurnover = r.Next(1, 101);
        if (chanceOfTurnover > randForTurnover)
        {
            //a turnover happened.
            res.PointsScored = 0;
            res.OffenseKeepsPosession = false;

            //determine if the turnover was an offensive foul
            //determine the odds by half of the defensive team's offensive freethrow attempt rate
            int offFoulChance = (int)Math.Round(team2.FTR_O / 2);
            int randForOffFoul = r.Next(1, 101);
            if (offFoulChance >= randForOffFoul)
            {
                res.OffensiveFoul = true;
                if(waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls + 1, defensiveFouls) + "---" + team1.Name + " committed an offensive foul.");
                }
                
            }
            else
            {
                res.OffensiveFoul = false;
                if(waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " turned the ball over.");
                }
                
            }
            return res;
        }
        
                    //------------------------------------------------------------- no turnover or offensive foul, now what? --------------------------------------------------------------------------------------------------

        //------------------------------------------------------------- see if a dead ball defensive foul occurs --------------------------------------------------------------------------------------------------

        //the chance of a deadball foul is going to be half of the average of the FT attempt rates.
        if (chanceOfDeadballFoul >= randForDeadballFoul)
        {
            //a deadball defensive foul happened.
            res.DefensiveFoul = true;
            //check if freethrows will happen
            if ((defensiveFouls + 1) > 6)
            {
                //freethrows will happen
                res.OffenseKeepsPosession = false;
                //check if it's double bonus
                int freethrowChance = (int)Math.Round(team1.FTP);
                if ((defensiveFouls + 1) > 9)
                {
                    //double bonus, two shots
                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on the floor, two free throws coming.");
                    }
                    res.PointsScored = 0;
                    int randFt1 = r.Next(1, 101);
                    if (freethrowChance >= randFt1)
                    {
                        //made the first one
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the first free throw.");
                            Thread.Sleep(waitTime);
                        }
                    }
                    else
                    {
                        //missed the first one
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the first free throw.");
                        }
                    }
                    int randFt2 = r.Next(1, 101);
                    if (freethrowChance >= randFt2)
                    {
                        //made the second one
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the second free throw.");
                        }
                    }
                    else
                    {
                        //missed the second one
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the second free throw.");
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                        }
                        
                    }
                    return res;
                }
                else
                {
                    //one and one
                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on the floor, one and one coming.");
                    }
                    
                    res.PointsScored = 0;
                    int randFt1 = r.Next(1, 101);
                    if (freethrowChance >= randFt1)
                    {
                        //made the first one
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the first free throw.");
                        }
                        
                        int randFt2 = r.Next(1, 101);
                        if (freethrowChance <= randFt2)
                        {
                            //made the second one
                            res.PointsScored++;
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the second free throw.");
                            }
                            
                            return res;
                        }
                        else
                        {
                            //missed the second one
                            if(waitTime > 0)
                            {
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the second free throw.");
                                Thread.Sleep(waitTime);
                                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                            }
                            
                            return res;
                        }

                    }
                    else
                    {
                        //missed the first one
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the first free throw.");
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                        }
                        
                        return res;
                    }
                }
            }

            //if we got to here, no freethrows happened.
            if(waitTime > 0)
            {
                Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on the floor.");
            }

            res.PointsScored = 0;
            res.OffenseKeepsPosession = true;
            return res;
        }
        
        if (randForShotType > threeTendency)
        {
            is3Pointer = false;
        }
        else
        {
            is3Pointer = true;
        }
        
                    //we now know what kind of shot is being taken, so split based on that
        //first calculate overall adjustment.  This takes into account how good the team is
        int adjuster = (int)Math.Round((team1.ADJOE + team2.ADJDE) / 2);
        int adjusterDef = (int)Math.Round((team2.ADJOE + team1.ADJDE) / 2);
        adjuster = adjuster - 100;
        adjusterDef = adjusterDef - 100;

        if (is3Pointer == true)
        {
            //3 pointer attempted
            int percentChance = (int)Math.Round((team1.PT3_O + team2.PT3_D) / 2);
            percentChance = percentChance + adjuster;
            //give home team a boost
            if(team1HasBall == false)
            {
                percentChance += adjusterHomeTeam;
            }
            percentChance += adjuster3PT;
            int randForShot = r.Next(1, 101);
            if (randForShot <= percentChance)
            {
                //he made that shit
                if(waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + 3, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " made 3 point basket.");
                }
                
                res.PointsScored = 3;
                res.OffenseKeepsPosession = false;
                return res;
            }
            else
            {
                //he missed that shit
                //but was he fouled? 
                //percent of fouled on a 3 point shot will be 1/4 average of FT attempt rates
                int chanceOf3ptFoul = (int)Math.Round((team1.FTR_O + team2.FTR_D) / 8);
                chanceOf3ptFoul += adjusterFoul;
                int randFor3ptFoul = r.Next(1, 101);
                if (chanceOf3ptFoul >= randFor3ptFoul)
                {
                    //fouled on a 3
                    if(waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " was fouled on a 3 point basket.");
                    }
                    
                    res.PointsScored = 0;
                    res.OffenseKeepsPosession = false;
                    res.DefensiveFoul = true;
                    int freethrowChance = (int)Math.Round(team1.FTP);
                    int randFt1 = r.Next(1, 101);
                    int randFt2 = r.Next(1, 101);
                    int randFt3 = r.Next(1, 101);
                    if (freethrowChance >= randFt1)
                    {
                        //made the first
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the first free throw.");
                        }
                       
                    }
                    else
                    {
                        //missed the first
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the first free throw.");
                        }
                       
                    }
                    if (freethrowChance >= randFt2)
                    {
                        //made the second
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the second free throw.");
                        }
                       
                    }
                    else
                    {
                        //missed the second
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the second free throw.");
                        }
                      
                    }
                    if (freethrowChance >= randFt3)
                    {
                        //made the third
                        res.PointsScored++;
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " made the third free throw.");
                        }
                       
                    }
                    else
                    {
                        //missed the third
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team1.Name + " missed the third free throw.");
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" + team2.Name + " got the defensive rebound.");
                        }
                        
                    }
                    return res;
                }

                if(waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " missed 3 point basket.");
                }
                
                //give them a chance to def rebound
                int defReboundChance = (int)Math.Round(team2.DRB);
                defReboundChance += adjusterDef;
                int randForDefRebound = r.Next(1, 101);
                if (randForDefRebound <= defReboundChance)
                {
                    //got the defensive rebound
                    if(waitTime > 0)
                    {
                        Thread.Sleep(waitTime);
                        Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team2.Name + " got the defensive rebound.");
                    }
                   
                    res.PointsScored = 0;
                    res.OffenseKeepsPosession = false;
                    return res;
                }
                else
                {
                    //didnt get the defensive rebound, chance to get the offensive rebound
                    int offReboundChance = (int)Math.Round(team1.ORB);
                    offReboundChance += adjuster;
                    int randForOffRebound = r.Next(1, 101);
                    if (randForOffRebound <= offReboundChance)
                    {
                        //got the offensive rebound
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name + " got the offensive rebound.");
                        }
                       
                        res.PointsScored = 0;
                        res.OffenseKeepsPosession = true;
                        return res;
                    }
                    else
                    {
                        //didn't get the offensive rebound
                        if(waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" + GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name, team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" + team2.Name + " got the defensive rebound.");
                        }
                       
                        res.PointsScored = 0;
                        res.OffenseKeepsPosession = false;
                        return res;
                    }
                }
            }
        }
        else
        {
            //2 pointer attempted
            int percentChance = (int)Math.Round((team1.PT2_O + team2.PT2_D) / 2);
            percentChance = percentChance + adjuster;
            percentChance += adjuster2PT;
            //give home team a boost
            if (team1HasBall == false)
            {
                percentChance += adjusterHomeTeam;
            }

            int randForShot = r.Next(1, 101);
            if (randForShot <= percentChance)
            {
                //he made that shit
                if (waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" +
                                      GetScoreboard(team1.Name, team1Score + 2, team2.Name, team2Score,
                                          team1HasBall, offensiveFouls, defensiveFouls) + "---" + team1.Name +
                                      " made 2 point basket.");
                }

                res.PointsScored = 2;
                res.OffenseKeepsPosession = false;
                return res;
            }
            else
            {
                //he missed that shit
                //but was he fouled?
                //chance of FT attemp rate average divided by 2
                int chanceOf2ptFoul = (int)Math.Round((team1.FTR_O + team2.FTR_D) / 4);
                chanceOf2ptFoul += adjusterFoul;
                int randFor2ptFoul = r.Next(1, 101);
                if (chanceOf2ptFoul >= randFor2ptFoul)
                {
                    //fouled on a 2
                    if (waitTime > 0)
                    {
                        Console.WriteLine("---" + timestamp + "---" +
                                          GetScoreboard(team1.Name, team1Score, team2.Name, team2Score,
                                              team1HasBall, offensiveFouls, defensiveFouls + 1) + "---" +
                                          team1.Name + " was fouled on a 2 point basket.");
                    }

                    res.PointsScored = 0;
                    res.OffenseKeepsPosession = false;
                    res.DefensiveFoul = true;
                    int freethrowChance = (int)Math.Round(team1.FTP);
                    int randFt1 = r.Next(1, 101);
                    int randFt2 = r.Next(1, 101);
                    if (freethrowChance >= randFt1)
                    {
                        //made the first
                        res.PointsScored++;
                        if (waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" +
                                              GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name,
                                                  team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) +
                                              "---" + team1.Name + " made the first free throw.");
                        }

                    }
                    else
                    {
                        //missed the first
                        if (waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" +
                                              GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name,
                                                  team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) +
                                              "---" + team1.Name + " missed the first free throw.");
                        }

                    }

                    if (freethrowChance >= randFt2)
                    {
                        //made the second
                        res.PointsScored++;
                        if (waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" +
                                              GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name,
                                                  team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) +
                                              "---" + team1.Name + " made the second free throw.");
                        }

                    }
                    else
                    {
                        //missed the second
                        if (waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" +
                                              GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name,
                                                  team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) +
                                              "---" + team1.Name + " missed the second free throw.");
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" +
                                              GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name,
                                                  team2Score, team1HasBall, offensiveFouls, defensiveFouls + 1) +
                                              "---" + team2.Name + " got the defensive rebound.");
                        }

                    }

                    return res;
                }

                if (waitTime > 0)
                {
                    Console.WriteLine("---" + timestamp + "---" +
                                      GetScoreboard(team1.Name, team1Score, team2.Name, team2Score, team1HasBall,
                                          offensiveFouls, defensiveFouls) + "---" + team1.Name +
                                      " missed 2 point basket.");
                }

                //give them a chance to def rebound
                int defReboundChance = (int)Math.Round(team2.DRB);
                defReboundChance += adjusterDef;
                int randForDefRebound = r.Next(1, 101);
                if (randForDefRebound <= defReboundChance)
                {
                    //got the defensive rebound
                    if (waitTime > 0)
                    {
                        Thread.Sleep(waitTime);
                        Console.WriteLine("---" + timestamp + "---" +
                                          GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name,
                                              team2Score, team1HasBall, offensiveFouls, defensiveFouls) + "---" +
                                          team2.Name + " got the defensive rebound.");
                    }

                    res.PointsScored = 0;
                    res.OffenseKeepsPosession = false;
                    return res;
                }
                else
                {
                    //didnt get the defensive rebound, chance to get the offensive rebound
                    int offReboundChance = (int)Math.Round(team1.ORB);
                    offReboundChance += adjuster;
                    int randForOffRebound = r.Next(1, 101);
                    if (randForOffRebound >= offReboundChance)
                    {
                        //got the offensive rebound
                        if (waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" +
                                              GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name,
                                                  team2Score, team1HasBall, offensiveFouls, defensiveFouls) +
                                              "---" + team1.Name + " got the offensive rebound.");
                        }

                        res.PointsScored = 0;
                        res.OffenseKeepsPosession = true;
                        return res;
                    }
                    else
                    {
                        //didn't get the offensive rebound
                        if (waitTime > 0)
                        {
                            Thread.Sleep(waitTime);
                            Console.WriteLine("---" + timestamp + "---" +
                                              GetScoreboard(team1.Name, team1Score + res.PointsScored, team2.Name,
                                                  team2Score, team1HasBall, offensiveFouls, defensiveFouls) +
                                              "---" + team2.Name + " got the defensive rebound.");
                        }

                        res.PointsScored = 0;
                        res.OffenseKeepsPosession = false;
                        return res;
                    }
                }
            }
        }
    }
    
    public static string GetTimeStamp(int secondsLeft)
    {
        if (secondsLeft <= 1)
        {
            return "0:01";
        }
        int minutes = secondsLeft / 60;
        int secs = secondsLeft % 60;
        string secSpacer = "";
        if (secs < 10)
        {
            secSpacer = "0";
        }
        return minutes.ToString() + ":" + secSpacer + secs.ToString();
    }

    public static string GetScoreboard(string team1, int team1Score, string team2, int team2Score, bool team1HasBall, int team1Fouls, int team2Fouls)
    {
        if (team1HasBall)
        {
            return "((" + team1Fouls.ToString() + ")" + team1 + " " + team1Score.ToString() + "-" + team2Score.ToString() + " " + team2 + "(" + team2Fouls.ToString() + "))";
        }
        else
        {
            return "((" + team2Fouls.ToString() + ")" + team2 + " " + team2Score.ToString() + "-" + team1Score.ToString() + " " + team1 + "(" + team1Fouls.ToString() + "))";
        }

    }

    public static string GetGameStampForSimulator(string s, string g, int round)
    {
        string popen = "";
        string pclose = "";
        for (var i = 0; i < round; i++)
        {
            popen += "(";
            pclose += ")";
        }
        if (s == g)
        {
            return popen + "X" + pclose + " " + s;
        }
        else
        {
            return popen + " " + pclose + " " + s;
        }
    }
    public static int GetGamePointsForSimulator(string s, string g, int round)
    {
        if (s != g)
        {
            return 0;
        }
        else
        {
            if (round == 1)
            {
                return 10;
            }
            else if (round == 2)
            {
                return 20;
            }
            else if (round == 3)
            {
                return 40;
            }
            else if (round == 4)
            {
                return 80;
            }
            else if (round == 5)
            {
                return 160;
            }
            else if (round == 6)
            {
                return 320;
            }
        }
        return 0;
    }

    //college data helper method.
    public static DataModel.CollegeModel ReadCollegeModelFromCSV(string csvLine)
    {
        try
        {
            string[] values = csvLine.Split(',');
            DataModel.CollegeModel model = new DataModel.CollegeModel();
            model.Rank = Convert.ToInt32(values[0]);
            model.Name = values[1];
            model.Conference = values[2];
            model.GamesPlayed = Convert.ToInt32(values[3]);
            model.Wins = Convert.ToInt32(values[4]);
            model.Losses = model.GamesPlayed - model.Wins;
            model.ADJOE = Convert.ToDecimal(values[5]);
            model.ADJDE = Convert.ToDecimal(values[6]);
            model.BARTHAG = Convert.ToDecimal(values[7]);
            model.EFG_O = Convert.ToDecimal(values[8]);
            model.EFG_D = Convert.ToDecimal(values[9]);
            model.TOR_O = Convert.ToDecimal(values[10]);
            model.TOR_D = Convert.ToDecimal(values[11]);
            model.ORB = Convert.ToDecimal(values[12]);
            model.DRB = Convert.ToDecimal(values[13]);
            model.FTR_O = Convert.ToDecimal(values[14]);
            model.FTR_D = Convert.ToDecimal(values[15]);
            model.PT2_O = Convert.ToDecimal(values[16]);
            model.PT2_D = Convert.ToDecimal(values[17]);
            model.PT3_O = Convert.ToDecimal(values[18]);
            model.PT3_D = Convert.ToDecimal(values[19]);
            model.ADJ_T = Convert.ToDecimal(values[20]);
            model.WAB = Convert.ToDecimal(values[21]);
            try
            {
                model.FTP = Convert.ToDecimal(values[23]);
            }
            catch
            {
                //just default FT% to 70 if they don't have one set.
                model.FTP = 70;
            }

            return model;
        }
        catch
        {
            return new DataModel.CollegeModel();
        }
        
    }
    
    public static string GetBig12PredictorLog(DataModel.MatchupResult pred, DataModel.MatchupResult act)
    {
        string result = "";
        if (act.HomeTeamScore == 00 || act.AwayTeamScore == 00)
        {
            result = "(?)";
        }
        else
        {
            if(pred.Winner == act.Winner)
            {
                result = "(X)";
            }
            else
            {
                result = "( )";
            }
        }
        result += " " + pred.AwayTeam + " " + pred.AwayTeamScore.ToString() + "-" + pred.HomeTeamScore.ToString() + " " + pred.HomeTeam;
        //display the plus minuses
        //int awayPM = pred.AwayTeamScore - act.AwayTeamScore;
        //int homePM = pred.HomeTeamScore - act.HomeTeamScore;
        //result += " (" + awayPM.ToString() + " , " + homePM.ToString() + ")";
        return result;
    }
    public static async Task<List<DataModel.CollegeModel>> ScrapeLive2025Data()
    {
        try
        {
            List<DataModel.CollegeModel> res = new List<DataModel.CollegeModel>();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load("https://www.barttorvik.com/trank.php?year=2025&sort=&conlimit=#");
            List<HtmlNode> rows = doc.DocumentNode.Descendants("tr").ToList();
            foreach(HtmlNode row in rows)
            {
                try
                {
                    DataModel.CollegeModel curr = new DataModel.CollegeModel();
                    List<HtmlNode> cells = row.Descendants("td").ToList();
                    curr.Rank = Convert.ToInt32(cells[0].InnerText);
                    string nameText = cells[1].InnerText;
                    if (nameText.Contains("&"))
                    {
                        nameText = nameText.Substring(0, nameText.IndexOf("&"));
                    }
                    curr.Name = nameText;
                    curr.Conference = cells[2].InnerText;
                    curr.GamesPlayed = Convert.ToInt32(cells[3].InnerText);
                    curr.Wins = 0;
                    curr.Losses = 0;
                    curr.ADJOE = Convert.ToDecimal(cells[5].InnerText);
                    curr.ADJDE = Convert.ToDecimal(cells[6].InnerText);
                    curr.BARTHAG = Convert.ToDecimal(cells[7].InnerText);
                    curr.EFG_O = Convert.ToDecimal(cells[8].InnerText);
                    curr.EFG_D = Convert.ToDecimal(cells[9].InnerText);
                    curr.TOR_O = Convert.ToDecimal(cells[10].InnerText);
                    curr.TOR_D = Convert.ToDecimal(cells[11].InnerText);
                    curr.ORB = Convert.ToDecimal(cells[12].InnerText);
                    curr.DRB = Convert.ToDecimal(cells[13].InnerText);
                    curr.FTR_O = Convert.ToDecimal(cells[14].InnerText);
                    curr.FTR_D = Convert.ToDecimal(cells[15].InnerText);
                    curr.PT2_O = Convert.ToDecimal(cells[16].InnerText);
                    curr.PT2_D = Convert.ToDecimal(cells[17].InnerText);
                    curr.PT3_O = Convert.ToDecimal(cells[18].InnerText);
                    curr.PT3_D = Convert.ToDecimal(cells[19].InnerText);
                    curr.ADJ_T = Convert.ToDecimal(cells[22].InnerText);
                    curr.WAB = Convert.ToDecimal(cells[23].InnerText);

                    //free throws
                    //this will get set later
                    curr.FTP = 0; 
                    res.Add(curr);

                }
                catch
                {
                    //just skip the team if they have a error.
                }
                
                
            }
            return res;

        }
        catch(Exception ex)
        {
            return new List<DataModel.CollegeModel>();
        }
    }

    public static DataModel.MatchupResult CreateMatchupResult(string awayTeam, int awayTeamScore, string homeTeam, int homeTeamScore)
    {
        DataModel.MatchupResult res = new DataModel.MatchupResult();
        res.AwayTeam = awayTeam;
        res.AwayTeamScore = awayTeamScore;
        res.HomeTeam = homeTeam;
        res.HomeTeamScore = homeTeamScore;
        if (res.AwayTeamScore > res.HomeTeamScore)
        {
            res.Winner = awayTeam;
            res.WinnerScore = awayTeamScore;
            res.Loser = homeTeam;
            res.LoserScore = homeTeamScore;
        }
        else
        {
            res.Winner = homeTeam;
            res.WinnerScore = homeTeamScore;
            res.Loser = awayTeam;
            res.LoserScore = awayTeamScore;
        }
        res.Overtimes = 0;
        return res;
    }

    public static void UpdateFreeThrow(DataModel.CollegeModel team)
    {
        //first check and see if team already has FTP
        if(team.FTP != 0)
        {
            //FTP is already set, we outta here
            return;
        }

        string urlTeamName = team.Name.Replace(" ", "+");
        if (urlTeamName.Contains("'"))
        {
            urlTeamName = urlTeamName.Replace("'", "%27");
        }

        if (urlTeamName.Contains("&"))
        {
            urlTeamName = urlTeamName.Replace("&", "%26");
        }

        HtmlWeb w = new HtmlWeb();
        HtmlDocument teamDoc = w.Load("https://barttorvik.com/team.php?team=" + urlTeamName + "&year=2025");
        HtmlNode ftpDiv = teamDoc.GetElementbyId("ft_per");
        string ftpString = ftpDiv.InnerText;
        decimal ftp = Convert.ToDecimal(ftpString);
        team.FTP = ftp;
    }

}