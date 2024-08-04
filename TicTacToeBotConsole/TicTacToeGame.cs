using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TicTacToeBotConsole
{
    internal class TicTacToeGame
    {
        static char[,] gameBoard = new char[3, 3];
        public char currentPlayer = 'X';

        private ITelegramBotClient _client;
        private long _chatId;

        public TicTacToeGame(ITelegramBotClient client, long chatId)
        {
            _client = client;
            _chatId = chatId;
        }

        public void InitializeGameBoard()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    gameBoard[i, j] = ' ';
                }
            }
        }

        public async void MakeMove(int row, int col)
        {
                
            if (gameBoard[row, col] == ' ')
            {
                gameBoard[row, col] = currentPlayer;

                if (CheckWin())
                {
                    InitializeGameBoard();
                    await _client.SendTextMessageAsync(_chatId, $"Player {currentPlayer} wins!", replyMarkup: GetKeyboard());
                }
                else if (CheckDraw())
                {
                    await _client.SendTextMessageAsync(_chatId, $"It's a draw!", replyMarkup: GetKeyboard());
                }
                else
                {
                    TogglePlayer();

                    await _client.SendTextMessageAsync(_chatId, $"Player {currentPlayer}, you turn", replyMarkup: GetKeyboard());

                }
            }
            else
            {
                await _client.SendTextMessageAsync(_chatId, $"This square is already taken", replyMarkup: GetKeyboard());
            }
        }

        internal InlineKeyboardMarkup GetKeyboard()
        {
            InlineKeyboardButton[][]buttons = new InlineKeyboardButton[3][];
            for (int i = 0; i < 3; i++)
            {
                buttons[i] = new InlineKeyboardButton[3];
                for(int j = 0; j < 3; j++)
                {
                    int move = i * 3 + j + 1;
                    buttons[i][j] = InlineKeyboardButton.WithCallbackData(gameBoard[i, j] == ' ' ? move.ToString() : gameBoard[i, j].ToString(), move.ToString());
                }
            }
            return new InlineKeyboardMarkup(buttons);
        }

        bool CheckWin()
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

        void TogglePlayer()
        {
            currentPlayer = (currentPlayer == 'X') ? 'O' : 'X';
        }
    }
}
