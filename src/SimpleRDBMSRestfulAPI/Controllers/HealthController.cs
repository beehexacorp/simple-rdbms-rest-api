using hexasync.common;
using hexasync.domain.managers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using SimpleRDBMSRestfulAPI;

namespace SimpleRDBMSRestfulAPI.Controllers;

/// <summary>
/// Liveness + readiness probes.
///
/// <para><b>/health</b> is liveness: it answers "is this process alive?" and
/// MUST NOT touch the database or any other dependency. This service is a
/// generic REST wrapper over RDBMS connections, so the temptation to prove
/// health by querying a database is strong — resist it. Liveness failures
/// restart the pod, so a database blip that fails liveness would restart every
/// replica at once and turn a blip into an outage.</para>
///
/// <para><b>/health/ready</b> is readiness: this is where the database probe
/// belongs. Readiness failures only pull the pod out of the Service endpoints,
/// so the fleet stops taking traffic it cannot serve and recovers on its own
/// once the database returns. The probe targets the service's own Postgres
/// metadata store (<see cref="ApplicationDbContext"/>) — never the tenant
/// databases reached through /api/connection, which are per-request and
/// arbitrary in number.</para>
/// </summary>
[Route("health")]
[ApiController]
public class HealthController(IDbFactory dbFactory, ILogger<HealthController> logger) : ControllerBase
{
    /// <summary>Deployment name, so an operator can tell which service answered.</summary>
    private const string ServiceName = "simple-rest-api";

    /// <summary>
    /// Hard ceiling for the readiness database probe. Kept well under a typical
    /// 3s probe timeout: Npgsql's default connect timeout is 15s, which would
    /// otherwise let a dead database hang the probe until kubelet gives up.
    /// </summary>
    private static readonly TimeSpan DatabaseProbeTimeout = TimeSpan.FromMilliseconds(1500);

    /// <summary>
    /// Driver-level ceiling, applied to the probe's own connection string. The
    /// response ceiling above only bounds what the caller waits for; the
    /// abandoned attempt keeps running, and readiness re-fires every few seconds
    /// for the whole outage. Without this, Npgsql's 15s default would stack up
    /// orphaned connect attempts against a database that is already struggling.
    /// </summary>
    private const int DatabaseProbeConnectTimeoutSeconds = 2;

    /// <summary>
    /// Liveness. No dependency calls — deliberately. Returns 200 whenever the
    /// process can still serve a request.
    /// </summary>
    [HttpGet]
    public IActionResult GetLiveness()
    {
        return Ok(new
        {
            status = "ok",
            service = ServiceName
        });
    }

    /// <summary>
    /// Readiness. 200 when the service's own database is reachable, 503 when it
    /// is not. Never 500: an unreachable database is an expected answer here.
    /// </summary>
    [HttpGet("ready")]
    public async Task<IActionResult> GetReadiness(CancellationToken cancellationToken)
    {
        var checks = new Dictionary<string, object>();

        var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeoutCts.CancelAfter(DatabaseProbeTimeout);

        // Task.Run so that nothing runs synchronously on the request path: an
        // async method still executes up to its first await on the caller's
        // thread, and a stalled DNS lookup there would blow straight through the
        // ceiling below. Off the request path, the ceiling always holds.
        var probeTask = Task.Run(() => ProbeDatabaseAsync(timeoutCts.Token), CancellationToken.None);

        // Dispose only once the probe has actually finished. A probe abandoned by
        // the race below still holds the linked token, and disposing the source
        // underneath it would throw ObjectDisposedException inside the driver.
        _ = probeTask.ContinueWith(finished => timeoutCts.Dispose(), TaskScheduler.Default);

        // Race the probe against a timer as well as cancelling it: a driver that
        // ignores the token must still never hold the probe past the ceiling.
        var timeoutTask = Task.Delay(DatabaseProbeTimeout, CancellationToken.None);
        var timedOut = await Task.WhenAny(probeTask, timeoutTask) != probeTask;
        var (databaseReady, databaseError) = timedOut ? (false, "timeout") : await probeTask;

        checks["database"] = databaseReady;
        if (databaseError is not null)
        {
            checks["database_error"] = databaseError;
        }

        if (!databaseReady)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                status = "not_ready",
                service = ServiceName,
                checks
            });
        }

        return Ok(new
        {
            status = "ready",
            service = ServiceName,
            checks
        });
    }

    /// <summary>
    /// Opens the service's own metadata database and confirms it answers. Uses
    /// the read path, the same one <c>AppSettings</c> resolves connections
    /// through on virtually every request.
    /// </summary>
    private async Task<(bool Ready, string? Error)> ProbeDatabaseAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var db = dbFactory.CreateDbContext<ApplicationDbContext>(DatabaseQueryType.DML_READ);
            ApplyDriverTimeout(db);

            return await db.Database.CanConnectAsync(cancellationToken)
                ? (true, null)
                : (false, "unreachable");
        }
        catch (Exception ex)
        {
            // Any driver-level failure — unreachable host, auth rejection, or the
            // probe timeout cancelling us — means "not ready", not a 500. The
            // response carries only the exception TYPE: an Npgsql message names the
            // host, port, database and sometimes the user. The full exception goes
            // to the server-side log only.
            logger.LogWarning(ex, "Readiness database probe failed: {ExceptionType}", ex.GetType().Name);
            return (false, ex.GetType().Name);
        }
    }

    /// <summary>
    /// Rewrites the probe context's connection string with a short connect and
    /// command timeout, so an abandoned probe dies with the response instead of
    /// lingering. Best-effort: if the string cannot be parsed the probe simply
    /// runs on the factory's original one rather than reporting a false outage.
    /// </summary>
    private void ApplyDriverTimeout(ApplicationDbContext db)
    {
        try
        {
            // Inside the try on purpose: both of these throw on a non-relational
            // provider, and a probe must never report a false outage because the
            // tightening step itself failed.
            var connectionString = db.Database.GetConnectionString();
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return;
            }

            db.Database.SetConnectionString(new NpgsqlConnectionStringBuilder(connectionString)
            {
                Timeout = DatabaseProbeConnectTimeoutSeconds,
                CommandTimeout = DatabaseProbeConnectTimeoutSeconds
            }.ConnectionString);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Could not apply readiness probe connection timeout: {ExceptionType}", ex.GetType().Name);
        }
    }
}
