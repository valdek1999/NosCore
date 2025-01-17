﻿//  __  _  __    __   ___ __  ___ ___
// |  \| |/__\ /' _/ / _//__\| _ \ __|
// | | ' | \/ |`._`.| \_| \/ | v / _|
// |_|\__|\__/ |___/ \__/\__/|_|_\___|
// 
// Copyright (C) 2019 - NosCore
// 
// NosCore is a free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using NosCore.Core.I18N;
using NosCore.Data.CommandPackets;
using NosCore.Data.Enumerations.I18N;
using NosCore.Data.Enumerations.Interaction;
using NosCore.Data.WebApi;
using NosCore.GameObject;
using NosCore.GameObject.HttpClients.PacketHttpClient;
using NosCore.GameObject.Networking.ClientSession;
using NosCore.Packets.Enumerations;
using NosCore.Packets.Interfaces;
using NosCore.Packets.ServerPackets.Chats;
using NosCore.Packets.ServerPackets.UI;
using NosCore.Shared.Enumerations;
using System.Collections.Generic;
using System.Threading.Tasks;
using Character = NosCore.Data.WebApi.Character;
//TODO stop using obsolete
#pragma warning disable 618

namespace NosCore.PacketHandlers.Command
{
    public class ShoutPacketHandler : PacketHandler<ShoutPacket>, IWorldPacketHandler
    {
        private readonly IPacketHttpClient _packetHttpClient;
        private readonly ISerializer _packetSerializer;
        private readonly IGameLanguageLocalizer _gameLanguageLocalizer;

        public ShoutPacketHandler(ISerializer packetSerializer, IPacketHttpClient packetHttpClient, IGameLanguageLocalizer gameLanguageLocalizer)
        {
            _packetSerializer = packetSerializer;
            _packetHttpClient = packetHttpClient;
            _gameLanguageLocalizer = gameLanguageLocalizer;
        }

        public override async Task ExecuteAsync(ShoutPacket shoutPacket, ClientSession session)
        {
            var message = $"({_gameLanguageLocalizer[LanguageKey.ADMINISTRATOR, session.Account.Language]}) {shoutPacket.Message}";

            var sayPacket = new Sayi2Packet
            {
                VisualType = VisualType.Player,
                VisualId = -1,
                Type = SayColorType.Yellow,
                Message = Game18NConstString.Administrator,
                Game18NArguments = { 999, message }
            };

            var msgiPacket = new MsgPacket
            {
                Type = MessageType.Shout,
                Message = message
            };

            var sayPostedPacket = new PostedPacket
            {
                Packet = _packetSerializer.Serialize(new[] { sayPacket }),
                SenderCharacter = new Character
                {
                    Name = session.Character.Name,
                    Id = session.Character.CharacterId
                },
                ReceiverType = ReceiverType.All
            };

            var msgPostedPacket = new PostedPacket
            {
                Packet = _packetSerializer.Serialize(new[] { msgiPacket }),
                ReceiverType = ReceiverType.All
            };

            await _packetHttpClient.BroadcastPacketsAsync(new List<PostedPacket>(new[] { sayPostedPacket, msgPostedPacket })).ConfigureAwait(false);
        }
    }
}