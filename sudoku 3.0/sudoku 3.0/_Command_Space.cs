using _SOA;
using _Base_Game_Class;
using BotCode;
using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace _Command_Space
{
    class Command
    {
        public static void User_load(Dictionary<long, SOA> data, Message message)
        {
            if (data.ContainsKey(message.Chat.Id) == false)
            {
                int cid = Convert.ToInt32(message.Chat.Id);
                using (var connection = new SqliteConnection("Data Source=database.db"))
                {
                    connection.Open();
                    SqliteCommand check = new SqliteCommand($"SELECT * FROM User_Base WHERE Chat_Id = '{cid}'", connection);
                    int _check = check.ExecuteNonQuery();
                    if (_check == 1)
                    {
                        SqliteCommand resultinf = new SqliteCommand($"SELECT * FROM User_Base WHERE Char_Id = '{cid}'", connection);
                        SqliteDataReader reader = resultinf.ExecuteReader();
                        if (reader.HasRows)
                        {
                            State a = new State();
                            switch (Convert.ToInt32(reader.GetValue(1)))
                            {
                                case 0: { a = State.Menu; break; }
                                case 1: { a = State.Game; break; }
                                case 2: { a = State.Conclusion; break; }
                                case 3: { a = State.Statistics; break; }
                                default: { a = State.Menu; break; }
                            }
                            int d = Convert.ToInt32(reader.GetValue(2)); // difficulty
                            string[,] ra = SOA.R_c(Convert.ToString(reader.GetValue(3))); // region array
                            string[] va = SOA.VH_c(Convert.ToString(reader.GetValue(4))); // vertical array
                            string[] ha = SOA.VH_c(Convert.ToString(reader.GetValue(5))); // horisontal array
                            int[,] e = SOA.E_c(Convert.ToString(reader.GetValue(6))); // elements
                            string[,] ep = SOA.EP_c(Convert.ToString(reader.GetValue(7))); // elements prediction
                            Base_Game b = new Base_Game(d, ra, va, ha, e, ep);
                            var c = new SOA(a, b);
                            data.Add(message.Chat.Id, c);
                        }
                        reader.Close();
                    }
                    else
                    {
                        Base_Game b = new Base_Game();
                        SOA a = new SOA(State.Menu, b);
                        data.Add(message.Chat.Id, a);
                    }
                    connection.Close();
                }
            }
        } // loads user info from database
        public static async void Admin(Message message, ITelegramBotClient botclient, Dictionary<long, SOA> data)
        {
            string text = message.Text.ToLower();
            if (message.Chat.Id == 1377091495) // admin chat id (can be replaced with array of admin chats from database)
            {
                if (text == "state") { await botclient.SendTextMessageAsync(message.Chat, Convert.ToString(data[message.Chat.Id].state)); }
                if (text == "enviroment.exit") { await botclient.SendTextMessageAsync(message.Chat, "emergency programm closing"); Environment.Exit(1); }
                if (text == "program.exit") { Exit_seq(botclient, data); await botclient.SendTextMessageAsync(message.Chat, "programm closing"); Environment.Exit(1); }
                if (text == "start.seq") { Command.Start_seq(botclient, data); }
            }
        } // set of admin commands
        public static async void Exit_seq(ITelegramBotClient botclient, Dictionary<long, SOA> data)
        {
            using (var connection = new SqliteConnection("Data Source=database.db"))
            {
                connection.Open();
                long cid = 0; // chat id
                int d = 0; // difficulty
                string ra; // region array
                string va; // vertical array
                string ha; // horisontal array
                string e; // elements
                string ep; // elements prediction
                SqliteCommand check = new SqliteCommand($"SELECT * FROM User_Base WHERE Chat_Id = '{cid}'", connection);
                SqliteCommand insert;
                foreach (var a in data)
                {
                    int i = check.ExecuteNonQuery();
                    cid = a.Key;
                    d = data[cid].game.Difficulty;
                    ra = SOA.R_b(data[cid].game.Region_Array);
                    va = SOA.VH_b(data[cid].game.Vertical_Array);
                    ha = SOA.VH_b(data[cid].game.Horizontal_Array);
                    e = SOA.E_b(data[cid].game.Elements);
                    ep = SOA.E_b(data[cid].game.Elements_Prediction);
                    insert = new SqliteCommand($"REPLACE INTO User_Base" +
                                                "(Chat_Id, State, Difficulty, Region_Array, Vertical_Array, Horizontal_Array, Elements, Elements_Prediction)" +
                                               $" VALUES('{cid}', '{Convert.ToInt32(data[cid].state)}', '{d}', '{ra}', '{va}', '{ha}', '{e}', '{ep}')", connection);
                    insert.ExecuteNonQuery();
                    await botclient.SendTextMessageAsync(cid, "For now bot is going offline, sorry for inconvenience.");
                }
                connection.Close();
            }
        } // safe exit, saves all current user data and notifies them about bot going down
        public static async void Start_seq(ITelegramBotClient botclient, Dictionary<long, SOA> data)
        {
            using (var connection = new SqliteConnection("Data Source=database.db"))
            {
                connection.Open();
                SqliteCommand allinf = new SqliteCommand($"SELECT * FROM User_Base", connection);
                SqliteDataReader reader = allinf.ExecuteReader();
                foreach(var a in reader)
                {
                    long cid = Convert.ToInt32(reader.GetValue(0));
                    if (!data.ContainsKey(cid))
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
                        int d = Convert.ToInt32(reader.GetValue(2)); // difficulty
                        string[,] ra = SOA.R_c(Convert.ToString(reader.GetValue(3))); // region array
                        string[] va = SOA.VH_c(Convert.ToString(reader.GetValue(4))); // vertical array
                        string[] ha = SOA.VH_c(Convert.ToString(reader.GetValue(5))); // horisontal array
                        int[,] e = SOA.E_c(Convert.ToString(reader.GetValue(6))); // elements
                        string[,] ep = SOA.EP_c(Convert.ToString(reader.GetValue(7))); // elements prediction
                        Base_Game b = new Base_Game(d, ra, va, ha, e, ep);
                        var c = new SOA(state, b);
                        data.Add(cid, c);
                        await botclient.SendTextMessageAsync(cid, "Bot is once again online!");
                    }
                }
                reader.Close();
                connection.Close();
            }
        } // safe start command, loads all user data and notifies them that bot is up
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
        } // grabs all game data from database and converts it to bunch of heatmaps for specific numbers
        public static string Render(int[,,] a, int i)
        {
            string result = "";
            result =
                      $"|{a[i, 0, 0]}|{a[i, 0, 1]}|{a[i, 0, 2]}|{a[i, 0, 3]}|{a[i, 0, 4]}|{a[i, 0, 5]}|{a[i, 0, 6]}|{a[i, 0, 7]}|{a[i, 0, 8]}|\n" +
                      $"|{a[i, 1, 0]}|{a[i, 1, 1]}|{a[i, 1, 2]}|{a[i, 1, 3]}|{a[i, 1, 4]}|{a[i, 1, 5]}|{a[i, 1, 6]}|{a[i, 1, 7]}|{a[i, 1, 8]}|\n" +
                      $"|{a[i, 2, 0]}|{a[i, 2, 1]}|{a[i, 2, 2]}|{a[i, 2, 3]}|{a[i, 2, 4]}|{a[i, 2, 5]}|{a[i, 2, 6]}|{a[i, 2, 7]}|{a[i, 2, 8]}|\n" +
                      $"|{a[i, 3, 0]}|{a[i, 3, 1]}|{a[i, 3, 2]}|{a[i, 3, 3]}|{a[i, 3, 4]}|{a[i, 3, 5]}|{a[i, 3, 6]}|{a[i, 3, 7]}|{a[i, 3, 8]}|\n" +
                      $"|{a[i, 4, 0]}|{a[i, 4, 1]}|{a[i, 4, 2]}|{a[i, 4, 3]}|{a[i, 4, 4]}|{a[i, 4, 5]}|{a[i, 4, 6]}|{a[i, 4, 7]}|{a[i, 4, 8]}|\n" +
                      $"|{a[i, 5, 0]}|{a[i, 5, 1]}|{a[i, 5, 2]}|{a[i, 5, 3]}|{a[i, 5, 4]}|{a[i, 5, 5]}|{a[i, 5, 6]}|{a[i, 5, 7]}|{a[i, 5, 8]}|\n" +
                      $"|{a[i, 6, 0]}|{a[i, 6, 1]}|{a[i, 6, 2]}|{a[i, 6, 3]}|{a[i, 6, 4]}|{a[i, 6, 5]}|{a[i, 6, 6]}|{a[i, 6, 7]}|{a[i, 6, 8]}|\n" +
                      $"|{a[i, 7, 0]}|{a[i, 7, 1]}|{a[i, 7, 2]}|{a[i, 7, 3]}|{a[i, 7, 4]}|{a[i, 7, 5]}|{a[i, 7, 6]}|{a[i, 7, 7]}|{a[i, 7, 8]}|\n" +
                      $"|{a[i, 8, 0]}|{a[i, 8, 1]}|{a[i, 8, 2]}|{a[i, 8, 3]}|{a[i, 8, 4]}|{a[i, 8, 5]}|{a[i, 8, 6]}|{a[i, 8, 7]}|{a[i, 8, 8]}|\n";
            return result;
        } // grid for heatmaps
        public static string Render(int[,] a)
        {
            string result = "";
            result =    $"|{a[0, 0]}|{a[0, 1]}|{a[0, 2]}|{a[0, 3]}|{a[0, 4]}|{a[0, 5]}|{a[0, 6]}|{a[0, 7]}|{a[0, 8]}|\n" +
                        $"|{a[1, 0]}|{a[1, 1]}|{a[1, 2]}|{a[1, 3]}|{a[1, 4]}|{a[1, 5]}|{a[1, 6]}|{a[1, 7]}|{a[1, 8]}|\n" +
                        $"|{a[2, 0]}|{a[2, 1]}|{a[2, 2]}|{a[2, 3]}|{a[2, 4]}|{a[2, 5]}|{a[2, 6]}|{a[2, 7]}|{a[2, 8]}|\n" +
                        $"|{a[3, 0]}|{a[3, 1]}|{a[3, 2]}|{a[3, 3]}|{a[3, 4]}|{a[3, 5]}|{a[3, 6]}|{a[3, 7]}|{a[3, 8]}|\n" +
                        $"|{a[4, 0]}|{a[4, 1]}|{a[4, 2]}|{a[4, 3]}|{a[4, 4]}|{a[4, 5]}|{a[4, 6]}|{a[4, 7]}|{a[4, 8]}|\n" +
                        $"|{a[5, 0]}|{a[5, 1]}|{a[5, 2]}|{a[5, 3]}|{a[5, 4]}|{a[5, 5]}|{a[5, 6]}|{a[5, 7]}|{a[5, 8]}|\n" +
                        $"|{a[6, 0]}|{a[6, 1]}|{a[6, 2]}|{a[6, 3]}|{a[6, 4]}|{a[6, 5]}|{a[6, 6]}|{a[6, 7]}|{a[6, 8]}|\n" +
                        $"|{a[7, 0]}|{a[7, 1]}|{a[7, 2]}|{a[7, 3]}|{a[7, 4]}|{a[7, 5]}|{a[7, 6]}|{a[7, 7]}|{a[7, 8]}|\n" +
                        $"|{a[8, 0]}|{a[8, 1]}|{a[8, 2]}|{a[8, 3]}|{a[8, 4]}|{a[8, 5]}|{a[8, 6]}|{a[8, 7]}|{a[8, 8]}|\n";
            return result;
        } // grid for games
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
