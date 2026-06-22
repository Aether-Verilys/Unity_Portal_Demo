public class TimeIncomeService
{
    private float _accumulator;

    public int Tick(float deltaTime, int incomePerSecond)
    {
        if (incomePerSecond <= 0)
        {
            return 0;
        }

        _accumulator += deltaTime * incomePerSecond;
        var income = (int)_accumulator;
        if (income > 0)
        {
            _accumulator -= income;
        }

        return income;
    }
}
