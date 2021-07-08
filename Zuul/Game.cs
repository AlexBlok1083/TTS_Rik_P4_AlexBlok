using System;

namespace Zuul
{
	public class Game
	{
		private Parser parser;
		private Player player;
		private bool FinishGame = false;

		public Game ()
		{
			player = new Player();
			parser = new Parser();
			CreateRooms();
		}

		private void CreateRooms()
		{
			// create the rooms
			Room outside = new Room("outside the main entrance of the university");
			Room theatre = new Room("in a lecture theatre");
			Room pub = new Room("in the campus pub");
			Room lab = new Room("in a computing lab");
			Room office = new Room("in the computing admin office");
			Room bar = new Room("in the bar above the pub");
			Room EndGame = new Room("This is the end of the game.");

			// initialise room exits
			outside.AddExit("east", theatre);
			outside.AddExit("south", lab);
			outside.AddExit("west", pub);

			theatre.AddExit("west", outside);

			bar.AddExit("down", pub);
			bar.AddExit("east", EndGame);

			pub.AddExit("up", bar);
			pub.AddExit("east", outside);

			lab.AddExit("north", outside);
			lab.AddExit("east", office);

			office.AddExit("west", lab);

			player.CurrentRoom = outside;  // start game outside

			bar.Chest.Put("medkit", new Item(5, "Heals 20."));
			pub.Chest.Put("key", new Item(5, "Key naar bar"));

			bar.LockDoor();
			EndGame.FinishRoom = true;
		}

		/**
		 *  Main play routine.  Loops until end of play.
		 */
		public void Play()
		{
			PrintWelcome();

			// Enter the main command loop.  Here we repeatedly read commands and
			// execute them until the player wants to quit.
			bool finished = false;
			while (!finished)
			{
				if (player.IsAlive())
				{
					Command command = parser.GetCommand();
					finished = ProcessCommand(command);
					if (player.CurrentRoom.FinishRoom)
					{
						finished = true;
						Console.WriteLine("You have finished the game.");
					}
				}
				else
				{
					finished = true;
					Console.WriteLine("You died");
				}
			}
			Console.WriteLine("Thank you for playing.");
			Console.WriteLine("Press [Enter] to continue.");
			Console.ReadLine();
		}

		/**
		 * Print out the opening message for the player.
		 */
		private void PrintWelcome()
		{
			Console.WriteLine();
			Console.WriteLine("Welcome to Zuul!");
			Console.WriteLine("Zuul is a new, incredibly boring adventure game.");
			Console.WriteLine("Type 'help' if you need help.");
			Console.WriteLine();
			Console.WriteLine(player.CurrentRoom.GetLongDescription());
		}

		/**
		 * Given a command, process (that is: execute) the command.
		 * If this command ends the game, true is returned, otherwise false is
		 * returned.
		 */
		private bool ProcessCommand(Command command)
		{
			bool wantToQuit = false;

			if(command.IsUnknown())
			{
				Console.WriteLine("I don't know what you mean...");
				return false;
			}

			string commandWord = command.GetCommandWord();
			switch (commandWord)
			{
				case "help":
					PrintHelp();
					break;
				case "go":
					GoRoom(command);
					player.Damage(10);
					break;
				case "quit":
					wantToQuit = true;
					break;
				case "look":
					look();
					break;
				case "status":
					player.Status();
					break;
				case "inventory":
					player.Inventory();
					break;
				case "take":
					Take(command);
					break;
				case "drop":
					Drop(command);
					break;
				case "use":
					player.Use(command);
					break;
			}

			return wantToQuit;
		}

		// implementations of user commands:

		/**
		 * Print out some help information.
		 * Here we print the mission and a list of the command words.
		 */
		private void PrintHelp()
		{
			Console.WriteLine("You are lost. You are alone.");
			Console.WriteLine("You wander around at the university.");
			Console.WriteLine();
			// let the parser print the commands
			parser.PrintValidCommands();
		}

		/**
		 * Try to go to one direction. If there is an exit, enter the new
		 * room, otherwise print an error message.
		 */
		private void GoRoom(Command command)
		{
			if(!command.HasSecondWord())
			{
				// if there is no second word, we don't know where to go...
				Console.WriteLine("Go where?");
				return;
			}

			string direction = command.GetSecondWord();

			// Try to go to the next room.
			Room nextRoom = player.CurrentRoom.GetExit(direction);

			if (nextRoom == null)
			{
				Console.WriteLine("There is no door to "+direction+"!");
			}
			else
			{
				if (nextRoom.Locked)
				{
					Console.WriteLine("This door is locked. You need a key for this.");
					return;
				}
				player.CurrentRoom = nextRoom;
				Console.WriteLine(player.CurrentRoom.GetLongDescription());
			}
		}

		private void look()
		{
			Console.WriteLine(player.CurrentRoom.GetLongDescription());
			Console.WriteLine(player.CurrentRoom.Chest.checkItemsRoom());
		}

		private void Take(Command command)
		{
			if ((command).HasSecondWord())
			{
				player.TakeFromChest(command.GetSecondWord());
			}
		}
		private void Drop(Command command)
		{
			if ((command).HasSecondWord())
			{
				player.DropToChest(command.GetSecondWord());
			}
		}
	}
}
