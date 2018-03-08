using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ASSbot
{
    class User
    {
        ulong id;
        int coins;

        public User(string userdata)
        {
            var data = userdata.Split(':');
            id = Convert.ToUInt64(data[0]);
            coins = Convert.ToInt32(data[1]);
        }

        public int GetCoins() { return coins; }

        public void GiveCoins(int amount) { coins += amount; Save(); }

        public void Save()
        {
            string userdata = id + ":" + coins;
            var users = File.ReadAllLines("Files/Users.txt");
            for (int i = 1; i < users.Count(); i++)
            {
                if (users[i].Split(':')[0] == Convert.ToString(id))
                {
                    users[i] = userdata;
                    break;
                }
            }
            File.WriteAllLines("Files/Users.txt", users);
        }
    }
}
