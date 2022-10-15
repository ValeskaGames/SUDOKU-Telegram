using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using System.Drawing;

namespace BotCode
{
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
        static ITelegramBotClient bot = new TelegramBotClient(""); // token
        public static async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            // actions on message recive
            Console.WriteLine($"{update.Message.From.Username}:{update.Message.Text}");
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {
                var message = update.Message;
                if (message.Text.ToLower() == "/start")
                {
                    await botClient.SendTextMessageAsync(message.Chat, "Добро пожаловать!");
                    return;
                }
                await botClient.SendTextMessageAsync(message.Chat, $"{message.Text}");
                Base_Game buba = new Base_Game(10);
                botClient.SendTextMessageAsync(message.Chat, buba.Render());
                //bot.DeleteMessageAsync(message.Chat.Id, message.MessageId, default);
                //bot.EditMessageTextAsync(message.Chat.Id , message.MessageId, "_-_-_-");
            }
        }
        public static async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            // exeption handling
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
        static void Main(string[] args) // console 
        {
            Console.WriteLine("Запущен бот " + bot.GetMeAsync().Result.FirstName);

            var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;
            var receiverOptions = new ReceiverOptions
            {
                AllowedUpdates = { }, // receive all update types
            };
            bot.StartReceiving(
                HandleUpdateAsync,
                HandleErrorAsync,
                receiverOptions,
                cancellationToken
            );
            Console.ReadLine();
        }
    }
    public class Base_Game
    {
        public int Difficulty = 0;
        string [,] Region_Array = new string[3, 3]; // box  array +
        string[] Vertical_Array = new string[9]; // collum array +
        string[] Horizontal_Array = new string[9]; // row array +
        int[,] Elements = new int[9, 9]; // basic array +
        string[,] Elements_Prediction = new string[9, 9]; // possible numbers for each cell + win/lose conditions +
        public Base_Game()
        {
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
            if (Difficulty > 0) { Enforce_Difficulty(Difficulty); }
            Prediction();
            Render();
        } // programm start + nullyfing of the arrays +
        public Base_Game(int d)
        {
            Difficulty = d;
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
            if (Difficulty > 0) { Enforce_Difficulty(Difficulty); }
            Prediction();
            Render();
        } // programm start + nullyfing of the arrays + custom difficulty (more = easier)
        public void Write(int i, int j, int arg)
        {
            if (Is_Valid(i, j, arg) == true && Elements[i, j] == null)
            {
                Elements[i, j] = arg; 
                Region_Array[i / 3, j / 3] += arg; Horizontal_Array[i] += arg; Vertical_Array[j] += arg;
            }
            else { Error(); }
            Prediction();
            Render();
        } // setting the value in box +
        public bool Is_Valid(int i, int j, int arg)
        {
            if (Horizontal_Array[i].Contains(arg.ToString())) { return false; }
            if (Vertical_Array[j].Contains(arg.ToString())) { return false; }
            if (Region_Array[i / 3, j / 3].Contains(arg.ToString())) { return false; }
            return true;
        } // checking if value is valid according to the rules +
        public async void Error()
        {
           /* 
              owo wats this
              you not suppose to be
              hewe
           */
        } // red blink of the screen
        public void Ending(bool arg)
        {
            /* 
              owo wats this
              you not suppose to be
              hewe
           */
        } // win/lose condition
        public void Reset()
        {
            Region_Array = null;
            Vertical_Array = null;
            Horizontal_Array = null;
            Elements = null;
            Elements_Prediction = null;
        } // nullify all arrays +
        public void Prediction()
        {
            int Counter_w = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (Elements[i, j] != null)
                    {
                        int Counter_l = 0;
                        for (int a = 1; a < 10; a++)
                        {
                            if (Is_Valid(i, j, a) == true) { Elements_Prediction[i, j] += Convert.ToString(a); }
                            else { Counter_l++; }
                        }
                        if (Counter_l == 9) { Ending(false); }
                    }
                    else { Counter_w++; }
                }
            }
            if (Counter_w == 81) { Ending(true); }
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
        public void Enforce_Difficulty(int Difficulty)
        {
            Random r = new Random();
            int i, j, v;
            while (Difficulty != 0)
            {
                i = r.Next(0, 9);
                j = r.Next(0, 9);
                v = r.Next(1, 9);
                if (Is_Valid(i, j, v) == true)
                {
                    Elements[i, j] = v; Difficulty--;
                    Region_Array[i / 3, j / 3] += v; Horizontal_Array[i] += v; Vertical_Array[j] += v;
                }
            }
        } // adding some numbers for start +
    }
}