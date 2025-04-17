using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading;

internal class Program
{
    static Player player = new Player();
    static Shop shop = new Shop();

    static void Main(string[] args)
    {
        Name();
    }

    static void TypeText(string text, int delay = 10)// 여기서 딜레이 시간 조절
    {
        foreach (char c in text)
        {
            Console.Write(c);
            Thread.Sleep(delay); // 딜레이 먹이는 기능
        }
        Console.WriteLine();
    }

    enum ItemType // enum은 열거형? 이라는 것으로 일일이 int들을 입력하지 않고 하나로 묶어놓는 형식이라 함
    {
        Weapon,
        Armor
    }

    class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Power { get; set; }
        public ItemType Type { get; set; }
        public int Price { get; set; }

        public Item(string name, string description, int power, ItemType type, int price)
        {
            Name = name;
            Description = description;
            Power = power;
            Type = type;
            Price = price;
        }
    }

    class Shop
    {
        public List<Item> shopList;

        public Shop()
        {
            shopList = new List<Item>
            {
                new Item("종이 검|공격력", "날카롭게 다듬어진 종이 입니다", 1, ItemType.Weapon, 100),
                new Item("낡은 검|공격력", "시간의 흔적이 느껴지는 낡은 검입니다.", 3, ItemType.Weapon, 500),
                new Item("스파르타의 창|공격력", "스파르타의 전사들이 사용했다는 전설의 창입니다.", 7, ItemType.Weapon, 1000),
                new Item("종이갑옷|방어력", "기름먹인 종이를 겹쳐 만든 갑옷입니다.", 1, ItemType.Armor, 100),
                new Item("가죽 갑옷|방어력", "얇은 가죽으로 만들어져 가벼운 방어구입니다.", 2, ItemType.Armor, 500),
                new Item("무쇠갑옷|방어력", "무쇠로 만들어져 튼튼한 갑옷입니다.", 5, ItemType.Armor, 1000),
            }; // ; 마무리되는 이유는 결국 한줄의 변수선언이라서.
        }
    }


    class Player
    {
        public string playerName = "";
        public int playerLevel = 1;
        public int baseAttack = 10;
        public int baseDefens = 5;
        public int playerHealthPoint;
        public int playerGold;

        public Item? equippedWeapon; // 본래 null을 추가할 수 없지만, ?를 추가함으로써 null을 허용하게 만든다고함
        public Item? equippedArmor;
        public List<Item> inventory;
        
        public Player()
        {
            playerName = "";
            playerLevel = 1;
            baseAttack = 10;
            baseDefens = 5;
            playerHealthPoint = 100;
            playerGold = 1500;

            inventory = new List<Item>
            {

            };
        }

        public int TotalAttack => baseAttack + (equippedWeapon?.Power ?? 0); // ?는 입력값이 없으면 바로 null로, ??는 파워값의 변동적인 수치를 반영하며 입력값이 없으면 null로
        public int TotalDefens => baseDefens + (equippedArmor?.Power ?? 0);
    }

    static void Name()
    {
        TypeText("종이의 던전에 오신 것을 환영합니다.");
        TypeText("당신의 이름은 무엇인가요?");
        Console.Write("\n>> ");
        player.playerName = Console.ReadLine() ?? "";

        while (true)
        {
            TypeText($"{player.playerName}님 환영합니다, 게임을 시작하시겠습니까? (y / n)");
            Console.Write("\n>> ");
            string input = Console.ReadLine()?.ToLower() ?? "";

            if (input == "y")
            {
                Console.Clear();
                Home();
                break;
            }

            else if (input == "n")
            {
                TypeText("게임을 종료합니다...");
                break;
            }

            else
            {
                Console.Clear();
                TypeText("잘못된 입력입니다!, y 또는 n을 입력해주세요");
            }
        }
    }

    class SaveData
    {
        public required string playerName { get; set; }
        public int playerLevel { get; set; }
        public int baseAttack { get; set; }
        public int baseDefens { get; set; }
        public int playerHealthPoint { get; set; }
        public int playerGold { get; set; }

        public required List<Item> inventory { get; set; }
        public Item? equippedWeapon { get; set; }
        public Item? equippedArmor { get; set; }
    }

    static void SaveGame() // 검색해서 만들긴 했는데, json이란 형식으로 저장된다고 함.
    {

        var saveData = new SaveData
        {
            playerName = player.playerName,
            playerLevel = player.playerLevel,
            baseAttack = player.baseAttack,
            baseDefens = player.baseDefens,
            playerHealthPoint = player.playerHealthPoint,
            playerGold = player.playerGold,
            inventory = player.inventory,
            equippedWeapon = player.equippedWeapon,
            equippedArmor = player.equippedArmor
        };

        Console.Clear();
        string json = JsonSerializer.Serialize(saveData);
        File.WriteAllText("savegame.json", json); // 뭔가 저장 파일 종류가 다양한데 json이 보기좋고 간결하게 저장된다고 함
        TypeText("게임이 저장되었습니다.");
    }

    static void LoadGame()
    {
        if (!File.Exists("savegame.json"))
        {
            Console.Clear();
            TypeText("저장된 데이터가 없습니다.");
            return;
        }

        string json = File.ReadAllText("savegame.json");
        var saveData = JsonSerializer.Deserialize<SaveData>(json);

        if (saveData != null)
        {
            Console.Clear();

            player.playerName = saveData.playerName;
            player.playerLevel = saveData.playerLevel;
            player.baseAttack = saveData.baseAttack;
            player.baseDefens = saveData.baseDefens;
            player.playerHealthPoint = saveData.playerHealthPoint;
            player.playerGold = saveData.playerGold;
            player.inventory = saveData.inventory ?? new List<Item>();
            player.equippedWeapon = saveData.equippedWeapon;
            player.equippedArmor = saveData.equippedArmor;

            // 불러오기 기능을 사용시, 세이브 데이터를 불러오긴 하지만 인벤토리에 E표시가 나오지 않아서 추가
            player.equippedWeapon = player.inventory.Find(i => i.Name == saveData.equippedWeapon?.Name);
            player.equippedArmor = player.inventory.Find(i => i.Name == saveData.equippedArmor?.Name);

            TypeText("게임을 불러왔습니다.");
        }
    }

    static void Home()
    {
        TypeText("마을에 오신 것을 환영합니다");

        while (true)
        {
            Console.WriteLine("\n[1]\t상태\n[2]\t가방\n[3]\t상점\n[4]\t저장하기\n[5]\t불러오기\n[0]\t종료");
            Console.Write("\n>> ");
            string input = Console.ReadLine() ?? "";

            switch (input)
            {
                case "1":
                    Console.Clear();
                    ShowState();
                    break;

                case "2":
                    Console.Clear();
                    Inventory();
                    break;

                case "3":
                    Console.Clear();
                    HomeShop();
                    break;

                case "4":
                    SaveGame();
                    break;

                case "5":
                    LoadGame();
                    break;

                case "0":
                    TypeText("게임을 종료합니다...");
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                    break;

                default:
                    Console.Clear();
                    TypeText("잘못된 입력입니다.");
                    break;
            }
        }
    }

    static void ShowState()
    {
        TypeText("===== 상태 =====");
        Console.WriteLine($"이름: {player.playerName}\t(전사)");
        Console.WriteLine($"레벨: {player.playerLevel}");

        int weaponPower = player.equippedWeapon?.Power ?? 0;
        int armorPower = player.equippedArmor?.Power ?? 0;

        Console.WriteLine($"공격력: {player.TotalAttack} + ({weaponPower})");
        Console.WriteLine($"방어력: {player.TotalDefens} + ({armorPower})");

        Console.WriteLine($"체력: {player.playerHealthPoint}");
        Console.WriteLine($"골드: {player.playerGold}");

        TypeText("\n[0] 돌아가기");

        while (true)
        {
            Console.Write("\n>> ");
            string input = Console.ReadLine() ?? "";

            if (input == "0")
            {
                Console.Clear();
                Home();
                break;
            }

            else
            {
                TypeText("잘못된 입력입니다.");
            }
        }
    }

    static void Inventory()
    {
        TypeText("===== 가방 =====");

        for (int i = 0; i < player.inventory.Count; i++)
        {
            var item = player.inventory[i];
            string equippedMark = "";

            if (player.equippedWeapon == item)
            {
                equippedMark = "[E]";
            }

            else if (player.equippedArmor == item)
            {
                equippedMark = "[E]";
            }

            Console.WriteLine($"{i + 1}\t{equippedMark}\t{item.Name} + {item.Power}\t{item.Description}");
        }

        TypeText("\n[1] 장비 관리\n[0] 돌아가기");
        Console.Write("\n>> ");
        string input = Console.ReadLine() ?? "";

        if (input == "1")
        {
            Console.Clear();
            ManageEquipment();
        }

        else if (input == "0")
        {
            Console.Clear();
            Home();
        }

        else
        {
            TypeText("올바른 입력이 아닙니다, 아무 키나 눌러주세요.");
            Console.ReadKey();
            Console.Clear();
            Inventory();
        }

    }

    static void ManageEquipment()
    {
        TypeText("===== 장비 관리 =====");

        for (int i = 0; i < player.inventory.Count; i++)
        {
            var item = player.inventory[i];
            string equippedMark = "";

            if (player.equippedWeapon == item)
            {
                equippedMark = "[E]";
            }

            else if (player.equippedArmor == item)
            {
                equippedMark = "[E]";
            }

            Console.WriteLine($"[{i + 1}]\t{equippedMark}\t{item.Name} + {item.Power}\t{item.Description}");
        }

        TypeText("\n아이템 번호를 입력해 장착하거나 장착 해제할 수 있습니다.");
        TypeText("[0] 돌아가기");

        Console.Write("\n>> ");
        string input = Console.ReadLine() ?? "";

        if (int.TryParse(input, out int choice))
        {
            switch (choice)
            {
                case 0:
                    Console.Clear();
                    Inventory();
                    return;

                default:
                    if (choice > 0 && choice <= player.inventory.Count)
                    {
                        var selectedItem = player.inventory[choice - 1];

                        if (player.equippedWeapon == selectedItem)
                        {
                            player.equippedWeapon = null; // 본래 null을 추가할 수 없지만, ?를 추가함으로써 null을 허용하게 만든다고함
                            TypeText($"{selectedItem.Name}을(를) 무기에서 해제했습니다.");
                        }
                        else if (player.equippedArmor == selectedItem)
                        {
                            player.equippedArmor = null; // 본래 null을 추가할 수 없지만, ?를 추가함으로써 null을 허용하게 만든다고함
                            TypeText($"{selectedItem.Name}을(를) 방어구에서 해제했습니다.");
                        }
                        else
                        {
                            if (selectedItem.Type == ItemType.Armor)
                            {
                                player.equippedArmor = selectedItem;
                                TypeText($"{selectedItem.Name}을(를) 방어구로 장착했습니다.");
                            }
                            else if (selectedItem.Type == ItemType.Weapon)
                            {
                                player.equippedWeapon = selectedItem;
                                TypeText($"{selectedItem.Name}을(를) 무기로 장착했습니다.");
                            }
                        }
                    }

                    else
                    {
                        TypeText("잘못된 입력입니다.");
                    }
                    break;
            }
        }

        else
        {
            TypeText("숫자를 입력해주세요.");
        }

        TypeText("\n아무 키나 눌러주세요.");
        Console.ReadKey();
        Console.Clear();
        ManageEquipment();
    }

    static void HomeShop()
    {
        TypeText("===== 상점 =====");

        for (int i = 0; i < shop.shopList.Count; i++)
        {
            var item = shop.shopList[i];
            Console.WriteLine($"{i + 1}\t{item.Name} +{item.Power}\t({item.Type})\t{item.Description}\t{item.Price}골드");
        }

        Console.WriteLine($"\n소지금\t {player.playerGold}골드");
        TypeText("\n[1] 구매하기");
        TypeText("[0] 돌아가기");

        Console.Write("\n>> ");
        string input = Console.ReadLine() ?? "";

        if (int.TryParse(input, out int num))
        {
            if (num == 0)
            {
                Console.Clear();
                Home();
            }

            else if (num == 1)
            {
                Console.Clear();
                HomeShopBuy();
            }
        }

        else
        {
            TypeText("숫자를 입력해주세요.");
        }

        TypeText("\n아무 키나 눌러주세요.");
        Console.ReadKey();
        Console.Clear();
        HomeShop();
    }

    static void HomeShopBuy()
    {
        TypeText("===== 구매 =====");

        for (int i = 0; i < shop.shopList.Count; i++)
        {
            var item = shop.shopList[i];
            Console.WriteLine($"[{i + 1}]\t{item.Name} +{item.Power}\t({item.Type})\t{item.Description}\t{item.Price}골드");
        }

        Console.WriteLine($"\n소지금\t {player.playerGold}골드");
        TypeText("\n아이템 번호를 입력해 구매할 수 있습니다.");
        TypeText("[0] 돌아가기");

        Console.Write("\n>> ");
        string input = Console.ReadLine() ?? "";

        if (int.TryParse(input, out int num))
        {
            if (num == 0)
            {
                Console.Clear();
                HomeShop();
            }
            else if (num > 0 && num <= shop.shopList.Count)
            {
                var selectedItem = shop.shopList[num - 1];

                bool alreadyOwned = player.inventory.Exists(item => item.Name == selectedItem.Name);

                if (alreadyOwned)
                {
                    TypeText("이미 구매한 아이템입니다.");
                }

                else if (player.playerGold >= selectedItem.Price)
                {
                    player.playerGold -= selectedItem.Price; // 해당 아이템에 매겨진 금액만큼 플레이어 골드를 차감한다는 내용
                    player.inventory.Add(selectedItem); // 플레이어 인벤토리에 추가한다는 내용
                    TypeText($"{selectedItem.Name}을(를) {selectedItem.Price}골드에 구매를 완료했습니다!");
                }

                else
                {
                    TypeText("골드가 부족합니다.");
                }
            }

            else
            {
                TypeText("잘못된 입력입니다.");
            }
        }
        else
        {
            TypeText("숫자를 입력해주세요.");
        }

        TypeText("\n아무 키나 눌러주세요.");
        Console.ReadKey();
        Console.Clear();
        HomeShop();
    }
}