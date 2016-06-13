using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Threading;
using System.Timers;


namespace LINQ
{
    class PlayerKill
    {

        public static void convertToCsv()
        {

            DateTime timeStart = DateTime.Now;

            //初始化需要记录的变量
            int l = 1;
            int killerId;
            int victimId;
            int killerChampionId;
            int victimChampionId;
            int killerLevel;
            int victimLevel;
            int killerGold;
            int victimGold;
            //int oneOnOne;
            //string killerLane ;
            //string victimLane ;
            int parNum;
            int par1Id;
            int par1ChampionId;
            int par1Level;
            int par1Gold;
            //string par1Lane ;
            int par2Id;
            int par2ChampionId;
            int par2Level;
            int par2Gold;
            //string par2Lane ;
            
            int eventIndex;
            int par3Id;
            int par3ChampionId;
            int par3Level;
            int par3Gold;
            int par4Id;
            int par4ChampionId;
            int par4Level;
            int par4Gold;
            string matchId;
            //string matchVersion;
            //int matchDuration;
            int x;
            int y;



            //创建包含所有文件名的FileInfo[]
            DirectoryInfo theFolder = new DirectoryInfo(@"E:\lolMatchData\GetMatchDetails\MatchDetail");
            FileInfo[] allMatchData = theFolder.GetFiles();

            // 创建文件。如果文件存在则覆盖
            FileStream fs = File.Open(@"e:\killList1.txt", FileMode.Create);
            StreamWriter wr = new StreamWriter(fs);
            


            //创建单行string
            string com = ",";
            string columnTitle;
            columnTitle = @"eventIndex,matchId,x,y,killerChampionId,killerLevel,killerGold,victimChampionId,victimLevel,victimGold,,parNum,par1ChampionId,par1Level,par1Gold,par2ChampionId,par2Level,par2Gold,par3ChampionId,par3Level,par3Gold,par4ChampionId,par4Level,par4Gold";
            wr.WriteLine(columnTitle);

            Console.WriteLine("hello ?");

            //实例化需要记录的变量
            for (int h = 1; h < 150; h++)
            {
                //记录比赛场次，监视数据处理进度
                Console.WriteLine(h.ToString());

                //将json以文本格式读取到流
                StreamReader ms = allMatchData[h].OpenText();
                string matchString = ms.ReadToEnd();

                //将流中的文本转化为JObject
                JObject match = JObject.Parse(matchString);

                //记录matchList
                matchId = match["matchId"].ToString();



                //记录killList
                for (int j = 1; j < match["timeline"]["frames"].Count(); j++)
                {
                    if (match["timeline"]["frames"][j].Count() == 3)
                    {
                        for (int k = 1; k < match["timeline"]["frames"][j]["events"].Count(); k++)
                        {
                            if (match["timeline"]["frames"][j]["events"][k]["eventType"].ToString() == "CHAMPION_KILL")
                            {
                                matchId = match["matchId"].ToString();
                                x = match["timeline"]["frames"][j]["events"][k]["position"].Value<int>("x");
                                y = match["timeline"]["frames"][j]["events"][k]["position"].Value<int>("y");

                                //killerInfo
                                killerId = match["timeline"]["frames"][j]["events"][k].Value<int>("killerId");
                                if (killerId == 0)
                                {
                                    killerChampionId = 0;
                                    killerLevel = 0;
                                    killerGold = 0;
                                    // killerLane="MONSTER");
                                }
                                else
                                {
                                    killerChampionId = match["participants"][killerId - 1].Value<int>("championId");
                                    killerLevel = match["timeline"]["frames"][j]["participantFrames"][killerId.ToString()].Value<int>("level");
                                    killerGold = match["timeline"]["frames"][j]["participantFrames"][killerId.ToString()].Value<int>("totalGold");
                                    //killerLane=match["participants"][killerId - 1]["timeline"].Value<string>("lane");

                                }

                                //wtf match["participants"]是[0]到[9]，是数组排序；match["timeline"]["frames"][j]["participantFrames"]是1到10，是字符串；

                                //victimInfo
                                victimId = match["timeline"]["frames"][j]["events"][k].Value<int>("victimId");
                                victimChampionId = match["participants"][victimId - 1].Value<int>("championId");
                                victimLevel = match["timeline"]["frames"][j]["participantFrames"][victimId.ToString()].Value<int>("level");
                                victimGold = match["timeline"]["frames"][j]["participantFrames"][victimId.ToString()].Value<int>("totalGold");
                                //victimLane=match["participants"][victimId - 1]["timeline"].Value<string>("lane");


                                //测试
                                //ster=match["timeline"]["frames"][j]["events"][k].Count().ToString();

                                //participantInfo

                                //对于PARNUM=0,1,2,3,4做枚举？

                                if (match["timeline"]["frames"][j]["events"][k].Count() == 5)
                                {
                                    par1Id = 0;
                                    par1ChampionId = 0;
                                    par1Level = 0;
                                    par1Gold = 0;
                                    //par1Lane="VOID");
                                    par2Id = 0;
                                    par2ChampionId = 0;
                                    par2Level = 0;
                                    par2Gold = 0;

                                    par3ChampionId = 0;
                                    par3Level = 0;
                                    par3Gold = 0;
                                    par3Id = 0;
                                    par4ChampionId = 0;
                                    par4Level = 0;
                                    par4Gold = 0;
                                    par4Id = 0;
                                    //par2Lane="VOID");
                                    parNum = 0;

                                }
                                else if (match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Count() == 1)
                                {
                                    par1Id = match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Value<int>(0);
                                    par1ChampionId = match["participants"][par1Id - 1].Value<int>("championId");
                                    par1Level = match["timeline"]["frames"][j]["participantFrames"][par1Id.ToString()].Value<int>("level");
                                    par1Gold = match["timeline"]["frames"][j]["participantFrames"][par1Id.ToString()].Value<int>("totalGold");
                                    //par1Lane=match["participants"][par1Id - 1]["timeline"].Value<string>("lane");
                                    par2Id = 0;
                                    par2ChampionId = 0;
                                    par2Level = 0;
                                    par2Gold = 0;
                                    par3Id = 0;
                                    par3ChampionId = 0;
                                    par3Level = 0;
                                    par3Gold = 0;
                                    par4Id = 0;
                                    par4ChampionId = 0;
                                    par4Level = 0;
                                    par4Gold = 0;
                                    //par2Lane="VOID");
                                    parNum = 1;

                                }
                                else if (match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Count() == 2)
                                {
                                    par1Id = match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Value<int>(0);
                                    par1ChampionId = match["participants"][par1Id - 1].Value<int>("championId");
                                    par1Level = match["timeline"]["frames"][j]["participantFrames"][par1Id.ToString()].Value<int>("level");
                                    par1Gold = match["timeline"]["frames"][j]["participantFrames"][par1Id.ToString()].Value<int>("totalGold");
                                    //par1Lane=match["participants"][par1Id - 1]["timeline"].Value<string>("lane");
                                    par2Id = match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Value<int>(1);
                                    par2ChampionId = match["participants"][par2Id - 1].Value<int>("championId");
                                    par2Level = match["timeline"]["frames"][j]["participantFrames"][par2Id.ToString()].Value<int>("level");
                                    par2Gold = match["timeline"]["frames"][j]["participantFrames"][par2Id.ToString()].Value<int>("totalGold");
                                    //par2Lane=match["participants"][par2Id - 1]["timeline"].Value<string>("lane");
                                    par3Id = 0;
                                    par3ChampionId = 0;
                                    par3Level = 0;
                                    par3Gold = 0;
                                    par4Id = 0;
                                    par4ChampionId = 0;
                                    par4Level = 0;
                                    par4Gold = 0;
                                    parNum = 2;
                                }
                                else if (match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Count() == 3)
                                {
                                    par1Id = match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Value<int>(0);
                                    par1ChampionId = match["participants"][par1Id - 1].Value<int>("championId");
                                    par1Level = match["timeline"]["frames"][j]["participantFrames"][par1Id.ToString()].Value<int>("level");
                                    par1Gold = match["timeline"]["frames"][j]["participantFrames"][par1Id.ToString()].Value<int>("totalGold");
                                    //par1Lane=match["participants"][par1Id - 1]["timeline"].Value<string>("lane");
                                    par2Id = match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Value<int>(1);
                                    par2ChampionId = match["participants"][par2Id - 1].Value<int>("championId");
                                    par2Level = match["timeline"]["frames"][j]["participantFrames"][par2Id.ToString()].Value<int>("level");
                                    par2Gold = match["timeline"]["frames"][j]["participantFrames"][par2Id.ToString()].Value<int>("totalGold");
                                    //par2Lane=match["participants"][par2Id - 1]["timeline"].Value<string>("lane");
                                    par3Id = match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Value<int>(2);
                                    par3ChampionId = match["participants"][par3Id - 1].Value<int>("championId");
                                    par3Level = match["timeline"]["frames"][j]["participantFrames"][par3Id.ToString()].Value<int>("level");
                                    par3Gold = match["timeline"]["frames"][j]["participantFrames"][par3Id.ToString()].Value<int>("totalGold");
                                    par4Id = 0;
                                    par4ChampionId = 0;
                                    par4Level = 0;
                                    par4Gold = 0;
                                    parNum = 3;
                                }
                                else if (match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Count() == 4)
                                {
                                    par1Id = match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Value<int>(0);
                                    par1ChampionId = match["participants"][par1Id - 1].Value<int>("championId");
                                    par1Level = match["timeline"]["frames"][j]["participantFrames"][par1Id.ToString()].Value<int>("level");
                                    par1Gold = match["timeline"]["frames"][j]["participantFrames"][par1Id.ToString()].Value<int>("totalGold");
                                    //par1Lane=match["participants"][par1Id - 1]["timeline"].Value<string>("lane");
                                    par2Id = match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Value<int>(1);
                                    par2ChampionId = match["participants"][par2Id - 1].Value<int>("championId");
                                    par2Level = match["timeline"]["frames"][j]["participantFrames"][par2Id.ToString()].Value<int>("level");
                                    par2Gold = match["timeline"]["frames"][j]["participantFrames"][par2Id.ToString()].Value<int>("totalGold");
                                    //par2Lane=match["participants"][par2Id - 1]["timeline"].Value<string>("lane");
                                    par3Id = match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Value<int>(2);
                                    par3ChampionId = match["participants"][par3Id - 1].Value<int>("championId");
                                    par3Level = match["timeline"]["frames"][j]["participantFrames"][par3Id.ToString()].Value<int>("level");
                                    par3Gold = match["timeline"]["frames"][j]["participantFrames"][par3Id.ToString()].Value<int>("totalGold");

                                    par4Id = match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Value<int>(3);
                                    par4ChampionId = match["participants"][par4Id - 1].Value<int>("championId");
                                    par4Level = match["timeline"]["frames"][j]["participantFrames"][par4Id.ToString()].Value<int>("level");
                                    par4Gold = match["timeline"]["frames"][j]["participantFrames"][par4Id.ToString()].Value<int>("totalGold");

                                    parNum = 4;
                                }
                                else
                                {
                                    par1Id = 999999;
                                    par1ChampionId = 999999;
                                    par1Level = 999999;
                                    par1Gold = 999999;
                                    //par1Lane="ANY");
                                    par2Id = 999;
                                    par2ChampionId = 999999;
                                    par2Level = 999999;
                                    par2Gold = 999999;
                                    // par2Lane="ANY");
                                    par3Id = 999;
                                    par3ChampionId = 999999;
                                    par3Level = 999999;
                                    par3Gold = 999999;
                                    par4Id = 999;
                                    par4ChampionId = 999999;
                                    par4Level = 999999;
                                    par4Gold = 999999;
                                    parNum = match["timeline"]["frames"][j]["events"][k]["assistingParticipantIds"].Count();
                                }


                                eventIndex = l;

                                wr.WriteLine(eventIndex + com + matchId + com + x+com+y+com+killerChampionId + com + killerLevel + com + killerGold + com + victimChampionId + com + victimLevel + com + victimGold + com  + parNum + com + par1ChampionId + com + par1Level + com + par1Gold + com + par2ChampionId + com + par2Level + com + par2Gold + com + par3ChampionId + com + par3Level + com + par3Gold + com + par4ChampionId + com + par4Level + com + par4Gold);
                                
                                l++;


                            }
                        }
                    }
                }
            }

            

            
               
            


            // 关闭写入流
            wr.Flush();
            wr.Close();

            //记录数据处理所需要的时间
            DateTime timeEnd = DateTime.Now;

            //标志处理结束
            Console.WriteLine("Done");
            Console.WriteLine(timeStart);
            Console.WriteLine(timeEnd);
            Console.ReadLine();

        }
    }
}
