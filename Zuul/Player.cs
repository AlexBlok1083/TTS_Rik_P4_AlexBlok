using System;

namespace Zuul
{
	public class Player
	{
		public Room CurrentRoom { get; set; }
		private int health;
		private Inventory inventory;

		public Player()
		{
			CurrentRoom = null;
			health = 100;
			inventory = new Inventory(25);
		}

		public void Status()
		{
			Console.WriteLine("Health: " + health + "/100");
		}

		public void Inventory()
		{
			Console.WriteLine(inventory.checkItemsInventory());
		}

		public string Use(Command command)
		{
			string itemName = command.GetSecondWord();
			Item item = inventory.Get(itemName);

			if (item == null)
			{
				Console.WriteLine("The item" + itemName + "is not in your inventory.");
				return "";
			}
			if (itemName == "medkit")
			{
				Heal(20);
				Console.WriteLine("You used your item and gained 20 more health.");
				return "";
			}
			if (itemName == "key")
			{
				string exitstring = command.GetThirdWord();
				Room next = CurrentRoom.GetExit(exitstring);
				next.UnlockDoor();
				Console.WriteLine("You used the key and opened the door.");
				return "";
			}
			return "";
		}

		public bool TakeFromChest(string itemName)
		{
			Item item = CurrentRoom.Chest.Get(itemName);
			if (item == null)
			{
				Console.WriteLine(itemName + " is not in this room.");
				return false;
			}
			
			if (inventory.Put(itemName, item))
			{
				Console.WriteLine(itemName + " is now in your inventory.");
				return true;
			}

			CurrentRoom.Chest.Put(itemName, item);
			Console.WriteLine("you are encumered.");
			return false;
		}

		public bool DropToChest(string itemName)
		{
			Item item = inventory.Get(itemName);
			if (item == null)
			{
				Console.WriteLine(itemName + " is not in your inventory.");
				return false;
			}

			if (CurrentRoom.Chest.Put(itemName, item))
			{
				Console.WriteLine(itemName + " is now in the chest.");
				return true;
			}

			inventory.Put(itemName, item);
			Console.WriteLine(itemName + " does not fit in the room. The room is full.");
			return false;
		}

		public int Damage (int amount)
		{
			health = health - amount;
			return amount;
		}

		public int Heal(int amount)
		{
			health = health + amount;
			return amount;
		}

		public bool IsAlive()
		{
			return health > 0;
		}

	}
}