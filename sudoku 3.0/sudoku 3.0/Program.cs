using Base_Game_Class;
using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;

namespace BotCode
{
    class Program
    {
        enum State
        {
            Menu,
            Game,
            Conclusion,
            Statistics
        }
        static int[,,] BaseDataStorage;
        static State state = State.Menu;
        static char[] integers = { '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        static Base_Game game = new Base_Game();
        static ITelegramBotClient bot = new TelegramBotClient(""); // token
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message != null && update.Message.Text != null)
            {
                var message = update.Message;
                Console.WriteLine(message.Chat.FirstName + " " + message.Chat.LastName + " @" + message.Chat.Username + ":" + message.Text);
                switch (state)
                {
                    case State.Menu:
                        {
                            if (message.Text.ToLower() == "/start")
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "Welcome!\nTo start blank game enter /start_game\n/start_game_n for game with custom difficulty, instead of n use number 1-39, higher = easier\n /stats to open heatmaps");
                            }
                            if (message.Text.ToLower() == "/start_game")
                            {
                                state = State.Game;
                                await botClient.SendTextMessageAsync(message.Chat, game.Render());
                            }
                            if (message.Text.ToLower().Contains("/start_game_"))
                            {
                                int n = message.Text.ToLower().LastIndexOf("_");
                                try { n = Convert.ToInt32(message.Text.Substring(n + 1)); }
                                catch { await botClient.SendTextMessageAsync(message.Chat, "enter a number instead of n"); break; }
                                if (n > 81) { await botClient.SendTextMessageAsync(message.Chat, "grid has less cells than that..."); break; }
                                if (n >= 40 && n <= 81) { await botClient.SendTextMessageAsync(message.Chat, "maybe leave yourself a little bit of challenge?"); break; }
                                if (n >= 0 && n <= 39) 
                                {
                                    game.EnforceDifficulty(n);
                                    state = State.Game;
                                    await botClient.SendTextMessageAsync(message.Chat, game.Render());
                                    await botClient.SendTextMessageAsync(message.Chat, "enter /write i j arg, to write value in a cell\n /prediction i j to see what numbers you can write in a cell\n/exit to exit to menu");
                                    break;
                                }
                            }
                            if (message.Text.ToLower() == "/stats") 
                            {
                                state = State.Statistics;
                                await botClient.SendTextMessageAsync(message.Chat, "Enter a number that you want to se a heatmap for, enter /to_menu , to get back to menu");
                            }
                            if (message.Text == "state")
                            {
                                await botClient.SendTextMessageAsync(message.Chat, Convert.ToString(state));
                            }
                            break;
                        }
                    case State.Game:
                        {
                            if (message.Text == "state")
                            {
                                await botClient.SendTextMessageAsync(message.Chat, Convert.ToString(state));
                            }
                            if (message.Text.Contains("/prediction"))
                            {
                                string s = message.Text.ToString().ToLower();
                                int n = s.IndexOf(' ');
                                s = s.Substring(n+1);
                                int i = Convert.ToInt32(s.Remove(0, 1));
                                n = s.IndexOf(' ');
                                int j = Convert.ToInt32(s.Substring(n+1));
                                game.Prediction();
                                s = game.PredictionRender(i, j);
                                await botClient.SendTextMessageAsync(message.Chat, $"Possible numbers for that cell {i},{j} = \"{s}\"");
                            }
                            if (message.Text.Contains("/write "))
                            {
                                int i, j, arg;
                                string s = " ";
                                s = message.Text.ToString().ToLower();
                                s = s.Substring(Convert.ToInt32(s.IndexOf(" ")+1));
                                (i, j, arg) = game.Input(s);
                                if (game.IsValid(i, j, arg) == true) { game.Write(i, j, arg); s = game.Render(); }
                                else if (game.IsValid(i, j, arg) == false) { s = "invalid input"; }
                                else if (game.Prediction() == 0) 
                                {
                                    await botClient.SendTextMessageAsync(message.Chat, "lose, to repeat send 1, to get back to menu send 0");
                                    Console.WriteLine("lose");
                                    state = State.Conclusion;
                                }
                                else if (game.Prediction() == 1) 
                                {
                                    await botClient.SendTextMessageAsync(message.Chat, "Win, to repeat send 1, to get back to menu send 0");
                                    Console.WriteLine("win");
                                    state = State.Conclusion;
                                }
                                await botClient.SendTextMessageAsync(message.Chat, s);
                            }
                            if (message.Text.ToLower().Contains("/exit")) 
                            {
                                state = State.Menu; 
                                game.Reset();
                                await botClient.SendTextMessageAsync(message.Chat, "To start blank game enter /start_game\n/start_game_n for game with custom difficulty, instead of n use number 1-39, higher = easier\n /stats to open heatmaps");
                            }
                            break;
                        }
                    case State.Conclusion:
                        {
                            if (message.Text == "1")
                            {
                                game.Reset();
                                game.EnforceDifficulty(game.Difficulty);
                                state = State.Game;
                            }
                            if (message.Text == "0")
                            {
                                game = null;
                                state = State.Menu;
                            }
                            break;
                        }
                    case State.Statistics:
                        {
                            if(message.Text.ToLower() == "/to_menu") { state = State.Menu; break; }
                            if (message.Text.IndexOfAny(integers) != -1)
                            {
                                await botClient.SendTextMessageAsync(message.Chat, R_ender(Convert.ToInt16(Convert.ToInt32(message.Text)-1)));
                            }
                            break;
                        }
                    default:
                        {
                            Console.WriteLine("state exception");
                            break;
                        }
                }
            }
        } // actions on message recive
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"base exeption + cancelation token {cancellationToken.GetType()} + exception type {exception.GetType()}");
            return;
        }// exeption handling
        public static int[,,] GetInf()
        {
            int[,,] big3dboy = new int[9, 9, 9];
            int max = 0;
            string storage;
            string[] result;
            using (var connection = new SqliteConnection("Data Source=database.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM sudoku", connection);
                SqliteCommand resultinf = new SqliteCommand("SELECT * FROM sudoku", connection);
                object nn = command.ExecuteScalar();
                int n = Convert.ToInt32(nn);
                SqliteDataReader reader = resultinf.ExecuteReader();
                string[] resultmass = new string[n];
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        resultmass[n - 1] = Convert.ToString(reader.GetValue(1));
                        n--;
                    }
                }
                reader.Close();
                connection.Close();
                result = resultmass;
            }
            for (int massn = 0; massn <= 99; massn++) // номер элемента базы данных
            {
                for (int i = 0; i < 9; i++) // строка
                {
                    for (int j = 0; j < 9; j++) // столбец
                    {
                        storage = result[massn];
                        switch (storage[i + (j * 9)])
                        {
                            case '1':
                                big3dboy[0, i, j] = 1 + big3dboy[0, i, j];
                                break;
                            case '2':
                                big3dboy[1, i, j] = 1 + big3dboy[1, i, j];
                                break;
                            case '3':
                                big3dboy[2, i, j] = 1 + big3dboy[2, i, j];
                                break;
                            case '4':
                                big3dboy[3, i, j] = 1 + big3dboy[3, i, j];
                                break;
                            case '5':
                                big3dboy[4, i, j] = 1 + big3dboy[4, i, j];
                                break;
                            case '6':
                                big3dboy[5, i, j] = 1 + big3dboy[5, i, j];
                                break;
                            case '7':
                                big3dboy[6, i, j] = 1 + big3dboy[6, i, j];
                                break;
                            case '8':
                                big3dboy[7, i, j] = 1 + big3dboy[7, i, j];
                                break;
                            case '9':
                                big3dboy[8, i, j] = 1 + big3dboy[8, i, j];
                                break;
                            default: break;
                        }
                    }
                }
            }
            for (int s = 0; s < 9; s++) // номер элемента базы данных
            {
                for (int i = 0; i < 9; i++) // строка
                {
                    for (int j = 0; j < 9; j++) // столбец
                    {
                        if (big3dboy[s, i, j] > max) { max = big3dboy[s, i, j]; }
                    }
                }
            }
            return big3dboy;
        }
        public static string R_ender(int i)
        {
            string a = 
                      $"|{BaseDataStorage[i, 0, 0]}|{BaseDataStorage[i, 0, 1]}|{BaseDataStorage[i, 0, 2]}|{BaseDataStorage[i, 0, 3]}|{BaseDataStorage[i, 0, 4]}|{BaseDataStorage[i, 0, 5]}|{BaseDataStorage[i, 0, 6]}|{BaseDataStorage[i, 0, 7]}|{BaseDataStorage[i, 0, 8]}|\n" +
                      $"|{BaseDataStorage[i, 1, 0]}|{BaseDataStorage[i, 1, 1]}|{BaseDataStorage[i, 1, 2]}|{BaseDataStorage[i, 1, 3]}|{BaseDataStorage[i, 1, 4]}|{BaseDataStorage[i, 1, 5]}|{BaseDataStorage[i, 1, 6]}|{BaseDataStorage[i, 1, 7]}|{BaseDataStorage[i, 1, 8]}|\n" +
                      $"|{BaseDataStorage[i, 2, 0]}|{BaseDataStorage[i, 2, 1]}|{BaseDataStorage[i, 2, 2]}|{BaseDataStorage[i, 2, 3]}|{BaseDataStorage[i, 2, 4]}|{BaseDataStorage[i, 2, 5]}|{BaseDataStorage[i, 2, 6]}|{BaseDataStorage[i, 2, 7]}|{BaseDataStorage[i, 2, 8]}|\n" +
                      $"|{BaseDataStorage[i, 3, 0]}|{BaseDataStorage[i, 3, 1]}|{BaseDataStorage[i, 3, 2]}|{BaseDataStorage[i, 3, 3]}|{BaseDataStorage[i, 3, 4]}|{BaseDataStorage[i, 3, 5]}|{BaseDataStorage[i, 3, 6]}|{BaseDataStorage[i, 3, 7]}|{BaseDataStorage[i, 3, 8]}|\n" +
                      $"|{BaseDataStorage[i, 4, 0]}|{BaseDataStorage[i, 4, 1]}|{BaseDataStorage[i, 4, 2]}|{BaseDataStorage[i, 4, 3]}|{BaseDataStorage[i, 4, 4]}|{BaseDataStorage[i, 4, 5]}|{BaseDataStorage[i, 4, 6]}|{BaseDataStorage[i, 4, 7]}|{BaseDataStorage[i, 4, 8]}|\n" +
                      $"|{BaseDataStorage[i, 5, 0]}|{BaseDataStorage[i, 5, 1]}|{BaseDataStorage[i, 5, 2]}|{BaseDataStorage[i, 5, 3]}|{BaseDataStorage[i, 5, 4]}|{BaseDataStorage[i, 5, 5]}|{BaseDataStorage[i, 5, 6]}|{BaseDataStorage[i, 5, 7]}|{BaseDataStorage[i, 5, 8]}|\n" +
                      $"|{BaseDataStorage[i, 6, 0]}|{BaseDataStorage[i, 6, 1]}|{BaseDataStorage[i, 6, 2]}|{BaseDataStorage[i, 6, 3]}|{BaseDataStorage[i, 6, 4]}|{BaseDataStorage[i, 6, 5]}|{BaseDataStorage[i, 6, 6]}|{BaseDataStorage[i, 6, 7]}|{BaseDataStorage[i, 6, 8]}|\n" +
                      $"|{BaseDataStorage[i, 7, 0]}|{BaseDataStorage[i, 7, 1]}|{BaseDataStorage[i, 7, 2]}|{BaseDataStorage[i, 7, 3]}|{BaseDataStorage[i, 7, 4]}|{BaseDataStorage[i, 7, 5]}|{BaseDataStorage[i, 7, 6]}|{BaseDataStorage[i, 7, 7]}|{BaseDataStorage[i, 7, 8]}|\n" +
                      $"|{BaseDataStorage[i, 8, 0]}|{BaseDataStorage[i, 8, 1]}|{BaseDataStorage[i, 8, 2]}|{BaseDataStorage[i, 8, 3]}|{BaseDataStorage[i, 8, 4]}|{BaseDataStorage[i, 8, 5]}|{BaseDataStorage[i, 8, 6]}|{BaseDataStorage[i, 8, 7]}|{BaseDataStorage[i, 8, 8]}|\n";
            return a;
        }
        static void Main(string[] args) // console 
        {
            Console.WriteLine("Started bot " + bot.GetMeAsync().Result.FirstName);
            BaseDataStorage = GetInf();
            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving
            (
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.Read();
        }
    }
}