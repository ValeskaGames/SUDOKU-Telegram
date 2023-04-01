using _Config;
using botcode;
using Npgsql;
using System;
using Telegram.Bot.Types;

namespace Bot.scripts
{
    internal class _Sql_Data
    {
        public static void user_load(Message message, long chat_id)
        {
            using (var connection = new NpgsqlConnection(Config.connection))
            {
                connection.Open();
                NpgsqlCommand check = new NpgsqlCommand($"SELECT * FROM user_base WHERE chat_id = '{chat_id}'", connection);
                int _check = check.ExecuteNonQuery();
                if (_check == 1)
                {
                    NpgsqlCommand resultinf = new NpgsqlCommand($"SELECT * FROM user_base WHERE chat_id = '{chat_id}'", connection);
                    NpgsqlDataReader reader = resultinf.ExecuteReader();
                    if (reader.HasRows)
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
                        int difficulty = Convert.ToInt32(reader.GetValue(2));
                        string[,] region_array = convert_from_base_2d(Convert.ToString(reader.GetValue(3)), 3);
                        string[] vertical_array = convert_from_base_1d(Convert.ToString(reader.GetValue(4)), 9);
                        string[] horisontal_array = convert_from_base_1d(Convert.ToString(reader.GetValue(5)), 9);
                        string[,] elements = convert_from_base_2d(Convert.ToString(reader.GetValue(6)), 9);
                        string[,] elements_prediction = convert_from_base_2d(Convert.ToString(reader.GetValue(7)), 9);
                        Base_Game game = new Base_Game(difficulty, region_array, vertical_array, horisontal_array, elements, elements_prediction);
                        var inst = new User_Instance(state, game);
                        Command._data.Add(message.Chat.Id, inst);
                    }
                    reader.Close();
                }
                else
                {
                    Base_Game b = new Base_Game();
                    User_Instance a = new User_Instance(State.Menu, b);
                    Command._data.Add(message.Chat.Id, a);
                }
                connection.Close();
            }
        }
        public static SqlData[] GetInf()
        {
            var data = new SqlData[9] { new SqlData(), new SqlData(), new SqlData(), new SqlData(), new SqlData(), new SqlData(), new SqlData(), new SqlData(), new SqlData() };
            string[] result;
            using (var connection = new NpgsqlConnection(Config.connection))
            {
                connection.Open();
                NpgsqlCommand command = new NpgsqlCommand("SELECT COUNT(*) FROM sudoku", connection);
                NpgsqlCommand resultinf = new NpgsqlCommand("SELECT * FROM sudoku", connection);
                var n = Convert.ToInt32(command.ExecuteScalar());
                NpgsqlDataReader reader = resultinf.ExecuteReader();
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
        public static string[] convert_from_base_1d(string a, int b)
        {
            string[] result = new string[b];
            for (int i = 0; i < b; i++)
            {
                result[i] = a.Remove(a.IndexOf(' '));
                a.Remove(a.IndexOf(" ")); a.Trim();
            }
            return result;
        }
        public static string[,] convert_from_base_2d(string a, int b)
        {
            string[,] result = new string[b, b];
            for (int i = 0; i < b; i++)
            {
                for (int j = 0; j < b; j++)
                {
                    result[i, j] = a.Remove(a.IndexOf(' '));
                    a.Remove(a.IndexOf(" ")); a.Trim();
                }
            }
            return result;
        }
        public static string convert_to_base_1d(string[] a, int b)
        {
            string[] _a = a;
            string result = "";
            for (int i = 0; i < b; i++)
            {
                result += _a[i] + " ";
            }
            return result;
        }
        public static string convert_to_base_2d(string[,] a, int b)
        {
            string[,] _a = a;
            string result = "";
            for (int i = 0; i < b; i++)
            {
                for (int j = 0; j < b; j++)
                {
                    result += _a[i, j] + " ";
                }
            }
            return result;
        }
    }
    public class SqlData
    {
        public int[,] heatmap = new int[9, 9];
        public void obj()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; i++)
                {
                    heatmap[i, j] = 0;
                }
            }
        }
        public void Add(int i, int j)
        {
            heatmap[i, j]++;
        }
        public int Get(int i, int j)
        {
            return heatmap[i, j];
        }
    }
}