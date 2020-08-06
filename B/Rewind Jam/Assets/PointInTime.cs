using UnityEngine;

public class PointInTime
{
    public Vector2 position;
    public Sprite sprite;
    public bool flipX, facingRight;

    public PointInTime (Vector2 _position, Sprite _sprite, bool _flipX, bool _facingRight)
    {
        position = _position;
        sprite = _sprite;
        flipX = _flipX;
        facingRight = _facingRight;
    }
}
