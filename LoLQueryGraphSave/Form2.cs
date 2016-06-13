using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace LoLQueryGraphSave
{
    public partial class Form2 : Form
    {
        crawlerHash c1 = new crawlerHash();
        public Form2()
        {
            Form.CheckForIllegalCrossThreadCalls = false;
            InitializeComponent();            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            c1.init();
            textBox1.AppendText("初始化已完成!");
            textBox1.AppendText(Environment.NewLine);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread downloadThread = new Thread(loopDownload);
            downloadThread.Start();
            textBox1.AppendText("正在执行下载!");
            textBox1.AppendText(Environment.NewLine);
            //Thread.Sleep(100);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            c1.stopDownload();            
            textBox1.AppendText("已成功结束!");
            textBox1.AppendText(Environment.NewLine);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void loopDownload()
        {
            while (!c1._shouldStop)
            {
                textBox1.AppendText(c1.dataDownloader());
                textBox1.AppendText(Environment.NewLine);
            }
        }
    }
}
