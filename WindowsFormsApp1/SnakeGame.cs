using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameCollection.Forms
{
    public class Snake1Form : Form
    {
        private const int CellSize = 30;
        private const int Width = 600;
        private const int Height = 600;
        private const int GameSpeed = 150;

        private List<Point> snake;
        private Point food;
        private Point direction;
        private bool isGameRunning;
        private Timer gameTimer;
        private Random random;
        private int score;

        public Snake1Form()
        {
            InitializeForm();
            InitializeGame();
        }

        private void InitializeForm()
        {
            this.Text = "Snake Game";
            this.ClientSize = new Size(Width, Height);
            this.DoubleBuffered = true;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.BackColor = Color.Black;
        }

        private void InitializeGame()
        {
            snake = new List<Point> { new Point(10, 10) };
            direction = new Point(1, 0);
            random = new Random();
            score = 0;
            isGameRunning = true;

            SpawnFood();

            gameTimer = new Timer();
            gameTimer.Interval = GameSpeed;
            gameTimer.Tick += GameUpdate;
            gameTimer.Start();

            this.KeyDown += HandleInput;
            this.Paint += Render;
            this.FormClosing += (s, e) => Cleanup();
        }

        private void Cleanup()
        {
            if (gameTimer != null)
            {
                gameTimer.Stop();
                gameTimer.Dispose();
            }
        }

        private void GameUpdate(object sender, EventArgs e)
        {
            if (!isGameRunning) return;

            MoveSnake();
            CheckCollision();
            this.Invalidate();
        }

        private void MoveSnake()
        {
            Point head = snake[0];
            Point newHead = new Point(head.X + direction.X, head.Y + direction.Y);
            snake.Insert(0, newHead);

            if (newHead == food)
            {
                score += 10;
                SpawnFood();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }
        }

        private void CheckCollision()
        {
            Point head = snake[0];

            if (head.X < 0 || head.Y < 0 ||
                head.X >= Width / CellSize || head.Y >= Height / CellSize ||
                snake.IndexOf(head, 1) != -1)
            {
                GameOver();
            }
        }

        private void GameOver()
        {
            isGameRunning = false;
            gameTimer.Stop();
            MessageBox.Show($"Игра закончена!Ваш счет: {score}");
            Cleanup();
            this.Close();
        }

        private void SpawnFood()
        {
            do
            {
                food = new Point(
                    random.Next(0, Width / CellSize),
                    random.Next(0, Height / CellSize));
            } while (snake.Contains(food));
        }

        private void HandleInput(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up when direction.Y != 1:
                    direction = new Point(0, -1);
                    break;
                case Keys.Down when direction.Y != -1:
                    direction = new Point(0, 1);
                    break;
                case Keys.Left when direction.X != 1:
                    direction = new Point(-1, 0);
                    break;
                case Keys.Right when direction.X != -1:
                    direction = new Point(1, 0);
                    break;
            }
        }

        private void Render(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            foreach (Point segment in snake)
            {
                g.FillRectangle(Brushes.Lime,
                    segment.X * CellSize,
                    segment.Y * CellSize,
                    CellSize, CellSize);
            }

            g.FillEllipse(Brushes.Red,
                food.X * CellSize,
                food.Y * CellSize,
                CellSize, CellSize);

            g.DrawString($"Score: {score}",
                new Font("Arial", 14),
                Brushes.White,
                new PointF(10, 10));
        }
    }
}