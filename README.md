# DualWield
Dual wielding for Rimworld

## Install

Download the zip from one of the releases, unpack and place the unpacked folder in the Rimworld mods folder. 

## Contributing

Translations are always very welcome. Just create a pull request. For other changes you can always contact me. 

## Patching 

(only relevant if you created a weapon mod)
This mod is shipped with advanced settings that allow you to configure how weapons are drawn when dual wielded. The mod has reasonable defaults, but these defaults may not be perfect for your weapon-adding mod. I therefore added a patching hook so you can set better defaults for your weapon mod. **Do mind that if users already configured their settings, the defaults won't be used! (otherwise they wouldn't be defaults :))**.  DefModExtensions are used as patching hooks, you can read more about those here: https://rimworldwiki.com/wiki/Modding_Tutorials/DefModExtension. 


### Setting a custom rotation
The following example patch ensures the spear is drawn with an 45 degree extra angle when viewed in the frontal view, using DefModExtension_CustomRotation. 
```
<?xml version="1.0" encoding="utf-8" ?>
<Patch>
	<Operation Class ="PatchOperationAddModExtension">
		<xpath>/ThingDef[defName = "MeleeWeapon_Spear"]</xpath>
		<value>
			<li Class="DualWield.DefModExtension_CustomRotation">
				<extraRotation>45</extraRotation>
			</li>
		</value>
	</Operation>
</Patch>
```

### Setting defaults for weapons being secondary, or two-handed
In the following example DefModExtension_DefaultSettings is used to set all weapons to be dual wieldable. 
```
	<Operation Class ="PatchOperationAddModExtension">
		<xpath>/ThingDef[@Name = "BaseWeapon"]</xpath>
		<value>
			<li Class="DualWield.DefModExtension_DefaultSettings">
				<dualWield>True</dualWield>
				<twoHand>False</twoHand> 
				<!-- both options default to False, so setting twoHand to False here explicitly is just for illustrative purposes, and isn't necessary -->
			</li>
		</value>
	</Operation>
```
**Feel free to request more patching hooks like these.**
