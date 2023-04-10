using _Config;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Bot.scripts
{
    public class Base_Game
    {
        public int Difficulty = 0;
        public int Region_Size = 3;
        public string[,] Elements = new string[9, 9]; // basic array
        public Base_Game()
        {
            Difficulty = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Elements[i, j] = "0";
                }
            }
            if (Difficulty > 0) { EnforceDifficulty(Difficulty); }
            Prediction();
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
                }
            }
        } // nullify all arrays
        public string[,] Prediction()
        {
            string[,] Elements_Prediction = new string[9,9];
            for(int i = 0; i < 81; i++) Elements_Prediction[i/9,i%9] = "0";
            for (int i = 0; i < 81; i++)
            {
                if (Elements[i % 9, i / 9] == "0")
                {
                    for (int j = 1; j <= 9; j++)
                    {
                        if (IsValid(i / 9, i % 9, j) == true && !Elements_Prediction[i / 9, i % 9].Contains((char)j)) 
                        { Elements_Prediction[i % 9, i / 9] += Convert.ToString(j) + " "; }
                    }
                }
            }
            return Elements_Prediction;
        } // providing possible values in boxes
        public int IsSolvable()
        {
            string[,] matrix = Elements;
            bool changed;
            do
            {
                changed = false;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (matrix[i, j] != "0") continue; // don't check full cells

                        HashSet<string> availableValues = new HashSet<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9" };

                        // removing already existing values (change later for scalability)
                        int rowStart = (i / 3) * 3;
                        int colStart = (j / 3) * 3;
                        for (int k = 0; k < 9; k++)
                        {
                            availableValues.Remove(matrix[i, k]);
                            availableValues.Remove(matrix[k, j]);
                            availableValues.Remove(matrix[rowStart + k / 3, colStart + k % 3]);
                        }

                        if (availableValues.Count == 0) return 0;
                        else if (availableValues.Count == 1)
                        {
                            matrix[i, j] = availableValues.First();
                            changed = true;
                        }
                    }
                }
            } while (changed);

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
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
                i = r.Next(0, 9);
                j = r.Next(0, 9);
                v = r.Next(1, 9);
                if (IsValid(i, j, v) == true) { Elements[i, j] = Convert.ToString(v); Difficulty--; }
            }
        } // adding some numbers for start
    }
}
