using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TicTacToeBotConsole
{
    internal static class Program
    {
        const string tokenFileName = "token.json";

        private static TicTacToeGame _game;
        public async static Task Main(string[] args)
        {
            try
            {
                var configBuilder = new ConfigurationBuilder().AddJsonFile(tokenFileName, false, false);
                var config = configBuilder.Build();
                if (config == null)
                {
                    throw new Exception($"Read configuration error");
                }

                TelegramBotClient botClient = new TelegramBotClient(config["token"]);
                User me = await botClient.GetMeAsync();
                Console.WriteLine($"User: {me.Id}, Name: {me.Username}");

                var cts = new CancellationTokenSource();

                botClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandleErrorAsync,
                    receiverOptions: new ReceiverOptions
                    {
                        AllowedUpdates = Array.Empty<UpdateType>()
                    },
                    cancellationToken: cts.Token);

                Console.WriteLine($"Start listening for {me.Username}");
                Console.ReadKey();

                cts.Cancel();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.Read();
            }


        }

        private static Task HandleErrorAsync(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine($"Error: {exception.Message}");
            return Task.CompletedTask;
        }

        private static async Task HandleUpdateAsync(ITelegramBotClient client, Update update, CancellationToken token)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        await BotOnMessageReceived(client, update.Message);
                        break;
                    case UpdateType.CallbackQuery:
                        await BotOnCallbackQueryReceived(client, update.CallbackQuery);
                        break;
                }
            }
            catch (Exception exeption)
            {
                await HandleErrorAsync(client, exeption, token);
            }
        }

        private static async Task BotOnCallbackQueryReceived(ITelegramBotClient client, CallbackQuery? callbackQuery)
        {
            var data = callbackQuery?.Data;

            if(data?.Length == 1 && int.TryParse(data, out int move) && move >= 1 && move <= 9)
            {
                int row = (move - 1) / 3;
                int col = (move - 1) % 3;

                _game.MakeMove(row, col);
            }
            await client.AnswerCallbackQueryAsync(callbackQuery?.Id);

        }

        private static async Task BotOnMessageReceived(ITelegramBotClient client, Message message)
        {
            Console.WriteLine($"Receive message type:{message.Type}");

            if (message.Type != MessageType.Text)
                return;

            var action = message.Text.Split(' ')[0];

            switch (action)
            {
                case "/start":
                    await StartGame(client, message); 
                    break;
                default:
                    await Echo(client, message);
                    break;
            }
        }

        private static async Task Echo(ITelegramBotClient client, Message message)
        {
            if(message == null)
                return;
            await client.SendTextMessageAsync(chatId: message.Chat.Id, text:$"{message.Text}");
        }

        private static async Task StartGame(ITelegramBotClient client, Message message)
        {
            _game = new TicTacToeGame(client, message.Chat.Id);
            _game.InitializeGameBoard();

            var currentPlayer = _game.currentPlayer;

            await client.SendTextMessageAsync(
                chatId: message.Chat.Id,
                $"Player {currentPlayer}, your turn!",
                replyMarkup: _game.GetKeyboard());
        }
    }
}
