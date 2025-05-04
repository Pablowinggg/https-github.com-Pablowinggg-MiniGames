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
            // 
            // Form1
            // 
            this.ClientSize = new System.Drawing.Size(300, 600);
            this.Name = "Form1";
            this.ResumeLayout(false);
        }

        private void SetupMenu()

        {
            {
                this.Text = "Коллекция игр";
                this.ClientSize = new Size(400, 300);
                this.BackColor = Color.FromArgb(30, 30, 40);

                var btnTetris = new Button
                {
                    Text = "Тетрис",
                    Size = new Size(200, 50),
                    Location = new Point(100, 100)
                };
                btnTetris.Click += (s, e) => OpenGame<TetrisForm>();

                var btnMatch3 = new Button
                {
                    Text = "Три в ряд",
                    Size = new Size(200, 50),
                    Location = new Point(100, 170)
                };
                btnMatch3.Click += (s, e) => OpenGame<Match3Form>();

                this.Controls.Add(btnTetris);
                this.Controls.Add(btnMatch3);
            }
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