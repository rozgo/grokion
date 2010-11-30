using UnityEngine;
using System.Collections;

public class IceBeast : MonoBehaviour {

    public Health health;
    public GameObject rightPost;
    public GameObject leftPost;
    public AudioClip hitClip;
    public AudioClip deathClip;
    public new Renderer renderer;
    public Collider head;

    GameObject post;
    Avalanche avalanche;
    bool hit = false;

    public enum State {
        Idle,
        Charge,
        Turn,
        Fall,
        Getup,
        Die,
    }

    public State state = State.Idle;
    
    IEnumerator IdleState () {
        animation.CrossFade("IDLE");
        float timer = 2.0f;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return 0;
        }
        state = State.Charge;
        NextState();
    }

    IEnumerator ChargeState () {
        int turnDistance = 7;
        float speed = 0;
        float distance = Vector3.Distance(transform.position, post.transform.position);
        Vector3 startPosition = transform.position;
        bool running = true;
        animation["WALK"].wrapMode = WrapMode.Loop;
        animation["RUN"].wrapMode = WrapMode.Loop;
        while (state == State.Charge) {
            speed = rigidbody.velocity.magnitude;
            if (running) {
                animation["RUN"].speed = Mathf.Max(speed / 10, 0.5f);
                if (speed < 5) {
                    animation.CrossFade("WALK");
                    running = false;
                }
            }
            else {
                animation["WALK"].speed = Mathf.Max(speed / 3, 0.5f);
                if (speed > 7) {
                    animation.CrossFade("RUN");
                    running = true;
                }
            }
            rigidbody.AddForce(transform.right * 4000, ForceMode.Force);
            if (rigidbody.velocity.magnitude > 10) {
                rigidbody.velocity = rigidbody.velocity.normalized * 10;
            }
            if (Vector3.Distance(startPosition, transform.position) > distance) {
                state = State.Turn;
                break;
            }
            if (Game.character != null) {
                Vector3 charPos = Game.character.transform.position;
                if (charPos.x < leftPost.transform.position.x && charPos.x > rightPost.transform.position.x) {
                    if (Vector3.Distance(charPos, transform.position) > turnDistance) {
                        if (charPos.x < transform.position.x) {
                            if (post != rightPost) {
                                state = State.Turn;
                                break;
                            }
                        }
                        else {
                            if (post != leftPost) {
                                state = State.Turn;
                            }
                        }
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
        NextState();
    }

    IEnumerator TurnState () {
        animation["WALK"].speed = 2;
        float speed = 200;
        if (post == leftPost) {
            post = rightPost;
        }
        else if (post == rightPost) {
            post = leftPost;
        }
        animation.CrossFade("WALK");
        while (state == State.Turn) {
            rigidbody.AddForce(-rigidbody.velocity * 300, ForceMode.Force);
            transform.rotation *= Quaternion.Euler(0, speed * Time.deltaTime, 0);
            if (post == leftPost) {
                if (transform.right.x > 0.99f) {
                    transform.right = Vector3.right;
                    break;
                }
            }
            else if (post == rightPost) {
                if (transform.right.x < -0.99f) {
                    transform.right = -Vector3.right;
                    break;
                }
            }
            yield return 0;
        }
        state = State.Charge;
        NextState();
    }

    IEnumerator FallState () {
        rigidbody.velocity = Vector3.zero;
        rigidbody.isKinematic = true;
        animation.CrossFade("FALL");
        float timer = 5.0f;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return 0;
        }
        if (avalanche != null) {
            avalanche.SendMessage("Kill", SendMessageOptions.DontRequireReceiver);
        }
        if (health.Alive()) {
            state = State.Getup;
        }
        NextState();
    }

    IEnumerator GetupState () {
        animation.CrossFade("GETUP");
        float timer = 2.0f;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return 0;
        }
        rigidbody.isKinematic = false;
        state = State.Charge;
        NextState();             
    }

    IEnumerator HitReact (int damage) {
        hit = true;
        if (state == State.Fall) {
            GameObject hitFX = (GameObject)Instantiate(Resources.Load("IceBeastHit", typeof(GameObject)));
            hitFX.transform.position = head.transform.position;
        }
        Game.fx.PlaySoundPitched(hitClip);
        health.Damage(damage);
        Material sharedMaterial = renderer.sharedMaterial;
        renderer.material.SetColor("_Color", Color.red);
        float timer = 0.1f;
        while (timer > 0) {
            timer -= Time.deltaTime;
            yield return 0;
        }
        renderer.material = sharedMaterial;
        hit = false;
    }

    void Hit (int damage) {
        if (health.Alive() && !hit && (state == State.Charge || state == State.Fall) ) {
            StartCoroutine(HitReact(damage));
        }
    }

    IEnumerator DieState () {
        Game.fx.PlaySound(deathClip);
        animation.CrossFade("FALL");
        rigidbody.isKinematic = true;
        while (true) {
            yield return new WaitForFixedUpdate();
        }
    }
    
    void OnDeath () {
        state = State.Die;
    }

    void OnProjectile (Projectile projectile) {
    }

    void OnCollisionEnter (Collision collision) {
        if (collision.collider.gameObject.layer == Game.projectilesLayer) {
            bool headCollision = false;
            for (int i=0; i<collision.contacts.Length; ++i) {
                if (collision.contacts[i].thisCollider == head) {
                    headCollision = true;
                    break;
                }
            }
            if (headCollision) {
                Projectile projectile = (Projectile)collision.collider.GetComponent(typeof(Projectile));
                if (state == State.Charge) {
                    Hit(projectile.Damage);
                }
                else if (state == State.Fall) {
                    Hit(projectile.Damage * 5);
                }
            }
        }
        else if (state == State.Charge && collision.collider.attachedRigidbody != null) {
            Avalanche avalanche = 
                (Avalanche)collision.collider.attachedRigidbody.GetComponent(typeof(Avalanche));
            if (avalanche != null) {
                if (collision.contacts[0].normal.y < -0.8f) {
                    state = State.Fall;
                    this.avalanche = avalanche;
                }
                else {
                    avalanche.SendMessage("Kill", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }

    void Awake () {
        post = leftPost;
    }

    void Start () {
        NextState();
    }

    void NextState () {
        string methodName = state.ToString() + "State";
        System.Reflection.MethodInfo info = 
            GetType().GetMethod(methodName, 
                                System.Reflection.BindingFlags.NonPublic |
                                System.Reflection.BindingFlags.Instance);
        StartCoroutine((IEnumerator)info.Invoke(this, null));
    }
    
}