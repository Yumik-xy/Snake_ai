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
        private List<Block> snakeQue;
        private Block food;
        private Block head;
        int[,] dir = { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 }, { 0, -1 }, { 1, 0 } };
        
        public SnakeAi(int width, int height, List<Block> snake, Block food)
        {
            this.width = width;
            this.height = height;
            this.snakeQue = snake;
            this.food = food;
            this.head = snake[0];
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
            FindFood = BFS(snakeQue, snakeQue[0], food);
            if (FindFood != null)
            {
                FindTail = Movesnake(FindFood);
                if (FindTail != null) ret = FindFood[FindFood.Count - 1];
            }
            bool foodflag = true, tailflag = true;
            if (FindFood == null || FindFood.Count == 0) foodflag = false;
            if (FindTail == null || FindTail.Count == 0) tailflag = false;
            if (foodflag == false || tailflag == false || ret == null) ret = Faraway();
            return ret;
        }

        private Point Movebody(List<Block> snake, Block step, bool istest)
        {
            while (step == null) step = Strategy();
            if (istest == false)
            {
                Console.WriteLine("");
                return step.Point;
            }

            int len = snake.Count;
            len = len - 1;
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
            return new Point();
        }


        private List<Block> Movesnake(List<Block> road)
        {        
            List<Block> testSnake = new List<Block>();
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


        private Block Faraway()
        {
            Block[] director = new Block[4];
            double[] distance = new double[4];
            int nx = snakeQue[0].Point.X;
            int ny = snakeQue[0].Point.Y;
            int fx = food.Point.X;
            int fy = food.Point.Y;
            for (int i = 0; i <= 3; i++)
            {
                distance[i] = Math.Abs(nx + dir[i, 0] - fx) + Math.Abs(ny + dir[i, 1] - fy);
                Block nnode = new Block(new Point(nx + dir[i, 0], ny + dir[i, 1]));
                if (!snakeQue.Contains(nnode) || (nnode.Point.X == snakeQue[snakeQue.Count - 1].Point.X && nnode.Point.Y == snakeQue[snakeQue.Count - 1].Point.Y))
                {
                    if (nnode.Point.X >= 0 && nnode.Point.X < width && nnode.Point.Y >= 0 && nnode.Point.Y < height)
                    {
                        director[i] = nnode;
                    }


                }
            }
            for (int i = 0; i < distance.Length - 1; i++)
            {
                for (int j = 0; j < distance.Length - 1 - i; j++)
                {
                    if (distance[j] < distance[j + 1])
                    {
                        double temp = distance[j];
                        Block tem = director[j];
                        distance[j] = distance[j + 1];
                        director[j] = director[j + 1];
                        distance[j + 1] = temp;
                        director[j + 1] = tem;
                    }
                }
            }

            for (int i = 0; i <= 3; i++)
            {
                if (director[i] != null)
                {
                    List<Block> testSnake2 = new List<Block>();
                    foreach (Block item in snakeQue)
                    {
                        Block nnode = new Block(new Point(item.Point.X, item.Point.Y));
                        testSnake2.Add(nnode);
                    }
                    Movebody(testSnake2, director[i], true);
                    List<Block> farway = BFS(testSnake2, director[i], snakeQue[snakeQue.Count - 1]);
                    if (farway != null)
                    {
                        return director[i];
                    }
                }

            }
            return null;
        }

        private List<Block> BFS(List<Block> snake, Block s, Block e)
        {
            Random rd = new Random();
            int st = rd.Next(0, 4);
            int[,] now = new int[width, height];
            foreach (Block item in snake)
            {
                now[item.Point.X, item.Point.Y] = 1;
            }
            now[snake[snake.Count - 1].Point.X, snake[snake.Count - 1].Point.Y] = 0;

            int f = 1;
            int t = 1;
            Block[] serch = new Block[width * height + 5];
            int[] preindex = new int[width * height + 5];
            List<Block> backway = new List<Block>();
            serch[t] = s;
            t++;
            if (s.Point.X == e.Point.X && s.Point.Y == e.Point.Y)
            {
                backway.Add(e);
                return backway;
            }
            do
            {
                for (int i = 0 + st; i <= 3 + st; i++)
                {
                    int nx = serch[f].Point.X + dir[i, 0];
                    int ny = serch[f].Point.Y + dir[i, 1];
                    if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                        continue;
                    if (now[nx, ny] != 1 || (nx == e.Point.X && ny == e.Point.Y))
                    {
                        Block n = new Block(new Point(nx, ny));
                        now[nx, ny] = 1;
                        serch[t] = n;
                        preindex[t] = f;
                        if (n.Point.X == e.Point.X && n.Point.Y == e.Point.Y)
                        {
                            while (preindex[t] != 0)
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
            } while (f < t);
            return null;
        }
    }
}
