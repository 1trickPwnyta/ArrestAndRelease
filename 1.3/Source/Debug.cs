namespace ArrestAndRelease
{
    public static class Debug
    {
        public static void Log(string message)
        {
#if DEBUG
            Verse.Log.Message($"[{ArrestAndReleaseMod.PACKAGE_NAME}] {message}");
#endif
        }
    }
}
