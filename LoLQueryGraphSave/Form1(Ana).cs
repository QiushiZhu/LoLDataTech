using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;

namespace LoLQueryGraphSave
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();
            Ana a1 = new Ana();
            a1.draw();
            a1 = null;
            
        }
    }

    //将查询出来的数据缓存至工程


    public class Ana
    {
        public double[,] statChart = new double[19, 11];
        public double[,] statSumChart = new double[19, 11];

        System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        System.Windows.Forms.DataVisualization.Charting.Chart chart3;
        System.Windows.Forms.DataVisualization.Charting.Chart chart4;
        System.Windows.Forms.DataVisualization.Charting.Chart chart5;
        System.Windows.Forms.DataVisualization.Charting.Chart chart6;
        System.Windows.Forms.DataVisualization.Charting.Chart chart7;


        public void ss(SQLiteCommand x, ref string queryInfo, ref int queryValue)//读取查询中的数据,并记录的方法
        {
            SQLiteDataReader b = x.ExecuteReader();
            while (b.Read())
            {
                //Console.WriteLine(b.GetInt32(0));
                queryInfo = queryInfo + "," + b.GetInt32(0);
                queryValue = b.GetInt32(0);
            }
            b.Close();
        }

        #region sql macros  要求选定对手  

        //所有对局比赛中，玩家每等级所有击杀数。
        public static string allMyKillN(string Id1, string Id2, string lv)
        {
            return "SELECT count(matchId) from killList WHERE (killerChampionId=" + Id1 + " AND killerLevel=" + lv + ") or (par1ChampionId=" + Id1 + " and par1Level=" + lv + ") or (par2ChampionId=" + Id1 + " and par2Level=" + lv + ") or (par3ChampionId=" + Id1 + " and par3Level=" + lv + ") or (par4ChampionId=" + Id1 + " and par4Level=" + lv + ")";
        }
        public static string allMyKillN(string Id1, string lv)
        {
            return "SELECT count(matchId) from allMatchData2 WHERE (killerChampionId=" + Id1 + " AND killerLevel=" + lv + ") or (par1ChampionId=" + Id1 + " and par1Level=" + lv + ") or (par2ChampionId=" + Id1 + " and par2Level=" + lv + ") or (par3ChampionId=" + Id1 + " and par3Level=" + lv + ") or (par4ChampionId=" + Id1 + " and par4Level=" + lv + ")";
        }

        //每等级死亡次数
        public static string deaths(string Id1, string Id2, string lv)
        {
            return "SELECT count(matchId) from killList WHERE victimChampionId= " + Id1 + " AND victimLevel = " + lv;
        }
        public static string deaths(string Id1, string lv)
        {
            return "SELECT count(matchId) from allMatchData2 WHERE victimChampionId= " + Id1 + " AND victimLevel = " + lv;
        }

        //查询某等级总击杀对手次数
        public static string killCountN(string Id1, string Id2, string lv)
        {
            return "SELECT COUNT(matchId)FROM killList WHERE(victimChampionId = " + Id2 + " AND((killerChampionId = " + Id1 + " AND killerLevel = " + lv + ") or(par1ChampionId = " + Id1 + " and par1Level = " + lv + ") or(par2ChampionId = " + Id1 + " and par2Level = " + lv + ") or(par3ChampionId = " + Id1 + " and par3Level = " + lv + ") or(par4ChampionId = " + Id1 + " and par4Level = " + lv + ")))";
        }

        //查询某等级总单杀次数
        public static string soloCountN(string Id1, string Id2, string lv)
        {
            return "SELECT COUNT(matchId)FROM killList WHERE(victimChampionId = " + Id2 + " AND killerChampionId = " + Id1 + " AND parNum = 0 AND killerLevel=" + lv + ")";
        }

        #endregion

        void chartGraphConstruction(int Id1T, int Id2T, bool sum)
        {
            string Series1Name = NameIndex.IndexParse(Id1T);
            string Series2Name = "";
            if (!sum)
            { Series2Name = NameIndex.IndexParse(Id2T); }
            else if (sum)
            { Series2Name = "总计"; }
            Font Title1Font = new Font("微软雅黑", 15);

            #region 表格初始化

            //击杀总量表
            chart3.Series.Add(Series1Name);
            chart3.Series.Add(Series2Name);

            chart3.Legends["Legend1"].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            chart3.Titles.Add("Title1");
            chart3.Titles["Title1"].Text = "全等级击杀量对比";
            chart3.Titles["Title1"].Font = Title1Font;
            chart3.ChartAreas["ChartArea1"].AxisX.Title = "等级";
            chart3.ChartAreas["ChartArea1"].AxisY.Title = "击杀量";

            //单挑表            
            chart1.Series.Add("单挑流行度");
            chart1.Series.Add("单挑胜率");
            chart1.Series["单挑胜率"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart1.Series["单挑流行度"].YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            chart1.Series["单挑胜率"].BorderWidth = 5;
            chart1.Series["单挑胜率"].Color = Color.DodgerBlue;
            chart1.Series["单挑流行度"].Color = Color.Orange;
            chart1.Legends["Legend1"].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            chart1.Titles.Add("Title1");
            chart1.Titles["Title1"].Text = "全等级单挑分析";
            chart1.Titles["Title1"].Font = Title1Font;
            chart1.ChartAreas["ChartArea1"].AxisX.Title = "等级";
            chart1.ChartAreas["ChartArea1"].AxisY.Title = "胜率百分比";
            chart1.ChartAreas["ChartArea1"].AxisY2.Title = "流行度百分比";
            chart1.ChartAreas["ChartArea1"].AxisY2.MajorGrid.Enabled = false;
            chart1.ChartAreas["ChartArea1"].AxisY.Maximum = 1.0;

            //GANK表            
            chart2.Series.Add("GANK流行度");
            chart2.Series.Add("GANK胜率");
            chart2.Series["GANK胜率"].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            chart2.Series["GANK流行度"].YAxisType = System.Windows.Forms.DataVisualization.Charting.AxisType.Secondary;
            chart2.Series["GANK胜率"].BorderWidth = 5;
            chart2.Series["GANK胜率"].Color = Color.DodgerBlue;
            chart2.Series["GANK流行度"].Color = Color.Orange;
            chart2.Legends["Legend1"].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            chart2.Titles.Add("Title1");
            chart2.Titles["Title1"].Text = "全等级GANK分析";
            chart2.Titles["Title1"].Font = Title1Font;
            chart2.ChartAreas["ChartArea1"].AxisX.Title = "等级";
            chart2.ChartAreas["ChartArea1"].AxisY.Title = "胜率百分比";
            chart2.ChartAreas["ChartArea1"].AxisY2.Title = "流行度百分比";
            chart2.ChartAreas["ChartArea1"].AxisY2.MajorGrid.Enabled = false;
            chart2.ChartAreas["ChartArea1"].AxisY.Maximum = 1.0;

            //死亡表
            chart4.Series.Add(Series1Name);
            chart4.Series.Add(Series2Name);
            chart4.Legends["Legend1"].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            chart4.Titles.Add("Title1");
            chart4.Titles["Title1"].Text = "全等级死亡量对比";
            chart4.Titles["Title1"].Font = Title1Font;
            chart4.ChartAreas["ChartArea1"].AxisX.Title = "等级";
            chart4.ChartAreas["ChartArea1"].AxisY.Title = "死亡量";

            //KDA表
            chart5.Series.Add(Series1Name);
            chart5.Series.Add(Series2Name);
            chart5.Legends["Legend1"].Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            chart5.Titles.Add("Title1");
            chart5.Titles["Title1"].Text = "全等级KDA对比";
            chart5.Titles["Title1"].Font = Title1Font;
            chart5.ChartAreas["ChartArea1"].AxisX.Title = "等级";
            chart5.ChartAreas["ChartArea1"].AxisY.Title = "KDA";

            #endregion

            if (!sum)
            {
                for (int i = 1; i < 19; i++)
                {

                    chart1.Series["单挑流行度"].Points.AddXY(i, statChart[i, 1]);
                    chart1.Series["单挑胜率"].Points.AddXY(i, statChart[i, 2]);
                    chart2.Series["GANK流行度"].Points.AddXY(i, statChart[i, 3]);
                    chart2.Series["GANK胜率"].Points.AddXY(i, statChart[i, 4]);
                    chart3.Series[Series1Name].Points.AddXY(i, statChart[i, 5]);
                    chart3.Series[Series2Name].Points.AddXY(i, statChart[i, 6]);
                    chart4.Series[Series1Name].Points.AddXY(i, statChart[i, 7]);
                    chart4.Series[Series2Name].Points.AddXY(i, statChart[i, 8]);
                    chart5.Series[Series1Name].Points.AddXY(i, statChart[i, 9]);
                    chart5.Series[Series2Name].Points.AddXY(i, statChart[i, 10]);

                }
                //        statChart[i, 1] = dSoloPerMatch;
                //        statChart[i, 2] = dSoloRate;
                //        statChart[i, 3] = dGankPerMatch;
                //        statChart[i, 4] = dGankRate;
                //        statChart[i, 5] = dAllKillPerMatch;
                //        statChart[i, 6] = dAllKilledPerMatch;
                //        statChart[i, 7] = dAllDeathsPerMatch;
                //        statChart[i, 8] = dAllDeathedsPerMatch;
                //        statChart[i, 9] = MyKDA;
                //        statChart[i, 10] = EKDA;



                //string chart1Title = "全等级单挑分析";
                //string chart2Title = "全等级Gank分析";
                //string chart3Title = "全等级击杀对比";
                //string chart4Title = "全等级死亡对比";
                //string chart5Title = "全等级KDA对比";

                Directory.CreateDirectory(@"E:\lolLaneStat\" + Series1Name);

                chart1.SaveImage(@"E:\lolLaneStat\" + Series1Name + @"\" + Series2Name + "_1.png", System.Drawing.Imaging.ImageFormat.Png);
                chart2.SaveImage(@"E:\lolLaneStat\" + Series1Name + @"\" + Series2Name + "_2.png", System.Drawing.Imaging.ImageFormat.Png);
                chart3.SaveImage(@"E:\lolLaneStat\" + Series1Name + @"\" + Series2Name + "_3.png", System.Drawing.Imaging.ImageFormat.Png);
                chart4.SaveImage(@"E:\lolLaneStat\" + Series1Name + @"\" + Series2Name + "_4.png", System.Drawing.Imaging.ImageFormat.Png);
                chart5.SaveImage(@"E:\lolLaneStat\" + Series1Name + @"\" + Series2Name + "_5.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            if (sum)
            {
                for (int i = 1; i < 19; i++)
                {

                    chart1.Series["单挑流行度"].Points.AddXY(i, statSumChart[i, 1]);
                    chart1.Series["单挑胜率"].Points.AddXY(i, statSumChart[i, 2]);
                    chart2.Series["GANK流行度"].Points.AddXY(i, statSumChart[i, 3]);
                    chart2.Series["GANK胜率"].Points.AddXY(i, statSumChart[i, 4]);
                    chart3.Series[Series1Name].Points.AddXY(i, statSumChart[i, 5]);
                    chart3.Series[Series2Name].Points.AddXY(i, statSumChart[i, 6]);
                    chart4.Series[Series1Name].Points.AddXY(i, statSumChart[i, 7]);
                    chart4.Series[Series2Name].Points.AddXY(i, statSumChart[i, 8]);
                    chart5.Series[Series1Name].Points.AddXY(i, statSumChart[i, 9]);
                    chart5.Series[Series2Name].Points.AddXY(i, statSumChart[i, 10]);

                }
                //        statChart[i, 1] = dSoloPerMatch;
                //        statChart[i, 2] = dSoloRate;
                //        statChart[i, 3] = dGankPerMatch;
                //        statChart[i, 4] = dGankRate;
                //        statChart[i, 5] = dAllKillPerMatch;
                //        statChart[i, 6] = dAllKilledPerMatch;
                //        statChart[i, 7] = dAllDeathsPerMatch;
                //        statChart[i, 8] = dAllDeathedsPerMatch;
                //        statChart[i, 9] = MyKDA;
                //        statChart[i, 10] = EKDA;



                //string chart1Title = "全等级单挑分析";
                //string chart2Title = "全等级Gank分析";
                //string chart3Title = "全等级击杀对比";
                //string chart4Title = "全等级死亡对比";
                //string chart5Title = "全等级KDA对比";

                Directory.CreateDirectory(@"E:\lolLaneStat\" + Series1Name);

                chart1.SaveImage(@"E:\lolLaneStat\" + Series1Name + @"\" + Series2Name + "1.png", System.Drawing.Imaging.ImageFormat.Png);
                chart2.SaveImage(@"E:\lolLaneStat\" + Series1Name + @"\" + Series2Name + "2.png", System.Drawing.Imaging.ImageFormat.Png);
                chart3.SaveImage(@"E:\lolLaneStat\" + Series1Name + @"\" + Series2Name + "3.png", System.Drawing.Imaging.ImageFormat.Png);
                chart4.SaveImage(@"E:\lolLaneStat\" + Series1Name + @"\" + Series2Name + "4.png", System.Drawing.Imaging.ImageFormat.Png);
                chart5.SaveImage(@"E:\lolLaneStat\" + Series1Name + @"\" + Series2Name + "5.png", System.Drawing.Imaging.ImageFormat.Png);
            }

            chart1.Series.Clear();
            chart1.Titles.Clear();
            chart2.Series.Clear();
            chart2.Titles.Clear();
            chart3.Series.Clear();
            chart3.Titles.Clear();
            chart4.Series.Clear();
            chart4.Titles.Clear();
            chart5.Series.Clear();
            chart5.Titles.Clear();

            GC.Collect();

        }

        void queryBegin(string Id1, string Id2)//运行查询的主体
        {

            string c = ",";
            //初始化所选择的英雄
            //Console.WriteLine("请输入Id1.");
            //string Id1T = Console.ReadLine();
            //Console.WriteLine("请输入Id2.");
            //string Id2T = Console.ReadLine();

            string Id1T = Id1;
            string Id2T = Id2;

            //创建写入流
            FileStream fs = File.Open(@"e:\" + Id1T + "对阵" + Id2T + ".csv", FileMode.Create);
            StreamWriter wr = new StreamWriter(fs);

            //BOM头
            fs.WriteByte(Convert.ToByte("EF", 16));
            fs.WriteByte(Convert.ToByte("BB", 16));
            fs.WriteByte(Convert.ToByte("BF", 16));


            //链接数据库
            SQLiteConnection conn = null;
            string dbPath = "Data Source =" + "E:\\workspace\\lolRProject\\LOLR.db";
            conn = new SQLiteConnection(dbPath);//创建数据库实例，指定文件位置
            conn.Open();//打开数据库   


            #region//queryInit
            //查询对线比赛
            SQLiteCommand test = new SQLiteCommand(@"
DROP TABLE IF EXISTS matchList1;
CREATE  TABLE matchList1(matchId TEXT);
INSERT INTO matchList1 SELECT DISTINCT matchId FROM allmatchData2 WHERE 
(
	victimChampionId=" + Id1T + @"
	AND
	(killerChampionId=" + Id2T + " or par1ChampionId=" + Id2T + " or par2ChampionId=" + Id2T + " or par3ChampionId=" + Id2T + " or par4ChampionId=" + Id2T + @")
) 
or 
(
	victimChampionId=" + Id2T + @" 
    AND
  (killerChampionId=" + Id1T + " or par1ChampionId=" + Id1T + "  or par2ChampionId=" + Id1T + "  or par3ChampionId=" + Id1T + "  or par4ChampionId=" + Id1T + @" )
);

", conn);
            //查询比赛场次
            SQLiteCommand test3 = new SQLiteCommand(@"
SELECT COUNT( matchId) from matchList1", conn);

            //建立临时表,包含比赛场次内击杀数据
            SQLiteCommand test7 = new SQLiteCommand(@"
DROP TABLE IF EXISTS killList;
CREATE  TABLE killList as 
SELECT * from allmatchData2 where matchId in matchList1;

", conn);
            #endregion

            test.ExecuteNonQuery();//比赛场次ID记录建表
            test7.ExecuteNonQuery();//根据比赛场次ID,建立击杀记录表,以提高查询效率


            SQLiteDataReader test3r = test3.ExecuteReader();//比赛场次的查询
            int matchCount = 0;
            while (test3r.Read())
            {
                //Console.WriteLine(test3r.GetInt32(0));
                wr.WriteLine("比赛场次" + "," + test3r.GetInt32(0));
                matchCount = test3r.GetInt32(0);
            }

            //开始关于等级中细节的查询
            string queryInfoTitle = "等级,击杀对手,被对手击杀,单杀对手,被对手单杀,玩家所有击杀,对手所有击杀,死亡数,对手死亡数";
            string statisticalInfoTitle = "单杀流行度,单杀成功率,GANK流行度,GANK成功率,击杀量,对手击杀量,死亡量,对手死亡量,玩家KDA,对手KDA";

            wr.WriteLine(queryInfoTitle + "," + statisticalInfoTitle);
            string[] kills = new string[19];
            for (int i = 1; i < 19; i++)
            {
                // Console.WriteLine("等级" + i.ToString());
                kills[i] = i.ToString();//以等级为标志,初始化要写入的字段      

                int nKill = 0, nKilled = 0, nSolo = 0, nSoloed = 0, nAllKill = 0, nAllkilled = 0, nDeath = 0, nDeathed = 0;
                double dSoloPerMatch = 0, dSoloRate = 0, dGankPerMatch = 0, dGankRate = 0, dAllKillPerMatch = 0, dAllKilledPerMatch = 0, dAllDeathsPerMatch = 0, dAllDeathedsPerMatch = 0, MyKDA = 0, EKDA = 0;

                SQLiteCommand a = new SQLiteCommand(killCountN(Id1T, Id2T, i.ToString()), conn);//击杀对手
                ss(a, ref kills[i], ref nKill);
                a = new SQLiteCommand(killCountN(Id2T, Id1T, i.ToString()), conn);//被对手击杀
                ss(a, ref kills[i], ref nKilled);
                a = new SQLiteCommand(soloCountN(Id1T, Id2T, i.ToString()), conn);//单杀对手
                ss(a, ref kills[i], ref nSolo);
                a = new SQLiteCommand(soloCountN(Id2T, Id1T, i.ToString()), conn);//被对手单杀
                ss(a, ref kills[i], ref nSoloed);
                a = new SQLiteCommand(allMyKillN(Id1T, Id2T, i.ToString()), conn);//玩家所有击杀(该等级下,击杀敌方5人中任意)
                ss(a, ref kills[i], ref nAllKill);
                a = new SQLiteCommand(allMyKillN(Id2T, Id1T, i.ToString()), conn);//对手所有击杀
                ss(a, ref kills[i], ref nAllkilled);
                a = new SQLiteCommand(deaths(Id1T, Id2T, i.ToString()), conn);//我方所有死亡
                ss(a, ref kills[i], ref nDeath);
                a = new SQLiteCommand(deaths(Id2T, Id1T, i.ToString()), conn);//对手所有死亡
                ss(a, ref kills[i], ref nDeathed);

                if ((nSolo + nSoloed) > 0)
                {
                    dSoloRate = nSolo * 1.0 / (nSoloed + nSolo);
                }
                else
                {
                    dSoloRate = 0.5;
                }
                if ((nKill + nKilled) > 0)
                {
                    dGankRate = nKill * 1.0 / (nKilled + nKill);
                }
                else
                {
                    dGankRate = 0.5;
                }

                dSoloPerMatch = nSolo * 1.0 / matchCount;
                dGankPerMatch = nKill * 1.0 / matchCount;
                dAllKillPerMatch = nAllKill * 1.0 / matchCount;
                dAllKilledPerMatch = nAllkilled * 1.0 / matchCount;
                dAllDeathsPerMatch = nDeath * 1.0 / matchCount;
                dAllDeathedsPerMatch = nDeathed * 1.0 / matchCount;
                MyKDA = nAllKill * 3.0 / nDeath;
                EKDA = nAllkilled * 3.0 / nDeathed;
                statChart[i, 1] = dSoloPerMatch;
                statChart[i, 2] = dSoloRate;
                statChart[i, 3] = dGankPerMatch;
                statChart[i, 4] = dGankRate;
                statChart[i, 5] = dAllKillPerMatch;
                statChart[i, 6] = dAllKilledPerMatch;
                statChart[i, 7] = dAllDeathsPerMatch;
                statChart[i, 8] = dAllDeathedsPerMatch;
                statChart[i, 9] = MyKDA;
                statChart[i, 10] = EKDA;
                wr.WriteLine(kills[i] + c + dSoloPerMatch.ToString("P") + c + dSoloRate.ToString("P") + c + dGankPerMatch.ToString("P") + c + dGankRate.ToString("P") + c + dAllKillPerMatch.ToString("P") + c + dAllKilledPerMatch.ToString("P") + c + dAllDeathsPerMatch.ToString("P") + c + dAllDeathedsPerMatch.ToString("P") + c + MyKDA.ToString("F2") + c + EKDA.ToString("F2"));//写入改等级的数据
            }

            conn.Close();
            wr.Flush();
            wr.Close();
            // Console.WriteLine("Done");


            GC.Collect();


        }

        public void draw()
        {


            for (int championIndex = 3; championIndex < 5; championIndex++)
            {
                int targetChampion = NameIndex.ChampionIndex[championIndex];
                bool sum = false;
                
                for (int i = 0; i < NameIndex.ChampionIndex.Length; i++)
                {
                    if (NameIndex.ChampionIndex[i] != targetChampion)
                    {
                        queryBegin(targetChampion.ToString(), NameIndex.ChampionIndex[i].ToString());
                        chartGraphConstruction(targetChampion, NameIndex.ChampionIndex[i], sum);
                        for (int j = 1; j < 19; j++)
                        {
                            for (int k = 1; k < 11; k++)
                            {
                                statSumChart[j, k] += statChart[j, k] / NameIndex.ChampionIndex.Length;
                            }
                        }

                    }

                }
                sum = true;
                chartGraphConstruction(targetChampion, 999, sum);
                Console.WriteLine(championIndex);

            }
        }
    }


}
