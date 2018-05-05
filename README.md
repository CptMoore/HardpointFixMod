# VisualHardpointLimits
BattleTech mod (using BTML) that limits your loadout possibilities to what mechs have as a hardpoint. Only if a mech can visually have an AC20 as a hardpoint will you be able to add the AC20 to the loadout.

** Warning: Uses the experimental BTML mod loader that might change, come here again to check for updates **

Features:
- Restrict weapon loadout that are supported by the mech model, so you have nice visual feedback on the battlefield.
- Fix bad visual loadout selection issues, sometimes the wrong or ugly looking loadout was selected and shown.
- Added a quickdraw with a second missle slot on the left torso. Both missle slots don't allow for more than 15 missle tubes, looks very cool now.

Example for the visual limitation portion of the mod, take the Highlander assault mech:
The left torso has 2 missle hardpoint slots, however only one can mount an LRM20, the other is limited to LRM10. Without this mod you can mount an LRM20 also for the second slot, but it visually would only be showing up as LRM10. With this mod you can't mount the second LRM20 anymore, you have to take either an LRM10 or LRM5. Of course SRMs are still an option.
The left arm is also limited to an LRM15 and you can't mount an LRM20 at all.

There are also currently 3 configuration settings available:
* allowLRMsInSmallerSlotsForAll, boolean, default false, set this to true so all mechs can field an LRM20 even if missing the required hardpoints.
* allowLRMsInSmallerSlotsForMechs, string[], default ["atlas"], a list of mechs that can field larger LRM sizes even in smaller slots. Allows cheating the same as battletech lore does.
* allowLRMsInLargerSlotsForAll, boolean, default true, allow smaller sized LRMs to be used in larger sized hardpoint slots. E.g. an LRM10 should fit into an LRM20 slot.

## Install
After installing BTML, put into \BATTLETECH\Mods\ folder and launch the game.
