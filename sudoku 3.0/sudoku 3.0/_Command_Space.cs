using _SOA;
using _Base_Game_Class;
using BotCode;
using Microsoft.Data.Sqlite;
using Telegram.Bot;
using Telegram.Bot.Types;
using Datastorage;

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
                            string[,] ra = SOA.convert_from_base_2(Convert.ToString(reader.GetValue(3)), 3); // region array
                            string[] va = SOA.convert_from_base_1(Convert.ToString(reader.GetValue(4)), 9); // vertical array
                            string[] ha = SOA.convert_from_base_1(Convert.ToString(reader.GetValue(5)), 9); // horisontal array
                            string[,] e = SOA.convert_from_base_2(Convert.ToString(reader.GetValue(6)), 9); // elements
                            string[,] ep = SOA.convert_from_base_2(Convert.ToString(reader.GetValue(7)), 9); // elements prediction
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
                    ra = SOA.convert_to_base_2(data[cid].game.Region_Array,3);
                    va = SOA.convert_to_base_1(data[cid].game.Vertical_Array,9);
                    ha = SOA.convert_to_base_1(data[cid].game.Horizontal_Array,9);
                    e = SOA.convert_to_base_2(data[cid].game.Elements,9);
                    ep = SOA.convert_to_base_2(data[cid].game.Elements_Prediction,9);
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
                        string[,] ra = SOA.convert_from_base_2(Convert.ToString(reader.GetValue(3)),3); // region array
                        string[] va = SOA.convert_from_base_1(Convert.ToString(reader.GetValue(4)),9); // vertical array
                        string[] ha = SOA.convert_from_base_1(Convert.ToString(reader.GetValue(5)),9); // horisontal array
                        string[,] e = SOA.convert_from_base_2(Convert.ToString(reader.GetValue(6)),9); // elements
                        string[,] ep = SOA.convert_from_base_2(Convert.ToString(reader.GetValue(7)),9); // elements prediction
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
        public static Data[] GetInf()
        {
            Data[] data = new Data[9];
            int max = 0;
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
            foreach (var a in result) // номер элемента базы данных
            {
                for (int i = 0; i < 9; i++) // строка
                {
                    for (int j = 0; j < 9; j++) // столбец
                    {
                        data[a[i + (j * 9)]].Add(i, j);
                    }
                }
            }
            return data;
        } // grabs all game data from database and converts it to bunch of heatmaps for specific numbers
        public static string Render(Data[] a, int i)
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
                      $"|{a[i].Get(8, 0)}|{a[i].Get(8, 1)}|{a[i].Get(8, 2)}|{a[i].Get(8, 3)}|{a[i].Get(8, 4)}|{a[i].Get(8, 5)}|{a[i].Get(8, 6)}|{a[i].Get(8, 7)}|{a[i].Get(8, 8)}|\n";
            return result;
        } // grid for heatmaps
        public static string Render(string[,] a)
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
