using Terraria.ModLoader;

namespace MarvelTerrariaUniverse.Content.IronMan;

public class IronManKeybinds : ModSystem
{
    public static ModKeybind ToggleFaceplate { get; private set; }
    public static ModKeybind ToggleFlight { get; private set; }

    public override void Load()
    {
        ToggleFaceplate = KeybindLoader.RegisterKeybind(Mod, "Iron Man: Toggle faceplate", "G");
        ToggleFlight = KeybindLoader.RegisterKeybind(Mod, "Iron Man: Toggle flight", "F");
    }

    public override void Unload()
    {
        ToggleFaceplate = null;
        ToggleFlight = null;
    }
}
