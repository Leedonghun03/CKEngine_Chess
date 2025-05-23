using UnityEngine;

public interface ILiftAble
{
    void LiftToParent(Transform parent);
    bool TryPlaceOnBoard(Vector3 dropWorldPosition);
}