using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameCollection.Forms
{
    public class Match3Form : Form
    {
        private const int GridSize = 8;
        private const int TileSize = 80;
        private int[,] grid = new int[GridSize, GridSize];
        private Point? selectedTile = null;
        private Random random = new Random();
        private int score = 0;
        private Bitmap desertBackground;

        private Color[] gemColors = new Color[]
        {
            Color.Red,
            Color.Gold,
            Color.SkyBlue,
            Color.Green,
            Color.Purple
        };

        public Match3Form()
        {
            InitializeComponent();
            SetupForm();
            LoadBackground();
            InitializeGrid();
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(300, 600);
            this.Name = "Form1";
            this.ResumeLayout(false);
        }

        private void SetupForm()
        {
            this.ClientSize = new Size(GridSize * TileSize, GridSize * TileSize + 40);
            this.Text = "Три в ряд: Пустынные сокровища";
            this.DoubleBuffered = true;
            this.BackColor = Color.SandyBrown;

            var scoreLabel = new Label
            {
                Text = "Счет: 0",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.DarkRed,
                Location = new Point(10, GridSize * TileSize + 5),
                AutoSize = true
            };
            this.Controls.Add(scoreLabel);
        }

        private void LoadBackground()
        {
            try
            {
                desertBackground = new Bitmap("desert_bg.jpg");
            }
            catch
            {
                desertBackground = new Bitmap(ClientSize.Width, ClientSize.Height);
                using (Graphics g = Graphics.FromImage(desertBackground))
                {
                    using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                        new Point(0, 0),
                        new Point(0, desertBackground.Height),
                        Color.LightGoldenrodYellow,
                        Color.SandyBrown))
                    {
                        g.FillRectangle(brush, 0, 0, desertBackground.Width, desertBackground.Height);
                    }

                    using (var pen = new Pen(Color.DarkGoldenrod, 3))
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            int x = random.Next(desertBackground.Width);
                            int y = random.Next(desertBackground.Height);
                            int size = random.Next(10, 30);
                            g.DrawEllipse(pen, x, y, size, size / 3);
                        }
                    }
                }
            }
        }

        private void InitializeGrid()
        {
            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    grid[i, j] = random.Next(gemColors.Length);
                }
            }
            RemoveMatches();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            g.DrawImage(desertBackground, 0, 0, ClientSize.Width, GridSize * TileSize);

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize; j++)
                {
                    Rectangle rect = new Rectangle(
                        j * TileSize,
                        i * TileSize,
                        TileSize - 2,
                        TileSize - 2);

                    if (selectedTile.HasValue && selectedTile.Value.X == j && selectedTile.Value.Y == i)
                    {
                        using (var brush = new SolidBrush(Color.FromArgb(100, Color.White)))
                        {
                            g.FillRectangle(brush, rect);
                        }
                    }

                    using (var brush = new SolidBrush(gemColors[grid[i, j]]))
                    {
                        g.FillEllipse(brush, rect);
                        g.DrawEllipse(Pens.Black, rect);
                    }
                }
            }
        }

        protected override void OnMouseClick(MouseEventArgs e)
        {
            base.OnMouseClick(e);

            int x = e.X / TileSize;
            int y = e.Y / TileSize;

            if (x >= 0 && x < GridSize && y >= 0 && y < GridSize)
            {
                if (!selectedTile.HasValue)
                {
                    selectedTile = new Point(x, y);
                }
                else
                {
                    Point prev = selectedTile.Value;

                    if ((Math.Abs(prev.X - x) == 1 && prev.Y == y) ||
                        (Math.Abs(prev.Y - y) == 1 && prev.X == x))
                    {
                        int temp = grid[y, x];
                        grid[y, x] = grid[prev.Y, prev.X];
                        grid[prev.Y, prev.X] = temp;

                        if (CheckMatches())
                        {
                            RemoveMatches();
                            score += 10;
                            this.Controls[0].Text = "Счет: " + score;
                        }
                        else
                        {
                            temp = grid[y, x];
                            grid[y, x] = grid[prev.Y, prev.X];
                            grid[prev.Y, prev.X] = temp;
                        }
                    }

                    selectedTile = null;
                }

                this.Invalidate();
            }
        }

        private bool CheckMatches()
        {
            bool matchesFound = false;

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize - 2; j++)
                {
                    if (grid[i, j] == grid[i, j + 1] && grid[i, j] == grid[i, j + 2])
                    {
                        matchesFound = true;
                    }
                }
            }

            for (int j = 0; j < GridSize; j++)
            {
                for (int i = 0; i < GridSize - 2; i++)
                {
                    if (grid[i, j] == grid[i + 1, j] && grid[i, j] == grid[i + 2, j])
                    {
                        matchesFound = true;
                    }
                }
            }

            return matchesFound;
        }

        private void RemoveMatches()
        {
            bool[,] toRemove = new bool[GridSize, GridSize];

            for (int i = 0; i < GridSize; i++)
            {
                for (int j = 0; j < GridSize - 2; j++)
                {
                    if (grid[i, j] == grid[i, j + 1] && grid[i, j] == grid[i, j + 2])
                    {
                        toRemove[i, j] = true;
                        toRemove[i, j + 1] = true;
                        toRemove[i, j + 2] = true;
                    }
                }
            }

            for (int j = 0; j < GridSize; j++)
            {
                for (int i = 0; i < GridSize - 2; i++)
                {
                    if (grid[i, j] == grid[i + 1, j] && grid[i, j] == grid[i + 2, j])
                    {
                        toRemove[i, j] = true;
                        toRemove[i + 1, j] = true;
                        toRemove[i + 2, j] = true;
                    }
                }
            }

            for (int j = 0; j < GridSize; j++)
            {
                int emptyPos = GridSize - 1;

                for (int i = GridSize - 1; i >= 0; i--)
                {
                    if (toRemove[i, j])
                    {
                        for (int k = i; k > 0; k--)
                        {
                            grid[k, j] = grid[k - 1, j];
                        }
                        grid[0, j] = random.Next(gemColors.Length);
                    }
                }
            }

            if (CheckMatches())
            {
                RemoveMatches();
            }
        }
    }
}
