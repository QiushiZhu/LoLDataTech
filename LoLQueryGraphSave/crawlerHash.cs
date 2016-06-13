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
using Newtonsoft;

namespace LoLQueryGraphSave
{
    class crawlerHash
    {

        List<RiotSharp.Queue> a = new List<RiotSharp.Queue> { RiotSharp.Queue.RankedSolo5x5 };

        HashSet<long> matchIDsDownloading = new HashSet<long> { };
        HashSet<long> matchIDsDownloaded = new HashSet<long> { };
        HashSet<long> summonerIDsDownloading = new HashSet<long> { };
        HashSet<long> summonerIDsDownloaded = new HashSet<long> { };
        string path = @"F:\lolMatchData\crawler\";
        string path2 = @"F:\lolMatchData\crawler\matches\";
        string fl1 = "matchIDsDownloading.txt";
        string fl2 = "matchIDsDownloaded.txt";
        string fl3 = "summonerIDsDownloading.txt";
        string fl4 = "summonerIDsDownloaded.txt";
        

        public volatile bool _shouldStop;

        public void init()
        {
            fileInit(ref matchIDsDownloading, fl1);
            fileInit(ref matchIDsDownloaded, fl2);
            fileInit(ref summonerIDsDownloading, fl3);
            fileInit(ref summonerIDsDownloaded, fl4);
        }

        void fileInit(ref HashSet<long> _list, string _f)
        {
            FileStream fs = new FileStream(path + _f, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            int counter = 0;
            while (sr.Peek() >= 0)
            {
                //if (!_list.Contains(long.Parse(sr.ReadLine())))
                
                    counter++;
                    _list.Add(long.Parse(sr.ReadLine()));
                
            }
            Console.WriteLine("该向量长度为: " + counter);
            fs.Flush();
            sr.Close();
            fs.Close();
        }

        void fileFinish(ref HashSet<long> _list, string _f)
        {
            FileStream fs = new FileStream(path + _f, FileMode.Create);
            StreamWriter wr = new StreamWriter(fs);
            if (_list != null && _list.Count > 0)
            {
                long[] listArray = new long[_list.Count];                
                _list.CopyTo(listArray);                
                for (int i = 0; i < _list.Count; i++)
                {
                    wr.WriteLine(listArray[i]);
                }
            }
            fs.Flush();
            wr.Flush();
            //fs.Close();
            //wr.Close();
        }

        

        public void stopDownload()
        {
            _shouldStop = true;
            fileFinish(ref matchIDsDownloading, fl1);
            fileFinish(ref matchIDsDownloaded, fl2);
            fileFinish(ref summonerIDsDownloading, fl3);
            fileFinish(ref summonerIDsDownloaded, fl4);
        }

        public string dataDownloader()
        {
            var api = RiotSharp.RiotApi.GetInstance("82d252ad-f7a4-4a28-8c3a-0f679dcf2ea5");
            DateTime st = DateTime.Now;
            if (matchIDsDownloading.Count > 0)
            {
                long targetMatchID = matchIDsDownloading.First<long>();

                MatchDetail Y = null;
                try { Y = api.GetMatch(RiotSharp.Region.kr, targetMatchID, includeTimeline: true); }
                catch (RiotSharp.RiotSharpException e)
                {
                    return e.Message;
                }

                //save match detail as json
                string matchJsonName = "match" + targetMatchID.ToString()+".txt";
                string matchJsonCont = Newtonsoft.Json.JsonConvert.SerializeObject(Y);
                using (StreamWriter sw = new StreamWriter(path2 + matchJsonName))
                {
                    sw.Write(matchJsonCont);
                }

                //TODO:transport match detail to SQL                

                for (int i = 0; i < 10; i++)
                {
                    long tempID = Y.ParticipantIdentities[i].Player.SummonerId;
                    if (!summonerIDsDownloading.Contains(tempID))
                    {
                        summonerIDsDownloading.Add(tempID);
                    }
                }
                matchIDsDownloaded.Add(targetMatchID);
                matchIDsDownloading.Remove(targetMatchID);

                return ("Excellent! New match: " + Y.MatchId + " .time: " + (DateTime.Now-st).Seconds);
            }

            else if (summonerIDsDownloading.Count > 0)
            {
                //DateTime startTime = DateTime.Today.AddDays(-60);
                long targetSummonerID = summonerIDsDownloading.First();

                MatchList Z = null;
                try { Z = api.GetMatchList(RiotSharp.Region.kr, summonerId: targetSummonerID, rankedQueues: a); }
                catch (RiotSharp.RiotSharpException e)
                {
                    return e.Message;
                }

                

                if (Z != null && Z.Matches != null && Z.Matches.Count > 0)
                {
                    for (int i = 0; i < Z.Matches.Count; i++)
                    {
                        long tempMatchID = Z.Matches[i].MatchID;
                        if (!matchIDsDownloading.Contains(tempMatchID) && !matchIDsDownloaded.Contains(tempMatchID))
                        {
                            matchIDsDownloading.Add(tempMatchID);
                        }
                    }
                    summonerIDsDownloaded.Add(targetSummonerID);
                    summonerIDsDownloading.Remove(targetSummonerID);
                    return ("Excellent! New summoner: " + targetSummonerID + " with " +Z.Matches.Count+" matches .time: " + (DateTime.Now - st).Seconds);
                }

                else
                {
                    summonerIDsDownloaded.Add(targetSummonerID);
                    summonerIDsDownloading.Remove(targetSummonerID);
                    return ("Panic! New summoner: " + targetSummonerID + " No matches!");
                }
                
            }

            else
            {                
                return "Panic!No match or summoner!";
            }


        }

    }
}
