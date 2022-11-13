using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    private GameObject sword;
    private GameObject sword1;
    private Dray dray;
    Rigidbody2D rigid;
    void Start()
    {
        // Find the Sword child of SwordController
        Transform swordT = transform.Find("Sword");
        Transform swordT1 = transform.Find("Sword1");
        // a
        if (swordT == null)
        {
            Debug.LogError("Could not find Sword child of SwordController.");
            return;
        }
        sword = swordT.gameObject;
        if (swordT1 == null)
        {
            Debug.LogError("Could not find Sword child of SwordController1.");
            return;
        }
        sword1 = swordT1.gameObject;
        // Find the Dray component on the parent ofSwordController
        dray = GetComponentInParent<Dray>();
        // b
        if (dray == null)
        {
            Debug.LogError("Could not find parent component Dray.");
            return;
        }

        // Deactivate the sword
        sword.SetActive(false);
        sword1.SetActive(false);
        rigid = sword1.GetComponent<Rigidbody2D>();
        // c
    }

    void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 90 * dray.facing); // d
        sword.SetActive(dray.mode == Dray.eMode.attack);
        sword1.SetActive(dray.mode == Dray.eMode.special);
        if (dray.mode == Dray.eMode.special)
        {
            Vector2 dir = Vector2.zero;
            if (dray.facing == 0)
                dir = Vector2.right;
            if (dray.facing == 1)
                dir = Vector2.up;
            if (dray.facing == 2)
                dir = Vector2.left;
            if (dray.facing == 3)
                dir = Vector2.down;
            rigid.velocity = dir * 5;
        }
        else
            ResetPosition();
    }

    void ResetPosition()
    {
        rigid.transform.position = dray.transform.position;
    }

}
