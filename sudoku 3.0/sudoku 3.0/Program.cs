using _User_Instance;
using _Command_Space;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Datastorage;

namespace BotCode
{
    enum State
    {
        Menu,
        Game,
        Conclusion,
        Statistics
    } // little simplification for menu navigation
    class Program
    {
        static Data[] heatmaps= new Data[9];
        static char[] integers = { '1', '2', '3', '4', '5', '6', '7', '8', '9' }; // allowed characters for the game
        static ITelegramBotClient bot = new TelegramBotClient("5668094294:AAH7kMNpgn7usiujgD99qdKFGC6P8i5aPDE"); // token
        static Dictionary<long,User_Instance> data = new Dictionary<long,User_Instance> { }; // enables work for multiple users
        public static async Task HandleUpdateAsync(ITelegramBotClient botclient, Update update, CancellationToken cancellationToken)
        {
            if (update.Message == null || update.Message.Text == null) return;
            var message = update.Message;
            long cid = message.Chat.Id;
            string text = message.Text.ToLower();
            Console.WriteLine(message.Chat.FirstName + " " + message.Chat.LastName + " @" + message.Chat.Username + ":" + message.Text);
            if (!data.ContainsKey(cid)) Command.User_load(data, message);
            switch (data[cid].state)
            {
                case State.Menu:
                    {
                        switch(text)
                        {
                            case "/start": await Command.Send(botclient,message.Chat, "Welcome!\nTo start blank game enter /start_game\n/start_game_n for game with custom difficulty, instead of n use number 1-39, higher = easier\n /stats to open heatmaps"); 
                                break;
                            case "/start_game":
                                data[cid].state = State.Game;
                                await Command.Send(botclient,message.Chat, Command.Render(data[cid].game.Elements));
                                await Command.Send(botclient,message.Chat, "enter /write i j arg, to write value in a cell\n /prediction i j to see what numbers you can write in a cell\n/exit to exit to menu");
                                break;
                            case "/stats":
                                heatmaps = Command.GetInf();
                                data[cid].state = State.Statistics;
                                await Command.Send(botclient,message.Chat, "Enter a number that you want to se a heatmap for, enter /to_menu , to get back to menu");
                                break;
                            default: break;
                        }
                        if (text.Contains("/start_game_"))
                        {
                            int n;
                            try {  n = Convert.ToInt32(text.Replace("/start_game_", "")); }
                            catch { await Command.Send(botclient,message.Chat, "enter a number instead of n"); break; }
                            if (n > 81) { await Command.Send(botclient,message.Chat, "grid has less cells than that..."); break; }
                            if (n >= 40 && n <= 81) { await Command.Send(botclient,message.Chat, "maybe leave yourself a little bit of challenge?"); break; }
                            if (n >= 0 && n <= 39)
                            {
                                data[cid].game.EnforceDifficulty(n);
                                data[cid].state = State.Game;
                                await Command.Send(botclient,message.Chat, Command.Render(data[cid].game.Elements));
                                await Command.Send(botclient,message.Chat, "enter /write i j arg, to write value in a cell\n /prediction i j to see what numbers you can write in a cell\n/exit to exit to menu");
                                break;
                            }
                        }
                        Command.Admin(message, botclient, data);
                        break;
                    }
                case State.Game:
                    {
                        if (text.Contains("/prediction"))
                        {
                            try
                            {
                                data[cid].game.Prediction();
                                string a = text.Replace("/prediction", "");
                                List<int> v = Command.Parse(a);
                                int i = v[0]; int j = v[1];
                                a = data[cid].game.PredictionRender(i, j);
                                await Command.Send(botclient,message.Chat, $"Possible numbers for that cell {i},{j} = \"{a}\"");
                            }
                            catch
                            {
                                await Command.Send(botclient,message.Chat, "there is an error, please try again");
                                break;
                            }
                        }
                        if (text.Contains("/write "))
                        {
                            try
                            {
                                string s = text.Replace("/write", "");
                                var v = Command.Parse(s);
                                int i = v[0] - 1, j = v[1] - 1, arg = v[2];
                                if (data[cid].game.IsValid(i, j, arg) == true)
                                {
                                    data[cid].game.Write(i, j, Convert.ToString(arg));
                                    s = Command.Render(data[cid].game.Elements);
                                }
                                else if (data[cid].game.IsValid(i, j, arg) == false) { s = "invalid input"; }
                                else if (data[cid].game.Prediction() == 0)
                                {
                                    await Command.Send(botclient,message.Chat, "lose, to repeat send 1, to get back to menu send 0");
                                    Console.WriteLine("lose");
                                    data[cid].game.Save(false, message);
                                    data[cid].state = State.Conclusion;
                                }
                                else if (data[cid].game.Prediction() == 1)
                                {
                                    await Command.Send(botclient,message.Chat, "Win, to repeat send 1, to get back to menu send 0");
                                    Console.WriteLine("win");
                                    data[cid].game.Save(true, message);
                                    data[cid].state = State.Conclusion;
                                }
                                await Command.Send(botclient,message.Chat, s);
                            }
                            catch
                            {
                                await Command.Send(botclient,message.Chat, "there is an error, try again");
                                break;

                            }
                        }
                        if (text.Contains("/exit"))
                        {
                            data[cid].state = State.Menu;
                            data[cid].game.Reset();
                            await Command.Send(botclient,message.Chat, "To start blank game enter /start_game\n/start_game_n for game with custom difficulty, instead of n use number 1-39, higher = easier\n /stats to open heatmaps");
                        }
                        Command.Admin(message, botclient, data);
                        break;
                    }
                case State.Conclusion:
                    {
                        if (text == "1")
                        {
                            data[cid].game.Reset();
                            data[cid].game.EnforceDifficulty(data[cid].game.Difficulty);
                            data[cid].state = State.Game;
                        }
                        if (text == "0")
                        {
                            data[cid].game = null;
                            data[cid].state = State.Menu;
                        }
                        Command.Admin(message, botclient, data);
                        break;
                    }
                case State.Statistics:
                    {
                        if (text == "/to_menu") { data[cid].state = State.Menu; break; }
                        if (text.IndexOfAny(integers) != -1 && text.Length == 1)
                        {
                            await Command.Send(botclient,message.Chat, Command.Render(heatmaps, Convert.ToInt16(Convert.ToInt32(text) - 1)));
                        }
                        Command.Admin(message, botclient, data);
                        break;
                    }
            }
        } // actions on message recive
        public static async Task HandleErrorAsync(ITelegramBotClient botclient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine($"base exeption + cancelation token {cancellationToken.GetType()} + exception type {exception.GetType()}");
            return;
        }// exeption handling
        static void Main(string[] args) // bot initialisation 
        {
            Console.WriteLine("Started bot " + bot.GetMeAsync().Result.FirstName);
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
            Console.ReadLine();
        }
    }
}