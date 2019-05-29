﻿using System;
using System.Collections.Generic;
using System.Linq;
using ChickenAPI.Packets.ClientPackets.Relations;
using ChickenAPI.Packets.Enumerations;
using ChickenAPI.Packets.ServerPackets.UI;
using NosCore.Core;
using NosCore.Core.I18N;
using NosCore.Core.Networking;
using NosCore.Data.Enumerations;
using NosCore.Data.Enumerations.I18N;
using NosCore.GameObject;
using NosCore.GameObject.Networking.ClientSession;
using Serilog;

namespace NosCore.PacketHandlers.Friend
{
    public class BlDelPacketHandler : PacketHandler<BlDelPacket>, IWorldPacketHandler
    {
        private readonly ILogger _logger;
        private readonly IWebApiAccess _webApiAccess;
     
        public BlDelPacketHandler(ILogger logger, IWebApiAccess webApiAccess)
        {
            _webApiAccess = webApiAccess;
            _logger = logger;
        }

        public override void Execute(BlDelPacket bldelPacket, ClientSession session)
        {
            var list = _webApiAccess.Get<List<CharacterRelation>>(WebApiRoute.Blacklist,
                session.Character.VisualId);
            var idtorem = list.FirstOrDefault(s => s.RelatedCharacterId == bldelPacket.CharacterId);
            if (idtorem != null)
            {
                _webApiAccess.Delete<Guid>(WebApiRoute.Blacklist, idtorem.CharacterRelationId);
            }
            else
            {
                session.SendPacket(new InfoPacket
                {
                    Message = Language.Instance.GetMessageFromKey(LanguageKey.NOT_IN_BLACKLIST,
                            session.Account.Language)
                });
            }
        }
    }
}
