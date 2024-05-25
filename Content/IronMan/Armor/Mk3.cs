using Terraria.ModLoader;

namespace MarvelTerrariaUniverse.Content.IronMan.Armor;

[AutoloadEquip(EquipType.Head)]
public class Mk3Helmet() : IronManArmorHelmet<Mk3Chestplate, Mk3Leggings>(2);

[AutoloadEquip(EquipType.Body)]
public class Mk3Chestplate() : IronManArmorChestplate(1, 3);

[AutoloadEquip(EquipType.Legs)]
public class Mk3Leggings() : IronManArmorLeggings<Mk3Chestplate>(1);