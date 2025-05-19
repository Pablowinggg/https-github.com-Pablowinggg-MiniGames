using System;
using System.Drawing;
using System.Windows.Forms;
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
            this.Text = "Список игр";
            this.ClientSize = new Size(800, 600);
            this.BackColor = Color.FromArgb(30, 30, 40);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // Стилизованный заголовок
            var titleLabel = new Label
            {
                Text = "СПИСОК ИГР",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(300, 50)
            };

            // Создание стилизованных кнопок
            var btnTetris = CreateGameButton("Волшебный Тетрис", 300, 150, Color.FromArgb(0, 150, 255));
            var btnMatch3 = CreateGameButton("Три в ряд", 300, 250, Color.FromArgb(255, 100, 0));
            var btnSnake = CreateGameButton("Змейка", 300, 350, Color.FromArgb(0, 200, 100));

            btnTetris.Click += (s, e) => OpenGame<TetrisForm>();
            btnMatch3.Click += (s, e) => OpenGame<Match3Form>();
            btnSnake.Click += (s, e) => OpenGame<Snake1Form>();

            this.Controls.Add(titleLabel);
            this.Controls.Add(btnTetris);
            this.Controls.Add(btnMatch3);
            this.Controls.Add(btnSnake);
        }

        private Button CreateGameButton(string text, int x, int y, Color baseColor)
        {
            var btn = new Button
            {
                Text = text,
                Size = new Size(200, 60),
                Location = new Point(x, y),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = baseColor,
                Cursor = Cursors.Hand
            };

            // Стиль кнопки
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(
                Math.Min(baseColor.R + 30, 255),
                Math.Min(baseColor.G + 30, 255),
                Math.Min(baseColor.B + 30, 255)
            );
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(
                Math.Max(baseColor.R - 30, 0),
                Math.Max(baseColor.G - 30, 0),
                Math.Max(baseColor.B - 30, 0)
            );

            // Эффект тени
            btn.Paint += (sender, e) =>
            {
                Button button = (Button)sender;
                ControlPaint.DrawBorder(
                    e.Graphics,
                    button.ClientRectangle,
                    Color.Transparent,
                    0,
                    ButtonBorderStyle.None,
                    Color.Transparent,
                    0,
                    ButtonBorderStyle.None,
                    Color.Transparent,
                    0,
                    ButtonBorderStyle.None,
                    Color.FromArgb(100, 0, 0, 0),
                    5,
                    ButtonBorderStyle.Outset
                );
            };

            // Анимация при наведении
            btn.MouseEnter += (sender, e) =>
            {
                btn.Size = new Size(210, 65);
                btn.Location = new Point(x - 5, y - 2);
            };

            btn.MouseLeave += (sender, e) =>
            {
                btn.Size = new Size(200, 60);
                btn.Location = new Point(x, y);
            };

            return btn;
        }

        private void OpenGame<T>() where T : Form, new()
        {
            this.Hide();
            using (var game = new T())
            {
                game.ShowDialog();
            }
            this.Show();
        }
    }
}