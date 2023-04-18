using _Config;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.scripts
{
    public class Base_Game
    {
        public int Difficulty;
        public int Region_Size;
        public int RegSqrt;
        public string[,] Elements;// basic array
        public Base_Game()
        {
            Difficulty = 0;
            Region_Size = 3; 
            RegSqrt = Region_Size*Region_Size;
            Elements = new string[RegSqrt, RegSqrt];
            for (int i = 0; i < RegSqrt; i++)
            {
                for (int j = 0; j < RegSqrt; j++)
                {
                    Elements[i, j] = "0";
                }
            }
            if (Difficulty > 0) { EnforceDifficulty(Difficulty); }
        } // programm start + nullyfing of the arrays
        public Base_Game(int difficulty, string[,] elements)
        {
            Difficulty = difficulty;
            Elements = elements;
        }
        public void Write(int i, int j, string arg)
        {
            Elements[i, j] = arg;
        } // setting the value in cell
        public bool IsValid(int i, int j, int arg)
        {
            if (arg < 0 || arg > 9) return false;
            if (Elements[i, j] != "0") return false;

            for (int c = 0; c < RegSqrt; c++)
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
            for (int i = 0; i < RegSqrt; i++)
            {
                for (int j = 0; j < RegSqrt; j++)
                {
                    Elements[i, j] = "0";
                }
            }
        } // nullify all arrays
        public string Prediction(int _i, int _j)
        {
            string result = "";
            for (int i = 1; i < 10; i++)
                if (IsValid(_i, _j, i)) result += $"{i} ";
            return result;
        } // providing possible values in boxes
        public int IsSolvable()
        {
            string[,] matrix = Elements;
            bool changed;
            do
            {
                changed = false;
                for (int i = 0; i < RegSqrt; i++)
                {
                    for (int j = 0; j < RegSqrt; j++)
                    {
                        if (matrix[i, j] != "0") continue; // don't check full cells

                        var a = Prediction(i, j);
                        a.Replace(" ", "");

                        if (a.Length == 0) return 0;
                        else if (a.Length == 1)
                        {
                            matrix[i, j] = a[0].ToString();
                            changed = true;
                        }
                    }
                }
            } while (changed);

            for (int i = 0; i < RegSqrt; i++)
            {
                for (int j = 0; j < RegSqrt; j++)
                {
                    if (matrix[i, j] == "0") return -1; // matrix is not full, but there is possible variations == continue
                }
            }
            return 1; // matrix is full == win
        }
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
                i = r.Next(0, RegSqrt);
                j = r.Next(0, RegSqrt);
                v = r.Next(1, RegSqrt);
                if (IsValid(i, j, v) == true) { Elements[i, j] = Convert.ToString(v); Difficulty--; }
            }
        } // adding some numbers for start
    }
}
