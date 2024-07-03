using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DoAnSnake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            {GridValue.Empty, Images.Empty },
            {GridValue.Snake,Images.Body },
            {GridValue.Food,Images.Food }
        };
        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            {Direction.Up,0 },
            {Direction.Down,180},
            {Direction.Left,270 },
            {Direction.Right,90 }
        };
        private readonly int rows=15, cols=15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        /*private bool gameRunning;
        private bool gamePaused;*/
        public MainWindow()
        {
            InitializeComponent();
            gridImages = setupGrid();
            gameState = new GameState(rows,cols);
        }

        private async Task RunGame()
        {
            Draw();
            await showCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await showgameOver();
            gameState = new GameState(rows,cols);
        }
        private async Task showCountDown()
        {
            for(int i=3;i>=1;i--)
            {
                OverLaytext.Text = i.ToString();
                await Task.Delay(1000);
            }
        }
        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if(Overlay.Visibility == Visibility.Visible && gameState.state != StateChanges.Pause)
            {
                e.Handled = true;
            }
            if (gameState.state==StateChanges.UnStart)
            {
                gameState.state = StateChanges.Start;
                await RunGame();
                gameState.state = StateChanges.UnStart;
            }
        }
        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(gameState.state==StateChanges.Over)
            {
                return;
            }
            if (gameState.state == StateChanges.UnStart)
            {
                return;
            }
            if (gameState.state==StateChanges.Start && e.Key == Key.Space)
            {
                gameState.state = StateChanges.Pause;
                return;
            }
            if (gameState.state == StateChanges.Pause && e.Key ==Key.Space)
            {
                gameState.state = StateChanges.Resuming;
                return;
            }
            switch (e.Key)
            { 
                case Key.Up or Key.W:
                    gameState.ChangeDirection(Direction.Up); break;
                case Key.Down or Key.S:
                    gameState.ChangeDirection(Direction.Down); break;
                case Key.Left or Key.A:
                    gameState.ChangeDirection(Direction.Left); break;
                case Key.Right or Key.D:
                    gameState.ChangeDirection(Direction.Right); break;
            }
        }
        private async Task GameLoop()
        {
            while (gameState.state != StateChanges.UnStart && gameState.state != StateChanges.Over)
            {
                await Task.Delay(200-(gameState.Score *4 ));
                if (gameState.state == StateChanges.Start)
                {
                    gameState.Move();
                    Draw();
                }else if(gameState.state == StateChanges.Pause)
                {
                    Overlay.Visibility = Visibility.Visible;
                    OverLaytext.Text = "PAUSED";
                }else if(gameState.state == StateChanges.Resuming)
                {
                    Overlay.Visibility = Visibility.Visible;
                    OverLaytext.Text = "RESUMING";
                    await Task.Delay(100);
                    Overlay.Visibility = Visibility.Hidden;
                    gameState.state = StateChanges.Start;
                }
                
            }
        }

        private Image[,] setupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            for(int r=0;r<rows; r++)
            {
                for(int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5,0.5)
                    };
                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }
            return images;
        }
        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            ScoreText.Text = $"Điểm {gameState.Score}";
        }


        private void DrawGrid()
        {
            for(int r = 0; r < rows; r++)
            {
                for(int c = 0;c < cols; c++)
                {
                    GridValue gridval = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridval];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }
        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Colum];
            image.Source = Images.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }
        private async Task DrawSnakeDead()
        {
            List<Position> position = new List<Position>(gameState.SnakePosition());
            for(int i=0; i < position.Count; i++)
            {
                Position pos = position[i];
                ImageSource source = (i==0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Colum].Source = source;
                await Task.Delay(50);
            }
        }
        private async Task showgameOver()
        {
            await DrawSnakeDead();
            await Task.Delay(1000);
            Overlay.Visibility = Visibility.Visible;
            OverLaytext.Text =  $"Điểm của bạn là {gameState.Score}";
            await Task.Delay(5000);
            OverLaytext.Text = "Nhấn phím bất kì để bắt đầu";
            /*gameState.state = StateChanges.Over;*/
        }
    }
}