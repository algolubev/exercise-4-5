namespace Services
{
    public class NumberProcessor: INumberProcessor
    {
        public bool EnsureNumberIsEven(int number)
        {
            return (number % 2 == 0);
        }
    }
}