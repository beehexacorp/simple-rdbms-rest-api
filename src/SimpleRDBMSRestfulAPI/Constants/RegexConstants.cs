namespace SimpleRDBMSRestfulAPI.Constants
{
    public static class RegexConstants
    {
        public const string DANGEROUS_COMMAND_REGEX =
            @"(?i)\b(DROP\s+(TABLE|INDEX|DATABASE)|ALTER\s+(TABLE|INDEX)|RENAME\s+INDEX)\b";

        public const string DANGEROUS_SQL_INJECTION_CONDITIONS_REGEX =
           @"(?i)(\b[\w.]+?\s*=\s*(?:ANY\([^()]+\)|'[^']*'|(?!@)[^()\s]+)|\b[\w.]+\s+IS\s+NOT\s+NULL|\b[\w.]+\s+IS\s+NULL)";
    }
}
