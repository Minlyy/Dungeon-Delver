using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]

public class Grappler : MonoBehaviour, IGadget
{
    public enum eMode { gIdle, gOut, gRetract, gPull }
    [Header("Inscribed")]
    [Tooltip("Speed at which Grappler extends (doubled in gRetract mode)")]
    public float grappleSpd = 10;
    [Tooltip("Maximum length that Grappler will reach")]
    public float maxLength = 7.25f;
    [Tooltip("Minimum distance of Grappler from Dray")]
    public float minLength = 0.375f;
    [Tooltip("Health deducted when Dray ends a grapple on an unsafe tile")]
    public int unsafeTileHealthPenalty = 2;
    [Header("Dynamic")]
    [SerializeField]
    private eMode _mode = eMode.gIdle;
    public eMode mode
    {
        get { return _mode; }
        private set { _mode = value; }
    }

    private LineRenderer line;
    private Rigidbody2D rigid;
    private Collider2D colld;

    private Vector3 p0, p1;
    private int facing;
    private Dray dray;
    private System.Func<IGadget, bool> gadgetDoneCallback;
    // a

    private Vector2[] directions = new Vector2[] {
// b
 Vector2.right, Vector2.up, Vector2.left,Vector2.down };
    private Vector3[] dirV3s = new Vector3[] {
 Vector3.right, Vector3.up, Vector3.left,Vector3.down };

    void Awake()
    { // Get component references to use throughout the script
        line = GetComponent<LineRenderer>();
        rigid = GetComponent<Rigidbody2D>();
        colld = GetComponent<Collider2D>();
    }

    void Start()
    {
        gameObject.SetActive(false); // Initially disable this GameObject // c
    }
    void SetGrappleMode(eMode newMode)
    {
        // d
        switch (newMode)
        {
            case eMode.gIdle:
                // e
                transform.DetachChildren(); // Release any child Transforms
                gameObject.SetActive(false);
                if (dray != null && dray.controlledBy == this as IGadget)
                { // b
                    dray.controlledBy = null;
                    dray.physicsEnabled = true;
                }
                break;

            case eMode.gOut:
                // f
                gameObject.SetActive(true);
                rigid.velocity = directions[facing] * grappleSpd;
                break;

            case eMode.gRetract:
                // g
                rigid.velocity = -directions[facing] * (grappleSpd * 2);
                break;

            case eMode.gPull:
                p1 = transform.position;
                rigid.velocity = Vector2.zero;
                dray.controlledBy = this;
                dray.physicsEnabled = false;

                break;
        }

        mode = newMode;
        // h
    }
    void FixedUpdate()
    {
        p1 = transform.position;
        // a
        line.SetPosition(1, p1);

        switch (mode)
        {
            case eMode.gOut: // Grappler shooting out 
                             // See if the Grappler reached its limit without hitting anything
                if ((p1 - p0).magnitude >= maxLength)
                {
                    SetGrappleMode(eMode.gRetract);
                }
                break;

            case eMode.gRetract: // Grappler missed; return at double speed
                                 // Check to see if the Grappler is no longer in front of Dray
                if (Vector3.Dot((p1 - p0), dirV3s[facing]) < 0)
                    GrappleDone(); // c
                break;

            case eMode.gPull:
                if ((p1 - p0).magnitude > minLength)
                {

                    // Move Dray toward the Grappler hit point
                    p0 += dirV3s[facing] * grappleSpd * Time.fixedDeltaTime;
                    dray.transform.position = p0;
                    line.SetPosition(0, p0);
                    // Stop Grappler from moving with Dray
                    transform.position = p1;
                }
                else
                {

                    // Dray is close enough to stop grappling
                    p0 = p1 - (dirV3s[facing] * minLength);
                    dray.transform.position = p0;
                    // Check whether Dray landed on an unsafetile
                    Vector2 checkPos = (Vector2)p0 + new Vector2(0, -0.25f); // g
                    if (MapInfo.UNSAFE_TILE_AT_VECTOR2(checkPos))
                    {
                        // Dray landed on an unsafe tile
                        dray.ResetInRoom(unsafeTileHealthPenalty);
                    }
                    GrappleDone();
                }

                break;
        }
    }

    // Ensures that p1 is aligned with the Grappler head
    void LateUpdate()
    {
        // d
        p1 = transform.position;
        line.SetPosition(1, p1);
    }

    /// <summary>
    /// Called when the Grappler hits a Trigger or Colliderin the GrapTiles,
    /// Items, or Enemies Physics Layers (Grappler’sCollider is a Trigger).
    /// </summary>
    /// <param name="coll"></param>
    void OnTriggerEnter2D(Collider2D colld)
    {
        // The Grappler has collided with something, butwhat ?
        string otherLayer = LayerMask.LayerToName(colld.gameObject.layer); // e

        switch (otherLayer)
        { // Please DOUBLE-CHECKlayer name spelling!
            case "Items": // We’ve possibly hit a PickUp 
                PickUp pup = colld.GetComponent<PickUp>();
                if (pup == null) return;
                // If this IS a PickUp, make it a child of thisTransform so it moves
                // with the Grappler head.
                pup.transform.SetParent(transform);
                pup.transform.localPosition = Vector3.zero;
                SetGrappleMode(eMode.gRetract);
                break;

            case "Enemies": // We’ve hit an Enemy 
                            // g
                            // The Grappler should return when it hits anEnemy
                Enemy e = colld.GetComponent<Enemy>();
                if (e != null) SetGrappleMode(eMode.gRetract);
                // Damaging the Enemy is handled by theDamageEffect & Enemy scripts
                break;

            case "GrapTiles": // We’ve hit a GrapTile
                SetGrappleMode(eMode.gPull);
                break;

            default:
                // h
                SetGrappleMode(eMode.gRetract);
                break;
        }
    }

    void GrappleDone()
    {
        // i
        SetGrappleMode(eMode.gIdle);

        // Callback to Dray so they return to normalcontrols
        gadgetDoneCallback(this);
        // j
    }

    #region IGadget_Implementation 
    //———————————————————— Implementation of IGadget———————————————————— // b

    // Called by Dray to use this IGadget
    public bool GadgetUse(Dray tDray, System.Func<IGadget, bool> tCallback)
    { // c
        if (mode != eMode.gIdle) return false;
        // d

        dray = tDray;
        gadgetDoneCallback = tCallback;
        // e
        transform.localPosition = Vector3.zero;

        facing = dray.GetFacing();
        // f
        p0 = dray.transform.position;
        p1 = p0 + (dirV3s[facing] * minLength);
        gameObject.transform.position = p1;
        gameObject.transform.rotation = Quaternion.Euler(0, 0, 90 * facing);

        line.positionCount = 2;
        // g
        line.SetPosition(0, p0);
        line.SetPosition(1, p1);
        SetGrappleMode(eMode.gOut);

        return true;
    }

    // Called by Dray if they are hit while grappling andmode != eMode.inHit
    public bool GadgetCancel()
    {
        // h
        // If pulling Dray to a wall, ignore GadgetCancel
        if (mode == eMode.gPull) return false;
        SetGrappleMode(eMode.gIdle);
        gameObject.SetActive(false);
        return true;
    }
    // string name is already part of Grappler (inheritedfrom Object) // i
    #endregion
}
