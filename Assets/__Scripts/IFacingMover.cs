using UnityEngine;

public interface IFacingMover
{
    // a
    int GetFacing();
    // b
    float GetSpeed();
    bool moving { get; }
    // c
    float gridMult { get; }
    // d
    bool isInRoom { get; }
    Vector2 roomNum { get; set; }
    Vector2 posInRoom { get; set; }
    // e
    Vector2 GetGridPosInRoom(float mult = -1);
    // f
}