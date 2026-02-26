namespace SimpleRDBMSRestfulAPI.Constants
{
    public static class RegexConstants
    {
        // prevent any query containing dangerous commands that can modify the database structure, such as:
        // DROP TABLE, DROP INDEX, DROP DATABASE, ALTER TABLE, ALTER INDEX, RENAME INDEX
        public const string DANGEROUS_COMMAND_REGEX =
            @"(?i)\b(DROP\s+(TABLE|INDEX|DATABASE)|ALTER\s+(TABLE|INDEX)|RENAME\s+INDEX)\b";
        // prevent any query containing conditions that can be used for SQL Injection, such as: 
        // column = value
        // column IS NULL
        // column IS NOT NULL
        public const string DANGEROUS_SQL_INJECTION_CONDITIONS_REGEX =
           @"(?i)(\b[\w.]+?\s*=\s*(?:ANY\([^()]+\)|'[^']*'|(?!@)[^()\s]+)|\b[\w.]+\s+IS\s+NOT\s+NULL|\b[\w.]+\s+IS\s+NULL)";
    }
}
