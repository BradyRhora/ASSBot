﻿using System;
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
        long coins;
        long owe;
        DateTime loanDate;

        public User(string userdata)
        {
            var data = userdata.Split(':');
            id = Convert.ToUInt64(data[0]);
            coins = Convert.ToInt32(data[1]);
            owe = Convert.ToInt32(data[2]);
            var dates = data[3].Split('-');
            int year = Convert.ToInt32(dates[0]);
            if (year == 0) year = DateTime.Now.Year;
            DateTime theDate = new DateTime(year, 1, 1).AddDays(Convert.ToInt32(dates[1]) - 1);
        }

        public long GetCoins() { return coins; }
        public long GetDebt() { return owe; }

        public void GiveCoins(long amount) { coins += amount; Save(); }
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
            owe += Convert.ToInt32(Math.Ceiling(owe * 0.01));
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
        
        public override string ToString(){ return Bot.client.GetUser(id).Username; }
    }
}
