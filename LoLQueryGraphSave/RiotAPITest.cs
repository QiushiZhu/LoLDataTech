using RiotSharp.ChampionEndpoint;
using RiotSharp.CurrentGameEndpoint;
using RiotSharp.FeaturedGamesEndpoint;
using RiotSharp.GameEndpoint;
using RiotSharp.LeagueEndpoint;
using RiotSharp.MatchEndpoint;
using RiotSharp.StatsEndpoint;
using RiotSharp.SummonerEndpoint;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Text;
using System.IO;


namespace LoLQueryGraphSave
{
    class RiotAPITest
    {
        const double millisecondsOneDay = 86400000;
            
        public static void MasterLeagueIDList() 
        {
            DateTime startTime = DateTime.Now;
            string fileName = "MasterLeagueIDs";
            string filepath = @"F:\lolMatchData\";
            FileStream fs1 = new FileStream(filepath + fileName, FileMode.Create);
            StreamWriter wr1 = new StreamWriter(fs1);
            var api = RiotSharp.RiotApi.GetInstance("82d252ad-f7a4-4a28-8c3a-0f679dcf2ea5");
            //var summoner = api.GetSummoner(RiotSharp.Region.euw, "StopOFlop");
            //string summonerId = summoner.ProfileIconId.ToString();
            //List<Game> X = summoner.GetRecentGames();
            League Y = api.GetMasterLeague(RiotSharp.Region.kr, RiotSharp.Queue.RankedSolo5x5);            
            for (int i = 0; i < Y.Entries.Count; i++)
            {
                string gameId = Y.Entries[i].PlayerOrTeamId;
                wr1.WriteLine(gameId);
                
            }
            DateTime endTime = DateTime.Now;
            TimeSpan duration =  endTime - startTime;
            Console.WriteLine(duration.TotalSeconds);       
        }

        public static void QueryMatchlistFromID(long summonerId)
        {
            string fileName = "MatchListOf"+summonerId;
            string filepath = @"F:\lolMatchData\";
            FileStream fs1 = new FileStream(filepath + fileName, FileMode.Create);
            StreamWriter wr1 = new StreamWriter(fs1);
            var api = RiotSharp.RiotApi.GetInstance("82d252ad-f7a4-4a28-8c3a-0f679dcf2ea5");
            List<RiotSharp.Queue> a = new List<RiotSharp.Queue>{ RiotSharp.Queue.RankedSolo5x5 };
            MatchList Y = api.GetMatchList(RiotSharp.Region.kr, summonerId, rankedQueues: a);

            
            
            for (int i = 0; i < Y.Matches.Count; i++)
            {
                long gameId = Y.Matches[i].MatchID;
                wr1.WriteLine(gameId);

            }
        }

        public static void QueryParticipantFromMatchID(long matchId)
        {
            string fileName = "ParticantOf" + matchId;
            string filepath = @"F:\lolMatchData\";
            FileStream fs1 = new FileStream(filepath + fileName, FileMode.Create);
            StreamWriter wr1 = new StreamWriter(fs1);
            var api = RiotSharp.RiotApi.GetInstance("82d252ad-f7a4-4a28-8c3a-0f679dcf2ea5");
            MatchDetail Y =  api.GetMatch(RiotSharp.Region.kr, matchId);

            for (int i = 0; i < Y.ParticipantIdentities.Count; i++)
            {
                long ParticipantSummonerId = Y.ParticipantIdentities[i].Player.SummonerId;                
                wr1.WriteLine(ParticipantSummonerId);
                Console.WriteLine(ParticipantSummonerId);           
            }
            wr1.Flush();
            wr1.Close();
        }

        public static void QueryLeagueFromSummonerId(int SummonerId)
        {
            //string fileName = "LeagueOf" + SummonerId;
            //string filepath = @"F:\lolMatchData\";
            //FileStream fs1 = new FileStream(filepath + fileName, FileMode.Create);
            //StreamWriter wr1 = new StreamWriter(fs1);
            var api = RiotSharp.RiotApi.GetInstance("82d252ad-f7a4-4a28-8c3a-0f679dcf2ea5");
            List<int> a = new List<int> { SummonerId };
            Summoner Z = api.GetSummoner(RiotSharp.Region.kr, SummonerId);
            League Y = Z.GetLeagues()[0];
            Console.WriteLine( Y.Tier.ToString()+Y.Name);            
        }

        public static void QueryAllMatchesOfMasterLeague()
        {
            string filePath = @"F:\lolMatchData\";
            string inputName = "MasterLeagueIDs";
            string outputName = "MasterLeagueMatches";
            FileStream fs1 = new FileStream(filePath + inputName, FileMode.Open);
            FileStream fs2 = new FileStream(filePath + outputName, FileMode.Create);
            StreamReader sr1 = new StreamReader(fs1);
            StreamWriter wr2 = new StreamWriter(fs2);
            List<long> MasterMatches = new List<long>();
            var api = RiotSharp.RiotApi.GetInstance("82d252ad-f7a4-4a28-8c3a-0f679dcf2ea5");
            List<RiotSharp.Queue> a = new List<RiotSharp.Queue> { RiotSharp.Queue.RankedSolo5x5 };
            int summonerCountIndex = 0;

            while (sr1.Peek() >= 0)
            {
                summonerCountIndex++;
                long summonerId = long.Parse(sr1.ReadLine());
                MatchList Y = api.GetMatchList(RiotSharp.Region.kr, summonerId, rankedQueues: a);
                if (Y!=null && Y.Matches!=null && Y.Matches.Count>0 )
                {
                    for (int i = 0; i < Y.Matches.Count; i++)
                    {
                        long gameId = Y.Matches[i].MatchID;
                        if (!MasterMatches.Contains(gameId) && gameId > 2000000000)
                        {
                            MasterMatches.Add(gameId);
                            wr2.WriteLine(gameId);
                        }
                    }
                }
                Console.WriteLine("现在进行分析的召唤师序号为: " + summonerCountIndex);
                Console.WriteLine("现在库内的MatchID数量为: " + MasterMatches.Count);
            }
            wr2.Flush();
        }

        public static void QueryEliteSummoners()
        {
            int matchCounter = 0;
            string filePath = @"F:\lolMatchData\";
            string inputName = "MasterLeagueMatches";
            string outputName = "EliteSummonerIDs";
            FileStream fs1 = new FileStream(filePath + inputName, FileMode.Open);
            FileStream fs2 = new FileStream(filePath + outputName, FileMode.Create);
            StreamReader sr1 = new StreamReader(fs1);
            StreamWriter wr2 = new StreamWriter(fs2);
            List<long> EliteSummonerIDS = new List<long>();
            var api = RiotSharp.RiotApi.GetInstance("82d252ad-f7a4-4a28-8c3a-0f679dcf2ea5");

           

            while (sr1.Peek() >= 0)
            {
                matchCounter++;
                long matchId = long.Parse(sr1.ReadLine());
                MatchDetail Y = api.GetMatch(RiotSharp.Region.kr, matchId);
                if (Y != null )
                {
                    for (int i = 0; i < Y.ParticipantIdentities.Count; i++)
                    {
                        long ParticipantSummonerId = Y.ParticipantIdentities[i].Player.SummonerId;
                        if (!EliteSummonerIDS.Contains(ParticipantSummonerId))
                        {
                            EliteSummonerIDS.Add(ParticipantSummonerId);
                            wr2.WriteLine(ParticipantSummonerId);
                        }
                    }
                }
                Console.WriteLine("现在进行分析的比赛计数为: " + matchCounter);
                Console.WriteLine("现在库内的召唤师数量为: "+EliteSummonerIDS.Count);
            }
            wr2.Flush();
        }

        public static void QueryRecentMatchlistFromEliteSummoners()
        {
            string filePath = @"F:\lolMatchData\";
            string inputName = "MasterLeagueIDs";
            string outputName = "EliteSummonerMatchIDs";
            FileStream fs1 = new FileStream(filePath + inputName, FileMode.Open);
            FileStream fs2 = new FileStream(filePath + outputName, FileMode.Create);
            StreamReader sr1 = new StreamReader(fs1);
            StreamWriter wr2 = new StreamWriter(fs2);
            int summonerCounter = 0;
            var api = RiotSharp.RiotApi.GetInstance("82d252ad-f7a4-4a28-8c3a-0f679dcf2ea5");
            List<long> matchIDs = new List<long> { };
            List<RiotSharp.Queue> a = new List<RiotSharp.Queue> { RiotSharp.Queue.RankedSolo5x5 };
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-70);

            while (sr1.Peek() >= 0)
            {
                summonerCounter++;
                long summonerID = long.Parse(sr1.ReadLine());
                MatchList Y = api.GetMatchList(RiotSharp.Region.kr, summonerID, rankedQueues: a,beginTime:yesterday);
                

                if (Y != null&&Y.Matches!=null&&Y.Matches.Count>0)
                {
                    for (int i = 0; i < Y.Matches.Count; i++)
                    {
                        long MatchID = Y.Matches[i].MatchID;
                        if (!matchIDs.Contains(MatchID))
                        {
                           matchIDs.Add(MatchID);
                            wr2.WriteLine(MatchID);
                        }
                    }
                }
                Console.WriteLine("现在进行分析的召唤师序号为: " + summonerCounter);
                Console.WriteLine("现在库内的MatchID数量为: " +matchIDs.Count);
            }
            wr2.Flush();
        }

        
    }
}
