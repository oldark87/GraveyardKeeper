### Installation:

File and details available at 

https://www.nexusmods.com/graveyardkeeper/mods/1
___

### Note to developers

To support your mod for the QMods system, you need to learn how `mod.json` is implemented. The critical keys are:  

```
{
  "Id":"energyMod",
  "DisplayName":"Graveyard Keeper Infinite Energy",
  "Author":"Oldark",
  "Version":"1.0.0",
  "Requires":[],
  "Enable":true,
  "AssemblyName":"InfiniteEnergy.dll",
  "EntryMethod":"InfiniteEnergy.MainPatcher.Patch",
  "Config":{}
}
```

`AssemblyName` must be the case sensitive name of the dll file containing your patching method

`EntryMethod` is the entry method for your patch

```cs
using Harmony;

namespace YOURNAMESPACE
{
    class QPatch()
    {
        public static void Patch()
        {
            // Harmony.PatchAll() or equivalent
        }
    }
}
```
