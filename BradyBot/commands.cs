using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace BradyBot
{
    public class Commands : ModuleBase
    {

        Random rdm = new Random();
        [Command("insult"), Alias(new string[] {"in"})]
        public async Task Test()
        {
            string[] words = {"stupid","fuck","mother","with","shit","bitch","from hell","ass","asshoel","motherfuck","bookshelf","your",
                "you","cunt","hell","in"};

            int wordCount = rdm.Next(3, words.Count());


            string[] pre = {"i","i will","im gonna",""};

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
    }
}
