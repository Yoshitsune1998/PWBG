using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PWBG_BOT.Core.UserAccounts;
using PWBG_BOT.Core.Items;
using Discord.Commands;
using Discord.WebSocket;

namespace PWBG_BOT.Core.PlayerInventory
{
    public static class Inventories
    {
        private static List<Inventory> invs;
        private static string InventFile = "Resources/inventories.json";

        static Inventories()
        {
            if (DataStorage.SaveExist(InventFile))
            {
                invs = DataStorage.LoadInventory(InventFile).ToList();
            }
            else
            {
                invs = new List<Inventory>();
                SaveInvent();
            }
        }

        public static async Task GiveItem(SocketUser user, Item item, SocketTextChannel channel)
        {
            if (CheckFullInventory(user))
            {
                await channel.SendMessageAsync($"Inventory full {user.Mention}");
                return;
            }
            Inventory inv = GetInventory(user);
            inv.Items.Add(item);
            Tasking.Sleep(2000);
            await channel.SendMessageAsync($"{user.Mention} get {item.Name}");
            SaveInvent(user);
        }

        public static bool CheckFullInventory(SocketUser user)
        {
            var savedInv = GetInventory(user);
            return savedInv.Items.Count >= 3;
        }

        public static Item GetItems(SocketUser user,uint slot)
        {
            var savedInv = GetInventory(user);
            return savedInv.Items[(int)slot];
        }

        public static Inventory GetInventory(SocketUser user)
        {
            return GetOrCreateInventory(user.Id);
        }

        public static Inventory GetOrCreateInventory(ulong id)
        {
            var result = from i in invs
                         where i.IDInvent == id
                         select i;
            var inventory = result.FirstOrDefault();
            if (inventory == null) inventory = CreateInventory(id);
            return inventory;
        }

        private static Inventory CreateInventory(ulong id)
        {
            var newInvent = new Inventory()
            {
                IDInvent = id,
                Items = new List<Item>()
            };
            invs.Add(newInvent);
            SaveInvent();
            return newInvent;
        }

        public static void SaveInvent(SocketUser u=null)
        {
            DataStorage.SaveInventory(invs, InventFile);
            if (u != null)
            {
                var user = UserAccounts.UserAccounts.GetUserAccount(u);
                user.Inventory = GetInventory(u);
                UserAccounts.UserAccounts.SaveAccount();
            }
        }

        public static string DropItem(SocketUser user,int index)
        {
            Inventory select = GetInventory(user);
            if (select.Items.Count < index) return "`Cant Drop Item`";
            select.Items.Remove(select.Items[index - 1]);
            SaveInvent(user);
            return "`Item Dropped`";
        }

        public static string UseItem(SocketUser user, int index, UserAccount target=null, SocketGuild guild = null)
        {
            var select = GetInventory(user);
            if (select.Items.Count < index) return "No item found";
            var use = select.Items[index - 1];
            string text = "success";
            switch (use.Type)
            {
                case "passive":
                    return "Can't use passive item";
                case "target":
                    if(!Drops.UseTargetItem(use, target))
                    {
                        return "Fail to use Item";
                    }
                    break;
                case "random":
                    //
                    var me = UserAccounts.UserAccounts.GetUserAccount(user);
                    do
                    {
                        target = UserAccounts.UserAccounts.GetRandomPlayer(guild);
                    } while (me == target);
                    //
                    if (target==null||!Drops.UseTargetItem(use, target))
                    {
                        return "Fail to use Item";
                    }
                    else
                    {
                        return $"random {(target.ID).ToString()}";
                    }
                case "self":
                    if (!Drops.UseSelfItem(use, target))
                    {
                        return "Fail to use Item";
                    }
                    break;
                default:
                    return "No item found with that type";
            }
            DropItem(user, index);
            return text;
        }

    }
}
