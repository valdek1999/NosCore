﻿//  __  _  __    __   ___ __  ___ ___  
// |  \| |/__\ /' _/ / _//__\| _ \ __| 
// | | ' | \/ |`._`.| \_| \/ | v / _|  
// |_|\__|\__/ |___/ \__/\__/|_|_\___| 
// 
// Copyright (C) 2018 - NosCore
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
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using NosCore.Core;
using NosCore.Core.Serializing;
using NosCore.Data.WebApi;
using NosCore.GameObject.Networking;
using NosCore.Shared.Enumerations.Account;
using NosCore.Shared.Enumerations.Interaction;

namespace NosCore.WorldServer.Controllers
{
    [Route("api/[controller]")]
    [AuthorizeRole(AuthorityType.GameMaster)]
    public class PacketController : Controller
    {
        // POST api/packet
        [HttpPost]
        public IActionResult PostPacket([FromBody] PostedPacket postedPacket)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var message = PacketFactory.Deserialize(postedPacket.Packet);

            switch (postedPacket.ReceiverType)
            {
                case ReceiverType.All:
                    ServerManager.Instance.Broadcast(message);
                    break;
                case ReceiverType.OnlySomeone:
                    ClientSession receiverSession;

                    if (postedPacket.ReceiverCharacter.Name != null)
                    {
                        receiverSession = ServerManager.Instance.Sessions.Values.FirstOrDefault(s =>
                            s.Character?.Name == postedPacket.ReceiverCharacter.Name);
                    }
                    else
                    {
                        receiverSession = ServerManager.Instance.Sessions.Values.FirstOrDefault(s => s.Character?.CharacterId == postedPacket.ReceiverCharacter.Id);
                    }

                    if (receiverSession == null)
                    {
                        return Ok();
                    }

                    receiverSession.SendPacket(message);
                    break;
            }

            return Ok();
        }
    }
}