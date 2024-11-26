using SimpleRDBMSRestfulAPI.Constants;
using System.Text.RegularExpressions;

namespace SimpleRDBMSRestfulAPI.Helpers;

public interface ISqlInjectionHelper
{
    Task EnsureValid(string sqlQuery);
}

public class SqlInjectionHelper : ISqlInjectionHelper
{
    public async Task EnsureValid(string sqlQuery)
    {
        // TODO: use AI or some external tools for validating SQL Injection
        var dangerousCommandsRegex = new Regex(
            RegexConstants.DANGEROUS_COMMAND_REGEX,
            RegexOptions.IgnoreCase
        );
        if (dangerousCommandsRegex.IsMatch(sqlQuery))
        {
            throw new Exception("Query contains forbidden SQL commands.");
        }

        var embeddedStringRegex = new Regex(
            RegexConstants.DANGEROUS_SQL_INJECTION_CONDITIONS_REGEX,
            RegexOptions.IgnoreCase
        );
        if (embeddedStringRegex.IsMatch(sqlQuery))
        {
            throw new Exception("Query contains forbidden SQL commands.");
        }
        await Task.CompletedTask;
    }
}