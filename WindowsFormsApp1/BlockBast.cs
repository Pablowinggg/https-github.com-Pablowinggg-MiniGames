using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SpaceDestroyer
{
    public partial class SpaceGame : Form
    {
        private const int GameWidth = 800;
        private const int GameHeight = 600;
        private const int PlayerSpeed = 8;
        private const int BulletSpeed = 15;
        private const int CometSpeed = 4;
        private const int CometSpawnRate = 60;

        private PlayerShip player;
        private List<Bullet> bullets;
        private List<Comet> comets;
        private Random random;
        private int gameTime;
        private int score;
        private bool isGameOver;

        private Bitmap playerSprite;
        private Bitmap cometSprite;
        private Bitmap bulletSprite;

        public SpaceGame()
        {
            InitializeComponent();
            InitializeGame();
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(300, 600);
            this.Name = "Form1";
            this.ResumeLayout(false);
        }

        private int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        private void InitializeGame()
        {
            this.ClientSize = new Size(GameWidth, GameHeight);
            this.DoubleBuffered = true;
            this.Text = "Space Comet Destroyer";
            this.BackColor = Color.Black;

            playerSprite = new Bitmap(50, 30);
            cometSprite = new Bitmap(40, 40);
            bulletSprite = new Bitmap(10, 5);

            using (Graphics g = Graphics.FromImage(playerSprite))
                g.FillPolygon(Brushes.Blue, new Point[] { new Point(0, 15), new Point(50, 0), new Point(50, 30) });
            using (Graphics g = Graphics.FromImage(cometSprite))
                g.FillEllipse(Brushes.Gray, 0, 0, 40, 40);
            using (Graphics g = Graphics.FromImage(bulletSprite))
                g.FillRectangle(Brushes.Red, 0, 0, 10, 5);

            player = new PlayerShip(GameWidth / 2, GameHeight - 100, playerSprite);
            bullets = new List<Bullet>();
            comets = new List<Comet>();
            random = new Random();
            gameTime = 0;
            score = 0;
            isGameOver = false;

            this.KeyDown += HandleInput;
            this.Paint += RenderGame;

            var gameTimer = new Timer();
            gameTimer.Interval = 16;
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
        }

        private void HandleInput(object sender, KeyEventArgs e)
        {
            if (isGameOver) return;

            switch (e.KeyCode)
            {
                case Keys.Left:
                    player.Move(-PlayerSpeed, 0);
                    break;
                case Keys.Right:
                    player.Move(PlayerSpeed, 0);
                    break;
                case Keys.Space:
                    bullets.Add(new Bullet(player.X + 45, player.Y + 12, bulletSprite));
                    break;
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (isGameOver) return;

            gameTime++;
            if (gameTime % CometSpawnRate == 0)
            {
                comets.Add(new Comet(random.Next(0, GameWidth - 40), -40, cometSprite));
            }

            foreach (var bullet in bullets.ToArray())
            {
                bullet.Move(0, -BulletSpeed);
                if (bullet.Y < 0) bullets.Remove(bullet);
            }

            foreach (var comet in comets.ToArray())
            {
                comet.Move(0, CometSpeed);
                if (comet.Y > GameHeight) comets.Remove(comet);
            }

            CheckCollisions();
            this.Invalidate();
        }

        private void CheckCollisions()
        {
            foreach (var bullet in bullets.ToArray())
            {
                foreach (var comet in comets.ToArray())
                {
                    if (bullet.Bounds.IntersectsWith(comet.Bounds))
                    {
                        bullets.Remove(bullet);
                        comets.Remove(comet);
                        score += 10;
                        break;
                    }
                }
            }

            foreach (var comet in comets)
            {
                if (player.Bounds.IntersectsWith(comet.Bounds))
                {
                    GameOver();
                    break;
                }
            }
        }

        private void GameOver()
        {
            isGameOver = true;
            MessageBox.Show($"Game Over! Your score: {score}", "Space Comet Destroyer");
            InitializeGame();
        }

        private void RenderGame(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            g.DrawImage(player.Sprite, player.X, player.Y);

            foreach (var bullet in bullets)
            {
                g.DrawImage(bullet.Sprite, bullet.X, bullet.Y);
            }

            foreach (var comet in comets)
            {
                g.DrawImage(comet.Sprite, comet.X, comet.Y);
            }

            g.DrawString($"Score: {score}", new Font("Arial", 16), Brushes.White, 10, 10);

            if (isGameOver)
            {
                g.DrawString("GAME OVER", new Font("Arial", 32), Brushes.Red, GameWidth / 2 - 100, GameHeight / 2 - 30);
            }
        }
    }

    class PlayerShip
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Bitmap Sprite { get; }
        public Rectangle Bounds => new Rectangle(X, Y, Sprite.Width, Sprite.Height);

        public PlayerShip(int x, int y, Bitmap sprite)
        {
            X = x;
            Y = y;
            Sprite = sprite;
        }

        public void Move(int dx, int dy)
        {
            X += dx;
            Y += dy;

            if (X < 0) X = 0;
            if (X > 800 - Sprite.Width) X = 800 - Sprite.Width;
            if (Y < 0) Y = 0;
            if (Y > 600 - Sprite.Height) Y = 600 - Sprite.Height;
        }
    }

    class Bullet
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Bitmap Sprite { get; }
        public Rectangle Bounds => new Rectangle(X, Y, Sprite.Width, Sprite.Height);

        public Bullet(int x, int y, Bitmap sprite)
        {
            X = x;
            Y = y;
            Sprite = sprite;
        }

        public void Move(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }
    }

    class Comet
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Bitmap Sprite { get; }
        public Rectangle Bounds => new Rectangle(X, Y, Sprite.Width, Sprite.Height);

        public Comet(int x, int y, Bitmap sprite)
        {
            X = x;
            Y = y;
            Sprite = sprite;
        }

        public void Move(int dx, int dy)
        {
            X += dx;
            Y += dy;
        }
    }
}