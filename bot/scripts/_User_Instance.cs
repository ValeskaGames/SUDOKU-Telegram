using botcode;

namespace Bot.scripts
{
    class User_Instance
    {
        public State state { get; set; }
        public Base_Game game { get; set; }
        public User_Instance(State s, Base_Game g)
        {
            state = s;
            game = g;
        } // users in database
        public User_Instance()
        {
            state = State.Menu;
            game = null;
        } // new users
    }
}
