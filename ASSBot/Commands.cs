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
        public async Task ConnectFour([Remainder]string param)
        {
            if (!Bot.cfGame.IsOngoing())
            {
                if (param == null || param == "") await Context.Channel.SendMessageAsync("Use `?cf [username]` to invite someone to a game of Connect Four!");
                else if (param == "join" && Bot.cfGame.Opponent().Id == Context.User.Id)
                {
                    await Context.Channel.SendMessageAsync($"Player {Bot.cfGame.Turn()} goes first!\nUse `?cf [num between 1-7]` to place a chip.");
                    Bot.lastCF = await Context.Channel.SendMessageAsync(Bot.cfGame.GenerateBoard());
                    Bot.cfGame.Start();
                }
            }
            else if ((Context.User.Id == Bot.cfGame.Challenger().Id && Bot.cfGame.Turn() == 1) || (Context.User.Id == Bot.cfGame.Opponent().Id && Bot.cfGame.Turn() == 2))
            {
                bool success = int.TryParse(param, out int choice);

                if (success && choice > 0 && choice <= 7)
                {
                    Bot.cfGame.Play(choice);
                    await Bot.lastCF.DeleteAsync();
                    Bot.lastCF = await Context.Channel.SendMessageAsync(Bot.cfGame.GenerateBoard());

                    if (Bot.cfGame.IsOngoing() == false) await Context.Channel.SendMessageAsync($"Game over! Player {Bot.cfGame.Turn()} wins!");
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

        [Command("givecoins")]
        public async Task GiveCoins(int amount, IUser user)
        {
            if (Context.User.Id == 108312797162541056)
            {
                Functions.GiveCoins(Functions.GetUser(user), amount);
                await Context.Channel.SendMessageAsync($":moneybag: | {user.Username} has been given {amount} coins.");
            }
            else await Context.Channel.SendMessageAsync("Only Brady can do this currently.");
        }
        
        [Command("slots"), Summary("Spin the slots and win cash!")]
        public async Task Slots(int bet)
        {
            var user = Functions.GetUser(Context.User);
            if (user.GetCoins() < bet) await Context.Channel.SendMessageAsync(":slot_machine: | You do not have that many coins!");
            else if (bet <= 0) await Context.Channel.SendMessageAsync(":slot_machine: | Your bet must be above 0.");
            else
            {
                user.GiveCoins(-bet);
                SlotMachine sm = new SlotMachine(Context.User,bet);
                sm.spinTimer = new Timer(1000);
                sm.Spin();
                var msg = await Context.Channel.SendMessageAsync(sm.Generate());
                sm.spinTimer.Elapsed += (sender, e) => Functions.SpinSlots(msg, sm);
                sm.spinTimer.Start();
            }
        }


    }
}