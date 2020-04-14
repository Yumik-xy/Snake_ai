using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Xml;

namespace Snake
{
    class Rank : IComparable<Rank>
    {
        int score;
        string name;
        string level;
        public int Score { get { return score; } set { score = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Level { get { return level; } set { level = value; } }
        public int CompareTo(Rank other)
        {
            if (null == other)
            {
                return 1;
            }
            return other.score.CompareTo(score);
        }

        public Rank(int score, string name, string level)
        {
            this.score = score;
            this.name = name;
            this.level = level;
        }
        public Rank()
        {
            score = -1;
            name = null;
            level = null;
        }
    }
    class Rank_Xml
    {
        public void AddXml(Rank xrz)
        {
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(Environment.CurrentDirectory + "\\Score.xml");

                XmlNode rank = xml.SelectSingleNode("Rank");

                XmlElement Player = xml.CreateElement("Player");

                CreateNode(xml, Player, "Score", xrz.Score.ToString());
                CreateNode(xml, Player, "Name", xrz.Name);
                CreateNode(xml, Player, "Level", xrz.Level);
                rank.AppendChild(Player);

                try { xml.Save(Environment.CurrentDirectory + "\\Score.xml"); }
                catch { }

            }
            catch
            {
                XmlNode node = xml.CreateXmlDeclaration("1.0", "utf-8", null);
                xml.AppendChild(node);

                XmlElement rank = xml.CreateElement("Rank");
                xml.AppendChild(rank);

                XmlNode Player = xml.CreateNode(XmlNodeType.Element, "Player", null);
                CreateNode(xml, Player, "Score", xrz.Score.ToString());
                CreateNode(xml, Player, "Name", xrz.Name);
                CreateNode(xml, Player, "Level", xrz.Level);
                rank.AppendChild(Player);

                try { xml.Save(Environment.CurrentDirectory + "\\Score.xml"); }
                catch { }
            }
        }

        public void LoadXml(ListView _listView)
        {
            List<Rank> ranks = new List<Rank>();
            XmlDocument xml = new XmlDocument();
            try
            {
                xml.Load(Environment.CurrentDirectory + "\\Score.xml");
            }
            catch
            {
                AddXml(new Rank(0, "UNKNOWN", "UNKNOWN"));
                xml.Load(Environment.CurrentDirectory + "\\Score.xml");
            }

            XmlNode root = xml.SelectSingleNode("Rank");
            foreach (XmlNode node in root.ChildNodes)
            {
                Rank rank = new Rank();
                XmlElement node1 = (XmlElement)node;
                rank.Score = int.Parse(node1.ChildNodes.Item(0).InnerText);
                rank.Name = node1.ChildNodes.Item(1).InnerText;
                rank.Level = node1.ChildNodes.Item(2).InnerText;
                ranks.Add(rank);
            }
            ranks.Sort();

            _listView.BeginUpdate();
            for (int i = 0; i < Math.Min(ranks.Count, 10); i++)
            {
                ListViewItem listView = new ListViewItem();
                listView.Text = ranks[i].Score.ToString();
                listView.SubItems.Add(ranks[i].Name);
                listView.SubItems.Add(ranks[i].Level);
                _listView.Items.Add(listView);
            }
            _listView.EndUpdate();
        }

        public void CreateNode(XmlDocument xml, XmlNode parentNode, string name, string value)
        {
            XmlNode node = xml.CreateNode(XmlNodeType.Element, name, null);
            node.InnerText = value;
            parentNode.AppendChild(node);
        }
    }
}
