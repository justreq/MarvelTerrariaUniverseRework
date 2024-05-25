using System.IO;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using MarvelTerrariaUniverse.Content.IronMan;

namespace MarvelTerrariaUniverse;

public class MarvelTerrariaUniverse : Mod
{
    internal enum MessageType : byte
    {
        IronManPlayerSync,
    }

    public override void HandlePacket(BinaryReader reader, int whoAmI)
    {
        MessageType messageType = (MessageType)reader.ReadByte();

        switch (messageType)
        {
            case MessageType.IronManPlayerSync:
                byte playerNumber = reader.ReadByte();
                IronManPlayer examplePlayer = Main.player[playerNumber].GetModPlayer<IronManPlayer>();
                examplePlayer.ReceivePlayerSync(reader);

                if (Main.netMode == NetmodeID.Server) examplePlayer.SyncPlayer(-1, whoAmI, false);
                break;
            default:
                Logger.WarnFormat($"{nameof(MarvelTerrariaUniverse)}: Unknown Message type: {0}", messageType);
                break;
        }
    }
}