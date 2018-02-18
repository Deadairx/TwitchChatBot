using System;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;
using TwitchLib.Models.API.v5.Users;

namespace TwitchChatBot
{
    class Program
    {
        static void Main(string[] args)
        {
            ChatBot bot = new ChatBot();
            bot.Connect();

            Console.ReadLine();

            bot.Disconnect();
        }
    }
}
