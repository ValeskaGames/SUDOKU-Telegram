using Microsoft.Data.Sqlite;
namespace Base_Game_Class
{
    public class Base_Game
    {
        public int Difficulty = 0;
        string[,] Region_Array = new string[3, 3]; // box  array +
        string[] Vertical_Array = new string[9]; // collum array +
        string[] Horizontal_Array = new string[9]; // row array +
        int[,] Elements = new int[9, 9]; // basic array +
        string[,] Elements_Prediction = new string[9, 9]; // possible numbers for each cell + win/lose conditions +
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
                for(int j = 0; j < 9; j++)
                {
                    Elements_Prediction[i, j] = "";
                }
            }
            if (Difficulty > 0) { EnforceDifficulty(Difficulty); }
            Prediction();
            Render();
        } // programm start + nullyfing of the arrays +
        public Base_Game(int difficulty, string[,] region_Array, string[] vertical_Array, string[] horizontal_Array, int[,] elements, string[,] elements_Prediction)
        {
            Difficulty = difficulty;
            Region_Array = region_Array;
            Vertical_Array = vertical_Array;
            Horizontal_Array = horizontal_Array;
            Elements = elements;
            Elements_Prediction = elements_Prediction;
        }

        public (int i, int j, int arg) Input(string a)
        {
            a.Trim();
            int _i = Convert.ToInt32(a.Remove(a.IndexOf(" "))) - 1;
            a = a.Substring(2);
            a.TrimStart();
            int _j = Convert.ToInt32(a.Remove(a.IndexOf(" "))) - 1;
            a = a.Substring(2);
            a.TrimStart();
            int _arg = Convert.ToInt32(a);
            return (_i, _j, _arg);
        }
        public string PredictionRender(int i, int j)
        {
            return (Elements_Prediction[i-1,j-1]);
        }
        public void Write(int i, int j, int arg)
        {
            Elements[i, j] = arg;
            Region_Array[i / 3, j / 3] += arg; Horizontal_Array[i] += arg; Vertical_Array[j] += arg;
        } // setting the value in cell +
        public bool IsValid(int i, int j, int arg)
        {
            if (Horizontal_Array[i].Contains(arg.ToString())) { return false; }
            if (Vertical_Array[j].Contains(arg.ToString())) { return false; }
            if (Region_Array[i / 3, j / 3].Contains(arg.ToString())) { return false; }
            if (Elements[i, j] != 0) { return false; }
            return true;
        } // checking if value is valid according to the rules +
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
                    Elements[i, j] = 0;
                    Elements_Prediction[i, j] = " ";
                }
            }
        } // nullify all arrays +
        public int Prediction()
        {
            int Counter_w = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    Elements_Prediction[i, j] = "";
                    if (Elements[i, j] == 0)
                    {

                        int Counter_l = 0;
                        for (int a = 1; a < 10; a++)
                        {
                            if (IsValid(i, j, a) == true && !Elements_Prediction[i, j].Contains((char)a)) { Elements_Prediction[i, j] += Convert.ToString(a); }
                            else { Counter_l++; }
                        }
                        if (Counter_l == 9) { Save(false); return (0); }
                    }
                    else if (Elements_Prediction[i,j] == "" && Elements[i,j] == 0) { Save(false); return 0; }
                    else { Counter_w++; }
                }
            }
            if (Counter_w == 81) { Save(true); return (1); }
            return (-1);
        } // providing possible values in boxes +
        public void Save(bool end)
        {
            string fieldinfo = "";
            foreach (int a in Elements)
            {
                fieldinfo += a.ToString();
            }
            using (var connection = new SqliteConnection("Data Source=database.db"))
            {
                connection.Open();
                SqliteCommand addin = new SqliteCommand($"INSERT INTO sudoku (field, wl) VALUES ('{fieldinfo}', '{end}')", connection);
                int number = addin.ExecuteNonQuery();
                connection.Close();
            }
        }
        public string Render()
        {
            string a =
                      $"|{Elements[0, 0]}|{Elements[0, 1]}|{Elements[0, 2]}|{Elements[0, 3]}|{Elements[0, 4]}|{Elements[0, 5]}|{Elements[0, 6]}|{Elements[0, 7]}|{Elements[0, 8]}|\n" +
                      $"|{Elements[1, 0]}|{Elements[1, 1]}|{Elements[1, 2]}|{Elements[1, 3]}|{Elements[1, 4]}|{Elements[1, 5]}|{Elements[1, 6]}|{Elements[1, 7]}|{Elements[1, 8]}|\n" +
                      $"|{Elements[2, 0]}|{Elements[2, 1]}|{Elements[2, 2]}|{Elements[2, 3]}|{Elements[2, 4]}|{Elements[2, 5]}|{Elements[2, 6]}|{Elements[2, 7]}|{Elements[2, 8]}|\n" +
                      $"|{Elements[3, 0]}|{Elements[3, 1]}|{Elements[3, 2]}|{Elements[3, 3]}|{Elements[3, 4]}|{Elements[3, 5]}|{Elements[3, 6]}|{Elements[3, 7]}|{Elements[3, 8]}|\n" +
                      $"|{Elements[4, 0]}|{Elements[4, 1]}|{Elements[4, 2]}|{Elements[4, 3]}|{Elements[4, 4]}|{Elements[4, 5]}|{Elements[4, 6]}|{Elements[4, 7]}|{Elements[4, 8]}|\n" +
                      $"|{Elements[5, 0]}|{Elements[5, 1]}|{Elements[5, 2]}|{Elements[5, 3]}|{Elements[5, 4]}|{Elements[5, 5]}|{Elements[5, 6]}|{Elements[5, 7]}|{Elements[5, 8]}|\n" +
                      $"|{Elements[6, 0]}|{Elements[6, 1]}|{Elements[6, 2]}|{Elements[6, 3]}|{Elements[6, 4]}|{Elements[6, 5]}|{Elements[6, 6]}|{Elements[6, 7]}|{Elements[6, 8]}|\n" +
                      $"|{Elements[7, 0]}|{Elements[7, 1]}|{Elements[7, 2]}|{Elements[7, 3]}|{Elements[7, 4]}|{Elements[7, 5]}|{Elements[7, 6]}|{Elements[7, 7]}|{Elements[7, 8]}|\n" +
                      $"|{Elements[8, 0]}|{Elements[8, 1]}|{Elements[8, 2]}|{Elements[8, 3]}|{Elements[8, 4]}|{Elements[8, 5]}|{Elements[8, 6]}|{Elements[8, 7]}|{Elements[8, 8]}|\n";

            return a;
        } // Returns nice table string
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
                    Elements[i, j] = v; Difficulty--;
                    Region_Array[i / 3, j / 3] += v; Horizontal_Array[i] += v; Vertical_Array[j] += v;
                }
            }
        } // adding some numbers for start +
    }
}
