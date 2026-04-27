using System.Collections.Generic;
using System.Drawing;
using System.Windows.Controls;

namespace ConnectronGUI
{
    class Game
    {
        public int[,] GameGrid { get; set; }
        public List<Player> Players { get; set; }
        public int WinLength { get; set; }
        public const int defaultValue = -1;
        public Game(int numRows, int numColumns, int winLength)
        {
            WinLength = winLength;
            GameGrid = new int[numRows, numColumns];
            Players = new List<Player>();

            ResetGrid();
        }

        public void ResetGrid()
        {
            //populate grid
            for (int row = 0; row < GameGrid.GetLength(0); row++)
            {
                for (int column = 0; column < GameGrid.GetLength(1); column++)
                {
                    GameGrid[row, column] = defaultValue;
                }
            }
        }
        public WinningLine DropCounter(int playerIndex, int targetColumn)
        {
            WinningLine dropResult;
            int resultRow = 0;
            for (int row = GameGrid.GetLength(0)-1; row >= 0; row--)
            {
                if (GameGrid[row, targetColumn] == defaultValue)
                {
                    GameGrid[row, targetColumn] = playerIndex;
                    resultRow = row;
                    break;
                }
            }
            dropResult = CheckHorizontalFromPoint(resultRow, targetColumn, playerIndex);
            if (!dropResult.IsWin)
            {
                dropResult = CheckVerticalFromPoint(resultRow, targetColumn, playerIndex);
                if (!dropResult.IsWin)
                {
                    dropResult = CheckDiagonalFromPoint(resultRow, targetColumn, playerIndex);
                }
            }
            return dropResult;
        }
        public WinningLine CheckGridForWins()
        {
            //check whole grid
            WinningLine result = new WinningLine(false);
            return result;
        }
        public WinningLine CheckHorizontalFromPoint(int row, int column, int playerIndex)
        {
            //variables
            WinningLine result = new WinningLine(false);
            int startingColumn = column - WinLength;
            //ensure no less than 0
            if (startingColumn < 0)
            {
                startingColumn = 0;
            }
            int currentConsec = 0; //stores current consecutive counters
            int checkSymbol = playerIndex;

            //check for wins
            for (int currentPos = startingColumn; currentPos < GameGrid.GetLength(1); currentPos++)
            {
                if (GameGrid[row, currentPos] == checkSymbol)
                {
                    currentConsec++;
                }
                else
                {
                    currentConsec = 0;
                }
                if (currentConsec == WinLength)
                {
                    result.WinnerIndex = playerIndex;
                    result.IsWin = true;
                    break;
                }
            }
            return result;
        }
        public WinningLine CheckVerticalFromPoint(int row, int column, int playerIndex)
        {
            //variables
            WinningLine result = new WinningLine(false);
            int startingRow = row + WinLength;
            //ensure no less than 0
            if (startingRow > GameGrid.GetLength(0)-1)
            {
                startingRow = GameGrid.GetLength(0)-1;
            }
            int currentConsec = 0; //current consecutive counters
            int checkSymbol = playerIndex;

            //check for wins
            for (int currentPos = startingRow; currentPos >= 0; currentPos--)
            {
                if (GameGrid[currentPos, column] == checkSymbol)
                {
                    currentConsec++;
                }
                else
                {
                    currentConsec = 0;
                }
                if (currentConsec == WinLength)
                {
                    result.WinnerIndex = playerIndex;
                    result.IsWin = true;
                    break;
                }
            }
            return result;
        }
        public WinningLine CheckDiagonalFromPoint(int row, int column, int playerIndex)
        {
            //variables
            WinningLine result = new WinningLine(false);
            int startingRow = 0;
            int startingColumn = 0;
            int currentRow = 0;
            int currentColumn = 0;
            int currentConsec = 0;
            int checkSymbol = playerIndex;

            //check upwards diagonal
            //get starting position
            startingRow = row;
            startingColumn = column;
            if (startingRow!= GameGrid.GetLength(0) - 1 && startingColumn != 0)
            {
                do
                {
                    startingRow++;
                    startingColumn--;
                } while (startingRow < GameGrid.GetLength(0) - 1 && startingColumn > 0);
            }
            currentRow = startingRow;
            currentColumn = startingColumn;

            //check ud for wins
            while (currentRow > 0 && currentColumn < GameGrid.GetLength(1))
            {
                if (GameGrid[currentRow, currentColumn] == checkSymbol)
                {
                    currentConsec++;
                }
                else
                {
                    currentConsec = 0;
                }
                if (currentConsec == WinLength)
                {
                    result.WinnerIndex = playerIndex;
                    result.IsWin = true;
                    break;
                }
                currentRow--;
                currentColumn++;
            }

            //check downwards diagonal if result isn't already a win
            if (!result.IsWin)
            {
                //get starting position
                startingRow = row;
                startingColumn = column;
                if (startingRow != 0 && startingColumn != 0)
                {
                    do
                    {
                        startingRow--;
                        startingColumn--;
                    } while (startingRow > 0 && startingColumn > 0);
                }
                currentRow = startingRow;
                currentColumn = startingColumn;

                //check dd for wins
                while (currentRow < GameGrid.GetLength(0) && currentColumn < GameGrid.GetLength(1))
                {
                    if (GameGrid[currentRow, currentColumn] == checkSymbol)
                    {
                        currentConsec++;
                    }
                    else
                    {
                        currentConsec = 0;
                    }
                    if (currentConsec == WinLength)
                    {
                        result.WinnerIndex = playerIndex;
                        result.IsWin = true;
                        break;
                    }
                    currentRow++;
                    currentColumn++;
                }
            }

            return result;
        }
    }
    class Player
    {
        public string PlayerName { get; }
        public int CurrentScore { get; }
        public System.Windows.Media.Color PlayerColor { get; }
        public Player(string playerName, System.Windows.Media.Color playerColor)
        {
            PlayerName = playerName;
            PlayerColor = playerColor;
            CurrentScore = 0;
        }
    }
    class WinningLine
    {
        public int WinnerIndex { get; set; }
        public bool IsWin { get; set; }
        public WinningLine(int winnerIndex)
        {

            WinnerIndex = winnerIndex;
            IsWin = true;
        }
        public WinningLine(bool isWin)
        {
            IsWin = isWin;
        }
    }
}