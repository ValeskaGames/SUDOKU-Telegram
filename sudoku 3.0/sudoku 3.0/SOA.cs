using Base_Game_Class;
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
        }
        public SOA()
        {
            state = State.Menu;
            game = null;
        }
        public static string[,] R_c(string a)
        {
            string[,] result = new string[3,3];
            char[] _a = a.ToCharArray();
            for (int i = 0; i < 3; i++)
            {
                for(int j = 0; j<3; j++)
                {
                    int b = (i * 3) + j;
                    result[i, j] = Convert.ToString(_a[b]);
                }
            }
            return result;
        }
        public static string[] VH_c(string a)
        {
            string[] result = new string[9];
            char[] _a = a.ToCharArray();
            for(int i = 0; i< 9; i++)
            {
                result[i] = Convert.ToString(_a[i]);
            }
            return result;
        }
        public static int[,] E_c(string a)
        {
            int[,] result = new int[9,9];
            char[] _a = a.ToCharArray();
            for (int i = 0; i<9; i++)
            {
                for (int j = 0; j<9; j++)
                {
                    result[i,j] = Convert.ToInt32(_a[(i*9)+j]);
                }
            }
            return result;
        }
        public static string[,] EP_c(string a)
        {
            string[,] result = new string[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; i++)
                {

                    result[i, j] = Convert.ToString(a.Remove(a.IndexOf(' ')));
                    a = a.Remove(a.IndexOf(' '));
                }
            }
            return result;
        }

    }
}
