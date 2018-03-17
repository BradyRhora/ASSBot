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
        
        
    }
}
