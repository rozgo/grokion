using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class PlayerPrefsActions : ScriptableObject {

    [UnityEditor.MenuItem ("PlayerPrefs/DeleteAll")]
    static void MenuGenesisPlayerPrefs() {
        Debug.Log("PlayerPrefs DeleteAll");
        PlayerPrefs.DeleteAll();
    }
    
    [UnityEditor.MenuItem ("PlayerPrefs/Reset")]
    static void MenuResetPlayerPrefs() {
        Debug.Log("PlayerPrefs reseted");
        PlayerPrefs.DeleteAll();
        PlayerPrefs.SetInt("Armor", 1);
        PlayerPrefs.SetInt("Suit", (int)Character.Suit.Armor);
        PlayerPrefs.SetInt("Projectile", 1);
        PlayerPrefs.SetInt("Weapon", (int)Character.Weapon.Projectile);
        PlayerPrefs.SetInt("Spirit", 1);
        if (Game.character != null) {
            Game.character.SetSuit(Character.Suit.Armor);
            Game.character.SetWeapon(Character.Weapon.Projectile);
        }
        if (Application.isPlaying && Game.spirit == null) {
            Instantiate(Resources.Load("Spirit"));
        }
    }

    [UnityEditor.MenuItem ("PlayerPrefs/Casual")]
    static void MenuCasualPlayerPrefs() {
        if (PlayerPrefs.HasKey("Casual")) {
        	PlayerPrefs.DeleteKey("Casual");
        	Debug.Log("Hardcore mode");
        	Game.casual = false;
        }
        else {
        	PlayerPrefs.SetInt("Casual", 1);
        	Debug.Log("Casual mode");
        	Game.casual = true;
        }
    }

    [UnityEditor.MenuItem ("PlayerPrefs/ClearObjectives")]
    static void MenuPlayerPrefsClearObjectives () {
        for (int i=1; i<=10; ++i) {
            string objectiveID = "Objective|" + i.ToString();
            PlayerPrefs.DeleteKey(objectiveID);
        }
        Debug.Log("Objectives cleared");
    }

    [UnityEditor.MenuItem ("PlayerPrefs/EnergyTankCount")]
    static void MenuEnergyTankCountPlayerPrefs() {
        if (PlayerPrefs.GetInt("EnergyTankCount") != 20) {
            PlayerPrefs.SetInt("EnergyTankCount", 20);
            Debug.Log("EnergyTankCount = 20");
            if (Game.character != null) {
                Game.character.energyTankCount = 20;
                Game.character.AddHP(3000);
            }
        }
        else {
            PlayerPrefs.SetInt("EnergyTankCount", 0);
            Debug.Log("EnergyTankCount = 0");
            if (Game.character != null) {
                Game.character.energyTankCount = 0;
                Game.character.AddHP(3000);
            }
        }
    }
    
    [UnityEditor.MenuItem ("PlayerPrefs/AddEnergyTank")]
    static void MenuAddEnergyTankPlayerPrefs() {
        if (Game.character != null) {
            Debug.Log("Added EnergyTank");
            Game.character.AddEnergyTank();
        }
    }
    
    [UnityEditor.MenuItem ("PlayerPrefs/RemoveEnergyTank")]
    static void MenuRemoveEnergyTankPlayerPrefs() {
        if (Game.character != null) {
            Debug.Log("Removed EnergyTank");
            Game.character.RemoveEnergyTank();
        }
    }
    
    [UnityEditor.MenuItem ("PlayerPrefs/HP")]
    static void MenuHPPlayerPrefs() {
        if (Game.character != null) {
            if (Game.character.IsHPFull()) {
                Debug.Log("HP depleted");
                Game.character.AddHP(-Game.character.HP);
            }
            else {
                Debug.Log("HP restored");
                Game.character.AddHP(3000);
            }
        }
    }
    
    [UnityEditor.MenuItem ("PlayerPrefs/Spirit")]
    static void MenuSpiritPlayerPrefs() {
        if (PlayerPrefs.HasKey("Spirit")) {
            Debug.Log("Spirit off");
            PlayerPrefs.DeleteKey("Spirit");
            if (Game.spirit != null) {
                Destroy(Game.spirit.gameObject);
            }
        }
        else {
            Debug.Log("Spirit on");
            PlayerPrefs.SetInt("Spirit", 1);
            if (Application.isPlaying && Game.spirit == null) {
                Instantiate(Resources.Load("Spirit"));
            }
        }
    }
    
    [UnityEditor.MenuItem ("PlayerPrefs/Suicide")]
    static void MenuSuicidePlayerPrefs() {
        if (Game.character != null) {
            Debug.Log("Commited suicide");
            Game.character.AddHP(-3000);
        }
    }
    
    [UnityEditor.MenuItem ("PlayerPrefs/Map")]
    static void MenuMapPlayerPrefs() {
        if (PlayerPrefs.HasKey("Map")) {
            Debug.Log("Map off");
            PlayerPrefs.DeleteKey("Map");
        }
        else {
            Debug.Log("Map on");
            PlayerPrefs.SetInt("Map", 1);
        }
    }
    
    [UnityEditor.MenuItem ("PlayerPrefs/GravityBoots")]
    static void MenuGravityBootsPlayerPrefs() {
        if (PlayerPrefs.HasKey("GravityBoots")) {
            Debug.Log("GravityBoots off");
            PlayerPrefs.DeleteKey("GravityBoots");
            if (Game.character != null) {
                Game.character.SetBoots(Character.Boots.Normal);
            }
        }
        else {
            Debug.Log("GravityBoots on");
            PlayerPrefs.SetInt("GravityBoots", 1);
            PlayerPrefs.SetInt("Boots", (int)Character.Boots.Gravity);
            if (Game.character != null) {
                Game.character.SetBoots(Character.Boots.Gravity);
            }
        }
    }
    
    [UnityEditor.MenuItem ("PlayerPrefs/Armor")]
    static void MenuArmorPlayerPrefs() {
        if (PlayerPrefs.HasKey("Armor")) {
            Debug.Log("Armor off");
            PlayerPrefs.DeleteKey("Armor");
            if (Game.character != null) {
                Game.character.SetSuit(Character.Suit.None);
            }
        }
        else {
            Debug.Log("Armor on");
            PlayerPrefs.SetInt("Armor", 1);
            PlayerPrefs.SetInt("Suit", (int)Character.Suit.Armor);
            if (Game.character != null) {
                Game.character.SetSuit(Character.Suit.Armor);
            }
        }
    }
    
    [UnityEditor.MenuItem ("PlayerPrefs/Swimsuit")]
    static void MenuSwimsuitPlayerPrefs() {
        if (PlayerPrefs.HasKey("Swimsuit")) {
            Debug.Log("Swimsuit off");
            PlayerPrefs.DeleteKey("Swimsuit");
            if (Game.character != null) {
                Game.character.SetSuit(Character.Suit.None);
            }
        }
        else {
            Debug.Log("Swimsuit on");
            PlayerPrefs.SetInt("Swimsuit", 1);
            PlayerPrefs.SetInt("Suit", (int)Character.Suit.Swimsuit);
            if (Game.character != null) {
                Game.character.SetSuit(Character.Suit.Swimsuit);
            }
        }
    }

    [UnityEditor.MenuItem ("PlayerPrefs/GridSuit")]
    static void MenuGridSuitPlayerPrefs() {
        if (PlayerPrefs.HasKey("GridSuit")) {
            Debug.Log("GridSuit off");
            PlayerPrefs.DeleteKey("GridSuit");
            if (Game.character != null) {
                Game.character.SetSuit(Character.Suit.GridSuit);
            }
        }
        else {
            Debug.Log("GridSuit on");
            PlayerPrefs.SetInt("GridSuit", 1);
            PlayerPrefs.SetInt("Suit", (int)Character.Suit.GridSuit);
            if (Game.character != null) {
                Game.character.SetSuit(Character.Suit.GridSuit);
            }
        }
    }

    [UnityEditor.MenuItem ("PlayerPrefs/Charge")]
    static void MenuChargePlayerPrefs() {
        if (PlayerPrefs.HasKey("Charge")) {
            Debug.Log("Charge off");
            PlayerPrefs.DeleteKey("Charge");
            if (Game.character != null) {
                Game.character.CanCharge = false;
            }
        }
        else {
            Debug.Log("Charge on");
            PlayerPrefs.SetInt("Charge", 1);
            if (Game.character != null) {
                Game.character.CanCharge = true;
            }
        }
    }
    
    [UnityEditor.MenuItem ("PlayerPrefs/Projectile")]
    static void MenuProjectilePlayerPrefs() {
        if (PlayerPrefs.HasKey("Projectile")) {
            Debug.Log("Projectile off");
            PlayerPrefs.DeleteKey("Projectile");
            PlayerPrefs.SetInt("Weapon", (int)Character.Weapon.None);
            if (Game.character != null) {
                Game.character.SetWeapon(Character.Weapon.None);
            }
        }
        else {
            Debug.Log("Projectile on");
            PlayerPrefs.SetInt("Projectile", 1);
            PlayerPrefs.SetInt("Weapon", (int)Character.Weapon.Projectile);
            if (Game.character != null) {
                Game.character.SetWeapon(Character.Weapon.Projectile);
            }
        }
        if (Game.hud != null) {
            Game.hud.UpdateWeapons();
        }
    }

    [UnityEditor.MenuItem ("PlayerPrefs/Missile")]
    static void MenuMissilePlayerPrefs() {
        if (PlayerPrefs.HasKey("Missile")) {
            Debug.Log("Missile off");
            PlayerPrefs.DeleteKey("Missile");
            PlayerPrefs.SetInt("Weapon", (int)Character.Weapon.Projectile);
            if (Game.character != null) {
                Game.character.SetWeapon(Character.Weapon.Projectile);
            }
        }
        else {
            Debug.Log("Missile on");
            PlayerPrefs.SetInt("Missile", 1);
            PlayerPrefs.SetInt("Weapon", (int)Character.Weapon.Missile);
            if (Game.character != null) {
                Game.character.SetWeapon(Character.Weapon.Missile);
            }
        }
        if (Game.hud != null) {
            Game.hud.UpdateWeapons();
        }
    }

    [UnityEditor.MenuItem ("PlayerPrefs/FlameThrower")]
    static void MenuFlameThrowerPlayerPrefs() {
        if (PlayerPrefs.HasKey("FlameThrower")) {
            Debug.Log("FlameThrower off");
            PlayerPrefs.DeleteKey("FlameThrower");
            PlayerPrefs.SetInt("Weapon", (int)Character.Weapon.Projectile);
            if (Game.character != null) {
                Game.character.SetWeapon(Character.Weapon.Projectile);
            }
        }
        else {
            Debug.Log("FlameThrower on");
            PlayerPrefs.SetInt("FlameThrower", 1);
            PlayerPrefs.SetInt("Weapon", (int)Character.Weapon.FlameThrower);
            if (Game.character != null) {
                Game.character.SetWeapon(Character.Weapon.FlameThrower);
            }
        }
        if (Game.hud != null) {
            Game.hud.UpdateWeapons();
        }
    }
    
    [UnityEditor.MenuItem ("PlayerPrefs/Grenade")]
    static void MenuGrenadePlayerPrefs() {
        if (PlayerPrefs.HasKey("Grenade")) {
            Debug.Log("Grenade off");
            PlayerPrefs.DeleteKey("Grenade");
            PlayerPrefs.SetInt("Weapon", (int)Character.Weapon.Projectile);
            if (Game.character != null) {
                Game.character.SetWeapon(Character.Weapon.Projectile);
            }
        }
        else {
            Debug.Log("Grenade on");
            PlayerPrefs.SetInt("Grenade", 1);
            PlayerPrefs.SetInt("Weapon", (int)Character.Weapon.Grenade);
            if (Game.character != null) {
                Game.character.SetWeapon(Character.Weapon.Grenade);
            }
        }
        if (Game.hud != null) {
            Game.hud.UpdateWeapons();
        }
    }

    [UnityEditor.MenuItem ("PlayerPrefs/Grapple")]
    static void MenuGrapplePlayerPrefs() {
        if (PlayerPrefs.HasKey("Grapple")) {
            Debug.Log("Grapple off");
            PlayerPrefs.DeleteKey("Grapple");
            PlayerPrefs.SetInt("Weapon", (int)Character.Weapon.Projectile);
            if (Game.character != null) {
                Game.character.SetWeapon(Character.Weapon.Projectile);
            }
        }
        else {
            Debug.Log("Grapple on");
            PlayerPrefs.SetInt("Grapple", 1);
            PlayerPrefs.SetInt("Weapon", (int)Character.Weapon.Grapple);
            if (Game.character != null) {
                Game.character.SetWeapon(Character.Weapon.Grapple);
            }
        }
        if (Game.hud != null) {
            Game.hud.UpdateWeapons();
        }
    }

    [UnityEditor.MenuItem ("PlayerPrefs/Sound")]
    static void MenuSoundPlayerPrefs() {
        if (PlayerPrefs.GetFloat("SoundVolume", 1.0f) > 0) {
            Debug.Log("Sound off");
            PlayerPrefs.SetFloat("SoundVolume", 0);
            if (Game.fx != null) {
                Game.fx.SetSoundVolume(0);
            }
        }
        else {
            Debug.Log("Sound on");
            PlayerPrefs.SetFloat("SoundVolume", 1);
            if (Game.fx != null) {
                Game.fx.SetSoundVolume(1);
            }
        }
    }

    [UnityEditor.MenuItem ("PlayerPrefs/Music")]
    static void MenuMusicPlayerPrefs() {
        if (PlayerPrefs.GetFloat("MusicVolume", 1.0f) > 0) {
            Debug.Log("Music off");
            PlayerPrefs.SetFloat("MusicVolume", 0);
            if (Game.fx != null) {
                Game.fx.SetMusicVolume(0);
            }
        }
        else {
            Debug.Log("Music on");
            PlayerPrefs.SetFloat("MusicVolume", 1);
            if (Game.fx != null) {
                Game.fx.SetMusicVolume(1);
            }
        }
    }

}
