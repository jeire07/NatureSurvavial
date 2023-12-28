using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IDamagable
{
    void TakePhysicalDamage(int damageAmount);
}

[System.Serializable]
public class Condition
{
    [HideInInspector]
    public float CurValue;
    public float MaxValue;
    public float StartValue;
    public float RegenRate;
    public float DecayRate;
    public Image UiBar;

    public void Add(float amount)
    {
        CurValue = Mathf.Min(CurValue+ amount, MaxValue);
    }

    public void Subtract(float amount)
    {
        CurValue = Mathf.Max(CurValue - amount, 0.0f);
    }

    public float GetPercentage()
    {
        return CurValue / MaxValue;
    }
}

public class PlayerConditions : MonoBehaviour, IDamagable
{
    public Condition Health;
    public Condition Hunger;
    public Condition Stamina;

    public float NoHungerHealthDecay;

    public UnityEvent OnTakeDamage;

    // Start is called before the first frame update
    void Start()
    {
        Health.CurValue = Health.StartValue;
        Hunger.CurValue = Hunger.StartValue;
        Stamina.CurValue = Stamina.StartValue;
    }

    // Update is called once per frame
    void Update()
    {
        Hunger.Subtract(Hunger.DecayRate * Time.deltaTime);
        Stamina.Add(Stamina.RegenRate * Time.deltaTime);

        if (Hunger.CurValue == 0.0f)
            Health.Subtract(NoHungerHealthDecay * Time.deltaTime);

        if (Health.CurValue == 0.0f)
            Die();

        Health.UiBar.fillAmount = Health.GetPercentage();
        Hunger.UiBar.fillAmount = Hunger.GetPercentage();
        Stamina.UiBar.fillAmount = Stamina.GetPercentage();
    }

    public void Heal(float amount)
    {
        Health.Add(amount);
    }

    public void Eat(float amount)
    {
        Hunger.Add(amount);
    }

    public bool UseStamina(float amount)
    {
        if (Stamina.CurValue - amount < 0)
            return false;

        Stamina.Subtract(amount);
        return true;
    }

    public void Die()
    {
        Debug.Log("플레이어가 죽었다.");
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        Health.Subtract(damageAmount);
        OnTakeDamage?.Invoke();
    }
}
