using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float Time;
    public float FullDayLength;
    public float StartTime = 0.4f;
    private float _timeRate;
    public Vector3 Noon;

    [Header("Sun")]
    public Light Sun;
    public Gradient SunColor;
    public AnimationCurve SunIntensity;

    [Header("Moon")]
    public Light Moon;
    public Gradient MoonColor;
    public AnimationCurve MoonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve LightingIntensityMultiplier;
    public AnimationCurve ReflectionIntensityMultiplier;

    private void Start()
    {
        _timeRate = 1.0f / FullDayLength;
        Time = StartTime;
    }

    private void Update()
    {
        Time = (Time + _timeRate * UnityEngine.Time.deltaTime) % 1.0f;

        UpdateLighting(Sun, SunColor, SunIntensity);
        UpdateLighting(Moon, MoonColor, MoonIntensity);

        RenderSettings.ambientIntensity = LightingIntensityMultiplier.Evaluate(Time);
        RenderSettings.reflectionIntensity = ReflectionIntensityMultiplier.Evaluate(Time);

    }

    void UpdateLighting(Light lightSource, Gradient colorGradiant, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(Time);

        lightSource.transform.eulerAngles = (Time - (lightSource == Sun ? 0.25f : 0.75f)) * Noon * 4.0f;
        lightSource.color = colorGradiant.Evaluate(Time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity ==0&&go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity>0 &&!go.activeInHierarchy)
            go.SetActive(true);
    }
}
