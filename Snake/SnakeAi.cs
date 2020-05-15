using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Snake
{
    class SnakeAi
    {
        private int width;
        private int height;
        private List<Block> snakeQue = new List<Block>();
        private List<Block> othersnakeQue = new List<Block>();
        private Block food;
        private Block head;
        int[,] dir = { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, 1 } };

        public SnakeAi(int width, int height, List<Block> snakeQue, List<Block> othersnakeQue, Block food)
        {
            this.width = width;
            this.height = height;
            //this.snakeQue = snakeQue;
            //this.othersnakeQue = othersnakeQue;
            foreach (Block item in snakeQue)
            {
                this.snakeQue.Add(new Block(new Point(item.Point.X, item.Point.Y)));
            }
            foreach (Block item in othersnakeQue)
            {
                this.othersnakeQue.Add(new Block(new Point(item.Point.X, item.Point.Y)));
            }
            this.food = food;
            this.head = snakeQue[0];
        }

        private int[,] RandomSort(int[,] sort)
        {
            int[,] ret = sort;
            Random rd = new Random();
            for (int i = sort.GetLength(0) - 1; i > 0; i--)
            {
                int rand = rd.Next(i + 1);
                int temx = ret[i, 0];
                int temy = ret[i, 1];
                ret[i, 0] = ret[rand, 0];
                ret[i, 1] = ret[rand, 1];
                ret[rand, 0] = temx;
                ret[rand, 1] = temy;
            }
            return ret;
        }

        public Direction Getdirection()
        {
            Point point = Movebody(snakeQue, Strategy(), false);
            if (head.Point.X - point.X == -1) return Direction.Right;
            else if (head.Point.X - point.X == 1) return Direction.Left;
            else if (head.Point.Y - point.Y == -1) return Direction.Down;
            else return Direction.Up;
        }

        private Block Strategy()
        {
            List<Block> FindFood = null;
            List<Block> FindTail = null;
            Block ret = null;
            FindFood = BFS(snakeQue, head, food);    //蛇头找食物
            if (FindFood != null)   //若能找到食物
            {
                FindTail = Movesnake(FindFood); //找到食物后能否回去找到尾巴
                if (FindTail != null) ret = FindFood[FindFood.Count - 1];   //若找得到尾巴
            }
            bool foodflag = true, tailflag = true;  //标记能找到食物和尾巴
            if (FindFood == null || FindFood.Count == 0) foodflag = false;  //标记找不到食物
            if (FindTail == null || FindTail.Count == 0) tailflag = false;  //标记找到食物后找不到尾巴
            if (foodflag == false || tailflag == false || ret == null)
            {
                int t = 1;
                while (ret == null)
                {
                    ret = Faraway(t++);
                }
            } //都不可则追现在的尾巴，且走最远的距离
            return ret;
        }

        private Point Movebody(List<Block> snake, Block step, bool istest)
        {
            while (step == null) step = Strategy();
            if (istest == false) return step.Point;
            //若为真蛇，则直接返回下一步的移动的坐标
            //若为测试，则若吃到食物，就将蛇长延长
            int len = snake.Count - 1;
            if (step.Point.X == food.Point.X && step.Point.Y == food.Point.Y)
            {
                len++;
                Block eat = new Block(new Point(food.Point.X, food.Point.Y));
                snake.Add(eat);
            }
            for (int i = len; i > 0; i--)
            {
                snake[i].Point = snake[i - 1].Point;
            }
            snake[0].Point = step.Point;
            return new Point(); //因为测试的蛇不需要返回值，所以返回空值
        }


        private List<Block> Movesnake(List<Block> road)
        {
            List<Block> testSnake = new List<Block>();  //创建测试蛇
            foreach (Block item in snakeQue)
            {
                Block nitem = new Block(new Point(item.Point.X, item.Point.Y));
                testSnake.Add(nitem);
            }

            for (int i = road.Count - 1; i >= 0; i--)
            {
                Movebody(testSnake, road[i], true);
            }

            List<Block> isFindTail = BFS(testSnake, testSnake[0], testSnake[testSnake.Count - 1]);
            return isFindTail;
        }


        private Block Faraway(int count = 1)
        {
            if (count >= snakeQue.Count) { return new Block(new Point(1, 0)); }

            Block[] director = new Block[4];
            double[] distance = new double[4];
            int nx = head.Point.X;
            int ny = head.Point.Y;
            int fx = snakeQue[snakeQue.Count - count].Point.X;
            int fy = snakeQue[snakeQue.Count - count].Point.Y;

            for (int i = 0; i <= 3; i++)
            {
                distance[i] = Math.Abs(nx + dir[i, 0] - fx) + Math.Abs(ny + dir[i, 1] - fy);
                Block nnode = new Block(new Point(nx + dir[i, 0], ny + dir[i, 1]));
                if ((!snakeQue.Contains(nnode) || (nnode.Point.X == snakeQue[snakeQue.Count - 1].Point.X && nnode.Point.Y == snakeQue[snakeQue.Count - 1].Point.Y)) && nnode.Point.X >= 0 && nnode.Point.X < width && nnode.Point.Y >= 0 && nnode.Point.Y < height && !othersnakeQue.Contains(nnode))  //1.蛇身不包含该点 2.该点是目标点 ->满足其一则可行走 且在范围内 不在另一条蛇身上
                {
                    director[i] = nnode;
                }
            }
            for (int i = 0; i < distance.Length - 1; i++)   //冒泡排序，以距离食物的距离为排序标准，从大到小
                for (int j = 0; j < distance.Length - 1 - i; j++)
                    if (distance[j] < distance[j + 1])
                    {
                        double temp = distance[j];
                        Block tem = director[j];
                        distance[j] = distance[j + 1];
                        director[j] = director[j + 1];
                        distance[j + 1] = temp;
                        director[j + 1] = tem;
                    }


            for (int i = 0; i <= 3; i++)
                if (director[i] != null)    //根据前面的条件
                {
                    List<Block> testSnake2 = new List<Block>(); //生成一条探路的蛇，copy原蛇
                    foreach (Block item in snakeQue)
                    {
                        Block nnode = new Block(new Point(item.Point.X, item.Point.Y));
                        testSnake2.Add(nnode);
                    }
                    Movebody(testSnake2, director[i], true);    //把蛇向该方向移动一位
                    List<Block> farway = BFS(testSnake2, director[i], testSnake2[testSnake2.Count - Math.Max(count - 1, 1)]);    //测试移动后后还能否找到尾巴的位置
                    if (farway != null) //能找到说明该点可走
                        return director[i];

                }
            return null;
        }

        private List<Block> BFS(List<Block> snake, Block s, Block e)
        {
            try
            {
                dir = RandomSort(dir);
                int[,] now = new int[width, height];
                foreach (Block item in snake)
                {
                    now[item.Point.X, item.Point.Y] = 1;
                }
                now[snake[snake.Count - 1].Point.X, snake[snake.Count - 1].Point.Y] = 0;
                foreach (Block item in othersnakeQue)
                {
                    now[item.Point.X, item.Point.Y] = 1;
                }
                //建立蛇的二维数组，尾巴不需要设为墙壁，因为跟着尾巴走死不了

                int f = 1;
                int t = 1;
                Block[] serch = new Block[width * height + 5];
                int[] preindex = new int[width * height + 5];
                List<Block> backway = new List<Block>();
                serch[t] = s;   //将头加入已搜索列表
                t++;
                if (s.Point.X == e.Point.X && s.Point.Y == e.Point.Y)   //头和终点重合，即可以走到
                {
                    backway.Add(e);
                    return backway;
                }
                do
                {
                    for (int i = 0; i <= 3; i++)
                    {
                        int nx = serch[f].Point.X + dir[i, 0];
                        int ny = serch[f].Point.Y + dir[i, 1];
                        if (nx < 0 || nx >= width || ny < 0 || ny >= height)    //分别向上下左右四个方向走
                            continue;
                        if (now[nx, ny] != 1 || (nx == e.Point.X && ny == e.Point.Y))   //若该方向非蛇身或墙壁或到达终点
                        {
                            Block n = new Block(new Point(nx, ny)); //将该点加入可行走列表
                            now[nx, ny] = 1;    //则该点设为已经走到的点
                            serch[t] = n;   //将该点Block类加入已搜索列表中
                            preindex[t] = f;    //记住该点是第几步
                            if (n.Point.X == e.Point.X && n.Point.Y == e.Point.Y)   //若走到了终点
                            {
                                while (preindex[t] != 0)    //根据行走了的步数反推到起点，加入backway列表中
                                {
                                    backway.Add(serch[t]);
                                    t = preindex[t];
                                }
                                return backway;
                            }
                            t++;
                        }
                    }
                    f++;
                } while (f < t);    //整个列表搜索完也找不到路径则返回null
                return null;
            }
            catch { return null; }
        }
    }
}
