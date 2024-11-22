namespace SimpleRDBMSRestfulAPI.Constants
{
    public static class RegexConstants
    {
        public const string dangerousCommands =
            @"(?i)\b(DROP\s+(TABLE|INDEX|DATABASE)|ALTER\s+(TABLE|INDEX)|RENAME\s+INDEX)\b";

        public const string embeddedString =
            @"(?i)(\b[\w.]+?\s*=\s*(?:ANY\([^()]+\)|'[^']*'|[^()\s]+)|\b[\w.]+\s+IS\s+NOT\s+NULL|\b[\w.]+\s+IS\s+NULL)";
    }
}
