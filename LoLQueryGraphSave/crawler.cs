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
    class crawler
    {
        
        List<RiotSharp.Queue> a = new List<RiotSharp.Queue> { RiotSharp.Queue.RankedSolo5x5 };

        

        List<long> matchIDsDownloading = new List<long> { };
        List<long> matchIDsDownloaded = new List<long> { };
        List<long> summonerIDsDownloading = new List<long> { };
        List<long> summonerIDsDownloaded = new List<long> { };
        string path = @"F:\lolMatchData\crawler\";
        string fl1 = "matchIDsDownloading.txt";
        string fl2 = "matchIDsDownloaded.txt";
        string fl3 = "summonerIDsDownloading.txt";
        string fl4 = "summonerIDsDownloaded.txt";

        volatile bool _shouldStop;

        public void init()
        {
            fileInit(ref matchIDsDownloading, fl1);
            fileInit(ref matchIDsDownloaded, fl2);
            fileInit(ref summonerIDsDownloading, fl3);
            fileInit(ref summonerIDsDownloaded, fl4);
        }

        void fileInit(ref List<long>_list,string _f)
        {
            FileStream fs = new FileStream(path + _f, FileMode.Open);
            StreamReader sr = new StreamReader(fs);
            int counter = 0;
            while(sr.Peek()>=0)
            {
                counter++;
                _list.Add(long.Parse(sr.ReadLine()));
            }
            Console.WriteLine("该向量长度为: " + counter);
            fs.Flush();
            sr.Close();
            fs.Close();
        }

        void fileFinish(ref List<long> _list, string _f)
        {
            FileStream fs = new FileStream(path + _f, FileMode.Create);
            StreamWriter wr = new StreamWriter(fs);
            if(_list!=null && _list.Count>0)
            {
                for (int i=0;i<_list.Count;i++)
                {
                    wr.WriteLine(_list[i]);
                }
            }
            fs.Flush();
            wr.Flush();
            //fs.Close();
            //wr.Close();
        }

        public void loopDownload()
        {
            while (!_shouldStop)
            {
                dataDownloader();
            }
        }

        public void stopDownload()
        {
            _shouldStop = true;
            fileFinish(ref matchIDsDownloading, fl1);
            fileFinish(ref matchIDsDownloaded, fl2);
            fileFinish(ref summonerIDsDownloading, fl3);
            fileFinish(ref summonerIDsDownloaded, fl4);
        }

        void dataDownloader()
        {
            var api = RiotSharp.RiotApi.GetInstance("82d252ad-f7a4-4a28-8c3a-0f679dcf2ea5");

            if (matchIDsDownloading.Count > 0)
            {
                MatchDetail Y = null;
                try
                { Y = api.GetMatch(RiotSharp.Region.kr, matchIDsDownloading[0], includeTimeline: true); }
                catch (RiotSharp.RiotSharpException e)
                {

                }

                //TODO:save match detail as json
                //TODO:transport match detail to SQL

                Console.WriteLine("Excellent! New match: " + Y.MatchId);

                for (int i = 0; i < 10; i++)
                {
                    long tempID = Y.ParticipantIdentities[i].Player.SummonerId;
                    if (!summonerIDsDownloading.Contains(tempID))
                    {
                        summonerIDsDownloading.Add(tempID);
                    }
                }
                matchIDsDownloaded.Add(matchIDsDownloading[0]);
                matchIDsDownloading.RemoveAt(0);                
            }

            else if(summonerIDsDownloading.Count>0)
            {
                DateTime startTime = DateTime.Today.AddDays(-60);

                MatchList Z = api.GetMatchList(RiotSharp.Region.kr, summonerId: summonerIDsDownloading[0], rankedQueues: a,beginTime:startTime);

                Console.WriteLine("Excellent! New summoner: " + summonerIDsDownloading[0]);

                if (Z != null && Z.Matches != null && Z.Matches.Count > 0)
                {
                    for (int i =0;i<Z.Matches.Count;i++)
                    {
                        long tempMatchID = Z.Matches[i].MatchID;
                        if(!matchIDsDownloading.Contains(tempMatchID)&&!matchIDsDownloaded.Contains(tempMatchID))
                        {
                            matchIDsDownloading.Add(tempMatchID);
                        }
                    }
                }

                summonerIDsDownloaded.Add(summonerIDsDownloading[0]);
                summonerIDsDownloading.RemoveAt(0);
            }

            else
            {
                Console.WriteLine("Panic!No match or summoner!");
                return;
            }
            
            
        }
        
    }
}
