using _Config;
using Npgsql;
using Telegram.Bot.Types;
using System;

namespace Bot.scripts
{
    public class Base_Game
    {
        public int Difficulty = 0;
        public string[,] Region_Array = new string[3, 3]; // box  array
        public string[] Vertical_Array = new string[9]; // collum array
        public string[] Horizontal_Array = new string[9]; // row array
        public string[,] Elements = new string[9, 9]; // basic array
        public string[,] Elements_Prediction = new string[9, 9]; // possible numbers for each cell + win/lose conditions
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
                for (int j = 0; j < 9; j++)
                {
                    Elements_Prediction[i, j] = "";
                    Elements[i, j] = "0";
                }
            }
            if (Difficulty > 0) { EnforceDifficulty(Difficulty); }
            Prediction();
        } // programm start + nullyfing of the arrays
        public Base_Game(int difficulty, string[,] region_array, string[] vertical_array, string[] horizontal_array, string[,] elements, string[,] elements_prediction)
        {
            Difficulty = difficulty;
            Region_Array = region_array;
            Vertical_Array = vertical_array;
            Horizontal_Array = horizontal_array;
            Elements = elements;
            Elements_Prediction = elements_prediction;
        }
        public string PredictionRender(int i, int j)
        {
            return Elements_Prediction[i - 1, j - 1];
        } // gets possible values for specified cell
        public void Write(int i, int j, string arg)
        {
            Elements[i, j] = arg;
            Region_Array[i / 3, j / 3] += arg; Horizontal_Array[i] += arg; Vertical_Array[j] += arg;
        } // setting the value in cell
        public bool IsValid(int i, int j, int arg)
        {
            if (Horizontal_Array[i].Contains(arg.ToString())) return false;
            if (Vertical_Array[j].Contains(arg.ToString())) return false;
            if (Region_Array[i / 3, j / 3].Contains(arg.ToString())) return false;
            if (Elements[i, j] != "0") return false;
            if (arg > 0 && arg < 10) return false;
            return true;
        } // checking if value is valid according to the rules
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
                for (int j = 0; j < 9; j++)
                {
                    Elements[i, j] = "0";
                    Elements_Prediction[i, j] = " ";
                }
            }
        } // nullify all arrays
        public int Prediction()
        {
            int Counter_w = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Elements_Prediction[i, j] = "";
                    if (Elements[i, j] == "0")
                    {

                        int Counter_l = 0;
                        for (int a = 1; a < 10; a++)
                        {
                            if (IsValid(i, j, a) == true && !Elements_Prediction[i, j].Contains((char)a)) { Elements_Prediction[i, j] += Convert.ToString(a); }
                            else { Counter_l++; }
                        }
                        if (Counter_l == 9) { return 0; }
                    }
                    if (Elements_Prediction[i, j] == "" && Elements[i, j] == "0") { return 0; }
                    if (Elements_Prediction[1, j] == "" && Elements[i, j] != "0") { Counter_w++; }
                }
            }
            if (Counter_w == 81) { return 1; }
            return -1;
        } // providing possible values in boxes
        public void Save(bool end, Message message)
        {
            string fieldinfo = "";
            foreach (string a in Elements)
            {
                fieldinfo += a.ToString();
            }
            using (var connection = new NpgsqlConnection(Config.connection))
            {
                long cid = message.Chat.Id;
                connection.Open();
                NpgsqlCommand addin = new NpgsqlCommand($"INSERT INTO sudoku (field, wl, Chat_Id) VALUES ('{fieldinfo}', '{end}','{cid}')", connection);
                int number = addin.ExecuteNonQuery();
                connection.Close();
            }
        } // gets current game info to the databse
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
                    Elements[i, j] = Convert.ToString(v); Difficulty--;
                    Region_Array[i / 3, j / 3] += v; Horizontal_Array[i] += v; Vertical_Array[j] += v;
                }
            }
        } // adding some numbers for start
    }
}
