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
        private int score = 0;
        private double level = 0;
        private Graphics graphics;
        private List<Block> snake = new List<Block>();
        private Direction old_direction = Direction.Left;
        private Direction new_direction = Direction.Left;
        private const int size = 20;
        private bool Ai = false;
        private Block food;
        private bool pause = false;

        public Main()
        {
            InitializeComponent();
        }

        private void Main_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.Up) { new_direction = Direction.Up; return; }
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left) { new_direction = Direction.Left; return; }
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.Down) { new_direction = Direction.Down; return; }
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right) { new_direction = Direction.Right; return; }
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
            menuStrip1.Enabled = false;
            old_direction = new_direction = Direction.Left;
            score = 0; level = 0;
            label1.Text = "当前难度分：0" + "  当前得分：0" + "  ";
            graphics = Graphics.FromHwnd(pictureBox1.Handle);
            snake.Add(new Block(size, new Point(width / 2, height / 2), Type.Head, old_direction, 0));
            snake.Add(new Block(size, new Point(width / 2 + 1, height / 2), Type.Tail, 0, old_direction));
            Createfood(out food);
            RePainting();
            timer1.Start();
        }

        private void End()
        {
            menuStrip1.Enabled = true;
            snake.Clear();
            timer1.Enabled = false;
            pause = false;
            SaveRank saveRank = new SaveRank();
            saveRank.score = score * 10;
            saveRank.level = level.ToString("0.00");
            saveRank.ShowDialog();
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
                for (int i = 0; i < snake.Count; i++)
                    if (snake[i].Point.X == wid && snake[i].Point.Y == hig)
                        insnake = true;
                if (!insnake) break;
            }
            food = new Block(size, new Point(wid, hig), Type.Food);
            food.ScanPoint(graphics);
        }

        private void MoveDir()
        {
            Point point;
            Block head = snake[0];

            if (Ai)
            {
                SnakeAi snakeAi = new SnakeAi(width, height, snake, food);
                new_direction = snakeAi.Getdirection();
            }

            if (snake.Count != 1 && Math.Abs(old_direction - new_direction) != 2)
                old_direction = new_direction;
            else if (snake.Count == 1)
                old_direction = new_direction;

            if (old_direction == Direction.Up) point = new Point(head.Point.X, head.Point.Y - 1);
            else if (old_direction == Direction.Down) point = new Point(head.Point.X, head.Point.Y + 1);
            else if (old_direction == Direction.Left) point = new Point(head.Point.X - 1, head.Point.Y);
            else point = new Point(head.Point.X + 1, head.Point.Y);

            Block block = new Block(size, point, Type.Head, old_direction);

            bool eatFood = food.Point.X == point.X && food.Point.Y == point.Y;

            if (!eatFood)
            {
                snake[snake.Count - 1].Clear(graphics);
                snake.RemoveAt(snake.Count - 1);
            }
            snake.Insert(0, block);
            if (snake.Count == 2)
            {
                snake[1].New_direction = old_direction;
                snake[1].Type = Type.Tail;
                snake[1].Clear(graphics);
                snake[1].ScanPoint(graphics);
            }
            else if (snake.Count > 2)
            {
                snake[1].New_direction = old_direction;
                snake[1].Type = Type.Body;
                snake[1].Clear(graphics);
                snake[1].ScanPoint(graphics);

                snake[snake.Count - 1].Type = Type.Tail;
                snake[snake.Count - 1].Clear(graphics);
                snake[snake.Count - 1].ScanPoint(graphics);
            }
            snake[0].ScanPoint(graphics);
            if (eatFood)
            {
                if (Win())
                {
                    timer1.Stop(); MessageBox.Show("恭喜你获得胜利！！"); End();
                }
                Createfood(out food);
                score++;
                level = score * score / (width * height) / (1 / (1 + Math.Exp(-1.0 * speed)));
                if (score > 0 && score % 5 == 0)
                {
                    speed = Math.Max(1, (int)(0.98 * speed));
                    timer1.Interval = speed;
                }
                label1.Text = "当前难度分：" + level.ToString("0.00") + "  当前得分：" + (score * 10).ToString() + "  ";
            }
        }

        private bool Dead()
        {
            if (snake.Count == 0) return false;
            Block head = snake[0];
            bool ret = false;
            if (head.Point.X < 0 || head.Point.X >= width || head.Point.Y < 0 || head.Point.Y >= height)
            {
                ret = true;
            }
            else
            {
                for (int i = 1; i < snake.Count; i++)
                    if (head.Point.X == snake[i].Point.X && head.Point.Y == snake[i].Point.Y)
                        ret = true;
            }
            return ret;
        }

        private bool Win()
        {
            return snake.Count == width * height;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            MoveDir();
            if (Dead())
            {
                timer1.Stop(); End();
            }
        }

        private void RePainting()
        {
            graphics.Clear(Color.White);
            food.ScanPoint(graphics);
            foreach (Block block in snake)
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
            snake.Clear();
            try { pictureBox1.Refresh(); }
            catch { }
        }

        private void other_setValue(int wid, int hig, int spd)
        {
            Changelevel(wid, hig, spd);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            初级ToolStripMenuItem.Checked = true;
            Changelevel(20, 15, 160);
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help help = new Help();
            help.ShowDialog();
        }

        private void snakeAiToolStripMenuItem_Click(object sender, EventArgs e)
        {
            snakeAiToolStripMenuItem.Checked = !snakeAiToolStripMenuItem.Checked;
            Ai = snakeAiToolStripMenuItem.Checked;
        }
    }
}
