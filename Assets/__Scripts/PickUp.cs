using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour, ISwappable
{
    public enum eType { none, key, health, grappler }

    [Header("Inscribed")]
    public eType itemType;
    private Collider2D colld;
    private const float colliderEnableDelay = 0.5f;
    // a

    void Awake()
    {
        colld = GetComponent<Collider2D>();
        // b
        colld.enabled = false;
        Invoke(nameof(EnableCollider), colliderEnableDelay); // c
    }

    void EnableCollider()
    {
        // d
        colld.enabled = true;
    }
    //———————————————————— Implementation of ISwappable————————————————————
    public GameObject guaranteedDrop { get; set; }

    public int tileNum { get; private set; }

    public virtual void Init(int fromTileNum, int tileX, int tileY)
    { // b > tileNum = fromTileNum;

        // Position this GameObject correctly
        transform.position = new Vector3(tileX, tileY, 0) + MapInfo.OFFSET;
    }

}

