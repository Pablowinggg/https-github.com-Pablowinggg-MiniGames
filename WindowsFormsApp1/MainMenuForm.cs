using System;
using System.Drawing;
using System.Windows.Forms;
using SpaceDestroyer;
using TetrisGame;

namespace GameCollection.Forms
{
    public partial class GameMainMenu : Form
    {
        public GameMainMenu()
        {
            InitializeComponent();
            SetupMenu();
        }
        private void InitializeComponent()
        {
            this.SuspendLayout();
            this.ClientSize = new System.Drawing.Size(300, 600);
            this.Name = "Form1";
            this.ResumeLayout(false);
        }

        private void SetupMenu()
        {
            this.Text = "Game Collection";
            this.ClientSize = new Size(800, 600);
            this.BackColor = Color.FromArgb(30, 30, 40);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            Label titleLabel = new Label
            {
                Text = "GAME COLLECTION",
                Font = new Font("Arial", 32, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(200, 50)
            };

            Button btnTetris = CreateGameButton("TETRIS", 300, 180, Color.FromArgb(0, 150, 255));
            Button btnMatch3 = CreateGameButton("MATCH 3", 300, 260, Color.FromArgb(255, 100, 0));
            Button btnSnake = CreateGameButton("SNAKE", 300, 340, Color.FromArgb(0, 200, 100));
            Button btnSpace = CreateGameButton("SPACE", 300, 420, Color.FromArgb(150, 0, 255));
            Button btnExit = CreateGameButton("EXIT", 300, 500, Color.FromArgb(200, 50, 50));

            btnTetris.Click += (s, e) => OpenGame<TetrisForm>();
            btnMatch3.Click += (s, e) => OpenGame<Match3Form>();
            btnSnake.Click += (s, e) => OpenGame<Snake1Form>();
            btnSpace.Click += (s, e) => OpenGame<SpaceGame>();
            btnExit.Click += (s, e) => Application.Exit();

            this.Controls.Add(titleLabel);
            this.Controls.Add(btnTetris);
            this.Controls.Add(btnMatch3);
            this.Controls.Add(btnSnake);
            this.Controls.Add(btnSpace);
            this.Controls.Add(btnExit);
        }

        private Button CreateGameButton(string text, int x, int y, Color baseColor)
        {
            Button btn = new Button
            {
                Text = text,
                Size = new Size(200, 50),
                Location = new Point(x, y),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Arial", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = baseColor,
                Cursor = Cursors.Hand
            };

            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                Math.Min(baseColor.R + 30, 255),
                Math.Min(baseColor.G + 30, 255),
                Math.Min(baseColor.B + 30, 255));
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(
                Math.Max(baseColor.R - 30, 0),
                Math.Max(baseColor.G - 30, 0),
                Math.Max(baseColor.B - 30, 0));

            return btn;
        }

        private void OpenGame<T>() where T : Form, new()
        {
            this.Hide();
            using (T game = new T())
            {
                game.FormClosed += (s, args) => this.Show();
                game.ShowDialog();
            }
        }
    }
}