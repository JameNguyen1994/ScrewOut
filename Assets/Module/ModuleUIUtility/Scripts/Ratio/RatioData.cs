using System;

[Serializable]
public class RatioData<T>
{
    public float Width;
    public float Height;
    public T Value;
    public float Ratio => RatioService.CalculateAspectRatio(Width, Height);
}