using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using PWBG_BOT.Core.UserAccounts;
using PWBG_BOT.Core.Items;
using Discord.Commands;
using Discord.WebSocket;

namespace PWBG_BOT.Core.SurvivorInventory
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
            await channel.SendMessageAsync($"{user.Mention} get {item.Name}");
            UserAccount acc = UserAccounts.UserAccounts.GetUserAccount(user);
            UserAccounts.UserAccounts.GiveBuff(acc,item,channel);
            SaveInvent(user);
        }

        public static async Task CountDownItem(SocketUser user)
        {
            if (user == null) return;
            Inventory inv = GetInventory(user);
            for (int i = inv.Items.Count - 1; i >= 0; i--)
            {
                if (inv.Items[i].Countdown == -1) continue;
                inv.Items[i].Countdown--;
                if (inv.Items[i].Countdown <= 0)
                {
                    Console.WriteLine($"{inv.Items[i].Name} countdown {inv.Items[i].Countdown}");
                    await DropAnyItem(user, inv.Items[i]);
                    SaveInvent();
                }
            }
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

        public static async Task DropItem(SocketUser user,int index)
        {
            Inventory select = GetInventory(user);
            if (select == null) return;
            if (select.Items.Count < index || !select.Items[index - 1].Active)
            {
                await GlobalVar.ChannelSelect.SendMessageAsync("`Can't Drop Item`");
                return;
            }
            Item item = select.Items[index - 1];
            select.Items.Remove(item);
            SaveInvent(user);
            await GlobalVar.ChannelSelect.SendMessageAsync($"`Item {item.Name} Dropped from` {user.Username} `inventory`");
        }

        public static async Task DropPassiveItem(SocketUser user, int index)
        {
            Inventory select = GetInventory(user);
            if (select.Items.Count < index || !select.Items[index - 1].Type.Equals("Passive"))
            {
                await GlobalVar.ChannelSelect.SendMessageAsync("`Can't Drop Item`");
                return;
            }
            select.Items.Remove(select.Items[index - 1]);
            SaveInvent(user);
        }

        public static async Task DropAnyItem(SocketUser user, Item item)
        {
            Inventory select = GetInventory(user);
            select.Items.Remove(item);
            SaveInvent(user);
            await GlobalVar.ChannelSelect.SendMessageAsync($"`Item {item.Name} Dropped from {user.Username} inventory`");
        }

        public static bool CheckHaveThisItem(UserAccount user, string name)
        {
            var realUser = GlobalVar.GuildSelect.GetUser(user.ID);
            Inventory inv = GetInventory(realUser);
            foreach (var i in inv.Items)
            {
                if (i.Name.Equals(name)) return true;
            }
            return false;
        }

        public static async Task PassiveItem(SocketUser user)
        {
            var select = GetInventory(user);
            var acc = UserAccounts.UserAccounts.GetUserAccount(user);
            foreach (var item in select.Items)
            {
                if (!item.Active)
                {
                    switch (item.Name)
                    {
                        case "Armlet Of Greed":
                            await ItemTech.ArmletOfGreed(acc, item);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static async Task UseActiveItem(SocketUser user, int index, UserAccount target = null,int optional = 0)
        {
            var select = GetInventory(user);
            if (select.Items.Count < index)
            {
                await GlobalVar.ChannelSelect.SendMessageAsync("No Item Selected");
                return;
            }
            var use = select.Items[index - 1];
            var me = UserAccounts.UserAccounts.GetUserAccount(user);
            if (!use.Active) {
                await GlobalVar.ChannelSelect.SendMessageAsync("Can't use passive item");
                return;
            } 
            List<UserAccount> despacito = new List<UserAccount>();
            #region List active items
            switch (use.Name)
            {
                case "Booby Trap":
                    target = UserAccounts.UserAccounts.GetRandomBesideMe(me);
                    if (target == null)
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("There is no more Survivor to target");
                        return;
                    }
                    if (UserAccounts.UserAccounts.CheckHaveThisBuff(target, "Stealth"))
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("That target can't be targeted");
                        return;
                    }
                    await ItemTech.UseDecreasingHPItem(me,use, target);
                    despacito.Add(target);
                    break;
                case "Broken Arrow":
                    target = UserAccounts.UserAccounts.GetRandomBesideMe(me);
                    if (target == null)
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("There is no more Survivor to target");
                        return;
                    }
                    if (UserAccounts.UserAccounts.CheckHaveThisBuff(target, "Stealth"))
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("That target can't be targeted");
                        return;
                    }
                    await ItemTech.UseDecreasingHPItem(me,use, target);
                    despacito.Add(target);
                    break;
                case "Combat Kit":
                    target = UserAccounts.UserAccounts.GetRandomBesideMe(me);
                    if (target == null)
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("There is no more Survivor to target");
                        return;
                    }
                    if (UserAccounts.UserAccounts.CheckHaveThisBuff(target, "Stealth"))
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("That target can't be targeted");
                        return;
                    }
                    await ItemTech.UseDecreasingHPItem(me, use, target);
                    await ItemTech.UseIncreasingHPItem(use,me);
                    despacito.Add(target);
                    break;
                case "Converter":
                    await ItemTech.UseDecreasingPointItem(use, me);
                    await ItemTech.UseIncreasingHPItem(use, me);
                    break;
                case "Copycat Generator":
                    if (select.Items.Count < optional) await GlobalVar.ChannelSelect.SendMessageAsync("No Item Selected");
                    var copy = select.Items[optional - 1];
                    await GiveItem(user,copy,GlobalVar.ChannelSelect);
                    break;
                case "Dagger":
                    if (target == null)
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("Tag a Survivor to use this item");
                        return;
                    }
                    if (UserAccounts.UserAccounts.CheckHaveThisBuff(target, "Stealth"))
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("That target can't be targeted");
                        return;
                    }
                    if (target.HP > 3)
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("Tag a Survivor with HP less than 3 to use this item");
                        return;
                    }
                    await ItemTech.AutoKO(me,target);
                    despacito.Add(target);
                    break;
                case "Ejector":
                    if (select.Items.Count <= 1)
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("There is no item to drop");
                        return;
                    }
                    for (int i = 0; i < select.Items.Count; i++)
                    {
                        if (select.Items[i] == use) continue;
                        await DropItem(user,i);
                    }
                    break;
                case "Energy Drink":
                    await ItemTech.UseIncreasingHPItem(use, me);
                    break;
                case "First Aid Kit":
                    await ItemTech.UseIncreasingHPItem(use, me);
                    break;
                case "Frag Grenade":
                    List<UserAccount> alives = UserAccounts.UserAccounts.GetAllAliveUsers();
                    int x = 5;
                    if (alives.Count - 1 <= 5)
                    {
                        x = alives.Count;
                        foreach (var a in alives)
                        {
                            if (a == null)
                            {
                                await GlobalVar.ChannelSelect.SendMessageAsync("There is no more Survivor to target");
                                return;
                            }
                            if (a == me) continue;
                            await ItemTech.UseDecreasingHPItem(me, use, a);
                            despacito.Add(a);
                        }
                        break;
                    }
                    List<UserAccount> targets = new List<UserAccount>();
                    for (int i = 0; i < x; i++)
                    {
                        target = UserAccounts.UserAccounts.GetRandomBesideMe(me);
                        if (target == null)
                        {
                            await GlobalVar.ChannelSelect.SendMessageAsync("There is no more Survivor to target");
                            return;
                        }
                        if (targets.Contains(target))
                        {
                            i--;
                            continue;
                        }
                        await ItemTech.UseDecreasingHPItem(me, use, target);
                        targets.Add(target);
                    }
                    despacito = targets;
                    break;
                case "Fresh Drink":
                    await ItemTech.UseIncreasingHPItem(use, me);
                    break;
                case "Homemade Dynamite":
                    if (target == null)
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("Tag a Survivor to use this item");
                        return;
                    }
                    if (UserAccounts.UserAccounts.CheckHaveThisBuff(target, "Stealth"))
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("That target can't be targeted");
                        return;
                    }
                    if (optional <= 0) return;
                    await ItemTech.UseDecreasingHPItem(me,optional,target);
                    despacito.Add(target);
                    break;
                case "Legendary Katana":
                    if (target == null)
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("Tag a Survivor to use this item");
                        return;
                    }
                    if (UserAccounts.UserAccounts.CheckHaveThisBuff(target, "Stealth"))
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("That target can't be targeted");
                        return;
                    }
                    var alive = UserAccounts.UserAccounts.GetAllAliveUsers();
                    if (alive.Count <= 1) return;
                    await ItemTech.UseDecreasingHPItem(me, use, target);
                    await ItemTech.UseDecreasingHPAOE(me, 2, alive);
                    alive.Remove(me);
                    despacito = alive;
                    break;
                case "Mini Catapult":
                    if (select.Items.Count < optional) return;
                    Item drop = select.Items[optional - 1];
                    if (drop == use) return;
                    DropAnyItem(user,drop);
                    break;
                case "Napalm Flare":
                    if (target == null)
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("Tag a Survivor to use this item");
                        return;
                    }
                    if (UserAccounts.UserAccounts.CheckHaveThisBuff(target, "Stealth"))
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("That target can't be targeted");
                        return;
                    }
                    despacito.Add(target);
                    break;
                case "Nullifier":
                    if (target == null)
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("Tag a Survivor to use this item");
                        return;
                    }
                    if (UserAccounts.UserAccounts.CheckHaveThisBuff(target, "Stealth"))
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("That target can't be targeted");
                        return;
                    }
                    if (target.Buffs.Count <= 0)
                    {
                        await GlobalVar.ChannelSelect.SendMessageAsync("This Survivor doesn't have buff to remove");
                        return;
                    }
                    await ItemTech.RemoveRandomTargetBuff(target);
                    despacito.Add(target);
                    break;
                    
            }
            #endregion
            foreach (var d in use.Debuffs)
            {
                foreach (var u in despacito)
                {
                    await ItemTech.InflictDebuff(u,d);
                }
            }
            await DropItem(user, index);
        }

    }
}
