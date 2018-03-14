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
        int owe;
        DateTime loanDate;

        public User(string userdata)
        {
            var data = userdata.Split(':');
            id = Convert.ToUInt64(data[0]);
            coins = Convert.ToInt32(data[1]);
            owe = Convert.ToInt32(data[2]);
            var dates = data[3].Split('-');
            DateTime theDate = new DateTime(Convert.ToInt32(dates[0]), 1, 1).AddDays(Convert.ToInt32(dates[1]) - 1);
        }

        public int GetCoins() { return coins; }
        public int GetDebt() { return owe; }

        public void GiveCoins(int amount) { coins += amount; Save(); }
        public void SetLoan(int amount) {
            owe = amount;
            coins += amount;
            loanDate = DateTime.Now;
            Save();
        }
        public void PayDebt(int amount)
        {
            coins -= amount;
            owe -= amount;
            Save();
        }

        public void Save()
        {
            string userdata = id + ":" + coins + ":" + owe + ":" + loanDate.Year + "-" + loanDate.DayOfYear;
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
