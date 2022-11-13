using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMove : MonoBehaviour
{
    private IFacingMover mover;

    void Awake()
    {
        mover = GetComponent<IFacingMover>();
        // a
        if (mover == null)
        {
            Debug.LogError("Cannot find IFacingMover on " + gameObject.name);
        }
    }

    void FixedUpdate()
    {
        // b
        if (!mover.moving) return; // If not moving, nothing to do here
        int facing = mover.GetFacing();

        // If we are moving in a direction, align to thegrid
        // First, get the grid location
        Vector2 posIR = mover.posInRoom;
        Vector2 posIRGrid = mover.GetGridPosInRoom();
        // This relies on IFacingMover (which uses InRoom)to choose grid spacing

        // Then move towards the grid line
        float delta = 0;
        // c
        if (facing == 0 || facing == 2)
        {
            delta = posIRGrid.y - posIR.y;
        }
        else
        {
            // If the movement is vertical, align toposIRGrid.x
            delta = posIRGrid.x - posIR.x;
        }
        if (delta == 0) return; // Already aligned to thegrid
        float gridAlignSpeed = mover.GetSpeed() * Time.fixedDeltaTime; // d
        gridAlignSpeed = Mathf.Min(gridAlignSpeed, Mathf.Abs(delta));
        if (delta < 0) gridAlignSpeed = -gridAlignSpeed;

        if (facing == 0 || facing == 2)
        {
            // e
            posIR.y += gridAlignSpeed;
        }
        else
        {
            posIR.x += gridAlignSpeed;
        }

        mover.posInRoom = posIR;
        // f
    }

}
