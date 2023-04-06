using botcode;
using Npgsql;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using _Config;
using Godot;

namespace Bot.scripts
{
    class Command
    {
        public static Dictionary<long, User_Instance> _data;
        public static ITelegramBotClient _botclient;
        public static RichTextLabel _logs;
        public static void Initialize(Dictionary<long, User_Instance> data, ITelegramBotClient botclient, RichTextLabel logs)
        {
            _data = data;
            _botclient = botclient;
            _logs = logs;
        }
        public static void User_load(Update _upd)
        {
            _Sql_Data.user_load(_upd.Message, _upd.Message.Chat.Id);
        } // loads user info from database
        public static async void Exit_seq()
        {
            var connection = new NpgsqlConnection(Config.connection);
            connection.Open();
            foreach (var a in _data)
            {
                long chat_id = a.Key;
                int difficulty = _data[chat_id].game.Difficulty;
                NpgsqlCommand insert = new NpgsqlCommand(
                "INSERT INTO User_Base (chat_id, state, difficulty elements, elements_prediction) " +
                "VALUES (@chat_id, @state, @difficulty, @elements, @elements_prediction) " +
                "ON CONFLICT (Chat_Id) DO UPDATE " +
                "SET State = @state, Difficulty = @difficulty," +
                " Elements = @elements, Elements_Prediction = @elements_prediction",
                connection);

                insert.Parameters.AddWithValue("chat_id", chat_id);
                insert.Parameters.AddWithValue("state", Convert.ToInt32(_data[chat_id].state));
                insert.Parameters.AddWithValue("difficulty", _data[chat_id].game.Difficulty);
                insert.Parameters.AddWithValue("elements", _Sql_Data.convert_to_base_2d(_data[chat_id].game.Elements, 9));
                insert.Parameters.AddWithValue("elements_prediction", _Sql_Data.convert_to_base_2d(_data[chat_id].game.Elements_Prediction, 9));
                
                insert.ExecuteNonQuery();

                await _botclient.SendTextMessageAsync(chat_id, "For now bot is going offline, sorry for inconvenience.");
            }
            connection.Close();
        } // safe exit, saves all current user data and notifies them about bot going down
        public static async void Start_seq()
        {
            var connection = new NpgsqlConnection(Config.connection);
            connection.Open();
            NpgsqlCommand allinfo = new NpgsqlCommand($"SELECT * FROM User_Base", connection);
            NpgsqlDataReader reader = allinfo.ExecuteReader();
            if (!reader.HasRows)
            {
                reader.Close();
                connection.Close();
                return;
            }
            while (reader.Read())
            {
                long cid = reader.GetInt32(0);
                if (!_data.ContainsKey(cid))
                {
                    State state;
                    switch (Convert.ToInt32(reader.GetValue(1)))
                    {
                        case 0: state = State.Menu; break;
                        case 1: state = State.Game; break;
                        case 2: state = State.Conclusion; break;
                        case 3: state = State.Statistics; break;
                        default: state = State.Menu; break;
                    }
                    int difficulty = Convert.ToInt32(reader.GetValue(2)); // difficulty
                    string[,] elements = _Sql_Data.convert_from_base_2d(Convert.ToString(reader.GetValue(6)), 9); // elements
                    string[,] elements_prediction = _Sql_Data.convert_from_base_2d(Convert.ToString(reader.GetValue(7)), 9); // elements prediction
                    Base_Game b = new Base_Game(difficulty, elements, elements_prediction);
                    var inst = new User_Instance(state, b);
                    _data.Add(cid, inst);
                    await _botclient.SendTextMessageAsync(cid, "Bot is once again online!");
                }
            }
            reader.Close();
            connection.Close();
        }// safe start command, loads all user data and notifies them that bot is up
        public static string Render(string a)
        {
            string result = "";
            for (int i = 0; i < 81; i++)
            {
                result += $"|{a[i]}|";
                if ((i+1) % 9 == 0 && i!=0) result += "\n";
            }
            return result;
        } // grid
        public static async Task Send(long cid, string message)
        {
            _logs.Text += $"\n{message}\n";
            _botclient.SendTextMessageAsync(cid, message);
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
