using _Config;
using botcode;
using Npgsql;
using System;
using System.Collections.Generic;
using Telegram.Bot.Types;

namespace Bot.scripts
{
	internal class _Sql_Data
	{
		public static void user_load(Message message, long chat_id)
		{
			var connection = new NpgsqlConnection(Config.connection);
			connection.Open();
			NpgsqlCommand resultinf = new NpgsqlCommand($"SELECT * FROM user_base WHERE chat_id = '{chat_id}'", connection);
            NpgsqlDataReader reader = resultinf.ExecuteReader();
            if (reader.Depth == 1)
			{
				State state = new State();
				switch (Convert.ToInt32(reader.GetValue(1)))
				{
					case 0: state = State.Menu; break;
					case 1: state = State.Game0; break;
					case 2: state = State.Game1; break;
					case 3: state = State.Conclusion; break;
					case 4: state = State.Stats; break;
					default: state = State.Menu; break;
				}
				int difficulty = Convert.ToInt32(reader.GetValue(2));
				string[,] elements = convert_from_base(Convert.ToString(reader.GetValue(6)), 9);
				Base_Game game = new Base_Game(difficulty, elements);
				var inst = new User_Instance(state, game);
				Command._data.Add(chat_id, inst);
				reader.Close();
			}
			else
			{
				Base_Game b = new Base_Game();
				User_Instance a = new User_Instance(State.Menu, b);
				Command._data.Add(chat_id, a);
			}
			connection.Close();
		}
		public static List<int[,]> GetInf()
		{
			List<int[,]> data = new List<int[,]> { };
			int[,] item = new int[9,9];
			for(int i = 0; i < 3; i++)	data.Add(item);

			List<string> resultmass = new List<string> { };
            NpgsqlConnection connection = new NpgsqlConnection(Config.connection);

			connection.Open();

            NpgsqlCommand resultinf = new NpgsqlCommand("SELECT * FROM sudoku", connection);
			NpgsqlDataReader reader = resultinf.ExecuteReader();

			if (reader.HasRows)
			{
				while (reader.Read())
				{
					resultmass.Add(Convert.ToString(reader.GetValue(0)));
				}
			}

			reader.Close();
			connection.Close();

            foreach (string a in resultmass)
            {
                for (int i = 0; i < 81; i++)
                {
                    int v = a[i] - '0';
					if (v > 0)  data[v - 1][i%9,i/9]++;
                }
            }

            return data;
		} // grabs all game data from database and converts it to bunch of heatmaps for specific numbers
		public static string[,] convert_from_base(string a, int b)
		{
			string[,] result = new string[b, b];
			for (int i = 0; i < b; i++)
			{
				for (int j = 0; j < b; j++)
				{
					result[i, j] = a.Remove(a.IndexOf(' '));
					a.Remove(a.IndexOf(" $")); a.Trim();
				}
			}
			return result;
		}
		public static string convert_to_base(string[,] a, int b)
		{
			string[,] _a = a;
			string result = $"";
			for (int i = 0; i < b; i++)
			{
				for (int j = 0; j < b; j++)
				{
					result += _a[i, j] + $" $";
				}
			}
			return result;
		}
	}
}
