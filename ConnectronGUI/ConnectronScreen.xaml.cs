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

namespace ConnectronGUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Game mainGame;
        private int currentPlayerIndex = 0;
        private List<Player> setupPlayers = new List<Player>();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartNewGame(int numRows, int numColumns, int winningLength, List<Player> playerList)
        {
            //start new game
            //make previous menu invisible
            gridContentStartScreen.Visibility = Visibility.Hidden;

            mainGame = new Game(numRows, numColumns, winningLength);
            CreateGameGrid(mainGame.GameGrid.GetLength(0), mainGame.GameGrid.GetLength(1));
            
            currentPlayerIndex = 0;
            mainGame.Players = playerList;

            //setup UI & make visible
            UpdateGameGrid();
            lblCurrentPlayer.Content = $"Current Player: {mainGame.Players[currentPlayerIndex].PlayerName}";
            lblWinLength.Content = $"Winning Line Length: {mainGame.WinLength}";
            gridContentGameplay.Visibility = Visibility.Visible;
        }
        private void CreateGameGrid(int numRows, int numColumns)
        {
            /*gridGameBoard & the game grid used to store the current game state are different!
             Consider that the former has an extra row for buttons allowing counter drops.*/
            string currentObjName = "";
            //increment numRows to add a row for drop buttons
            numRows++;

            //clear previous
            //loop through and unregister all names
            //buttons
            for (int c = 0; c < gridGameBoard.ColumnDefinitions.Count; c++)
            {
                currentObjName = $"btn_Col_{c}";
                UnregisterName(currentObjName);
            }
            //labels
            for (int c = 0; c< gridGameBoard.ColumnDefinitions.Count;c++)
            {
                for (int r = 0; r < gridGameBoard.RowDefinitions.Count;r++)
                {
                    currentObjName = $"lbl_{r}_{c}";
                    UnregisterName(currentObjName);
                }
            }
            gridGameBoard.Children.Clear();
            gridGameBoard.RowDefinitions.Clear();
            gridGameBoard.ColumnDefinitions.Clear();

            //set row & column definitions
            for (int r = 0; r < numRows; r++)
            {
                gridGameBoard.RowDefinitions.Add(new RowDefinition());
            }
            for (int c = 0; c < numColumns; c++)
            {
                gridGameBoard.ColumnDefinitions.Add(new ColumnDefinition());
            }

            //add buttons for dropping counters
            for (int c = 0; c< numColumns; c++)
            {
                Button dropButton = new Button();
                dropButton.Content = $"\\/";
                dropButton.Name = $"btn_Col_{c}";
                dropButton.Background = Brushes.Red;
                dropButton.Foreground = Brushes.White;
                dropButton.Click += Column_Button_Click;
                dropButton.BorderBrush = new SolidColorBrush(Color.FromArgb(255,114, 252, 255));
                dropButton.BorderThickness = new Thickness(3, 1, 3, 3);
                RegisterName(dropButton.Name, dropButton);
                Grid.SetRow(dropButton, 0);
                Grid.SetColumn(dropButton, c);

                gridGameBoard.Children.Add(dropButton);
            }

            //add labels to colour cells
            for (int c = 0; c < numColumns; c++)
            {
                for (int r = numRows-1; r >= 0; r--)
                {
                    Label cellLabel = new Label();
                    cellLabel.Name = $"lbl_{r}_{c}"; //labels get name based on where in game grid data storage, not UI element
                    cellLabel.HorizontalAlignment = HorizontalAlignment.Stretch;
                    cellLabel.VerticalAlignment = VerticalAlignment.Stretch;
                    cellLabel.Background = Brushes.White;
                    cellLabel.BorderBrush = new SolidColorBrush(Color.FromRgb(114, 252, 255));
                    cellLabel.BorderThickness = new Thickness(2, 2, 2, 2);
                    RegisterName(cellLabel.Name, cellLabel);
                    Grid.SetRow(cellLabel, r+1);
                    Grid.SetColumn(cellLabel, c);

                    gridGameBoard.Children.Add(cellLabel);
                }
            }
        }

        private void UpdateGameGrid()
        {
            Label dynamicLabel;
            string labelName = "";
            int currentValue = -1;
            //update grid with colours of each player
            for (int r = 0; r < mainGame.GameGrid.GetLength(0); r++)
            {
                for (int c = 0; c < mainGame.GameGrid.GetLength(1); c++)
                {
                    currentValue = mainGame.GameGrid[r,c];
                    labelName = $"lbl_{r}_{c}";
                    dynamicLabel = (Label)gridGameBoard.FindName(labelName);
                    if (currentValue == -1)
                    {
                        dynamicLabel.Background = Brushes.White;
                    }
                    else
                    {
                        dynamicLabel.Background = new SolidColorBrush(mainGame.Players[currentValue].PlayerColor);
                    }
                }
            }
        }

        private void ResetGame()
        {
            gridContentGameplay.Visibility = Visibility.Hidden;
            mainGame.ResetGrid();
            UpdateGameGrid();
            currentPlayerIndex = 0;
            setupPlayers.Clear();
            gridContentStartScreen.Visibility = Visibility.Visible;
        }

        private void UpdateColorPreview()
        {
            //variables
            byte red = 0;
            byte green = 0;
            byte blue = 0;

            //get values from sliders
            red = (byte)sldRed.Value;
            green = (byte)sldGreen.Value;
            blue = (byte)sldBlue.Value;

            lblColorPreview.Background = new SolidColorBrush(Color.FromRgb(red, green, blue));
        }
        //event handler for grid button clicks
        private void Column_Button_Click(object sender, RoutedEventArgs e)
        {
            //set up result variable
            WinningLine dropResult;
            //deal with column drop button click
            Button currentBtn = (Button)sender;
            //get targetColumn
            int targetColumn = Grid.GetColumn(currentBtn);
            //drop counter
            dropResult = mainGame.DropCounter(currentPlayerIndex, targetColumn);
            //update UI
            UpdateGameGrid();

            //if a win, display winner and end game
            if (dropResult.IsWin)
            {
                MessageBox.Show($"Game Over! The winner is: {mainGame.Players[dropResult.WinnerIndex].PlayerName}");
                ResetGame();
            }
            else
            {
                //update current player index
                currentPlayerIndex++;
                if (currentPlayerIndex > mainGame.Players.Count - 1)
                {
                    currentPlayerIndex = 0;
                }
                lblCurrentPlayer.Content = $"Current Player: {mainGame.Players[currentPlayerIndex].PlayerName}";
            }
        }

        private void colorSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateColorPreview();
        }

        private void sldGreen_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateColorPreview();
        }

        private void sldBlue_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            UpdateColorPreview();
        }

        private void btnAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            //initialise vars
            string playerName = txtPlayerNameIn.Text;
            byte red = (byte)sldRed.Value;
            byte green = (byte)sldGreen.Value;
            byte blue = (byte)sldBlue.Value;

            //add player
            setupPlayers.Add(new Player(playerName, System.Windows.Media.Color.FromRgb(red, green, blue)));

            //clear UI
            sldRed.Value = 0;
            sldGreen.Value = 0;
            sldBlue.Value = 0;
            txtPlayerNameIn.Text = "";
        }

        private void btnStartGame_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame(int.Parse(txtRowsIn.Text), int.Parse(txtColumnsIn.Text), int.Parse(txtWinLengthIn.Text), setupPlayers);
        }
    }
}
