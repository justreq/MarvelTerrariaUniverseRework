using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace MarvelTerrariaUniverse.Content.IronMan;

#if DEBUG

class EquipCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat;
    public override string Command => "im";
    public override string Usage => "/im <mark>";

    public override void Action(CommandCaller caller, string input, string[] args)
    {

        var player = caller.Player.GetModPlayer<IronManPlayer>();

        /*if (args.Length == 0) ironManPlayer.UnequipSuit();
        else
        {
            if (!int.TryParse(args[0], out int _))
            {
                Main.NewText("Mark required as integer");
                return;
            }

            ironManPlayer.EquipSuit(int.Parse(args[0]));
        }*/
    }
}

#endif

public class IronManPlayer : ModPlayer
{
    public readonly int FrameDelay = 5;

    public bool IsIronMan => Mark != 0;

    public int Mark = 0;
    public readonly int[] ArmorFrame = [0, 0, 0];

    public bool FaceplateOn = true;
    public bool FaceplateMoving = false;
    public int FaceplateFrameDelay = 0;

    public bool Flying = false;
    public bool Hovering => Flying && InputVector == Vector2.Zero;

    // 1 when the player is moving to a specific direction, 0 otherwise
    int dirleft, dirright, dirup, dirdown;
    Point mousePosChunk; // divide the screen into 4x4 tiles grid, and send the location of the mouse 
    const int mouseChunkSize = 64;

    public Vector2 InputVector => new Vector2(dirright - dirleft, dirdown - dirup); //Player.whoAmI != Main.myPlayer ? Vector2.Zero : new Vector2((PlayerInput.Triggers.Current.Right ? 1 : 0) - (PlayerInput.Triggers.Current.Left ? 1 : 0), (PlayerInput.Triggers.Current.Down ? 1 : 0) - (PlayerInput.Triggers.Current.Up ? 1 : 0)).SafeNormalize(Vector2.Zero);

    public override void ResetEffects()
    {
        Mark = 0;
    }

    public override void FrameEffects()
    {
        if (!IsIronMan) return;

        if (ArmorFrame[0] > 0) Player.head = EquipLoader.GetEquipSlot(Mod, $"IronManMk{Mark}Helmet_HeadAlt{ArmorFrame[0]}", EquipType.Head);
        if (ArmorFrame[1] > 0) Player.body = EquipLoader.GetEquipSlot(Mod, $"IronManMk{Mark}Chestplate_BodyAlt{ArmorFrame[1]}", EquipType.Body);
        if (ArmorFrame[2] > 0) Player.legs = EquipLoader.GetEquipSlot(Mod, $"IronManMk{Mark}Leggings_LegsAlt{ArmorFrame[2]}", EquipType.Legs);
    }

    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (!IsIronMan) return;

        if (IronManKeybinds.ToggleFaceplate.JustPressed) FaceplateMoving = true;
        if (IronManKeybinds.ToggleFlight.JustPressed) Flying = !Flying;
    }

    public override void PreUpdateMovement()
    {
        if (!IsIronMan) return;

        if (Flying)
        {
            if (Main.myPlayer == Player.whoAmI)
            {
                dirleft = PlayerInput.Triggers.Current.Left ? 1 : 0;
                dirright = PlayerInput.Triggers.Current.Right ? 1 : 0;
                dirup = PlayerInput.Triggers.Current.Up ? 1 : 0;
                dirdown = PlayerInput.Triggers.Current.Down ? 1 : 0;
            }
            Player.velocity = Vector2.Lerp(Player.velocity, InputVector * 10f, 0.1f);
        }
    }

    public override void PreUpdate()
    {
        if (!IsIronMan) return;

        if (Flying) Player.gravity = 0f;
    }

    public override void PostUpdate()
    {
        if (!IsIronMan) return;

        if (FaceplateMoving) UpdateFaceplateFrame();

        if (Flying)
        {
            Vector2 mouseOffset;
            Vector2 off = new Vector2(mouseChunkSize / 2, -mouseChunkSize / 2);
            if (Player.whoAmI == Main.myPlayer)
            {
                mouseOffset = Main.MouseWorld - Player.Center;
                mousePosChunk = ((mouseOffset - off) / mouseChunkSize).ToPoint();
            }
            else
            {
                mouseOffset = mousePosChunk.ToVector2() * mouseChunkSize + off;
            }

            Player.fullRotationOrigin = Player.Hitbox.Size() / 2;
            Player.bodyFrame.Y = 0;
            Player.legFrame.Y = 0;

            ArmorFrame[1] = 1;
            ArmorFrame[2] = Hovering ? 1 : 0;

            Player.direction = Hovering ? Math.Sign(mouseOffset.X) : Math.Sign(mouseOffset.RotatedBy(-Player.fullRotation).X);
            Player.fullRotation = Utils.AngleLerp(Player.fullRotation, Hovering ? (mouseOffset * Player.direction).ToRotation() * 0.55f : Player.velocity.ToRotation() + MathHelper.PiOver2, 0.05f);
        }
        else
        {
            ArmorFrame[1] = ArmorFrame[2] = 0;
            Player.fullRotation = Utils.AngleLerp(Player.fullRotation, 0f, 0.25f);
        }
    }

    private void UpdateFaceplateFrame()
    {
        FaceplateFrameDelay++;

        if (FaceplateFrameDelay >= FrameDelay)
        {
            FaceplateFrameDelay = 0;
            ArmorFrame[0] += FaceplateOn ? 1 : -1;

            if (ArmorFrame[0] is <= 0 or >= 2)
            {
                FaceplateMoving = false;
                FaceplateOn = ArmorFrame[0] <= 0;
            }
        }
    }

    public override void SyncPlayer(int toWho, int fromWho, bool newPlayer)
    {
        ModPacket packet = Mod.GetPacket();
        packet.Write((byte)MarvelTerrariaUniverse.MessageType.IronManPlayerSync);
        packet.Write((byte)Player.whoAmI);
        packet.Write((byte)Mark);
        packet.Write((byte)ArmorFrame[0]);
        packet.Write((byte)ArmorFrame[1]);
        packet.Write((byte)ArmorFrame[2]);
        packet.Write((byte)FaceplateFrameDelay);
        BitsByte flags1 = 0;
        flags1[0] = FaceplateOn;
        flags1[1] = FaceplateMoving;
        flags1[2] = Flying;
        flags1[3] = dirleft == 1;
        flags1[4] = dirright == 1;
        flags1[5] = dirup == 1;
        flags1[6] = dirdown == 1;
        packet.Write((byte)flags1);
        packet.Write7BitEncodedInt(mousePosChunk.X);
        packet.Write7BitEncodedInt(mousePosChunk.Y);
        packet.Send(toWho, fromWho);
    }

    public void ReceivePlayerSync(BinaryReader reader)
    {
        Mark = reader.ReadByte();
        ArmorFrame[0] = reader.ReadByte();
        ArmorFrame[1] = reader.ReadByte();
        ArmorFrame[2] = reader.ReadByte();
        FaceplateFrameDelay = reader.ReadByte();
        BitsByte flags1 = reader.ReadByte();
        FaceplateOn = flags1[0];
        FaceplateMoving = flags1[1];
        Flying = flags1[2];
        dirleft = flags1[3] ? 1 : 0;
        dirright = flags1[4] ? 1 : 0;
        dirup = flags1[5] ? 1 : 0;
        dirdown = flags1[6] ? 1 : 0;
        mousePosChunk = new Point(reader.Read7BitEncodedInt(), reader.Read7BitEncodedInt());
    }

    public override void CopyClientState(ModPlayer targetCopy)
    {
        IronManPlayer clone = (IronManPlayer)targetCopy;
        clone.Mark = Mark;
        clone.ArmorFrame[0] = ArmorFrame[0];
        clone.ArmorFrame[1] = ArmorFrame[1];
        clone.ArmorFrame[2] = ArmorFrame[2];
        clone.FaceplateOn = FaceplateOn;
        clone.FaceplateMoving = FaceplateMoving;
        clone.FaceplateFrameDelay = FaceplateFrameDelay;
        clone.Flying = Flying;
        clone.dirleft = dirleft;
        clone.dirright = dirright;
        clone.dirup = dirup;
        clone.dirdown = dirdown;
        clone.mousePosChunk = mousePosChunk;
    }

    public override void SendClientChanges(ModPlayer clientPlayer)
    {
        IronManPlayer clone = (IronManPlayer)clientPlayer;

        if (clone.Mark != Mark ||
        clone.ArmorFrame[0] != ArmorFrame[0] ||
        clone.ArmorFrame[1] != ArmorFrame[1] ||
        clone.ArmorFrame[2] != ArmorFrame[2] ||
        clone.FaceplateOn != FaceplateOn ||
        clone.FaceplateMoving != FaceplateMoving ||
        clone.FaceplateFrameDelay != FaceplateFrameDelay ||

        clone.dirleft != dirleft ||
        clone.dirright != dirright ||
        clone.dirup != dirup ||
        clone.dirdown != dirdown ||
        clone.mousePosChunk != mousePosChunk ||

        clone.Flying != Flying) SyncPlayer(toWho: -1, fromWho: Main.myPlayer, newPlayer: false);
    }
}
