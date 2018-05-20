# HardpointFixMod
BattleTech mod (using ModTek and DynModLib) that makes sure your biggest weapons are always visible on a mech.

## Requirements
** Warning: Uses the experimental BTML mod loader and ModTek **

either
* install BattleTechModTools using [BattleTechModInstaller](https://github.com/CptMoore/BattleTechModTools/releases)

or
* install [BattleTechModLoader](https://github.com/Mpstark/BattleTechModLoader/releases) using [instructions here](https://github.com/Mpstark/BattleTechModLoader)
* install [ModTek](https://github.com/Mpstark/ModTek/releases) using [instructions here](https://github.com/Mpstark/ModTek)
* install [DynModLib](https://github.com/CptMoore/DynModLib/releases) using [instructions here](https://github.com/CptMoore/DynModLib)

## Features

- Fix bad visual loadout issues, sometimes the wrong or ugly looking loadout was shown, a new algorithm should improve upon this.
  Sometimes attaching a PPC after having already attached many small lasers would hide the PPC, this should be fixed now.
- Restrict weapon loadouts to what is supported by the mech model.
  BattleTech has some of the best looking models due to MWO, however we never know what mechs support what visible hardpoints and therefore we might be mounting weapons on invisible hardpoints.
  Has to be activated through configuration, open Settings.json.txt and change `allowLRMsInSmallerSlotsForAll` to `false`.


An example of how the weapon loadout restrictions feature work for the Highlander assault mech:
The left torso has 2 missle hardpoint slots, however only one can mount an LRM20, the other is limited to LRM10. Without this mod you can mount an LRM20 also for the second slot, but it visually would only be showing up as LRM10. With this mod you can't mount the second LRM20 anymore, you have to take either an LRM10 or LRM5. Of course SRMs are still an option.
The left arm is also limited to an LRM15 and you can't mount an LRM20 at all.

There are currently the following configuration settings available:

Setting | Type | Default | Description
--- | --- | --- | ---
enforceHardpointLimits | bool | true | set this to false to deactivate the hardpoint limits in mechlab
allowDefaultLoadoutWeapons | bool | true | always allow to reattach weapons the mech comes with by default
allowLRMsInSmallerSlotsForAll | bool | true | set this to false so only mechs with a proper sized hardpoint can field an LRM20.
allowLRMsInSmallerSlotsForMechs | string[] | default ["atlas"] | a list of mechs that can field larger LRM sizes even in smaller slots, even if allowLRMsInSmallerSlotsForAll is false.
allowLRMsInLargerSlotsForAll | bool | true | allow smaller sized LRMs to be used in larger sized hardpoint slots. E.g. an LRM10 should fit into an LRM20 slot.

## Limitations

- can't replace weapons by dragging another weapon ontop of it, you have to remove the weapon first and then add another one (you dont have to leave the mechlab for this to work)

## Download

Downloads can be found on [github](https://github.com/CptMoore/HardpointFixMod/releases).

## Install

After installing BTML and DynTechMod, put the mod into the \BATTLETECH\Mods\ folder and launch the game.

## Development

* Use git
* Use Visual Studio or DynTechMod to compile the project
