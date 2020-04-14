using System;
using System.Windows.Forms;

namespace Snake
{
    public partial class SaveRank : Form
    {
        public int score = 0;
        public string level = "UNKNOWN";

        Rank_Xml rank_Xml = new Rank_Xml();

        public SaveRank()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text == "")
            {
                MessageBox.Show("请输入非空的名字"); return;
            }
            rank_Xml.AddXml(new Rank(score, textBox1.Text, level));
            Close();
        }

        private void SaveRank_Load(object sender, EventArgs e)
        {
            label4.Text = score.ToString();
            label5.Text = level;
        }
    }
}
