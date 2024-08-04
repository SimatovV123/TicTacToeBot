using System;

namespace TicTacToe
{
    class Program
    {
        static char[,] gameBoard = new char[3, 3];
        static char currentPlayer = 'X';

        static void Main(string[] args)
        {
            InitializeGameBoard();
            PlayGame();
        }

        static void InitializeGameBoard()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    gameBoard[i, j] = ' ';
                }
            }
        }

        static void PlayGame()
        {
            bool gameOver = false;
            while (!gameOver)
            {
                DrawGameBoard();
                MakeMove();
                gameOver = CheckWin() || CheckDraw();
                if (gameOver) break;
                TogglePlayer();
            }
            DrawGameBoard();
            if (CheckWin())
            {
                Console.WriteLine($"Player {currentPlayer} wins!");
            }
            else
            {
                Console.WriteLine("It's a draw!");
            }
        }

        static void DrawGameBoard()
        {
            Console.Clear();
            Console.WriteLine("   0   1   2");
            Console.WriteLine("0  {0} | {1} | {2}", gameBoard[0, 0], gameBoard[0, 1], gameBoard[0, 2]);
            Console.WriteLine("  ---+---+---");
            Console.WriteLine("1  {0} | {1} | {2}", gameBoard[1, 0], gameBoard[1, 1], gameBoard[1, 2]);
            Console.WriteLine("  ---+---+---");
            Console.WriteLine("2  {0} | {1} | {2}", gameBoard[2, 0], gameBoard[2, 1], gameBoard[2, 2]);
        }

        static void MakeMove()
        {
            Console.WriteLine($"Player {currentPlayer}, enter your move (row column): ");
            string input = Console.ReadLine();
            string[] coordinates = input.Split(' ');
            int row = int.Parse(coordinates[0]);
            int col = int.Parse(coordinates[1]);
            if (gameBoard[row, col] == ' ')
            {
                gameBoard[row, col] = currentPlayer;
            }
            else
            {
                Console.WriteLine("That square is already taken. Try again.");
                MakeMove();
            }
        }

        static bool CheckWin()
        {
            // Check rows
            for (int i = 0; i < 3; i++)
            {
                if (gameBoard[i, 0] == currentPlayer && gameBoard[i, 1] == currentPlayer && gameBoard[i, 2] == currentPlayer)
                {
                    return true;
                }
            }
            // Check columns
            for (int i = 0; i < 3; i++)
            {
                if (gameBoard[0, i] == currentPlayer && gameBoard[1, i] == currentPlayer && gameBoard[2, i] == currentPlayer)
                {
                    return true;
                }
            }
            // Check diagonals
            if (gameBoard[0, 0] == currentPlayer && gameBoard[1, 1] == currentPlayer && gameBoard[2, 2] == currentPlayer)
            {
                return true;
            }
            if (gameBoard[0, 2] == currentPlayer && gameBoard[1, 1] == currentPlayer && gameBoard[2, 0] == currentPlayer)
            {
                return true;
            }
            return false;
        }

        static bool CheckDraw()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (gameBoard[i, j] == ' ')
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        static void TogglePlayer()
        {
            currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';
        }
    }
}