using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;


namespace Snake
{
    public enum Direction { Left, Up, Right, Down }
    public partial class Main : Form
    {
        private int width = 20;
        private int height = 10;
        private int speed = 160;
        private Graphics graphics;
        private List<Block> snake1 = new List<Block>();
        private Direction old_direction1 = Direction.Left;
        private Direction new_direction1 = Direction.Left;
        private List<Block> snake2 = new List<Block>();
        private Direction old_direction2 = Direction.Left;
        private Direction new_direction2 = Direction.Left;
        private List<Block> oldsnake1;
        private const int size = 20;
        private bool Ai1 = false;
        private bool Ai2 = false;
        private int player = 1;
        private Block food;
        private bool pause = false;
        private int score;
        private double level;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Up) { new_direction1 = Direction.Up; return; }
            if (e.KeyCode == Keys.Left) { new_direction1 = Direction.Left; return; }
            if (e.KeyCode == Keys.Down) { new_direction1 = Direction.Down; return; }
            if (e.KeyCode == Keys.Right) { new_direction1 = Direction.Right; return; }
            if (e.KeyCode == Keys.W) { new_direction2 = Direction.Up; return; }
            if (e.KeyCode == Keys.A) { new_direction2 = Direction.Left; return; }
            if (e.KeyCode == Keys.S) { new_direction2 = Direction.Down; return; }
            if (e.KeyCode == Keys.D) { new_direction2 = Direction.Right; return; }
            if (e.KeyCode == Keys.Q && timer1.Enabled == false && pause == false) { Start(); return; }
            if (e.KeyCode == Keys.Space) { Pause(); return; }
        }

        private void Pause()
        {
            pause = !pause;
            switch (pause)
            {
                case true:
                    timer1.Stop();
                    menuStrip1.Enabled = true;
                    break;
                case false:
                    timer1.Start();
                    menuStrip1.Enabled = false;
                    RePainting();
                    break;
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (graphics != null && timer1.Enabled)
                RePainting();
        }

        private void Start()
        {
            snake1.Clear();
            snake2.Clear();
            menuStrip1.Enabled = false;
            score = 0; level = 0;
            graphics = Graphics.FromHwnd(pictureBox1.Handle);
            if (player == 1)
                label1.Text = "当前难度分：0" + "  当前得分：0" + "  ";
            old_direction1 = new_direction1 = Direction.Left;
            snake1.Add(new Block(size, new Point(width / 2, height / 2 - 3), Type.Head, old_direction1, 0));
            snake1.Add(new Block(size, new Point(width / 2 + 1, height / 2 - 3), Type.Tail, 0, old_direction1));
            if (player > 1)
            {
                old_direction2 = new_direction2 = Direction.Left;
                snake2.Add(new Block(size, new Point(width / 2, height / 2 + 3), Type.Head, old_direction1, 0));
                snake2.Add(new Block(size, new Point(width / 2 + 1, height / 2 + 3), Type.Tail, 0, old_direction1));
            }
            Createfood(out food);
            RePainting();
            timer1.Start();
        }

        private void End(int dead)
        {
            menuStrip1.Enabled = true;
            snake1.Clear();
            snake2.Clear();
            timer1.Enabled = false;
            pause = false;
            if (player == 1)
                if (dead == 1)
                {
                    SaveRank saveRank = new SaveRank();
                    saveRank.score = score * 10;
                    saveRank.level = level.ToString("0.00");
                    saveRank.ShowDialog();
                }
            if (player > 1)
            {
                if (dead == 0) MessageBox.Show("平局！", "贪吃蛇");
                if (dead == 2) MessageBox.Show("一号蛇蛇获得胜利！", "贪吃蛇");
                if (dead == 1) MessageBox.Show("二号蛇蛇获得胜利！", "贪吃蛇");
            }
        }

        private void Createfood(out Block food)
        {
            Random random = new Random();
            int wid = random.Next(width), hig = random.Next(height);
            bool insnake = false;
            while (true)
            {
                insnake = false;
                wid = random.Next(width);
                hig = random.Next(height);
                for (int i = 0; i < snake1.Count; i++)
                    if (snake1[i].Point.X == wid && snake1[i].Point.Y == hig)
                        insnake = true;
                for (int i = 0; i < snake2.Count; i++)
                    if (snake2[i].Point.X == wid && snake2[i].Point.Y == hig)
                        insnake = true;
                if (!insnake) break;
            }
            food = new Block(size, new Point(wid, hig), Type.Food);
            food.ScanPoint(graphics);
        }

        private void Snake1MoveDir()
        {
            Point point;
            Block head = snake1[0];

            if (Ai1)
            {
                SnakeAi snakeAi = new SnakeAi(width, height, snake1, snake2, food);
                new_direction1 = snakeAi.Getdirection();
            }

            if (snake1.Count != 1 && Math.Abs(old_direction1 - new_direction1) != 2)
                old_direction1 = new_direction1;
            else if (snake1.Count == 1)
                old_direction1 = new_direction1;

            if (old_direction1 == Direction.Up) point = new Point(head.Point.X, head.Point.Y - 1);
            else if (old_direction1 == Direction.Down) point = new Point(head.Point.X, head.Point.Y + 1);
            else if (old_direction1 == Direction.Left) point = new Point(head.Point.X - 1, head.Point.Y);
            else point = new Point(head.Point.X + 1, head.Point.Y);

            Block block = new Block(size, point, Type.Head, old_direction1);

            bool eatFood = food.Point.X == point.X && food.Point.Y == point.Y;

            if (!eatFood)
            {
                snake1[snake1.Count - 1].Clear(graphics);
                snake1.RemoveAt(snake1.Count - 1);
            }
            snake1.Insert(0, block);
            if (snake1.Count == 2)
            {
                snake1[1].New_direction = old_direction1;
                snake1[1].Type = Type.Tail;
                snake1[1].Clear(graphics);
                snake1[1].ScanPoint(graphics);
            }
            else if (snake1.Count > 2)
            {
                snake1[1].New_direction = old_direction1;
                snake1[1].Type = Type.Body;
                snake1[1].Clear(graphics);
                snake1[1].ScanPoint(graphics);

                snake1[snake1.Count - 1].Type = Type.Tail;
                snake1[snake1.Count - 1].Clear(graphics);
                snake1[snake1.Count - 1].ScanPoint(graphics);
            }
            snake1[0].ScanPoint(graphics);
            if (eatFood)
            {
                if (player == 1)
                {
                    if (Win())
                    {
                        timer1.Stop(); MessageBox.Show("恭喜你获得胜利！！"); End(1);
                    }
                    score++;
                    level = score * score / (width * height) / (1 / (1 + Math.Exp(-1.0 * speed)));
                    label1.Text = "当前难度分：" + level.ToString("0.00") + "  当前得分：" + (score * 10).ToString() + "  ";
                }
                if (player > 1)
                    label1.Text = "S1长度：" + snake1.Count.ToString("0.00") + "  S2长度：" + snake2.Count.ToString("0.00") + "  ";

                Createfood(out food);
                if ((snake1.Count + snake2.Count) % 5 == 0)
                {
                    speed = Math.Max(1, (int)(0.98 * speed));
                    timer1.Interval = speed;
                }
            }
        }

        private bool Win()
        {
            return snake1.Count == width * height;
        }

        private void Snake2MoveDir()
        {
            Point point;
            Block head = snake2[0];

            if (Ai2)
            {
                SnakeAi snakeAi = new SnakeAi(width, height, snake2, oldsnake1, food);
                new_direction2 = snakeAi.Getdirection();
            }

            if (snake2.Count != 1 && Math.Abs(old_direction2 - new_direction2) != 2)
                old_direction2 = new_direction2;
            else if (snake2.Count == 1)
                old_direction2 = new_direction2;

            if (old_direction2 == Direction.Up) point = new Point(head.Point.X, head.Point.Y - 1);
            else if (old_direction2 == Direction.Down) point = new Point(head.Point.X, head.Point.Y + 1);
            else if (old_direction2 == Direction.Left) point = new Point(head.Point.X - 1, head.Point.Y);
            else point = new Point(head.Point.X + 1, head.Point.Y);

            Block block = new Block(size, point, Type.Head, old_direction2);

            bool eatFood = food.Point.X == point.X && food.Point.Y == point.Y;

            if (!eatFood)
            {
                snake2[snake2.Count - 1].Clear(graphics);
                snake2.RemoveAt(snake2.Count - 1);
            }
            snake2.Insert(0, block);
            if (snake2.Count == 2)
            {
                snake2[1].New_direction = old_direction2;
                snake2[1].Type = Type.Tail;
                snake2[1].Clear(graphics);
                snake2[1].ScanPoint(graphics);
            }
            else if (snake2.Count > 2)
            {
                snake2[1].New_direction = old_direction2;
                snake2[1].Type = Type.Body;
                snake2[1].Clear(graphics);
                snake2[1].ScanPoint(graphics);

                snake2[snake2.Count - 1].Type = Type.Tail;
                snake2[snake2.Count - 1].Clear(graphics);
                snake2[snake2.Count - 1].ScanPoint(graphics);
            }
            snake2[0].ScanPoint(graphics);
            if (eatFood)
            {
                Createfood(out food);
                if ((snake1.Count + snake2.Count) % 5 == 0)
                {
                    speed = Math.Max(1, (int)(0.98 * speed));
                    timer1.Interval = speed;
                }
                label1.Text = "S1长度：" + snake1.Count.ToString("0.00") + "  S2长度：" + snake2.Count.ToString("0.00") + "  ";
            }
        }

        private bool Snake1Dead()
        {
            if (snake1.Count == 0) return false;
            Block head = snake1[0];
            bool ret = false;
            if (head.Point.X < 0 || head.Point.X >= width || head.Point.Y < 0 || head.Point.Y >= height)
            {
                ret = true;
            }
            else
            {
                for (int i = 1; i < snake1.Count; i++)
                    if (head.Point.X == snake1[i].Point.X && head.Point.Y == snake1[i].Point.Y)
                        ret = true;
                for (int i = 0; i < snake2.Count; i++)
                    if (head.Point.X == snake2[i].Point.X && head.Point.Y == snake2[i].Point.Y)
                        ret = true;
            }
            return ret;
        }

        private bool Snake2Dead()
        {
            if (snake2.Count == 0) return false;
            Block head = snake2[0];
            bool ret = false;
            if (head.Point.X < 0 || head.Point.X >= width || head.Point.Y < 0 || head.Point.Y >= height)
            {
                ret = true;
            }
            else
            {
                for (int i = 1; i < snake2.Count; i++)
                    if (head.Point.X == snake2[i].Point.X && head.Point.Y == snake2[i].Point.Y)
                        ret = true;
                for (int i = 0; i < snake1.Count; i++)
                    if (head.Point.X == snake1[i].Point.X && head.Point.Y == snake1[i].Point.Y)
                        ret = true;
            }
            return ret;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            bool dead1 = false, dead2 = false;
            Random random = new Random();
            oldsnake1 = snake1;
            Snake1MoveDir();
            if (Snake1Dead())
            {
                timer1.Stop(); dead1 = true;
            }
            if (player > 1)
            {

                Snake2MoveDir();
                if (Snake2Dead())
                {
                    timer1.Stop(); dead2 = true;
                }
            }
            if (dead1 && dead2) End(0);
            else if (dead1 && !dead2) End(1);
            else if (!dead1 && dead2) End(2);
        }

        private void RePainting()
        {
            graphics.Clear(Color.White);
            food.ScanPoint(graphics);
            foreach (Block block in snake1)
                block.ScanPoint(graphics);
            foreach (Block block in snake2)
                block.ScanPoint(graphics);
        }


        private void 初级ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Changelevel(20, 15, 160);
            初级ToolStripMenuItem.Checked = true;
            中级ToolStripMenuItem.Checked = false;
            高级ToolStripMenuItem.Checked = false;
            自定义ToolStripMenuItem.Checked = false;
        }

        private void 中级ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Changelevel(35, 25, 100);
            初级ToolStripMenuItem.Checked = false;
            中级ToolStripMenuItem.Checked = true;
            高级ToolStripMenuItem.Checked = false;
            自定义ToolStripMenuItem.Checked = false;
        }

        private void 高级ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Changelevel(35, 25, 60);
            初级ToolStripMenuItem.Checked = false;
            中级ToolStripMenuItem.Checked = false;
            高级ToolStripMenuItem.Checked = true;
            自定义ToolStripMenuItem.Checked = false;
        }

        private void 自定义ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            other other = new other();
            other.setformValue += new setValue(other_setValue);
            other.ShowDialog();
            初级ToolStripMenuItem.Checked = false;
            中级ToolStripMenuItem.Checked = false;
            高级ToolStripMenuItem.Checked = false;
            自定义ToolStripMenuItem.Checked = true;
        }

        private void 排行榜ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Score score = new Score();
            score.ShowDialog();
        }

        public void Changelevel(int wid, int hig, int spd)
        {
            width = wid;
            height = hig;
            speed = spd;
            pictureBox1.Width = width * size;
            pictureBox1.Height = height * size;
            Width = pictureBox1.Width + 40;
            Height = pictureBox1.Height + 86;
            timer1.Interval = speed;
            snake1.Clear();
            snake2.Clear();
            try { pictureBox1.Refresh(); }
            catch { }
        }

        private void other_setValue(int wid, int hig, int spd)
        {
            Changelevel(wid, hig, spd);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            单人游戏ToolStripMenuItem_Click(null, null);
            初级ToolStripMenuItem_Click(null, null);
            //初级ToolStripMenuItem.Checked = true;
            //Changelevel(20, 15, 160);
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.ShowDialog();
        }

        private void 单人游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            player = 1;
            Ai1 = false;
            Ai2 = false;
            单人游戏ToolStripMenuItem.Checked = true;
            单人AI游戏ToolStripMenuItem.Checked = false;
            双人游戏ToolStripMenuItem.Checked = false;
            对战AI游戏ToolStripMenuItem.Checked = false;
            aI对战游戏ToolStripMenuItem.Checked = false;
            timer1.Stop();
            pause = false;
        }

        private void 单人AI游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            player = 1;
            Ai1 = true;
            Ai2 = false;
            单人游戏ToolStripMenuItem.Checked = false;
            单人AI游戏ToolStripMenuItem.Checked = true;
            双人游戏ToolStripMenuItem.Checked = false;
            对战AI游戏ToolStripMenuItem.Checked = false;
            aI对战游戏ToolStripMenuItem.Checked = false;
            timer1.Stop();
            pause = false;
        }

        private void 双人游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            player = 2;
            Ai1 = false;
            Ai2 = false;
            单人游戏ToolStripMenuItem.Checked = false;
            单人AI游戏ToolStripMenuItem.Checked = false;
            双人游戏ToolStripMenuItem.Checked = true;
            对战AI游戏ToolStripMenuItem.Checked = false;
            aI对战游戏ToolStripMenuItem.Checked = false;
            timer1.Stop();
            pause = false;
        }

        private void 对战AI游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            player = 2;
            Ai1 = false;
            Ai2 = true;
            单人游戏ToolStripMenuItem.Checked = false;
            单人AI游戏ToolStripMenuItem.Checked = false;
            双人游戏ToolStripMenuItem.Checked = false;
            对战AI游戏ToolStripMenuItem.Checked = true;
            aI对战游戏ToolStripMenuItem.Checked = false;
            timer1.Stop();
            pause = false;
        }

        private void aI对战游戏ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            player = 2;
            Ai1 = true;
            Ai2 = true;
            单人游戏ToolStripMenuItem.Checked = false;
            单人AI游戏ToolStripMenuItem.Checked = false;
            双人游戏ToolStripMenuItem.Checked = false;
            对战AI游戏ToolStripMenuItem.Checked = false;
            aI对战游戏ToolStripMenuItem.Checked = true;
            timer1.Stop();
            pause = false;
        }
    }
}
