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
        public static string[] convert_from_base_1(string a, int b)
        {
            string[] result = new string[b];
            for (int i = 0; i < b; i++)
            {
                result[i] = a.Remove(a.IndexOf(' '));
                a.Remove(a.IndexOf(" ")); a.Trim();
            }
            return result;
        }
        public static string[,] convert_from_base_2(string a, int b)
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
        public static string convert_to_base_1(string[] a, int b)
        {
            string[] _a = a;
            string result = "";
            for (int i = 0; i < b; i++)
            {
                result += _a[i] + " ";
            }
            return result;
        }
        public static string convert_to_base_2(string[,] a, int b)
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
}
