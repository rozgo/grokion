using UnityEngine;
using System.Collections;

public class ControlPanel : MonoBehaviour {
    
    public CtrlCover coverArmor;
    public CtrlCover coverSwimsuit;
    public CtrlCover coverBoots;
    public CtrlCover coverGravityBoots;
    public CtrlCover coverProjectile;
    public CtrlCover coverMissile;
    public CtrlCover coverFlameThrower;
    public CtrlCover coverGrenade;
    public CtrlCover coverGrapple;
    public GameObject compas;
    public GameObject objective;
    public GameObject trail;
    public GameObject map;
    public AudioClip selectClip;
    public AudioClip ctrlClip;
    public new AudioSource audio;
    public Button exitButton;
    public TextMesh[] objectiveLines;
    public TextMesh musicVolume;
    public TextMesh soundVolume;
    public Button soundPlus;
    public Button soundMinus;
    public Button musicPlus;
    public Button musicMinus;

    string letters = "ABCDEFGHIJKLMNOP";    
    
    static string lastLocation = "";

    void Start () {
        audio.volume = Game.fx.soundVolume;
        audio.PlayOneShot(ctrlClip);
        Setup();
    }
    
    IEnumerator WaitForRealSeconds(float time) {
        float startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup - startTime < time)
            yield return 1;
    }
    
    void OnButtonUp (Button button) {
    	
        if (button == exitButton) {
            Game.game.HideControlPanel();
        }
        else if (button == soundPlus) {
        	Game.fx.SetSoundVolume(Game.fx.soundVolume + 0.1f);
        	audio.PlayOneShot(ctrlClip);
        }
        else if (button == soundMinus) {
        	Game.fx.SetSoundVolume(Game.fx.soundVolume - 0.1f);
        	audio.PlayOneShot(ctrlClip);
        }
        else if (button == musicPlus) {
        	Game.fx.SetMusicVolume(Game.fx.musicVolume + 0.1f);
        	audio.PlayOneShot(ctrlClip);
        }
        else if (button == musicMinus) {
        	Game.fx.SetMusicVolume(Game.fx.musicVolume - 0.1f);
        	audio.PlayOneShot(ctrlClip);
        }
        
		soundVolume.text = Mathf.Round(Game.fx.soundVolume * 100).ToString();
		musicVolume.text = Mathf.Round(Game.fx.musicVolume * 100).ToString();
    }

    IEnumerator CompasBlink () {
        while(true) {
            compas.active = !compas.active;
            yield return StartCoroutine(WaitForRealSeconds(0.2f));
        }
    }

    IEnumerator ObjectiveBlink () {
        while(true) {
            objective.active = !objective.active;
            yield return StartCoroutine(WaitForRealSeconds(0.2f));
        }
    }
    
    void OnCoverClicked (CtrlCover cover) {
        if (Game.character == null) {
            return;
        }
        if (cover == coverArmor) {
            Game.character.SetSuit(Character.Suit.Armor);
        }
        else if (cover == coverSwimsuit) {
            Game.character.SetSuit(Character.Suit.Swimsuit);
        }
        else if (cover == coverBoots) {
            Game.character.SetBoots(Character.Boots.Normal);
        }
        else if (cover == coverGravityBoots) {
            Game.character.SetBoots(Character.Boots.Gravity);
        }
        else if (cover == coverProjectile) {
            Game.character.SetWeapon(Character.Weapon.Projectile);
        }
        else if (cover == coverMissile) {
            Game.character.SetWeapon(Character.Weapon.Missile);
        }
        else if (cover == coverFlameThrower) {
            Game.character.SetWeapon(Character.Weapon.FlameThrower);
        }
        else if (cover == coverGrenade) {
            Game.character.SetWeapon(Character.Weapon.Grenade);
        }
        else if (cover == coverGrapple) {
            Game.character.SetWeapon(Character.Weapon.Grapple);
        }
        audio.PlayOneShot(selectClip);
        Setup();
    }
    
    void Setup () {
    	soundVolume.text = ((int)(Game.fx.soundVolume * 100)).ToString();
    	musicVolume.text = ((int)(Game.fx.musicVolume * 100)).ToString();
        string objectiveID = "";
        string objectiveLocation = "";
        for (int i=1; i<=10; ++i) {
            objectiveID = "Objective|" + i.ToString();
            if (!PlayerPrefs.HasKey(objectiveID)) {
                break;
            }
        }
        for (int i=0; i<objectiveLines.Length; ++i) {
            objectiveLines[i].text = "";
        }
        string[] objectiveStrings = TextTable.GetLines(objectiveID);
        for (int i=0; i<objectiveStrings.Length; ++i) {
            if (objectiveStrings[i].StartsWith(">")) {
                objectiveLocation = objectiveStrings[i].Substring(1);
            }
            else if (i<objectiveLines.Length) {
                objectiveLines[i].text = objectiveStrings[i];
            }
        }
        
        trail.SetActiveRecursively(false);
        compas.SetActiveRecursively(false);
        objective.SetActiveRecursively(false);
        string location = Application.loadedLevelName;
        if (location.Length != 2) {
            location = lastLocation;
        }
        float dx = 0.0625f;
        float dy = 0.0625f;
        Vector3 ipos = compas.transform.position + new Vector3(0, -dy * 7, 0);
        for (int x = 0; x<letters.Length; ++x) {
            for (int y = 0; y<letters.Length; ++y) {
                string trailKey = "Trail|" + letters[x] + letters[y];
                int locX = 65 - letters[x];
                int locY = 72 - letters[y];
                if (location.Length == 2 && location[0] == letters[x] && location[1] == letters[y]) {
                    lastLocation = location;
                    StartCoroutine(CompasBlink());
                    compas.transform.position = ipos + new Vector3(dx * locX, dy * locY, 0);
                }
                else if (objectiveLocation.Length == 2 && 
                         objectiveLocation[0] == letters[x] && objectiveLocation[1] == letters[y]) {
                   	StartCoroutine(ObjectiveBlink());
                   	objective.transform.position = ipos + new Vector3(dx * locX, dy * locY, 0);
                }
                else if (PlayerPrefs.HasKey(trailKey)) {
                    GameObject newTrail = (GameObject)Instantiate(trail);
                    newTrail.transform.position = ipos + new Vector3(dx * locX, dy * locY, 0);
                    newTrail.transform.parent = transform;
                    newTrail.transform.localRotation = Quaternion.Euler(270, 180, 0);
                }
            }
        }
        if (PlayerPrefs.HasKey("Map")) {
            map.active = true;
        }
        else {
            map.active = false;
        }
        if ((Character.Suit)PlayerPrefs.GetInt("Suit", 0) == Character.Suit.None) {
            if (PlayerPrefs.HasKey("Armor")) {
                coverArmor.Unselect();
            }
            else {
                coverArmor.Off();
            }
            if (PlayerPrefs.HasKey("Swimsuit")) {
                coverSwimsuit.Unselect();
            }
            else {
                coverSwimsuit.Off();
            }
        }
        else if ((Character.Suit)PlayerPrefs.GetInt("Suit", 0) == Character.Suit.Armor) {
            coverArmor.Select();
            if (PlayerPrefs.HasKey("Swimsuit")) {
                coverSwimsuit.Unselect();
            }
            else {
                coverSwimsuit.Off();
            }
        }
        else if ((Character.Suit)PlayerPrefs.GetInt("Suit", 0) == Character.Suit.Swimsuit) {
            if (PlayerPrefs.HasKey("Armor")) {
                coverArmor.Unselect();
            }
            else {
                coverArmor.Off();
            }
            coverSwimsuit.Select();
        }
        if ((Character.Suit)PlayerPrefs.GetInt("Suit", 0) == Character.Suit.None) {
            coverBoots.Off();
            if (PlayerPrefs.HasKey("GravityBoots")) {
                coverGravityBoots.Unselect();
            }
            else {
                coverGravityBoots.Off();
            }
        }
        else if ((Character.Boots)PlayerPrefs.GetInt("Boots", 0) == Character.Boots.Normal) {
            coverBoots.Select();
            if (PlayerPrefs.HasKey("GravityBoots")) {
                coverGravityBoots.Unselect();
            }
            else {
                coverGravityBoots.Off();
            }
        }
        else if ((Character.Boots)PlayerPrefs.GetInt("Boots", 0) == Character.Boots.Gravity) {
            coverBoots.Unselect();
            coverGravityBoots.Select();
        }
        if ((Character.Weapon)PlayerPrefs.GetInt("Weapon", 0) == Character.Weapon.None) {
            if (PlayerPrefs.HasKey("Projectile")) {
                coverProjectile.Unselect();
            }
            else {
                coverProjectile.Off();
            }
            if (PlayerPrefs.HasKey("Missile")) {
                coverMissile.Unselect();
            }
            else {
                coverMissile.Off();
            }
            if (PlayerPrefs.HasKey("FlameThrower")) {
                coverFlameThrower.Unselect();
            }
            else {
                coverFlameThrower.Off();
            }
            if (PlayerPrefs.HasKey("Grenade")) {
                coverGrenade.Unselect();
            }
            else {
                coverGrenade.Off();
            }
            if (PlayerPrefs.HasKey("Grapple")) {
                coverGrapple.Unselect();
            }
            else {
                coverGrapple.Off();
            }
        }
        else if ((Character.Weapon)PlayerPrefs.GetInt("Weapon", 0) == Character.Weapon.Projectile) {
            coverProjectile.Select();
            if (PlayerPrefs.HasKey("Missile")) {
                coverMissile.Unselect();
            }
            else {
                coverMissile.Off();
            }
            if (PlayerPrefs.HasKey("FlameThrower")) {
                coverFlameThrower.Unselect();
            }
            else {
                coverFlameThrower.Off();
            }
            if (PlayerPrefs.HasKey("Grenade")) {
                coverGrenade.Unselect();
            }
            else {
                coverGrenade.Off();
            }
            if (PlayerPrefs.HasKey("Grapple")) {
                coverGrapple.Unselect();
            }
            else {
                coverGrapple.Off();
            }
        }
        else if ((Character.Weapon)PlayerPrefs.GetInt("Weapon", 0) == Character.Weapon.Missile) {
            if (PlayerPrefs.HasKey("Projectile")) {
                coverProjectile.Unselect();
            }
            else {
                coverProjectile.Off();
            }
            coverMissile.Select();
            if (PlayerPrefs.HasKey("FlameThrower")) {
                coverFlameThrower.Unselect();
            }
            else {
                coverFlameThrower.Off();
            }
            if (PlayerPrefs.HasKey("Grenade")) {
                coverGrenade.Unselect();
            }
            else {
                coverGrenade.Off();
            }
            if (PlayerPrefs.HasKey("Grapple")) {
                coverGrapple.Unselect();
            }
            else {
                coverGrapple.Off();
            }
        }
        else if ((Character.Weapon)PlayerPrefs.GetInt("Weapon", 0) == Character.Weapon.FlameThrower) {
            if (PlayerPrefs.HasKey("Projectile")) {
                coverProjectile.Unselect();
            }
            else {
                coverProjectile.Off();
            }
            if (PlayerPrefs.HasKey("Missile")) {
                coverMissile.Unselect();
            }
            else {
                coverMissile.Off();
            }
            coverFlameThrower.Select();
            if (PlayerPrefs.HasKey("Grenade")) {
                coverGrenade.Unselect();
            }
            else {
                coverGrenade.Off();
            }
            if (PlayerPrefs.HasKey("Grapple")) {
                coverGrapple.Unselect();
            }
            else {
                coverGrapple.Off();
            }
        }
        else if ((Character.Weapon)PlayerPrefs.GetInt("Weapon", 0) == Character.Weapon.Grenade) {
            if (PlayerPrefs.HasKey("Projectile")) {
                coverProjectile.Unselect();
            }
            else {
                coverProjectile.Off();
            }
            if (PlayerPrefs.HasKey("Missile")) {
                coverMissile.Unselect();
            }
            else {
                coverMissile.Off();
            }
            if (PlayerPrefs.HasKey("FlameThrower")) {
                coverFlameThrower.Unselect();
            }
            else {
                coverFlameThrower.Off();
            }
            coverGrenade.Select();
            if (PlayerPrefs.HasKey("Grapple")) {
                coverGrapple.Unselect();
            }
            else {
                coverGrapple.Off();
            }
        }
        else if ((Character.Weapon)PlayerPrefs.GetInt("Weapon", 0) == Character.Weapon.Grapple) {
            if (PlayerPrefs.HasKey("Projectile")) {
                coverProjectile.Unselect();
            }
            else {
                coverProjectile.Off();
            }
            if (PlayerPrefs.HasKey("Missile")) {
                coverMissile.Unselect();
            }
            else {
                coverMissile.Off();
            }
            if (PlayerPrefs.HasKey("FlameThrower")) {
                coverFlameThrower.Unselect();
            }
            else {
                coverFlameThrower.Off();
            }
            if (PlayerPrefs.HasKey("Grenade")) {
                coverGrenade.Unselect();
            }
            else {
                coverGrenade.Off();
            }
            coverGrapple.Select();
        }
    }
    
}
