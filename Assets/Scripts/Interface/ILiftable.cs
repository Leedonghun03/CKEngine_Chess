using UnityEngine;

public interface ILiftAble
{
    void LiftToParent(Transform parent);
    void PlaceAt(Vector3 worldPosition);
}