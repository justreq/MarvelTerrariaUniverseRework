using Terraria.ModLoader;

namespace MarvelTerrariaUniverse.Content.IronMan.Armor;

[AutoloadEquip(EquipType.Head)]
public class Mk1Helmet() : IronManArmorHelmet<Mk1Chestplate, Mk1Leggings>(2);

[AutoloadEquip(EquipType.Body)]
public class Mk1Chestplate() : IronManArmorChestplate;

[AutoloadEquip(EquipType.Legs)]
public class Mk1Leggings() : IronManArmorLeggings<Mk1Chestplate>;