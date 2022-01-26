namespace RES.ATM.API.Shared.Helpers
{
    public static class IntExtensions
    {
        public static bool IsValidPin(this int value)
        {
            if (value == default || (value < 1000 || value > 9999))
                return false;
            return true;
        }
    }
}
