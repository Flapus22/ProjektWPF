using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ProjektWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public DispatcherTimer GameTimer { get; set; } = new DispatcherTimer();
        List<Meteorite> Meteor { get; set; } = new List<Meteorite>();
        List<Meteorite> ToRemove { get; set; } = new List<Meteorite>();

        Random rnd { get; set; } = new Random();

        public ImageBrush ShipImage { get; set; } = new ImageBrush();

        public double Speed { get; set; } = 15;
        public int PlayerSpeed { get; set; } = 10;

        public double Score { get; set; }

        public Stopwatch Stopwatch { get; set; } = new Stopwatch();
        public double TimeToNewMeteor { get; set; } = 1;

        bool moveLeft, moveRight, moveDown, moveUp, gameOver;


        public MainWindow()
        {
            InitializeComponent();

            MyCanvas.Focus();
            var img = new ImageBrush();
            img.ImageSource = new BitmapImage(new Uri("D:\\Projekty\\Visual\\Semestr 4\\Programowanie IV\\ProjektWPF\\ProjektWPF\\files\\background.jpg", UriKind.Relative));
            MyCanvas.Background = img;

            ShipImage.ImageSource = new BitmapImage(new Uri("D:\\Projekty\\Visual\\Semestr 4\\Programowanie IV\\ProjektWPF\\ProjektWPF\\files\\spaceship.png", UriKind.Relative));
            Player.Fill = ShipImage;
            //Ship = Player;

            //Canvas.SetTop(Ship,500);
            GameTimer.Tick += GameLoop;
            GameTimer.Interval = TimeSpan.FromMilliseconds(20);
            //CreateMeteorite();
            Stopwatch = Stopwatch.StartNew();
            StartGame();
        }

        public void GameLoop(object sender, EventArgs e)
        {
            if (moveLeft == true && Canvas.GetLeft(Player) > 0)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - PlayerSpeed);
            }
            if (moveRight == true && Canvas.GetLeft(Player) + Player.Width + 20d < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + PlayerSpeed);
            }
            if (moveUp == true && Canvas.GetTop(Player) > 0)
            {
                Canvas.SetTop(Player, Canvas.GetTop(Player) - PlayerSpeed);
            }
            if (moveDown == true && Canvas.GetTop(Player) + Player.Height + 10 < Application.Current.MainWindow.Height)
            {
                Canvas.SetTop(Player, Canvas.GetTop(Player) + PlayerSpeed);
            }
            if (Meteor.Count < 20 && Stopwatch.Elapsed > TimeSpan.FromSeconds(TimeToNewMeteor))
            {
                CreateMeteorite();
                Stopwatch.Restart();
            }

            var playerHitBox = new Rect(Canvas.GetLeft(Player)+10, Canvas.GetTop(Player)+10, Player.Width-20, Player.Height-20);

            foreach (var item in Meteor)
            {
                Canvas.SetTop(item.Meteo, Canvas.GetTop(item.Meteo) + (Speed / 2));
                Canvas.SetLeft(item.Meteo, Canvas.GetLeft(item.Meteo) + (Speed / 2) * item.HorizontalMove);

                if (Canvas.GetTop(item.Meteo) > Application.Current.MainWindow.Height + 50 || Canvas.GetLeft(item.Meteo) < -50 || Canvas.GetLeft(item.Meteo) > Application.Current.MainWindow.Width)
                {
                    MyCanvas.Children.Remove(item.Meteo);
                    ToRemove.Add(item);
                }

                var meteorHitBox = new Rect(Canvas.GetLeft(item.Meteo), Canvas.GetTop(item.Meteo), item.Meteo.Width, item.Meteo.Height);
                if (playerHitBox.IntersectsWith(meteorHitBox))
                {
                    GameTimer.Stop();
                    gameOver = true;
                }
            }

            foreach (var item in ToRemove)
            {
                Meteor.Remove(item);
            }
            ToRemove.Clear();

            Score += 0.05;
            if (Score % 20 == 0) TimeToNewMeteor -= 0.05;
            ScoreToString();
        }

        public void CreateMeteorite()
        {
            ImageBrush MeteorImgSource = new ImageBrush();

            switch (rnd.Next(2))
            {
                case 0:
                    MeteorImgSource.ImageSource = new BitmapImage(new Uri("D:\\Projekty\\Visual\\Semestr 4\\Programowanie IV\\ProjektWPF\\ProjektWPF\\files\\meteoryt1.png", UriKind.Relative));
                    break;
                case 1:
                    MeteorImgSource.ImageSource = new BitmapImage(new Uri("D:\\Projekty\\Visual\\Semestr 4\\Programowanie IV\\ProjektWPF\\ProjektWPF\\files\\meteoryt2.png", UriKind.Relative));
                    break;
                default:
                    break;
            }
            Meteorite newMeteor = new Meteorite(MeteorImgSource);

            Canvas.SetLeft(newMeteor.Meteo, rnd.Next(0, (int)Application.Current.MainWindow.Width));
            Canvas.SetTop(newMeteor.Meteo, -50);

            Meteor.Add(newMeteor);
            MyCanvas.Children.Add(newMeteor.Meteo);

        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.A)
            {
                moveLeft = true;
            }
            if (e.Key == Key.Right || e.Key == Key.D)
            {
                moveRight = true;
            }
            if (e.Key == Key.Up || e.Key == Key.W)
            {
                moveUp = true;
            }
            if (e.Key == Key.Down || e.Key == Key.S)
            {
                moveDown = true;
            }
        }
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left || e.Key == Key.A)
            {
                moveLeft = false;
            }
            if (e.Key == Key.Right || e.Key == Key.D)
            {
                moveRight = false;
            }
            if (e.Key == Key.Up || e.Key == Key.W)
            {
                moveUp = false;
            }
            if (e.Key == Key.Down || e.Key == Key.S)
            {
                moveDown = false;
            }

            if (e.Key == Key.Enter && gameOver == true)
            {
                StartGame();
            }
        }
        private void StartGame()
        {
            Speed = 8;
            GameTimer.Start();

            moveLeft = false;
            moveRight = false;
            gameOver = false;

            Score = 0;
            Canvas.SetTop(Player, 400);
            Canvas.SetLeft(Player, 200);

            foreach (var item in Meteor)
            {
                MyCanvas.Children.Remove(item.Meteo);
            }
            Meteor.Clear();
            ToRemove.Clear();
        }

        public void ScoreToString()
        {
            var str = $"Score: {Math.Round(Score, 2)}";
            if (gameOver) str = $"{str}\tClick Enter";
            scoreLabel.Content = str;
        }
    }
    class Meteorite
    {
        public Rectangle Meteo { get; set; }
        public double HorizontalMove { get; set; }
        public Meteorite(ImageBrush fill)
        {
            Random rnd = new Random();
            Meteo = new Rectangle()
            {
                Tag = "Meteor",
                Width = rnd.Next(20, 80),
                Height = rnd.Next(20, 80),
                Fill = fill
            };

            HorizontalMove = rnd.Next(-2, 2);
            HorizontalMove += rnd.NextDouble();
        }
    }
}
