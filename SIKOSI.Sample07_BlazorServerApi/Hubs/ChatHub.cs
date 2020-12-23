// (C) 2020 FOTEC Forschungs- und Technologietransfer GmbH
// Research Subsidiary of FH Wiener Neustadt
// 
// Contact biss@fotec.at / www.fotec.at
// 
// Created 03.12.2020 13:23
// Developer      Roman Jahn
// Project         SIKOSI
// 
// Released under GPL-3.0 or any later version

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace SIKOSI.Sample07_EncryptedChat.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(ILogger<ChatHub> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        [Authorize]
        public async Task RequestEncryptedGroupKeyDistribution()
        {
            await Clients.Others.SendAsync("DistributeGroupKey");
        }
    }
}