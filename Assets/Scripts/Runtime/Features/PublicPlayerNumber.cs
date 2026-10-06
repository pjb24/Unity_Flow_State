namespace FlowState.Runtime.Features
{
    public static class PublicPlayerNumber
    {
        public static bool IsValid(string value)
        {
            if (value == null || value.Length != 10 || value == "0000000000") return false;
            for (int i = 0; i < value.Length; i++)
                if (value[i] < '0' || value[i] > '9') return false;
            return true;
        }

        public static string Format(string value, bool isMe)
        {
            if (!IsValid(value)) return string.Empty;
            return isMe ? value + " (You)" : value;
        }
    }
}
