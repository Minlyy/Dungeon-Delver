using UnityEngine;

public interface IKeyMaster
{
    int keyCount { get; set; }
    // a
    Vector2 pos { get; }
    // b
    int GetFacing();
    // c
}