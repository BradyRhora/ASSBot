using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using System.Timers;

namespace ASSbot
{
    public class Commands : ModuleBase
    {

        Random rdm = new Random();
        [Command("help"), Summary("Displays commands and descriptions.")]
        public async Task Help()
        {
            JEmbed emb = new JEmbed();
            emb.Author.Name = "Commands";
            emb.ColorStripe = Constants.Colours.PEACH;

            foreach (CommandInfo command in Bot.commands.Commands)
            {
                emb.Fields.Add(new JEmbedField(x =>
                {
                    string header = "?" + command.Name;
                    foreach (ParameterInfo parameter in command.Parameters)
                    {
                        header += " [" + parameter.Name + "]";
                    }
                    x.Header = header;
                    x.Text = command.Summary;
                }));
            }

            await Context.Channel.SendMessageAsync("", embed: emb.Build());
        }

        [Command("insult"), Alias(new string[] { "in" })]
        public async Task Test()
        {
            string[] words = {"stupid","fuck","mother","with","shit","bitch","from hell","ass","asshoel","motherfuck","bookshelf","your",
                "you","cunt","hell","in"};

            int wordCount = rdm.Next(3, words.Count());


            string[] pre = { "i", "i will", "im gonna", "" };

            string sentence = pre[rdm.Next(pre.Count())];

            List<int> addedWords = new List<int>();

            for (int i = 0; i < wordCount; i++)
            {
                int ind = rdm.Next(words.Count());
                if (!addedWords.Contains(ind))
                {
                    addedWords.Add(ind);
                    sentence += " " + words[ind];
                }
            }

            await Context.Channel.SendMessageAsync(sentence);
        }

        [Command("connectfour"), Alias(new string[] { "cf" }), Summary("Play a game of Connect Four with another user.")]
        public async Task ConnectFour([Remainder]string user)
        {
            string param = user;
            if (!Bot.cfGame.IsOngoing())
            {
                if (param == null || param == "") await Context.Channel.SendMessageAsync("Use `?cf [username]` to invite someone to a game of Connect Four!");
                else if (param == "join" && Bot.cfGame.GetPlayer(0).Id == Context.User.Id)
                {
                    await Context.Channel.SendMessageAsync($"Player {Bot.cfGame.Turn()} goes first!\nUse `?cf [num between 1-7]` to place a chip.");
                    Bot.lastCF = await Context.Channel.SendMessageAsync(Bot.cfGame.GenerateBoard());
                    Bot.cfGame.Start();
                }
            }
            else if ((Context.User.Id == Bot.cfGame.GetPlayer(1).Id && Bot.cfGame.Turn() == 1) || (Context.User.Id == Bot.cfGame.GetPlayer(1).Id && Bot.cfGame.Turn() == 2))
            {
                bool success = int.TryParse(param, out int choice);

                if (success && choice > 0 && choice <= 7)
                {
                    Bot.cfGame.Play(choice);
                    await Bot.lastCF.DeleteAsync();
                    Bot.lastCF = await Context.Channel.SendMessageAsync(Bot.cfGame.GenerateBoard());

                    if (Bot.cfGame.IsOngoing() == false) await Context.Channel.SendMessageAsync($"Game over! Player {Bot.cfGame.Turn()} wins! And recieves {Bot.cfGame.TurnCount() * 2} coins.");
                    Functions.GiveCoins(Functions.GetUser(Bot.cfGame.GetPlayer(Bot.cfGame.Turn())), Bot.cfGame.TurnCount() * 2);
                }
                else await Context.Channel.SendMessageAsync("Please enter a valid column number between 1 and 7.");
            }
        }

        [Command("connectfour"), Alias(new string[] { "cf" })]
        public async Task ConnectFour(IUser user)
        {
            Bot.cfGame = new ConnectFour(Context.User, user);
            await Context.Channel.SendMessageAsync(user.Mention + ", " + Context.User.Username + " has challenged you to a game of Connect Four! Use `?cf join` to accept!");
        }

        [Command("profile"), Summary("View yours or another users profile.")]
        public async Task Profile() { await Profile(Context.User); }

        [Command("profile")]
        public async Task Profile(IUser user)
        {
            User u = Functions.GetUser(user.Id);

            await Context.Channel.SendMessageAsync($"```md\n{user.Username}'s Profile\n===================\n\nCoins: {u.GetCoins()}\n```");
        }

        [Command("givecoins"), Alias(new string[] { "gc" })]
        public async Task GiveCoins(int amount, IUser user)
        {
            if (Context.User.Id == 108312797162541056)
            {
                Functions.GiveCoins(Functions.GetUser(user), amount);
                await Context.Channel.SendMessageAsync($":moneybag: | {user.Username} has been given {amount} coins.");
            }
            else await Context.Channel.SendMessageAsync("Only Brady can do this currently.");
        }

        [Command("donate"), Summary("Give some of your coins to someone else.")]
        public async Task Donate(int amount, IUser user)
        {
            User u1 = Functions.GetUser(Context.User);
            User u2 = Functions.GetUser(user);

            if (u1.GetCoins() >= amount)
            {
                u1.GiveCoins(-amount);
                u2.GiveCoins(amount);

                await Context.Channel.SendMessageAsync($"{u1} has successfully given {u2} {amount} coins.");
            }
            else await Context.Channel.SendMessageAsync("You do not have that many coins.");
        }
        
        [Command("slots"), Summary("Spin the slots and win cash!")]
        public async Task Slots(int bet)
        {
            try
            {
                var user = Functions.GetUser(Context.User);

                if (user.GetCoins() == 0) await Context.Channel.SendMessageAsync("Shady Guy: \"Looks like you're outta coin... Come meet me if you need some *assistance*...\" `?loan`");
                else if (user.GetCoins() < bet) await Context.Channel.SendMessageAsync(":slot_machine: | You do not have that many coins!");
                else if (bet <= 0) await Context.Channel.SendMessageAsync(":slot_machine: | Your bet must be above 0.");
                else
                {
                    Properties.Settings.Default.jackpot += bet;
                    Properties.Settings.Default.Save();
                    user.GiveCoins(-bet);
                    SlotMachine sm = new SlotMachine(Context.User, bet);
                    var result = sm.Spin();
                    var msg = await Context.Channel.SendMessageAsync(sm.Generate() + "\n" + result);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Slots error!\n" + e.Message);
                await Context.Channel.SendMessageAsync("Slots error.\n" + e.Message);
            }
        }

        [Command("loan")]
        public async Task Loan() { await Loan(""); }

        [Command("loan")]
        public async Task Loan([Remainder]string command)
        {
            int amount;
            var user = Functions.GetUser(Context.User);

            if (command == "")
            {
                if (user.GetDebt() == 0)
                {
                    await Context.Channel.SendMessageAsync("Shady Guy: \"Hey... I can get you some quick cash, " +
                                                           "but it'll be at a high interest.. How much you want?\"" +
                                                           " **Use `?loan amount` to take out a loan up to 1000 coins.**");
                }
                else await Context.Channel.SendMessageAsync("Shady Guy: \"Where's my money, man? You still owe me " + user.GetDebt() + " coins! Hurry up!\"");
            }
            else if (int.TryParse(command,out amount))
            {
                if (amount <= 1000 && amount > 0 && user.GetDebt() == 0)
                {
                    user.SetLoan(amount);
                    await Context.Channel.SendMessageAsync("Shady Guy: \"Alright.. Make sure you get it back to me!\" **You now have a loan " +
                                                           "of " + amount + " coins.. Make sure to pay it back quickly with `?loan pay [amount]` " +
                                                           "or the interest could go through the roof!**");
                }
                else if (user.GetDebt() > 0) await Context.Channel.SendMessageAsync("Shady Guy: \"You really think I would let you take out *another* loan? Get outta here!\"");
                else await Context.Channel.SendMessageAsync("**Invalid loan amount**");
            }
            else if (command.StartsWith("pay "))
            {
                string payAmount = command.Replace("pay ", "").Replace(" ", "");
                int pay;
                if (int.TryParse(payAmount, out pay))
                {
                    if (user.GetCoins() >= pay)
                    {
                        user.PayDebt(pay);
                        if (user.GetDebt() > 0) await Context.Channel.SendMessageAsync("Shady Dealer: \"Thanks... But you still owe me! " +
                                                                                       user.GetDebt() + " more coins! Now get out!\"");
                        else if (user.GetDebt() == 0) await Context.Channel.SendMessageAsync("Shady Dealer: \"Hey.. About time! Thanks, " +
                                                                                             "you've paid off your entire debt.\"");
                        else if (user.GetDebt() < 0)
                        {
                            await Context.Channel.SendMessageAsync("Shady Dealer: \"Uh, hey buddy, I think you overpaid me." +
                                                                  " Since I'm such a nice guy, I'll give you your change back.\n");
                            user.GiveCoins(-user.GetDebt());
                            user.SetLoan(0);
                        }
                    }
                }
                else await Context.Channel.SendMessageAsync("**Invalid payment amount");
            }
        }
    }
}