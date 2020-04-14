using System;
using System.Windows.Forms;

namespace Snake
{
    public partial class Score : Form
    {
        Rank_Xml rank_Xml = new Rank_Xml();
        public Score()
        {
            InitializeComponent();
        }

        private void Score_Load(object sender, EventArgs e)
        {
            rank_Xml.LoadXml(listView1);
        }
    }
}
