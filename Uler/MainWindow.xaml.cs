using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Media;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using MySql.Data.MySqlClient;
namespace Uler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<Backgroundgrid, ImageSource> gridValToImage = new()
        {
            {Backgroundgrid.Empty, Property.Empty},
            {Backgroundgrid.Uler, Property.Badan},
            {Backgroundgrid.Makanan, Property.Makanan},
            {Backgroundgrid.Shield, Property.Shield}
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            {Direction.Up,0 },
            {Direction.Right, 90},
            {Direction.Down, 180 },
            {Direction.Left, 270 }
        };

        private readonly int rows = 20, cols = 20;
        private readonly Image[,] gridImages;
        private Mekanisme mekanisme;
        private bool gameberjalan;
        private string username;
        public MainWindow()
        {
            InitializeComponent();
            gridImages = AturGrid();
            mekanisme  = new Mekanisme(rows, cols);
            
        }

        private async Task jalankangame()
        {
            Menggambar();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            exit.Visibility = Visibility.Hidden;
            await UlangGame();
            await TampilanGameOver();
            mekanisme = new Mekanisme(rows, cols);
           
        }

        private void btnq_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(bxinput.Text))
            {
                username = bxinput.Text;
                gameberjalan = true;
                txtname.Text = $"Nama Player : {username}";
                txtnameo.Text = $"'{username}'";
                jalankangame();
            }
            else
            {
                MessageBox.Show("Anda Harus Menginputkan Username!");
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (mekanisme.gameover)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Left:
                    mekanisme.UbahDirection(Direction.Left);
                    break;
                case Key.Right:
                    mekanisme.UbahDirection(Direction.Right);
                    break;
                case Key.Up:
                    mekanisme.UbahDirection(Direction.Up);
                    break;
                case Key.Down:
                    mekanisme.UbahDirection(Direction.Down);
                    break;
            }
        }

        private async Task UlangGame()
        {
            while (!mekanisme.gameover)
            {
                await Task.Delay(100);
                mekanisme.Gerak();
                Menggambar();
            }
        }

        private Image[,] AturGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Property.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }

        private void Menggambar()
        {
            GambarGrid();
            GambarKepalaUler();
            ScoreText.Text = $"SCORE {mekanisme.score}";
            int a = Int32.Parse(HighScore.Text);
            if (mekanisme.score > a)
            {
                HighScore.Text = mekanisme.score.ToString();
                Properties.Settings.Default.h_score = HighScore.Text;
                Properties.Settings.Default.Save();
                HighScore.Text = Properties.Settings.Default.h_score;
            }
        }


        private void GambarGrid()
        {
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Backgroundgrid gridval = mekanisme.Grid[r, c];
                    if (gridval == Backgroundgrid.Immune)
                    {
                        gridImages[r, c].Source = Property.Shield;
                    }
                    else
                    {
                        gridImages[r, c].Source = gridValToImage[gridval];
                    }
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void GambarKepalaUler()
        {
            Posisi kepala = mekanisme.PosisiKepala();
            Image gambar = gridImages[kepala.Row, kepala.Col];
            gambar.Source = Property.Kepala;

            int rotation = dirToRotation[mekanisme.Dir];
            gambar.RenderTransform = new RotateTransform(rotation);
        }

        private async Task GambarUlerMati()
        {
            List<Posisi> posisi = new List<Posisi>(mekanisme.posisiuler());
            
            for (int i = 0; i < posisi.Count; i++)
            {
                Posisi pos = posisi[i];
                ImageSource source = (i == 0) ? Property.KepalaMati : Property.BadanMati;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i = 3; i >= 1; i--)
            {
                Overlay1.Visibility = Visibility.Hidden;
                pnlover.Visibility = Visibility.Hidden;
                btnq.Visibility = Visibility.Hidden;
                exit.Visibility = Visibility.Hidden;
                bxinput.Visibility = Visibility.Hidden;
                OverlayText.Visibility = Visibility.Hidden;
                setting.Visibility = Visibility.Hidden;
                ScoreText.Text = i.ToString();
                await Task.Delay(1000);
            }
        }

        private void bxinput_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(bxinput_TextChanged != null)
            {
                btnq.IsEnabled = true;
            }
            else
            {
                btnq.IsEnabled = false;
            }
        }

        private async void btnag_Click(object sender, RoutedEventArgs e)
        {
            if(btnag.IsEnabled == true)
            {
                RestartGame();
            }
        }

        private void Suara_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Score_Click(object sender, RoutedEventArgs e)
        {
            if (Score.IsEnabled == true)
            {
                ScoreText.Visibility = Visibility.Hidden;
                Score.Content = $"Munculkan Score";
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if(Back.IsEnabled == true)
            {
                Overlay2.Visibility = Visibility.Hidden;
                pnlsetting.Visibility = Visibility.Hidden;
                Overlay.Visibility= Visibility.Visible;
                pnlmulai.Visibility= Visibility.Visible;
            }
        }

        private void setting_Click(object sender, RoutedEventArgs e)
        {
            if(setting.IsEnabled == true)
            {
                Overlay2.Visibility = Visibility.Visible;
                pnlsetting.Visibility = Visibility.Visible;
                Overlay.Visibility = Visibility.Hidden;
                pnlmulai.Visibility = Visibility.Hidden;
            }
        }

        private async Task TampilanGameOver()
        {
            await GambarUlerMati();
            await Task.Delay(1000);
            InsertScoreToDatabase(username, mekanisme.score);
            Overlay1.Visibility = Visibility.Visible;
            pnlover.Visibility = Visibility.Visible;
            MakeLeaderboardMenu();
        }
        private void MakeLeaderboardMenu()
        {
            List<string> username = new();
            List<int> score = new();
            try
            {
                string connection_string = "server=localhost;user id=root;password='';database=leaderboard";
                using (MySqlConnection conn = new MySqlConnection(connection_string))
                {
                    conn.Open();
                    string query = "SELECT * FROM leaderboard ORDER BY score DESC";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                username.Add(reader.GetString("username"));
                                score.Add(reader.GetInt32("score"));
                            }
                        }
                    }
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            Grid leaderboard = FindName("LeaderboardMenu") as Grid;
            ClearChilrenGrid(leaderboard, 1);

            for (int i = 1; i <= score.Count; i++)
            {
                RowDefinition row = new RowDefinition();
                row.Height = new GridLength(0, GridUnitType.Auto);
                LeaderboardMenu.RowDefinitions.Add(row);

                Label lblposisi = new Label();
                lblposisi.Content = i;
                lblposisi.FontSize = 30;
                lblposisi.Foreground = new SolidColorBrush(Colors.White);
                lblposisi.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(lblposisi, i);
                Grid.SetColumn(lblposisi, 0);
                leaderboard.Children.Add(lblposisi);

                Label lblnama = new Label();
                lblnama.Content = username[i - 1];
                lblnama.FontSize = 30;
                lblnama.Foreground = new SolidColorBrush(Colors.White);
                lblnama.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(lblnama, i);
                Grid.SetColumn(lblnama, 1);
                leaderboard.Children.Add(lblnama);

                Label lblscore = new Label();
                lblscore.Content = score[i - 1];
                lblscore.FontSize = 30;
                lblscore.Foreground = new SolidColorBrush(Colors.White);
                lblscore.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(lblscore, i);
                Grid.SetColumn(lblscore, 2);
                leaderboard.Children.Add(lblscore);

                if(i >= 3)
                {
                    break;
                }

            }
        }
        private void InsertScoreToDatabase(string username, int score)
        {
            try
            {
                string connectionString = "server=localhost;user id=root;password='';database=leaderboard";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    
                    string checkQuery = "SELECT COUNT(*) FROM leaderboard WHERE username = @username AND score = @score";
                    using (MySqlCommand cmd = new MySqlCommand(checkQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@score", score);
                        int count = Convert.ToInt32(cmd.ExecuteScalar());

                        
                        if (count > 0)
                        {
                            Console.WriteLine("Skor sudah ada dalam leaderboard. Tidak dimasukkan lagi.");
                            return;
                        }
                    }

                    // Masukkan skor pemain baru ke dalam leaderboard
                    string insertQuery = "INSERT INTO leaderboard (username, score) VALUES (@username, @score)";
                    using (MySqlCommand cmd = new MySqlCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@score", score);
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
        }
        private void ClearChilrenGrid(Grid grid, int startRow)
        {
            for (int i = grid.Children.Count - 1; i >= 0; i--)
            {
                var child = grid.Children[i];
                int row = Grid.GetRow(child);

                if (row >= startRow)
                {
                    grid.Children.Remove(child);
                }
            }
        }

        private void Leaderboard_Click(object sender, RoutedEventArgs e)
        {
            Overlay1.Visibility = Visibility.Hidden;
            Leadboard.Visibility = Visibility.Visible;
        }

        private void resets_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string connectionString = "server=localhost;user id=root;password='';database=leaderboard";
                using (MySqlConnection conn = new MySqlConnection(connectionString))
                {
                    conn.Open();

                    string query = "TRUNCATE TABLE leaderboard";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            MessageBox.Show("Data Berhasil Dihapus:)");
        }

        private void btnme_Click(object sender, RoutedEventArgs e)
        {
            if (btnme.IsEnabled == true)
            {
                gameberjalan = true;
                txtname.Text = $"Nama Player : ";
                txtnameo.Text = $"''";
            }
            Overlay.Visibility = Visibility.Visible;
            pnlmulai.Visibility = Visibility.Visible;
            btnq.Visibility = Visibility.Visible;
            OverlayText.Visibility = Visibility.Visible;
            setting.Visibility = Visibility.Visible;
            exit.Visibility = Visibility.Visible;
            bxinput.Visibility = Visibility.Visible;
            Overlay1.Visibility = Visibility.Hidden;
        }

        private void exit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btback_Click(object sender, RoutedEventArgs e)
        {
            Overlay1.Visibility = Visibility.Visible;
            Leadboard.Visibility = Visibility.Hidden;
        }
        private async Task RestartGame()
        {
            Menggambar();
            await ShowCountDown();
            await UlangGame();
            await TampilanGameOver();
            mekanisme = new Mekanisme(rows, cols);
        }
        
    }
}
