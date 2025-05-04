using System;
using System.Drawing;
using System.Windows.Forms;

namespace TetrisGame
{
    public partial class TetrisForm : Form
    {
        private Timer gameTimer;
        private Timer clockTimer;
        private int[,] grid = new int[20, 10];
        private int[,] currentPiece;
        private int currentX, currentY;
        private int score = 0;
        private int gameTime = 0;
        private int level = 1;
        private Random random = new Random();
        private Bitmap background;
        private Color[] pieceColors = new Color[]
        {
            Color.Cyan,     
            Color.Yellow,    
            Color.Magenta,   
            Color.Red,       
            Color.Green,     
            Color.Orange,    
            Color.Blue,      
            Color.Purple,    
            Color.Pink,      
            Color.Lime,
            Color.Teal,
            Color.Gold
        };
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(300, 600);
            this.Name = "Form1";
            this.ResumeLayout(false);
        }

        private int[][,] pieces = new int[][,]
        {
            new int[,] { {1,1,1,1} },                         
            new int[,] { {1,1}, {1,1} },                      
            new int[,] { {0,1,0}, {1,1,1} },                   
            new int[,] { {1,1,0}, {0,1,1} },                   
            new int[,] { {0,1,1}, {1,1,0} },                   
            new int[,] { {1,0,0}, {1,1,1} },                   
            new int[,] { {0,0,1}, {1,1,1} },                  
            new int[,] { {1,0}, {1,0}, {1,1} },                
            new int[,] { {0,1}, {0,1}, {1,1} },              
            new int[,] { {1,1,1}, {0,1,0}, {0,1,0} },          
            new int[,] { {1,1}, {1,0}, {1,1} },                
            new int[,] { {0,1,1,1}, {1,1,0,0} }                
        };
        private Panel sidePanel;
        private Label scoreLabel;
        private Label timeLabel;
        private Label levelLabel;
        private Label nextPieceLabel;
        private PictureBox nextPieceBox;

        public TetrisForm()
        {
            InitializeComponent();
            SetupForm();
            SetupSidePanel();
            SetupTimers();
            LoadBackground();
            NewPiece();
        }

        private void SetupForm()
        {
            this.ClientSize = new Size(500, 600);
            this.Text = "Вселеннский Тетрис";
            this.DoubleBuffered = true;
            this.KeyPreview = true;
            this.BackColor = Color.Black;
            this.Padding = new Padding(0, 0, 150, 0);
        }

        private void LoadBackground()
        {
            try
            {
                background = new Bitmap("space_bg.jpg");
            }
            catch
            {
                background = new Bitmap(ClientSize.Width, ClientSize.Height);
                using (var g = Graphics.FromImage(background))
                {
                    g.Clear(Color.Black);
                    Random rnd = new Random();
                    for (int i = 0; i < 200; i++)
                    {
                        int size = rnd.Next(1, 3);
                        g.FillEllipse(Brushes.White,
                                     rnd.Next(0, background.Width),
                                     rnd.Next(0, background.Height),
                                     size, size);
                    }
                }
            }
        }

        private void SetupSidePanel()
        {
            sidePanel = new Panel
            {
                Width = 200,
                Height = ClientSize.Height,
                BackColor = Color.FromArgb(30, 30, 60),
                Location = new Point(ClientSize.Width - 200, 0)
            };
            this.Controls.Add(sidePanel);

            var scoreTitle = new Label
            {
                Text = "СЧЕТ",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.Cyan,
                Location = new Point(20, 20),
                AutoSize = true
            };
            sidePanel.Controls.Add(scoreTitle);

            scoreLabel = new Label
            {
                Text = "0",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 50),
                AutoSize = true
            };
            sidePanel.Controls.Add(scoreLabel);

            var timeTitle = new Label
            {
                Text = "ВРЕМЯ",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.Cyan,
                Location = new Point(20, 120),
                AutoSize = true
            };
            sidePanel.Controls.Add(timeTitle);

            timeLabel = new Label
            {
                Text = "00:00",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 150),
                AutoSize = true
            };
            sidePanel.Controls.Add(timeLabel);

            var levelTitle = new Label
            {
                Text = "УРОВЕНЬ",
                Font = new Font("Arial", 14, FontStyle.Bold),
                ForeColor = Color.Cyan,
                Location = new Point(20, 220),
                AutoSize = true
            };
            sidePanel.Controls.Add(levelTitle);

            levelLabel = new Label
            {
                Text = "1",
                Font = new Font("Arial", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 250),
                AutoSize = true
            };
            sidePanel.Controls.Add(levelLabel);

 
            nextPieceLabel = new Label
            {
                Text = "СЛЕДУЮЩАЯ",
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.Cyan,
                Location = new Point(20, 320),
                AutoSize = true
            };
            sidePanel.Controls.Add(nextPieceLabel);

            nextPieceBox = new PictureBox
            {
                Width = 100,
                Height = 100,
                Location = new Point(25, 350),
                BackColor = Color.Transparent
            };
            sidePanel.Controls.Add(nextPieceBox);
        }

        private void SetupTimers()
        {
            gameTimer = new Timer { Interval = 500 };
            gameTimer.Tick += UpdateGame;
            gameTimer.Start();

            clockTimer = new Timer { Interval = 1000 };
            clockTimer.Tick += (s, e) => {
                gameTime++;
                timeLabel.Text = $"{gameTime / 60:00}:{gameTime % 60:00}";
                
                if (gameTime % 30 == 0 && gameTime > 0)
                {
                    level++;
                    levelLabel.Text = level.ToString();
                    gameTimer.Interval = Math.Max(100, 500 - (level * 50));
                }
            };
            clockTimer.Start();
        }

        private void NewPiece()
        {
            currentPiece = pieces[random.Next(pieces.Length)];
            currentX = 4;
            currentY = 0;

            UpdateNextPieceDisplay();

            if (CheckCollision())
            {
                gameTimer.Stop();
                clockTimer.Stop();
                MessageBox.Show($"Игра окончена!\nВаш счет: {score}\nВремя: {timeLabel.Text}", "Конец игры");
                Application.Exit();
            }
        }

        private void UpdateNextPieceDisplay()
        {
            Bitmap nextBmp = new Bitmap(100, 100);
            using (Graphics g = Graphics.FromImage(nextBmp))
            {
                g.Clear(Color.Transparent);
                int[,] nextPiece = pieces[random.Next(pieces.Length)];
                int colorIndex = Array.IndexOf(pieces, nextPiece);

                for (int i = 0; i < nextPiece.GetLength(0); i++)
                {
                    for (int j = 0; j < nextPiece.GetLength(1); j++)
                    {
                        if (nextPiece[i, j] == 1)
                        {
                            int x = j * 20 + 30;
                            int y = i * 20 + 30;
                            g.FillRectangle(new SolidBrush(pieceColors[colorIndex]), x, y, 18, 18);
                            g.DrawRectangle(new Pen(Color.White, 1), x, y, 18, 18);
                        }
                    }
                }
            }
            nextPieceBox.Image = nextBmp;
        }

        private bool CheckCollision()
        {
            for (int i = 0; i < currentPiece.GetLength(0); i++)
            {
                for (int j = 0; j < currentPiece.GetLength(1); j++)
                {
                    if (currentPiece[i, j] == 1)
                    {
                        int newX = currentX + j;
                        int newY = currentY + i;

                        if (newX < 0 || newX >= 10 || newY >= 20 || (newY >= 0 && grid[newY, newX] != 0))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private void MergePiece()
        {
            int colorIndex = Array.IndexOf(pieces, currentPiece);
            for (int i = 0; i < currentPiece.GetLength(0); i++)
            {
                for (int j = 0; j < currentPiece.GetLength(1); j++)
                {
                    if (currentPiece[i, j] == 1)
                    {
                        grid[currentY + i, currentX + j] = colorIndex + 1;
                    }
                }
            }
        }

        private void ClearLines()
        {
            int linesCleared = 0;
            for (int i = 19; i >= 0; i--)
            {
                bool lineComplete = true;
                for (int j = 0; j < 10; j++)
                {
                    if (grid[i, j] == 0)
                    {
                        lineComplete = false;
                        break;
                    }
                }

                if (lineComplete)
                {
                    linesCleared++;
                    for (int k = i; k > 0; k--)
                    {
                        for (int j = 0; j < 10; j++)
                        {
                            grid[k, j] = grid[k - 1, j];
                        }
                    }

                    for (int j = 0; j < 10; j++)
                    {
                        grid[0, j] = 0;
                    }
                    i++;
                }
            }

            if (linesCleared > 0)
            {
                score += 100 * linesCleared * level;
                scoreLabel.Text = score.ToString();
            }
        }

        private void RotatePiece()
        {
            int[,] rotated = new int[currentPiece.GetLength(1), currentPiece.GetLength(0)];

            for (int i = 0; i < currentPiece.GetLength(0); i++)
            {
                for (int j = 0; j < currentPiece.GetLength(1); j++)
                {
                    rotated[j, currentPiece.GetLength(0) - 1 - i] = currentPiece[i, j];
                }
            }

            int[,] oldShape = currentPiece;
            currentPiece = rotated;

            if (CheckCollision())
            {
                currentPiece = oldShape;
            }
        }

        private void UpdateGame(object sender, EventArgs e)
        {
            currentY++;
            if (CheckCollision())
            {
                currentY--;
                MergePiece();
                ClearLines();
                NewPiece();
            }
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            
            g.DrawImage(background, 0, 0, ClientSize.Width - 150, ClientSize.Height);
            
            using (var gridBrush = new SolidBrush(Color.FromArgb(100, 0, 0, 50)))
            {
                g.FillRectangle(gridBrush, 0, 0, 300, 600);
            }

            for (int i = 0; i < 20; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    if (grid[i, j] > 0)
                    {
                        Color pieceColor = pieceColors[grid[i, j] - 1];
                        g.FillRectangle(new SolidBrush(pieceColor), j * 30, i * 30, 30, 30);
                        g.DrawRectangle(new Pen(Color.White, 1), j * 30, i * 30, 30, 30);
                    }
                }
            }

            if (currentPiece != null)
            {
                int colorIndex = Array.IndexOf(pieces, currentPiece);
                Color currentColor = pieceColors[colorIndex];

                for (int i = 0; i < currentPiece.GetLength(0); i++)
                {
                    for (int j = 0; j < currentPiece.GetLength(1); j++)
                    {
                        if (currentPiece[i, j] == 1)
                        {
                            g.FillRectangle(new SolidBrush(currentColor), (currentX + j) * 30, (currentY + i) * 30, 30, 30);
                            g.DrawRectangle(new Pen(Color.White, 1), (currentX + j) * 30, (currentY + i) * 30, 30, 30);
                        }
                    }
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (!gameTimer.Enabled) return base.ProcessCmdKey(ref msg, keyData);

            switch (keyData)
            {
                case Keys.Left:
                    currentX--;
                    if (CheckCollision()) currentX++;
                    break;
                case Keys.Right:
                    currentX++;
                    if (CheckCollision()) currentX--;
                    break;
                case Keys.Down:
                    currentY++;
                    if (CheckCollision()) currentY--;
                    break;
                case Keys.Up:
                    RotatePiece();
                    break;
                case Keys.Space:
                    while (!CheckCollision()) currentY++;
                    currentY--;
                    MergePiece();
                    ClearLines();
                    NewPiece();
                    break;
                case Keys.P:
                    gameTimer.Enabled = !gameTimer.Enabled;
                    clockTimer.Enabled = !clockTimer.Enabled;
                    break;
            }
            this.Invalidate();
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }
}
