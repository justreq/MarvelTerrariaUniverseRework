using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using System.Collections.Generic;

namespace MarvelTerrariaUniverse.Content.IronMan.Armor;

public abstract class IronManArmorHelmet<Chestplate, Leggings>(int altFrameCount = 0) : ModItem
    where Chestplate : IronManArmorChestplate
    where Leggings : IronManArmorLeggings<Chestplate>
{
    public int AltFrameCount = altFrameCount;
    public int Mark => ModContent.GetInstance<Chestplate>().Mark;

    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server || AltFrameCount == 0) return;

        for (int i = 1; i <= AltFrameCount; i++)
        {
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Head}Alt{i}", EquipType.Head, name: $"IronManMk{Mark}Helmet_{EquipType.Head}Alt{i}");
        }
    }

    public override void SetStaticDefaults()
    {
        if (Main.netMode == NetmodeID.Server || AltFrameCount == 0) return;

        for (int i = 1; i <= AltFrameCount; i++)
        {
            ArmorIDs.Head.Sets.DrawHead[EquipLoader.GetEquipSlot(Mod, $"IronManMk{Mark}Helmet_{EquipType.Head}Alt{i}", EquipType.Head)] = true;
        }
    }

    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ModContent.ItemType<Chestplate>() && legs.type == ModContent.ItemType<Leggings>();
    }

    public override void UpdateArmorSet(Player player)
    {
        player.GetModPlayer<IronManPlayer>().Mark = Mark;
        player.setBonus = $"Mark {Mark}";
    }
}

public abstract class IronManArmorChestplate(int altFrameCount = 0, int mark = 1) : ModItem
{
    public int AltFrameCount = altFrameCount;
    public int Mark = mark;

    public List<int> Arsenal => [];

    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server || AltFrameCount == 0) return;

        for (int i = 1; i <= AltFrameCount; i++)
        {
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Body}Alt{i}", EquipType.Body, name: $"IronManMk{Mark}Chestplate_{EquipType.Body}Alt{i}");
        }
    }

    public override void SetStaticDefaults()
    {
        if (Main.netMode == NetmodeID.Server || AltFrameCount == 0) return;

        for (int i = 1; i <= AltFrameCount; i++)
        {
            int slot = EquipLoader.GetEquipSlot(Mod, $"IronManMk{Mark}Chestplate_{EquipType.Body}Alt{i}", EquipType.Body);
            ArmorIDs.Body.Sets.HidesTopSkin[slot] = true;
            ArmorIDs.Body.Sets.HidesArms[slot] = true;
        }
    }
}

public abstract class IronManArmorLeggings<Chestplate>(int altFrameCount = 0) : ModItem
    where Chestplate : IronManArmorChestplate
{
    public int AltFrameCount = altFrameCount;
    public int Mark => ModContent.GetInstance<Chestplate>().Mark;

    public override void Load()
    {
        if (Main.netMode == NetmodeID.Server || AltFrameCount == 0) return;

        for (int i = 1; i <= AltFrameCount; i++)
        {
            EquipLoader.AddEquipTexture(Mod, $"{Texture}_{EquipType.Legs}Alt{i}", EquipType.Legs, name: $"IronManMk{Mark}Leggings_{EquipType.Legs}Alt{i}");
        }
    }

    public override void SetStaticDefaults()
    {
        if (Main.netMode == NetmodeID.Server || AltFrameCount == 0) return;

        for (int i = 1; i <= AltFrameCount; i++)
        {
            ArmorIDs.Legs.Sets.HidesBottomSkin[EquipLoader.GetEquipSlot(Mod, $"IronManMk{Mark}Leggings_{EquipType.Legs}Alt{i}", EquipType.Legs)] = true;
        }
    }
}