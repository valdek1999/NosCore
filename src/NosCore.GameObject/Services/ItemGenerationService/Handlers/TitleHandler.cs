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
using NosCore.Data.Enumerations.Items;
using NosCore.GameObject.Networking.ClientSession;
using NosCore.GameObject.Services.InventoryService;
using NosCore.Packets.ClientPackets.Inventory;
using NosCore.Packets.Enumerations;
using NosCore.Packets.ServerPackets.UI;
using System;
using System.Threading.Tasks;

namespace NosCore.GameObject.Services.ItemGenerationService.Handlers
{
    public class TitleHandler : IUseItemEventHandler
    {
        public bool Condition(Item.Item item) => item.ItemType == ItemType.Title;

        public Task ExecuteAsync(RequestData<Tuple<InventoryItemInstance, UseItemPacket>> requestData)
        {
            return requestData.ClientSession.SendPacketAsync(new QnaiPacket
            {
                YesPacket = new GuriPacket
                {
                    Type = GuriPacketType.Title,
                    Argument = (uint)requestData.Data.Item1.ItemInstance!.ItemVNum,
                    EntityId = requestData.Data.Item1.Slot
                },
                Question = Game18NConstString.AskAddTitle
            });
        }
    }
}