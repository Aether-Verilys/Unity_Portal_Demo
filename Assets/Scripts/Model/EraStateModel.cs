using System;

public class EraStateModel
{
    public EraType CurrentEra { get; private set; } = EraType.StoneAge;

    public event Action<EraType> OnEraChanged;

    public void SwitchEra()
    {
        CurrentEra = CurrentEra == EraType.StoneAge ? EraType.Tech : EraType.StoneAge;
        OnEraChanged?.Invoke(CurrentEra);
    }
}
