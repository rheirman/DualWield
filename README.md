# DualWield
Dual wielding for Rimworld

## Install

Download the zip from one of the releases, unpack and place the unpacked folder in the Rimworld mods folder. 

## Contributing

Translations are always very welcome. Just create a pull request. For other changes you can always contact me. 

## Patching 

(only relevant if you created a weapon mod)
This mod is shipped with advanced settings that allow you to configure how weapons are drawn when dual wielded. The mod has reasonable defaults, but these defaults may not be perfect for your weapon-adding mod. I therefore added a patching hook so you can set better defaults for your weapon mod. 

The following example patch ensures the spear is drawn with an 45 degree extra angle when viewed in the frontal view. 
```
<?xml version="1.0" encoding="utf-8" ?>
<Patch>
	<Operation Class ="PatchOperationAddModExtension">
		<xpath>*/ThingDef[defName = "MeleeWeapon_Spear"]</xpath>
		<value>
			<li Class="DualWield.DefModExtension_CustomRotation">
				<extraRotation>45</extraRotation>
			</li>
		</value>
	</Operation>
</Patch>
```
**Feel free to request more patching hooks like these.**
