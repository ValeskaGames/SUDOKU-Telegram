using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using System.Drawing;
using System.Reflection.Metadata.Ecma335;

namespace BotCode
{
    enum State
    {
        Menu,
        Game,
        Conclusion,
        Log,
        Statistics
    }
    enum Verticals
    {
        A = 1,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I = 9,
    }
    class Program
    {
        public static State state = State.Menu;
        public static Base_Game game = new Base_Game();
        static ITelegramBotClient bot = new TelegramBotClient(""); // token
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            
            if (update.Message != null && update.Message.Text != null)
            {
                var message = update.Message;
                Console.WriteLine(message.Chat.Id + ":" + message.Text);
                switch (state)
                {
                    case State.Menu:
                        {
                            if (message.Text.ToLower() == "/start")
                            {
                                await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать!\nДля начала игры напипише /start_game\n/start_game_n для выбора сложности, чем больше n, тем больше клеток будет заполнено.");
                            }
                            if (message.Text.ToLower() == "/start_game")
                            {
                                state = State.Game;
                                await botClient.SendTextMessageAsync(message.Chat, game.Render());
                            }
                            if (message.Text.ToLower().Contains("/start_game_n"))
                            {
                                break;
                            }
                            if (message.Text.ToLower().Contains("/start_game_"))
                            {
                                int n = message.Text.ToLower().LastIndexOf("_");
                                n = Convert.ToInt32(message.Text.Substring(n + 1));
                                if (n > 81) {await botClient.SendTextMessageAsync(message.Chat, "В сетке даже столько ячеек нет..."); break; }
                                if (n >= 40 && n <= 81) { await botClient.SendTextMessageAsync(message.Chat, "Не хорошо задавать такую низкую сложность :)"); }
                                if (n >= 0) { game.EnforceDifficulty(n); state = State.Game; await botClient.SendTextMessageAsync(message.Chat, game.Render()); }
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
                            if (message.Text != null)
                            {
                                int i, j, arg;
                                string s = " ";
                                (i, j, arg) = game.Input(message.Text);
                                if (game.IsValid(i, j, arg) == true) { game.Write(i, j, arg); s = game.Render(); }
                                else if (game.IsValid(i, j, arg) == false) { s = "invalid input"; }
                                else if (game.Prediction() == 0) { Console.Write("lose"); state = State.Conclusion; }
                                else if (game.Prediction() == 1) { Console.Write("Win"); state = State.Conclusion; }
                                await botClient.SendTextMessageAsync(message.Chat, s);
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
            Console.WriteLine("base exeption");
        }// exeption handling
        static void Main(string[] args) // console 
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);
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
            for(; ; )
            {
                int cid = 1377091495;
                string s = Console.ReadLine();
                string c = s.Remove(s.IndexOf(" ")).Trim();
                if(c == "саня") { cid = 1583017092; }
                else if(c == "я") { cid = 1377091495; }
                else if(c == "владик") { cid = 904653772; }
                s = s.Substring(s.IndexOf(" ")).Trim();
                bot.SendTextMessageAsync(Convert.ToInt32(cid),s);
            }
        }
    }
    public class Base_Game
    {
        public int Difficulty = 0;
        string [,] Region_Array = new string[3, 3]; // box  array +
        string[] Vertical_Array = new string[9]; // collum array +
        string[] Horizontal_Array = new string[9]; // row array +
        int[,] Elements = new int[9, 9] ; // basic array +
        string[,] Elements_Prediction = new string[9, 9]; // possible numbers for each cell + win/lose conditions +
        public Base_Game()
        {
            Difficulty = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Region_Array[i, j] = "";
                }
            }
            for (int i = 0; i < 9; i++)
            {
                Horizontal_Array[i] = "";
                Vertical_Array[i] = "";
            }
            if (Difficulty > 0) { EnforceDifficulty(Difficulty); }
            Prediction();
            Render();
        } // programm start + nullyfing of the arrays +
        public (int i, int j, int arg) Input(string a)
        {
            a.Trim();
            int _i = Convert.ToInt32(a.Remove(a.IndexOf(" "))) - 1;
            a = a.Substring(2);
            a.TrimStart();
            int _j = Convert.ToInt32(a.Remove(a.IndexOf(" "))) - 1;
            a = a.Substring(2);
            a.TrimStart();
            int _arg = Convert.ToInt32(a);
            return (_i, _j, _arg);
        }
        public void Write(int i, int j, int arg)
        {
            if (IsValid(i, j, arg) == true && Elements[i, j] == 0)
            {
                Elements[i, j] = arg; 
                Region_Array[i / 3, j / 3] += arg; Horizontal_Array[i] += arg; Vertical_Array[j] += arg;
            }
            else { Error(); }
            Render();
        } // setting the value in cell +
        public bool IsValid(int i, int j, int arg)
        {
            if (Horizontal_Array[i].Contains(arg.ToString())) { return false; }
            if (Vertical_Array[j].Contains(arg.ToString())) { return false; }
            if (Region_Array[i / 3, j / 3].Contains(arg.ToString())) { return false; }
            if (Elements[i,j] != 0) { return false; }
            return true;
        } // checking if value is valid according to the rules +
        public void Error()
        {
           /* 
              owo wats this
              you not suppose to be
              hewe
           */
        } // red blink of the screen
        public void Reset()
        {
            Difficulty = 0;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Region_Array[i, j] = "";
                }
            }
            for (int i = 0; i < 9; i++)
            {
                Horizontal_Array[i] = "";
                Vertical_Array[i] = "";
                for(int j = 0; j < 9; j++)
                {
                    Elements[i, j] = 0;
                    Elements_Prediction[i, j] = " ";
                }
            }
        } // nullify all arrays +
        public int Prediction()
        {
            int Counter_w = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Elements[i, j] != 0)
                    {
                        int Counter_l = 0;
                        for (int a = 1; a < 10; a++)
                        {
                            if (IsValid(i, j, a) == true) { Elements_Prediction[i, j] += Convert.ToString(a); }
                            else { Counter_l++; }
                        }
                        if (Counter_l == 9) { return (0); }
                    }
                    else { Counter_w++; }
                }
            }
            if (Counter_w == 81) { return (1); }
            return(-1);
        } // providing possible values in boxes +
        public string Render()
        {
            string a = "*******************\n" +
                      $"*{Elements[0, 0]}*{Elements[0, 1]}*{Elements[0, 2]}*{Elements[0, 3]}*{Elements[0, 4]}*{Elements[0, 5]}*{Elements[0, 6]}*{Elements[0, 7]}*{Elements[0, 8]}*\n" +
                      $"*{Elements[1, 0]}*{Elements[1, 1]}*{Elements[1, 2]}*{Elements[1, 3]}*{Elements[1, 4]}*{Elements[1, 5]}*{Elements[1, 6]}*{Elements[1, 7]}*{Elements[1, 8]}*\n" +
                      $"*{Elements[2, 0]}*{Elements[2, 1]}*{Elements[2, 2]}*{Elements[2, 3]}*{Elements[2, 4]}*{Elements[2, 5]}*{Elements[2, 6]}*{Elements[2, 7]}*{Elements[2, 8]}*\n" +
                      $"*{Elements[3, 0]}*{Elements[3, 1]}*{Elements[3, 2]}*{Elements[3, 3]}*{Elements[3, 4]}*{Elements[3, 5]}*{Elements[3, 6]}*{Elements[3, 7]}*{Elements[3, 8]}*\n" +
                      $"*{Elements[4, 0]}*{Elements[4, 1]}*{Elements[4, 2]}*{Elements[4, 3]}*{Elements[4, 4]}*{Elements[4, 5]}*{Elements[4, 6]}*{Elements[4, 7]}*{Elements[4, 8]}*\n" +
                      $"*{Elements[5, 0]}*{Elements[5, 1]}*{Elements[5, 2]}*{Elements[5, 3]}*{Elements[5, 4]}*{Elements[5, 5]}*{Elements[5, 6]}*{Elements[5, 7]}*{Elements[5, 8]}*\n" +
                      $"*{Elements[6, 0]}*{Elements[6, 1]}*{Elements[6, 2]}*{Elements[6, 3]}*{Elements[6, 4]}*{Elements[6, 5]}*{Elements[6, 6]}*{Elements[6, 7]}*{Elements[6, 8]}*\n" +
                      $"*{Elements[7, 0]}*{Elements[7, 1]}*{Elements[7, 2]}*{Elements[7, 3]}*{Elements[7, 4]}*{Elements[7, 5]}*{Elements[7, 6]}*{Elements[7, 7]}*{Elements[7, 8]}*\n" +
                      $"*{Elements[8, 0]}*{Elements[8, 1]}*{Elements[8, 2]}*{Elements[8, 3]}*{Elements[8, 4]}*{Elements[8, 5]}*{Elements[8, 6]}*{Elements[8, 7]}*{Elements[8, 8]}*\n" +
                      $"*******************";

            return a;
        } // Returns nice table string
        public void EnforceDifficulty(int Difficulty)
        {
            Random r = new Random();
            int i, j, v;
            while (Difficulty != 0)
            {
                i = r.Next(0, 9);
                j = r.Next(0, 9);
                v = r.Next(1, 9);
                if (IsValid(i, j, v) == true)
                {
                    Elements[i, j] = v; Difficulty--;
                    Region_Array[i / 3, j / 3] += v; Horizontal_Array[i] += v; Vertical_Array[j] += v;
                }
            }
        } // adding some numbers for start +
    }
}