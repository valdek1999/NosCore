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
using NosCore.Data.Enumerations.I18N;
using NosCore.GameObject;
using NosCore.GameObject.ComponentEntities.Extensions;
using NosCore.GameObject.ComponentEntities.Interfaces;
using NosCore.GameObject.Networking;
using NosCore.GameObject.Networking.ClientSession;
using NosCore.Packets.ClientPackets.Movement;
using NosCore.Shared.Enumerations;
using NosCore.Shared.I18N;
using Serilog;
using System.Linq;
using System.Threading.Tasks;

namespace NosCore.PacketHandlers.Movement
{
    public class SitPacketHandler : PacketHandler<SitPacket>, IWorldPacketHandler
    {
        private readonly ILogger _logger;
        private readonly ILogLanguageLocalizer<LogLanguageKey> _logLanguage;

        public SitPacketHandler(ILogger logger, ILogLanguageLocalizer<LogLanguageKey> logLanguage)
        {
            _logger = logger;
            _logLanguage = logLanguage;
        }

        public override Task ExecuteAsync(SitPacket sitpacket, ClientSession clientSession)
        {
            return Task.WhenAll(sitpacket.Users!.Select(u =>
            {
                IAliveEntity entity;

                switch (u!.VisualType)
                {
                    case VisualType.Player:
                        entity = Broadcaster.Instance.GetCharacter(s => s.VisualId == u.VisualId)!;
                        if (entity.VisualId != clientSession.Character.VisualId)
                        {
                            _logger.Error(
                                _logLanguage[LogLanguageKey.DIRECT_ACCESS_OBJECT_DETECTED],
                                clientSession.Character, sitpacket);
                            return Task.CompletedTask;
                        }

                        break;
                    default:
                        _logger.Error(_logLanguage[LogLanguageKey.VISUALTYPE_UNKNOWN],
                            u.VisualType);
                        return Task.CompletedTask;
                }

                return entity.RestAsync();
            }));
        }
    }
}