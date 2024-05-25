using Terraria.ModLoader;

namespace MarvelTerrariaUniverse.Content.IronMan.Armor;

[AutoloadEquip(EquipType.Head)]
public class Mk2Helmet() : IronManArmorHelmet<Mk2Chestplate, Mk2Leggings>(2);

[AutoloadEquip(EquipType.Body)]
public class Mk2Chestplate() : IronManArmorChestplate(1, 2);

[AutoloadEquip(EquipType.Legs)]
public class Mk2Leggings() : IronManArmorLeggings<Mk2Chestplate>(1);