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
    class PlayerGold
    {

        public static void convertToCsv()
        {

            DateTime timeStart = DateTime.Now;

            //初始化需要记录的变量
            string matchId;
            //bool win = false;
            int minutes;
            int[] totalGold = new int[10]; ;
            int[] xp = new int[10]; ;
            int[] championID = new int[10];


            //创建包含所有文件名的FileInfo[]
            DirectoryInfo theFolder = new DirectoryInfo(@"E:\lolMatchData\GetMatchDetails\MatchDetail");
            FileInfo[] allMatchData = theFolder.GetFiles();

            // 创建文件。如果文件存在则覆盖
            FileStream fs = File.Open(@"e:\PlayerGold.txt", FileMode.Create);

            // 创建写入流
            StreamWriter wr = new StreamWriter(fs);



            //创建单行string
            string com = ",";
            string columnTitle;
            columnTitle = @"matchID,minutes,totalGold,xp,championID";
            wr.WriteLine(columnTitle);


            //实例化需要记录的变量
            for (int h = 1; h < 150000; h++)
            {
                //记录比赛场次，监视数据处理进度
                Console.WriteLine(h.ToString());

                //将json以文本格式读取到流
                StreamReader ms = allMatchData[h].OpenText();
                string matchString = ms.ReadToEnd();

                //将流中的文本转化为JObject
                JObject match = JObject.Parse(matchString);

                if(match["participants"].Count()==10)
                {
                    matchId = match["matchId"].ToString();
                    for (int j = 0; j < 10; j++)
                    {
                        championID[j] = match["participants"][j].Value<int>("championId");
                    }


                    for (int j = 1; j < match["timeline"]["frames"].Count(); j++)
                    {
                        minutes = j;


                        for (int k = 0; k < 10; k++)
                        {
                            totalGold[k] = match["timeline"]["frames"][j]["participantFrames"][(k + 1).ToString()].Value<int>("totalGold");
                            xp[k] = match["timeline"]["frames"][j]["participantFrames"][(k + 1).ToString()].Value<int>("xp");

                            wr.WriteLine(matchId + com + minutes + com + totalGold[k] + com + xp[k] + com + championID[k]);
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
