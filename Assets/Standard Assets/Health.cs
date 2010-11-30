using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	
    public int maxHP = 100;

	int hp = 100;

    void Awake () {
        hp = maxHP;
    }

    public int HP {get {return hp;}}

    public bool Alive () {
        return hp >= 0;
    }

    public void Kill () {
        Damage(maxHP + 1);
    }

    public void Damage (int damage) {
        AddHP(-damage);
    }

    public void AddHP (int hp) {
        if (this.hp >= 0) {
            this.hp += hp;
            if (this.hp < 0) {
                SendMessage("OnDeath", this, SendMessageOptions.DontRequireReceiver);
            }
            else if (this.hp > maxHP) {
                this.hp = maxHP;
            }
        }
    }

}

