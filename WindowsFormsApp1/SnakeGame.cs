using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace GameCollection.Forms
{
    public class Snake1Form : Form
    {
        private const int GridSize = 20;
        private const int Width = 600;
        private const int Height = 600;
        private const int TimerInterval = 100; // СКОРОСТЬ
        private readonly Brush BlackBrush = Brushes.Black;
        private readonly Brush GreenBrush = Brushes.Green;
        private readonly Brush RedBrush = Brushes.Red;
        private readonly Brush WhiteBrush = Brushes.White;
        private List<Point> snake;
        private Point food;
        private Point direction;
        private bool isGameOver;
        private Random random;
        private Timer gameTimer;
        private Button menuButton;
        private Label scoreLabel;
        private int score;

        public Snake1Form()
        {
            this.Text = "Змейка";
            this.ClientSize = new Size(Width, Height);
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(30, 30, 40);
            this.KeyDown += HandleKeyPress;
            this.KeyPreview = true;
            random = new Random();
            InitializeGame();

            menuButton = new Button
            {
                Text = "Меню",
                Font = new Font("Arial", 10),
                Size = new Size(80, 30),
                Location = new Point(10, 10),
                BackColor = Color.LightGray,
                ForeColor = Color.Black
            };
            menuButton.Click += (sender, e) => this.Close();
            this.Controls.Add(menuButton);
            gameTimer = new Timer();
            gameTimer.Interval = TimerInterval;
            gameTimer.Tick += UpdateGame;
            gameTimer.Start();
        }

        private void InitializeGame()
        {
            snake = new List<Point>
            {
                new Point(Width / 2 / GridSize, Height / 2 / GridSize) 
            };
            direction = new Point(1, 0); 
            isGameOver = false;
            score = 0;
            SpawnFood();
            UpdateScoreLabel();
        }

        private void UpdateScoreLabel()
        {
            if (scoreLabel == null)
            {
                scoreLabel = new Label
                {
                    Font = new Font("Arial", 14),
                    ForeColor = Color.White,
                    AutoSize = true,
                    Location = new Point(Width - 150, 10)
                };
                this.Controls.Add(scoreLabel);
            }
            scoreLabel.Text = $"Счёт: {score}";
        }

        private void SpawnFood()
        {
            int maxX = Width / GridSize;
            int maxY = Height / GridSize;
            do
            {
                food = new Point(random.Next(0, maxX), random.Next(0, maxY));
            } while (snake.Contains(food)); 
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            if (isGameOver) return;

            Point head = snake[0];
            Point newHead = new Point(head.X + direction.X, head.Y + direction.Y);

            if (newHead.X < 0 || newHead.Y < 0 || newHead.X >= Width / GridSize || newHead.Y >= Height / GridSize)
            {
                GameOver();
                return;
            }

            if (snake.Contains(newHead))
            {
                GameOver();
                return;
            }

            snake.Insert(0, newHead);
            if (newHead == food)
            {
                score += 10;
                UpdateScoreLabel();
                SpawnFood();
            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }

            this.Invalidate(); 
        }

        private void GameOver()
        {
            isGameOver = true;
            gameTimer.Stop();
            MessageBox.Show($"Игра окончена! Ваш счёт: {score}", "Змейка", MessageBoxButtons.OK);
            InitializeGame();
            gameTimer.Start();
        }

        private void HandleKeyPress(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (direction.Y != 1) direction = new Point(0, -1);
                    break;
                case Keys.Down:
                    if (direction.Y != -1) direction = new Point(0, 1);
                    break;
                case Keys.Left:
                    if (direction.X != 1) direction = new Point(-1, 0);
                    break;
                case Keys.Right:
                    if (direction.X != -1) direction = new Point(1, 0);
                    break;
                case Keys.Escape:
                    this.Close();
                    break;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            foreach (var segment in snake)
            {
                g.FillRectangle(GreenBrush, segment.X * GridSize, segment.Y * GridSize, GridSize, GridSize);
                g.DrawRectangle(Pens.DarkGreen, segment.X * GridSize, segment.Y * GridSize, GridSize, GridSize);
            }

            g.FillEllipse(RedBrush, food.X * GridSize, food.Y * GridSize, GridSize, GridSize);
        }
    }
}