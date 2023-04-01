using Godot;
using System;
using System.Threading;
using Telegram.Bot.Polling;
using Telegram.Bot;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using _Config;
using System.Collections.Generic;
using Bot.scripts;

namespace botcode
{
	enum State
	{
		Menu,
		Game,
		Conclusion,
		Statistics
	} // little simplification for menu navigation
	public partial class Botcode : Node2D
	{
		RichTextLabel _logs = new RichTextLabel();
		Button _onoff = new Button();
		Label _status = new Label();

		SqlData[] heatmaps = new SqlData[9];
		char[] integers = { '1', '2', '3', '4', '5', '6', '7', '8', '9' }; // allowed characters for the game
		ITelegramBotClient bot = new TelegramBotClient(Config.token); // token
		Dictionary<long, User_Instance> data = new Dictionary<long, User_Instance> { }; // enables work for multiple user
		static CancellationTokenSource cts = new CancellationTokenSource();
		CancellationToken cancellationToken = cts.Token;
		ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } }; // receive all update types
		public async Task HandleUpdateAsync(ITelegramBotClient botclient, Update update, CancellationToken cancellationToken)
		{
			if (update.Message == null || update.Message.Text == null) return;
			var message = update.Message;
			long cid = update.Message.Chat.Id;
			string text = message.Text.ToLower();
			_logs.Text += message.Chat.FirstName + " " + message.Chat.LastName + " @" + message.Chat.Username + " date-" + message.Date + " : " + message.Text;
			if (!data.ContainsKey(cid)) Command.User_load(update);
			switch (data[cid].state)
			{
				case State.Menu:
					{
						switch (text)
						{
							case "/start":
								await Command.Send(cid, "Welcome!\nTo start blank game enter /start_game\n/start_game_n for game with custom difficulty, instead of n use number 1-39, higher = easier\n /stats to open heatmaps");
								break;
							case "/start_game":
								data[cid].state = State.Game;
								await Command.Send(cid, Command.Render(data[cid].game.Elements));
								await Command.Send(cid, "enter /write i j arg, to write value in a cell\n /prediction i j to see what numbers you can write in a cell\n/exit to exit to menu");
								break;
							case "/stats":
								heatmaps = Command.GetInf();
								data[cid].state = State.Statistics;
								await Command.Send(cid, "Enter a number that you want to se a heatmap for, enter /to_menu , to get back to menu");
								break;
							default: break;
						}
						if (text.Contains("/start_game_"))
						{
							int n;
							try { n = Convert.ToInt32(text.Replace("/start_game_", "")); }
							catch { await Command.Send(cid, "enter a number instead of n"); break; }
							if (n > 81) { await Command.Send(cid, "grid has less cells than that..."); break; }
							if (n >= 40 && n <= 81) { await Command.Send(cid, "maybe leave yourself a little bit of challenge?"); break; }
							if (n >= 0 && n <= 39)
							{
								data[cid].game.EnforceDifficulty(n);
								data[cid].state = State.Game;
								await Command.Send(cid, Command.Render(data[cid].game.Elements));
								await Command.Send(cid, "enter /write i j arg, to write value in a cell\n /prediction i j to see what numbers you can write in a cell\n/exit to exit to menu");
								break;
							}
						}
						break;
					}
				case State.Game:
					{
						if (text.Contains("/prediction"))
						{
							try
							{
								data[cid].game.Prediction();
								string a = text.Replace("/prediction", "");
								List<int> v = Command.Parse(a);
								int i = v[0]; int j = v[1];
								a = data[cid].game.PredictionRender(i, j);
								await Command.Send(cid, $"Possible numbers for that cell {i},{j} = \"{a}\"");
							}
							catch
							{
								await Command.Send(cid, "there is an error, please try again");
								break;
							}
						}
						if (text.Contains("/write "))
						{
							try
							{
								string s = text.Replace("/write", "");
								var v = Command.Parse(s);
								int i = v[0] - 1, j = v[1] - 1, arg = v[2];
								switch (data[cid].game.Prediction())
								{
									case 0:
										Console.WriteLine("lose");
										data[cid].game.Save(false, message);
										data[cid].state = State.Conclusion;
										await Command.Send(cid, "lose, to repeat send 1, to get back to menu send 0");
										break;
									case 1:
										Console.WriteLine("win");
										data[cid].game.Save(true, message);
										data[cid].state = State.Conclusion;
										await Command.Send(cid, "Win, to repeat send 1, to get back to menu send 0");
										break;
									case -1: break;
								}
								if (data[cid].game.IsValid(i, j, arg) == true && data[cid].state == State.Game)
								{
									data[cid].game.Write(i, j, Convert.ToString(arg));
									await Command.Send(cid, Command.Render(data[cid].game.Elements));
								}
								else if (data[cid].game.IsValid(i, j, arg) == false && data[cid].state == State.Game)
								{
									await Command.Send(cid, "invalid input");
								}
							}
							catch
							{
								await Command.Send(cid, "there is an error, try again");
								break;
							}
						}
						if (text.Contains("/exit"))
						{
							data[cid].state = State.Menu;
							data[cid].game.Reset();
							await Command.Send(cid, "To start blank game enter /start_game\n/start_game_n for game with custom difficulty, instead of n use number 1-39, higher = easier\n /stats to open heatmaps");
						}
						break;
					}
				case State.Conclusion:
					{
						if (text == "1")
						{
							data[cid].game.Reset();
							data[cid].game.EnforceDifficulty(data[cid].game.Difficulty);
							data[cid].state = State.Game;
							await Command.Send(cid, Command.Render(data[cid].game.Elements));
							await Command.Send(cid, "enter /write i j arg, to write value in a cell\n /prediction i j to see what numbers you can write in a cell\n/exit to exit to menu");
						}
						if (text == "0")
						{
							data[cid].game.Reset();
							data[cid].state = State.Menu;
							await Command.Send(cid, "To start blank game enter /start_game\n/start_game_n for game with custom difficulty, instead of n use number 1-39, higher = easier\n /stats to open heatmaps");
						}
						break;
					}
				case State.Statistics:
					{
						if (text == "/to_menu")
						{
							data[cid].state = State.Menu;
							await Command.Send(cid, "enter /write i j arg, to write value in a cell\n /prediction i j to see what numbers you can write in a cell\n/exit to exit to menu");
							break;
						}
						if (text.IndexOfAny(integers) != -1 && text.Length == 1)
						{
							await Command.Send(cid, Command.Render(heatmaps, Convert.ToInt16(Convert.ToInt32(text) - 1)));
						}
						break;
					}
			}
		}
		public async Task HandleErrorAsync(ITelegramBotClient botclient, Exception exception, CancellationToken cancellationToken)
		{
			_logs.Text += $"\nbase exeption + cancelation token {cancellationToken.GetType()} + exception type {exception.GetType()}";
		}// exeption handling
		public override void _Ready()
		{
			_logs = (RichTextLabel)GetNode("log");
			_onoff = (Button)GetNode("onoff");
			_onoff.Pressed += on_off;
			_status = (Label)GetNode("status");
			Command.Initialize(data, bot,_logs);
		}
		public void on_off()
		{
			bot.StartReceiving
				(
					HandleUpdateAsync,
					HandleErrorAsync,
					receiverOptions,
					cancellationToken
				);
			_status.Text = "статус:включен";
			_logs.Text = "Started bot " + bot.GetMeAsync().Result.FirstName;
		}
		public override void _Process(double delta)
		{
		}
	}
}