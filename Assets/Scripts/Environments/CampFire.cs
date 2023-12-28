using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int Damage;
    public float DamageRate;

    private List<IDamagable> _thingsToDamage = new List<IDamagable>();

    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating("DealDamage", 0, DamageRate);
    }

    void DealDamage()
    {
        for (int i = 0; i<_thingsToDamage.Count; i++)
        {
            _thingsToDamage[i].TakePhysicalDamage(Damage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            _thingsToDamage.Add(damagable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            _thingsToDamage.Remove(damagable);
        }
    }
}
