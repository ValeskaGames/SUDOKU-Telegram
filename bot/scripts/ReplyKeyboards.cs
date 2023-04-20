using Telegram.Bot.Types.ReplyMarkups;

namespace Bot.scripts
{
    class ReplyKeyboards
    {
        public static InlineKeyboardMarkup Input2d(string[,] el)
        {
            InlineKeyboardMarkup result = new(new[] 
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,0]}", callbackData: $"00"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,1]}", callbackData: $"01"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,2]}", callbackData: $"02"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,3]}", callbackData: $"03"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,4]}", callbackData: $"04"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,5]}", callbackData: $"05"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,6]}", callbackData: $"06"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,7]}", callbackData: $"07"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,8]}", callbackData: $"08"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,8]}", callbackData: $"08"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,0]}", callbackData: $"10"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,1]}", callbackData: $"11"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,2]}", callbackData: $"12"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,3]}", callbackData: $"13"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,4]}", callbackData: $"14"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,5]}", callbackData: $"15"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,6]}", callbackData: $"16"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,7]}", callbackData: $"17"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,8]}", callbackData: $"18"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,0]}", callbackData: $"20"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,1]}", callbackData: $"21"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,2]}", callbackData: $"22"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,3]}", callbackData: $"23"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,4]}", callbackData: $"24"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,5]}", callbackData: $"25"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,6]}", callbackData: $"26"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,7]}", callbackData: $"27"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,8]}", callbackData: $"28"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,0]}", callbackData: $"30"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,1]}", callbackData: $"31"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,2]}", callbackData: $"32"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,3]}", callbackData: $"33"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,4]}", callbackData: $"34"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,5]}", callbackData: $"35"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,6]}", callbackData: $"36"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,7]}", callbackData: $"37"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,8]}", callbackData: $"38"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,0]}", callbackData: $"40"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,1]}", callbackData: $"41"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,2]}", callbackData: $"42"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,3]}", callbackData: $"43"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,4]}", callbackData: $"44"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,5]}", callbackData: $"45"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,6]}", callbackData: $"46"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,7]}", callbackData: $"47"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,8]}", callbackData: $"48"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,0]}", callbackData: $"50"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,1]}", callbackData: $"51"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,2]}", callbackData: $"52"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,3]}", callbackData: $"53"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,4]}", callbackData: $"54"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,5]}", callbackData: $"55"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,6]}", callbackData: $"56"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,7]}", callbackData: $"57"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,8]}", callbackData: $"58"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,0]}", callbackData: $"60"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,1]}", callbackData: $"61"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,2]}", callbackData: $"62"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,3]}", callbackData: $"63"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,4]}", callbackData: $"64"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,5]}", callbackData: $"65"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,6]}", callbackData: $"66"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,7]}", callbackData: $"67"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,8]}", callbackData: $"68"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,0]}", callbackData: $"70"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,1]}", callbackData: $"71"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,2]}", callbackData: $"72"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,3]}", callbackData: $"73"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,4]}", callbackData: $"74"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,5]}", callbackData: $"75"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,6]}", callbackData: $"76"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,7]}", callbackData: $"77"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,8]}", callbackData: $"78"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,0]}", callbackData: $"80"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,1]}", callbackData: $"81"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,2]}", callbackData: $"82"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,3]}", callbackData: $"83"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,4]}", callbackData: $"84"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,5]}", callbackData: $"85"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,6]}", callbackData: $"86"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,7]}", callbackData: $"87"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,8]}", callbackData: $"88"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"exit", callbackData: $"exit"),
                    InlineKeyboardButton.WithCallbackData(text: $"prediction", callbackData: $"prediction")
                }
            });
            return result;
        }
        public static InlineKeyboardMarkup Input2d(string[,] el, string a)
        {
            InlineKeyboardMarkup result = new(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,0]}", callbackData: $"00{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,1]}", callbackData: $"01{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,2]}", callbackData: $"02{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,3]}", callbackData: $"03{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,4]}", callbackData: $"04{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,5]}", callbackData: $"05{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,6]}", callbackData: $"06{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,7]}", callbackData: $"07{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[0,8]}", callbackData: $"08{a}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,0]}", callbackData: $"10{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,1]}", callbackData: $"11{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,2]}", callbackData: $"12{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,3]}", callbackData: $"13{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,4]}", callbackData: $"14{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,5]}", callbackData: $"15{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,6]}", callbackData: $"16{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,7]}", callbackData: $"17{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[1,8]}", callbackData: $"18{a}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,0]}", callbackData: $"20{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,1]}", callbackData: $"21{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,2]}", callbackData: $"22{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,3]}", callbackData: $"23{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,4]}", callbackData: $"24{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,5]}", callbackData: $"25{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,6]}", callbackData: $"26{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,7]}", callbackData: $"27{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[2,8]}", callbackData: $"28{a}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,0]}", callbackData: $"30{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,1]}", callbackData: $"31{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,2]}", callbackData: $"32{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,3]}", callbackData: $"33{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,4]}", callbackData: $"34{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,5]}", callbackData: $"35{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,6]}", callbackData: $"36{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,7]}", callbackData: $"37{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[3,8]}", callbackData: $"38{a}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,0]}", callbackData: $"40{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,1]}", callbackData: $"41{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,2]}", callbackData: $"42{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,3]}", callbackData: $"43{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,4]}", callbackData: $"44{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,5]}", callbackData: $"45{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,6]}", callbackData: $"46{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,7]}", callbackData: $"47{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[4,8]}", callbackData: $"48{a}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,0]}", callbackData: $"50{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,1]}", callbackData: $"51{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,2]}", callbackData: $"52{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,3]}", callbackData: $"53{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,4]}", callbackData: $"54{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,5]}", callbackData: $"55{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,6]}", callbackData: $"56{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,7]}", callbackData: $"57{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[5,8]}", callbackData: $"58{a}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,0]}", callbackData: $"60{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,1]}", callbackData: $"61{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,2]}", callbackData: $"62{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,3]}", callbackData: $"63{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,4]}", callbackData: $"64{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,5]}", callbackData: $"65{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,6]}", callbackData: $"66{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,7]}", callbackData: $"67{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[6,8]}", callbackData: $"68{a}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,0]}", callbackData: $"70{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,1]}", callbackData: $"71{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,2]}", callbackData: $"72{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,3]}", callbackData: $"73{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,4]}", callbackData: $"74{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,5]}", callbackData: $"75{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,6]}", callbackData: $"76{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,7]}", callbackData: $"77{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[7,8]}", callbackData: $"78{a}"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,0]}", callbackData: $"80{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,1]}", callbackData: $"81{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,2]}", callbackData: $"82{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,3]}", callbackData: $"83{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,4]}", callbackData: $"84{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,5]}", callbackData: $"85{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,6]}", callbackData: $"86{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,7]}", callbackData: $"87{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"{el[8,8]}", callbackData: $"88{a}"),
                },
            });
            return result;
        }
        public static InlineKeyboardMarkup Menu()
        {
            InlineKeyboardMarkup result = new(new[]
            {
                    InlineKeyboardButton.WithCallbackData(text: $"Start game", callbackData: $"startgame"),
                    InlineKeyboardButton.WithCallbackData(text: $"Stats", callbackData: $"stats")
            });
            return result;
        }
        public static InlineKeyboardMarkup Input1d() 
        {
            InlineKeyboardMarkup result = new(new[]
            {
                    InlineKeyboardButton.WithCallbackData(text: $"1", callbackData: $"1"),
                    InlineKeyboardButton.WithCallbackData(text: $"2", callbackData: $"2"),
                    InlineKeyboardButton.WithCallbackData(text: $"3", callbackData: $"3"),
                    InlineKeyboardButton.WithCallbackData(text: $"4", callbackData: $"4"),
                    InlineKeyboardButton.WithCallbackData(text: $"5", callbackData: $"5"),
                    InlineKeyboardButton.WithCallbackData(text: $"6", callbackData: $"6"),
                    InlineKeyboardButton.WithCallbackData(text: $"7", callbackData: $"7"),
                    InlineKeyboardButton.WithCallbackData(text: $"8", callbackData: $"8"),
                    InlineKeyboardButton.WithCallbackData(text: $"9", callbackData: $"9")
            });
            return result;
        }
        public static InlineKeyboardMarkup Input1dEx()
        {
            InlineKeyboardMarkup result = new(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"1", callbackData: $"1"),
                    InlineKeyboardButton.WithCallbackData(text: $"2", callbackData: $"2"),
                    InlineKeyboardButton.WithCallbackData(text: $"3", callbackData: $"3"),
                    InlineKeyboardButton.WithCallbackData(text: $"4", callbackData: $"4"),
                    InlineKeyboardButton.WithCallbackData(text: $"5", callbackData: $"5"),
                    InlineKeyboardButton.WithCallbackData(text: $"6", callbackData: $"6"),
                    InlineKeyboardButton.WithCallbackData(text: $"7", callbackData: $"7"),
                    InlineKeyboardButton.WithCallbackData(text: $"8", callbackData: $"8"),
                    InlineKeyboardButton.WithCallbackData(text: $"9", callbackData: $"9")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(text: $"exit", callbackData: "exit")
                }
            });
            return result;
        }
        public static InlineKeyboardMarkup Input1d(string a)
        {
            InlineKeyboardMarkup result = new(new[]
            {
                    InlineKeyboardButton.WithCallbackData(text: $"1", callbackData: $"1{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"2", callbackData: $"2{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"3", callbackData: $"3{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"4", callbackData: $"4{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"5", callbackData: $"5{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"6", callbackData: $"6{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"7", callbackData: $"7{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"8", callbackData: $"8{a}"),
                    InlineKeyboardButton.WithCallbackData(text: $"9", callbackData: $"9{a}")
            });
            return result;
        }
        public static InlineKeyboardMarkup InputBool() 
        {
            InlineKeyboardMarkup result = new(new[]
            {
                InlineKeyboardButton.WithCallbackData(text: $"yes", callbackData: $"yes"),
                InlineKeyboardButton.WithCallbackData(text: $"no", callbackData: $"no")
            });
            return result;
        }

    }
}
