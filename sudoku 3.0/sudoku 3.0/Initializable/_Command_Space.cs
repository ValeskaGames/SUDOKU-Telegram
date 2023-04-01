using BotCode;
using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace sudoku_3._0.Initializable
{
    class Command
    {
        public static Dictionary<long, User_Instance> _data;
        public static ITelegramBotClient _botclient;
        public static void Initialize(Dictionary<long, User_Instance> data, ITelegramBotClient botclient)
        {
            _data = data;
            _botclient = botclient;
        }
        public static void User_load(Update _upd)
        {
            _Sql_Data.user_load(_upd.Message,_upd.Message.Chat.Id);
        } // loads user info from database
        public static async void Admin(long cid, string message)
        {
            if (cid != 1377091495) return; // admin chat id (can be replaced with array of admin chats from database)
            switch (message)
            {
                case "state": await _botclient.SendTextMessageAsync(cid, Convert.ToString(_data[cid].state)); break;
                case "enviroment.exit": await _botclient.SendTextMessageAsync(cid, "emergency programm closing"); Environment.Exit(1); break;
                case "programm.exit": Exit_seq(); await _botclient.SendTextMessageAsync(cid, "programm closing"); Environment.Exit(1); break;
                case "start.seq": Start_seq(); break;
            }
        } // set of admin commands
        public static async void Exit_seq()
        {
            using (var connection = new SqliteConnection("Data Source=database.db"))
            {
                connection.Open();
                long chat_id = 0;
                SqliteCommand check = new SqliteCommand($"SELECT * FROM User_Base WHERE Chat_Id = '{chat_id}'", connection);
                SqliteCommand insert;
                foreach (var a in _data)
                {
                    int i = check.ExecuteNonQuery();
                    chat_id = a.Key;
                    int difficulty = _data[chat_id].game.Difficulty;
                    string region_array = _Sql_Data.convert_to_base_2d(_data[chat_id].game.Region_Array, 3);
                    string vertical_array = _Sql_Data.convert_to_base_1d(_data[chat_id].game.Vertical_Array, 9);
                    string horisontal_array = _Sql_Data.convert_to_base_1d(_data[chat_id].game.Horizontal_Array, 9);
                    string elements = _Sql_Data.convert_to_base_2d(_data[chat_id].game.Elements, 9);
                    string elements_prediction = _Sql_Data.convert_to_base_2d(_data[chat_id].game.Elements_Prediction, 9);
                    insert = new SqliteCommand($"REPLACE INTO User_Base" +
                                                "(Chat_Id, State, Difficulty, Region_Array, Vertical_Array, Horizontal_Array, Elements, Elements_Prediction)" +
                                               $" VALUES('{chat_id}', '{Convert.ToInt32(_data[chat_id].state)}', '{difficulty}', '{region_array}', '{vertical_array}'," +
                                               $" '{horisontal_array}', '{elements}', '{elements_prediction}')", connection);
                    insert.ExecuteNonQuery();
                    await _botclient.SendTextMessageAsync(chat_id, "For now bot is going offline, sorry for inconvenience.");
                }
                connection.Close();
            }
        } // safe exit, saves all current user data and notifies them about bot going down
        public static async void Start_seq()
        {
            using (var connection = new SqliteConnection("Data Source=database.db"))
            {
                connection.Open();
                SqliteCommand allinfo = new SqliteCommand($"SELECT * FROM User_Base", connection);
                SqliteDataReader reader = allinfo.ExecuteReader();
                foreach (var a in reader)
                {
                    long cid = Convert.ToInt32(reader.GetValue(0));
                    if (!_data.ContainsKey(cid))
                    {
                        State state = new State();
                        switch (Convert.ToInt32(reader.GetValue(1)))
                        {
                            case 0: { state = State.Menu; break; }
                            case 1: { state = State.Game; break; }
                            case 2: { state = State.Conclusion; break; }
                            case 3: { state = State.Statistics; break; }
                            default: { state = State.Menu; break; }
                        }
                        int difficulty = Convert.ToInt32(reader.GetValue(2)); // difficulty
                        string[,] region_array = _Sql_Data.convert_from_base_2d(Convert.ToString(reader.GetValue(3)), 3); // region array
                        string[] vertical_array = _Sql_Data.convert_from_base_1d(Convert.ToString(reader.GetValue(4)), 9); // vertical array
                        string[] horisontal_array = _Sql_Data.convert_from_base_1d(Convert.ToString(reader.GetValue(5)), 9); // horisontal array
                        string[,] elements = _Sql_Data.convert_from_base_2d(Convert.ToString(reader.GetValue(6)), 9); // elements
                        string[,] elements_prediction = _Sql_Data.convert_from_base_2d(Convert.ToString(reader.GetValue(7)), 9); // elements prediction
                        Base_Game b = new Base_Game(difficulty, region_array, vertical_array, horisontal_array, elements, elements_prediction);
                        var inst = new User_Instance(state, b);
                        _data.Add(cid, inst);
                        await _botclient.SendTextMessageAsync(cid, "Bot is once again online!");
                    }
                }
                reader.Close();
                connection.Close();
            }
        } // safe start command, loads all user data and notifies them that bot is up
        public static SqlData[] GetInf()
        {
            var data = new SqlData[9] { new SqlData(), new SqlData(), new SqlData(), new SqlData(), new SqlData(), new SqlData(), new SqlData(), new SqlData(), new SqlData() };
            string[] result;
            using (var connection = new SqliteConnection("Data Source=database.db"))
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand("SELECT COUNT(*) FROM sudoku", connection);
                SqliteCommand resultinf = new SqliteCommand("SELECT * FROM sudoku", connection);
                var n = Convert.ToInt32(command.ExecuteScalar());
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
            foreach (var a in result)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        var f = a.Length;
                        var v = a[i + j * 9] - 48;
                        if (a[i + j * 9] - 48 > 0) data[a[i + j * 9] - 49].Add(i, j);
                    }
                }
            }
            return data;
        } // grabs all game data from database and converts it to bunch of heatmaps for specific numbers
        public static string Render(SqlData[] a, int i)
        {
            string result = "";
            result =
                      $"|{a[i].Get(0, 0)}|{a[i].Get(0, 1)}|{a[i].Get(0, 2)}|{a[i].Get(0, 3)}|{a[i].Get(0, 4)}|{a[i].Get(0, 5)}|{a[i].Get(0, 6)}|{a[i].Get(0, 7)}|{a[i].Get(0, 8)}|\n" +
                      $"|{a[i].Get(1, 0)}|{a[i].Get(1, 1)}|{a[i].Get(1, 2)}|{a[i].Get(1, 3)}|{a[i].Get(1, 4)}|{a[i].Get(1, 5)}|{a[i].Get(1, 6)}|{a[i].Get(1, 7)}|{a[i].Get(1, 8)}|\n" +
                      $"|{a[i].Get(2, 0)}|{a[i].Get(2, 1)}|{a[i].Get(2, 2)}|{a[i].Get(2, 3)}|{a[i].Get(2, 4)}|{a[i].Get(2, 5)}|{a[i].Get(2, 6)}|{a[i].Get(2, 7)}|{a[i].Get(2, 8)}|\n" +
                      $"|{a[i].Get(3, 0)}|{a[i].Get(3, 1)}|{a[i].Get(3, 2)}|{a[i].Get(3, 3)}|{a[i].Get(3, 4)}|{a[i].Get(3, 5)}|{a[i].Get(3, 6)}|{a[i].Get(3, 7)}|{a[i].Get(3, 8)}|\n" +
                      $"|{a[i].Get(4, 0)}|{a[i].Get(4, 1)}|{a[i].Get(4, 2)}|{a[i].Get(4, 3)}|{a[i].Get(4, 4)}|{a[i].Get(4, 5)}|{a[i].Get(4, 6)}|{a[i].Get(4, 7)}|{a[i].Get(4, 8)}|\n" +
                      $"|{a[i].Get(5, 0)}|{a[i].Get(5, 1)}|{a[i].Get(5, 2)}|{a[i].Get(5, 3)}|{a[i].Get(5, 4)}|{a[i].Get(5, 5)}|{a[i].Get(5, 6)}|{a[i].Get(5, 7)}|{a[i].Get(5, 8)}|\n" +
                      $"|{a[i].Get(6, 0)}|{a[i].Get(6, 1)}|{a[i].Get(6, 2)}|{a[i].Get(6, 3)}|{a[i].Get(6, 4)}|{a[i].Get(6, 5)}|{a[i].Get(6, 6)}|{a[i].Get(6, 7)}|{a[i].Get(6, 8)}|\n" +
                      $"|{a[i].Get(7, 0)}|{a[i].Get(7, 1)}|{a[i].Get(7, 2)}|{a[i].Get(7, 3)}|{a[i].Get(7, 4)}|{a[i].Get(7, 5)}|{a[i].Get(7, 6)}|{a[i].Get(7, 7)}|{a[i].Get(7, 8)}|\n" +
                      $"|{a[i].Get(8, 0)}|{a[i].Get(8, 1)}|{a[i].Get(8, 2)}|{a[i].Get(8, 3)}|{a[i].Get(8, 4)}|{a[i].Get(8, 5)}|{a[i].Get(8, 6)}|{a[i].Get(8, 7)}|{a[i].Get(8, 8)}|";
            return result;
        } // grid for heatmaps
        public static string Render(string[,] a)
        {
            string result = "";
            result = $"|{a[0, 0]}|{a[0, 1]}|{a[0, 2]}|{a[0, 3]}|{a[0, 4]}|{a[0, 5]}|{a[0, 6]}|{a[0, 7]}|{a[0, 8]}|\n" +
                        $"|{a[1, 0]}|{a[1, 1]}|{a[1, 2]}|{a[1, 3]}|{a[1, 4]}|{a[1, 5]}|{a[1, 6]}|{a[1, 7]}|{a[1, 8]}|\n" +
                        $"|{a[2, 0]}|{a[2, 1]}|{a[2, 2]}|{a[2, 3]}|{a[2, 4]}|{a[2, 5]}|{a[2, 6]}|{a[2, 7]}|{a[2, 8]}|\n" +
                        $"|{a[3, 0]}|{a[3, 1]}|{a[3, 2]}|{a[3, 3]}|{a[3, 4]}|{a[3, 5]}|{a[3, 6]}|{a[3, 7]}|{a[3, 8]}|\n" +
                        $"|{a[4, 0]}|{a[4, 1]}|{a[4, 2]}|{a[4, 3]}|{a[4, 4]}|{a[4, 5]}|{a[4, 6]}|{a[4, 7]}|{a[4, 8]}|\n" +
                        $"|{a[5, 0]}|{a[5, 1]}|{a[5, 2]}|{a[5, 3]}|{a[5, 4]}|{a[5, 5]}|{a[5, 6]}|{a[5, 7]}|{a[5, 8]}|\n" +
                        $"|{a[6, 0]}|{a[6, 1]}|{a[6, 2]}|{a[6, 3]}|{a[6, 4]}|{a[6, 5]}|{a[6, 6]}|{a[6, 7]}|{a[6, 8]}|\n" +
                        $"|{a[7, 0]}|{a[7, 1]}|{a[7, 2]}|{a[7, 3]}|{a[7, 4]}|{a[7, 5]}|{a[7, 6]}|{a[7, 7]}|{a[7, 8]}|\n" +
                        $"|{a[8, 0]}|{a[8, 1]}|{a[8, 2]}|{a[8, 3]}|{a[8, 4]}|{a[8, 5]}|{a[8, 6]}|{a[8, 7]}|{a[8, 8]}|";
            return result;
        } // grid for games
        public static async Task Send(long cid,string message)
        {
            await _botclient.SendTextMessageAsync(cid, message);
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Green;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.WriteLine(message);
            Console.ResetColor();
            Console.WriteLine();
        }
        public static List<int> Parse(string arg)
        {
            arg += " "; arg = arg.Substring(arg.IndexOf(' ') + 1);
            List<int> result = new List<int> { };
            while (arg.Length > 1)
            {
                int a = Convert.ToInt32(arg.Remove(arg.IndexOf(" ")));
                result.Add(a);
                arg = arg.Substring(arg.IndexOf(" ")).TrimStart();
            }
            return result;
        }

    }
}
