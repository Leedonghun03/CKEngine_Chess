using System;
using UnityEngine;

public class FracturePawn : MonoBehaviour
{
    private float timer = 0.0f;

    private float maxLifetime = 5.0f;

    [SerializeField]
    public Material material;

    [SerializeField]
    public float ExplosionForce = 600.0f;

    [SerializeField]
    public float ExplosionRadius = 5.0f;

    [SerializeField]
    public float UpwardsModifier = -1.0f;

    void Start()
    {
        for (int i = 0; i < transform.childCount; ++i)
        {
            var obj = transform.GetChild(i).gameObject;
            obj.GetComponent<MeshRenderer>().material = material;
            obj.GetComponent<Rigidbody>().AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius, UpwardsModifier);
        }
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer > maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < transform.childCount; ++i)
        {
            var obj = transform.GetChild(i).gameObject;
            var mat = obj.GetComponent<MeshRenderer>().material;
            mat.SetFloat("_DissolveAmount", Math.Max(0, timer - 3));
        }
    }
}
