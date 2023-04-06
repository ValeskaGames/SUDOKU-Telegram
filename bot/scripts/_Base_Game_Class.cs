using _Config;
using Godot;
using Npgsql;
using System;

namespace Bot.scripts
{
    public class Base_Game
    {
        public int Difficulty = 0;
        public int Region_Size = 3;
        public string[,] Elements = new string[9, 9]; // basic array
        public string[,] Elements_Prediction = new string[9, 9]; // possible numbers for each cell + win/lose conditions
        public Base_Game()
        {
            Difficulty = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Elements_Prediction[i, j] = "";
                    Elements[i, j] = "0";
                }
            }
            if (Difficulty > 0) { EnforceDifficulty(Difficulty); }
            Prediction();
        } // programm start + nullyfing of the arrays
        public Base_Game(int difficulty, string[,] elements, string[,] elements_prediction)
        {
            Difficulty = difficulty;
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
        } // setting the value in cell
        public bool IsValid(int i, int j, int arg)
        {
            if (arg < 0 || arg > 9) return false;
            if (Elements[i, j] != "0") return false;

            for (int c = 0; c < Math.Sqrt(Elements.Length); c++)
                if (Elements[i,c].Contains(arg.ToString()) || Elements[c,j].Contains(arg.ToString())) return false;

            int regH = ((i+1) % Region_Size) == 0 ? ((i / Region_Size) * 3) + 2 : (((i / Region_Size) + 1) * 3) - 1;
            int regV = ((j+1) % Region_Size) == 0 ? ((j / Region_Size) * 3) + 2 : (((j / Region_Size) + 1) * 3) - 1;
            for (int c = 0; c < Region_Size; c++)
                if (Elements[regH - c, regV - c].Contains(arg.ToString())) return false;

            return true;
        } // checking if value is valid according to the rules (setting as given that region size is 3x3)(buе with only one matrix)
        public void Reset()
        {
            Difficulty = 0;
            for (int i = 0; i < 9; i++)
            {
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
            int Counter_l = 0;
            for (int i = 0; i < 81; i++)
            {
                Elements_Prediction[i%9, i/9] = "";
                if (Elements[i%9, i/9] == "0")
                {
                    Counter_l = 0;
                    for (int j = 1; j < 10; j++)
                    {
                        if (IsValid(i % 9, i / 9, j) == true && !Elements_Prediction[i % 9, i / 9].Contains((char)j)) { Elements_Prediction[i % 9, i / 9] += Convert.ToString(j); }
                        else { Counter_l++; }
                    }
                    if (Counter_l == 9) { return 0; }
                }
                if (Elements_Prediction[i % 9, i / 9] == "" && Elements[i % 9, i / 9] == "0") { return 0; }
                if (Elements_Prediction[i % 9, i / 9] == "" && Elements[i % 9, i / 9] != "0") { Counter_w++; }
            }
            return (Counter_w == 81) ? 1 : -1;
        } // providing possible values in boxes
        public void Save(bool end, long cid)
        {
            string fieldinfo = "";
            foreach (string a in Elements)
            {
                fieldinfo += a.ToString();
            }
            using (var connection = new NpgsqlConnection(Config.connection))
            {
                connection.Open();
                NpgsqlCommand addin = new NpgsqlCommand($"INSERT INTO sudoku (field, wl, chat_id) VALUES ('{fieldinfo}', '{end}','{cid}')", connection);
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
                if (IsValid(i, j, v) == true) { Elements[i, j] = Convert.ToString(v); Difficulty--; }
            }
        } // adding some numbers for start
    }
}
