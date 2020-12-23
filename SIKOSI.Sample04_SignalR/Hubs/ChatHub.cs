using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SIKOSI.Exchange.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using SIKOSI.Sample04_SignalR.Services;

namespace SIKOSI.Sample04_SignalR.Hubs
{
    public class ChatHub : Hub
    {
        //private UserService userService;

        public ChatHub(/*UserService userService*/)
        {
            //this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        [Authorize]
        public async Task SendSecretMessage(string user, string message)
        {
            var i = this.Context.User.Identity;

            await Clients.All.SendAsync("ReceiveMessage", user, "SECRET: " + message);
        }

        [Authorize]
        public async Task SendEncryptedBytes(byte[] encryptedBytes)
        {
            await Clients.Others.SendAsync("ReceiveEncryptedMessage", encryptedBytes);
        }

        [Authorize]
        public async Task SendEncryptedBytesToGroup(byte[] encryptedBytesForGroup)
        {
            await Clients.Others.SendAsync("ReceiveEncryptedGroupMessage", encryptedBytesForGroup);
        }

        [Authorize]
        public async Task SendEncryptedGroupKey(byte[] encryptedGroupKey)
        {
            await Clients.Others.SendAsync("ReceiveEncryptedGroupKey", encryptedGroupKey);
        }
    }
}