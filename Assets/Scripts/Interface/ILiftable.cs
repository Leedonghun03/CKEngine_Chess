using UnityEngine;

public interface ILiftAble
{
    void LiftToParent(Transform parent);
    bool IsCanPlaceOnBoard(Vector3 dropWorldPosition, out Vector2Int boardPos);
    bool TryPlaceOnBoard(Vector3 dropWorldPosition);
}