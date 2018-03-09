using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Discord;

namespace ASSbot
{
    class SlotMachine
    {
        IUser gambler;
        int bet;
        int[] spins = new int[3];
        Random rdm = new Random();
        int spinCounter = 0;
        public Timer spinTimer;
        bool cashedOut = false;

        public SlotMachine(IUser user, int bet)
        {
            gambler = user;
            this.bet = bet;
        }

        static Slot BlankSlot = new Slot("black_large_square", 0);
        Slot[] slots =
        {
            BlankSlot,
            new Slot("apple",2,"fruit"),
            new Slot("grapes",3,"fruit"),
            new Slot("cherries",10,"fruit"),
            BlankSlot,
            new Slot("bell",20),
            new Slot("seven",50),
            new Slot("gem",100)
        };

        public void Spin()
        {
            for (int i = 0; i < 3; i++) spins[i] = rdm.Next(slots.Count());
            spinCounter++;
        }
        public string Generate()
        {
            string board = ":slot_machine: **SLOTS** :slot_machine:\n" +
                           "===========\n" +
                           $"{slots[Prev(spins[0])]} : {slots[Prev(spins[1])]} : {slots[Prev(spins[2])]}\n" +
                           $"{slots[spins[0]]} : {slots[spins[1]]} : {slots[spins[2]]} :arrow_left:\n" +
                           $"{slots[Next(spins[0])]} : {slots[Next(spins[1])]} : {slots[Next(spins[2])]}";

            if (SpinCount() >= 3)
            {
                string resultMSG = ":poop: YOU LOSE";
                if (GetMultiplier() != 0) resultMSG = ":star: YOU WIN!";

                board += "\n===========\n" +
                         resultMSG + "\n" +
                         "===========";
            }

            return board;
        }
        public int SpinCount() { return spinCounter; }

        public double GetMultiplier()
        {
            for (int i = 0; i < slots.Count(); i++)
            {
                if (spins[0] == i && spins[1] == i && spins[2] == i) return slots[i].GetValue();
                else if (SlotCount("seven") == 2) return 10;
                else if (CategoryCount("fruit") == 3) return 5;
                else if (CategoryCount("fruit") == 2) return 3;
                else if (SlotCount("cherries") == 2) return 5;

            }
            return 0;
        }
        public IUser GetGambler() { return gambler; }
        public int GetBet() { return bet; }
        public bool CashedOut() { return cashedOut; }

        int Prev(int index)
        {
            if (index - 1 < 0) return slots.Count() - 1;
            else return index - 1;
        }
        int Next(int index)
        {
            if (index + 1 > slots.Count() - 1) return 0;
            else return index + 1;
        }

        int CategoryCount(string category)
        {
            int categoryCount = 0;
            for (int i = 0; i<3;i++)
            {
                if (slots[spins[i]].GetCategory() == category) categoryCount++;
            }
            return categoryCount;
        }
        int SlotCount(string slot)
        {
            int slotCount = 0;
            for (int i = 0; i < 3; i++)
            {
                if (slots[spins[i]].ToString() == slot) slotCount++;
            }
            return slotCount;
        }
    }
}
