using System;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Models.Client;
using TwitchLib.Models.API.v5.Users;

namespace TwitchChatBot
{
	internal class ChatBot
	{
		readonly ConnectionCredentials credentials = new ConnectionCredentials(TwitchInfo.BotUsername, TwitchInfo.BotToken);
		TwitchClient client;
		TwitchAPI api;

		const int MaxMessagesPer30Secs = 20;

		internal void Connect()
		{
			Console.WriteLine("Connecting...");

			client = new TwitchClient(credentials, TwitchInfo.ChannelName, logging: true);
			api = new TwitchAPI();

			api.Settings.ClientId = TwitchInfo.ClientId;

			//client.ChatThrottler = new TwitchLib.Services.MessageThrottler(client, MaxMessagesPer30Secs, TimeSpan.FromSeconds(30));
			//client.WhisperThrottler = new TwitchLib.Services.MessageThrottler(client, MaxMessagesPer30Secs, TimeSpan.FromSeconds(30));

			client.OnLog += Client_OnLog;
			client.OnConnectionError += Client_OnConnectionError;
			client.OnMessageReceived += Client_OnMessageReceived;
			client.OnWhisperReceived += Client_OnWhisperReceived;
			client.OnUserTimedout += Client_OnUserTimedout;

			client.Connect();
		}

		private void Client_OnUserTimedout(object sender, OnUserTimedoutArgs e)
		{
			throw new NotImplementedException();
		}

		private void Client_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
		{
			client.SendWhisper(e.WhisperMessage.Username, $"I like to whisper too! You said: {e.WhisperMessage.Message}");
		}

		private void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
		{
			if (e.ChatMessage.Message.StartsWith("hi", StringComparison.InvariantCultureIgnoreCase))
			{
				client.SendMessage($"Hello {e.ChatMessage.DisplayName}");
			}
			else if (e.ChatMessage.Message.StartsWith("!uptime", StringComparison.InvariantCultureIgnoreCase))
			{
				client.SendMessage(GetUptime()?.ToString() ?? "Offline");
			}
		}

		private TimeSpan? GetUptime()
		{
			var userId = GetUserId(TwitchInfo.ChannelName);
			if (userId == null)
			{
				return null;
			}

			return api.Streams.v5.GetUptimeAsync(userId).Result;
		}

		private string GetUserId(string username)
		{
			User[] userList = api.Users.v5.GetUserByNameAsync(username).Result.Matches;
			if (userList == null || userList.Length == 0)
			{
				return null;
			}

			return userList[0].Id;
		}

		private void Client_OnLog(object sender, OnLogArgs e)
		{
			//Console.WriteLine(e.Data);
		}

		private void Client_OnConnectionError(object sender, OnConnectionErrorArgs e)
		{
			Console.WriteLine($"Error!! {e.Error}");
		}

		internal void Disconnect()
		{
			Console.WriteLine("Disconnecting...");
		}
	}
}