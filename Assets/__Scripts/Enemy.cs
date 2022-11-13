using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, ISwappable
{
    protected static Vector2[] directions = new Vector2[] {
        Vector2.right, Vector2.up, Vector2.left,
        Vector2.down, Vector2.zero };

    [Header("Inscribed: Enemy")]
    // b
    public float maxHealth = 1;
    public float knockbackSpeed = 10;
    public float knockbackDuration = 0.25f;
    public float invincibleDuration = 0.5f;

    [SerializeField]
    public GameObject _guaranteedDrop = null;
    public List<GameObject> randomItems;
    // c

    [Header("Dynamic: Enemy")]
    // b
    public float health;
    // c
    public bool invincible = false;
    public bool knockback = false;
    private float invincibleDone = 0;
    private float knockbackDone = 0;
    private Vector2 knockbackVel;

    protected Animator anim;
    // c
    protected Rigidbody2D rigid;
    // c
    protected SpriteRenderer sRend;
    // c

    protected virtual void Awake()
    {
        health = maxHealth;
        anim = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody2D>();
        sRend = GetComponent<SpriteRenderer>();
    }

    protected virtual void Update()
    {

        // Check knockback and invincibility
        if (invincible && Time.time > invincibleDone)
            invincible = false;
        sRend.color = invincible ? Color.red : Color.white;
        if (knockback)
        {
            rigid.velocity = knockbackVel;
            if (Time.time < knockbackDone) return;
        }

        anim.speed = 1;
        // c
        knockback = false;
    }

    void OnTriggerEnter2D(Collider2D colld)
    {
        // d
        if (invincible) return; // Return if this can’t bedamaged
        DamageEffect dEf = colld.gameObject.GetComponent<DamageEffect>();
        if (dEf == null) return; // If no DamageEffect, exitthis method

        health -= dEf.damage; // Subtract the damage amountfrom health
        if (health <= 0) Die();
        // e

        invincible = true; // Make this invincible
        invincibleDone = Time.time + invincibleDuration;

        if (dEf.knockback)
        { // Knockback this Enemy Vector2 delta;
            Vector2 delta;
            // Is an IFacingMover attached to the Colliderthat triggered this ?
            IFacingMover iFM = colld.GetComponentInParent<IFacingMover>(); // f
            if (iFM != null)
            {
                // Determine the direction of knockback fromthe iFM’s facing
                delta = directions[iFM.GetFacing()];
            }
            else
            {
                // Determine the direction of knockback fromrelative position
                delta = transform.position - colld.transform.position;
                if (Mathf.Abs(delta.x) >= Mathf.Abs(delta.y))
                {
                    // Knockback should be horizontal
                    delta.x = (delta.x > 0) ? 1 : -1;
                    delta.y = 0;
                }
                else
                {
                    // Knockback should be vertical
                    delta.x = 0;
                    delta.y = (delta.y > 0) ? 1 : -1;
                }
            }

            // Apply knockback speed to the Rigidbody
            knockbackVel = delta * knockbackSpeed;
            rigid.velocity = knockbackVel;

            // Set mode to knockback and set time to stopknockback
            knockback = true;
            knockbackDone = Time.time + knockbackDuration;
            anim.speed = 0;
        }
    }

    void Die()
    {
        GameObject go;
        if (guaranteedDrop != null)
        {

            go = Instantiate<GameObject>(guaranteedDrop);
            go.transform.position = transform.position;
        }
        else if (randomItems.Count > 0)
        {
            // a
            int n = Random.Range(0, randomItems.Count);
            GameObject prefab = randomItems[n];
            if (prefab != null)
            {
                // b
                go = Instantiate<GameObject>(prefab);
                go.transform.position = transform.position;
            }
        }
        Destroy(gameObject);
    }

    //———————————————————— Implementation of ISwappable————————————————————
    public GameObject guaranteedDrop
    {
        // c
        get { return _guaranteedDrop; }
        set { _guaranteedDrop = value; }
    }
    public int tileNum { get; private set; }
    // d

    public virtual void Init(int fromTileNum, int tileX, int tileY)
    { // e
        tileNum = fromTileNum;

        // Position this GameObject correctly
        transform.position = new Vector3(tileX, tileY, 0) + MapInfo.OFFSET;
    }
}
