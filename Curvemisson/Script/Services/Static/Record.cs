namespace Services.Static
{
    public static class Record
    {
        public static bool IsValidRecordValue(float recordValue)
        {
            if (UnityEngine.Mathf.Abs(recordValue - Constants.Record.NoneValue) < float.Epsilon)
            {
                return false;
            }

            return true;
        }
    }
}