using _Base_Game_Class;
using BotCode;

namespace _SOA
{
    class SOA
    {
        public State state { get; set; }
        public Base_Game game { get; set; }
        public SOA(State s, Base_Game g)
        {
            state = s;
            game = g;
        } // for users that already in the base
        public SOA()
        {
            state = State.Menu;
            game = null;
        } // for new users
        public static string[,] R_c(string a)
        {
            string[,] result = new string[3,3];
            for (int i = 0; i < 3; i++)
            {
                for(int j = 0; j<3; j++)
                {
                    result[i, j] = a.Remove(a.IndexOf(' '));
                    a.Remove(a.IndexOf(" ")); a.Trim();
                }
            }
            return result;
        } // region array from base
        public static string[] VH_c(string a)
        {
            string[] result = new string[9];
            for(int i = 0; i< 9; i++)
            {
                result[i] = a.Remove(a.IndexOf(' '));
                a.Remove(a.IndexOf(" ")); a.Trim();
            }
            return result;
        } // vert/hori array from base
        public static int[,] E_c(string a)
        {
            int[,] result = new int[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    result[i, j] = Convert.ToInt32(a.Remove(a.IndexOf(' ')));
                    a.Remove(a.IndexOf(" ")); a.Trim();
                }
            }
            return result;
        } // elements from base 
        public static string[,] EP_c(string a)
        {
            string[,] result = new string[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    result[i, j] = a.Remove(a.IndexOf(' '));
                    a.Remove(a.IndexOf(" ")); a.Trim();
                }
            }
            return result;
        } // elements prediction from base
        public static string R_b(string[,] a) // region array to base
        {
            string[,] _a = a;
            string result = "";
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result += _a[i, j] + " ";
                }
            }
            return result;
        }
        public static string VH_b(string[] a)
        {
            string[] _a = a;
            string result = "";
            for (int i = 0; i < 9; i++)
            {
                result += _a[i] + " ";
            }
            return result;
        } // vert/hori array to base
        public static string E_b<T>(T[,] a)
        {
            T[,] _a = a;
            string result = "";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    result += _a[i, j] + " ";
                }
            }
            return result;
        } // elements prediction/elements to base
    }
}
