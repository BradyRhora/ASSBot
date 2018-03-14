using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Discord;

namespace ASSbot
{
    class Functions
    {
        public static User GetUser(ulong id)
        {
            while (true)
            {
                string[] users = File.ReadAllLines("Files/Users.txt");
                foreach (string userdata in users)
                {
                    if (userdata.Split(':')[0] == Convert.ToString(id))
                    {
                        return new User(userdata);
                    }
                }

                File.AppendAllText("Files/Users.txt", "\n" + id + ":1:0:0-0");
            }
        }

        public static User GetUser(IUser user)
        {
            return GetUser(user.Id);
        }

        public static void GiveCoins(User user, int amount)
        {
            user.GiveCoins(amount);
        }
        
        public static async void SpinSlots(IUserMessage msg, SlotMachine slot)
        {
            slot.Spin();
            await msg.ModifyAsync(x => x.Content = slot.Generate());
            if (slot.SpinCount() >= 3 && !slot.CashedOut())
            {
                int winnings = Convert.ToInt32(slot.GetWinnings());
                GiveCoins(GetUser(slot.GetGambler()), winnings);

                string resultMSG;
                if (slot.GetWinnings() != 0) resultMSG = $"You got {winnings} coins!";
                else resultMSG = $"You lost {slot.GetBet()} coins.";

                await msg.Channel.SendMessageAsync(resultMSG);

                slot.spinTimer.Dispose();
            }
        }
    }
}
