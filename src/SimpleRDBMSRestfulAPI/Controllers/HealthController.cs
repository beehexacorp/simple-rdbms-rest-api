using Microsoft.AspNetCore.Mvc;

namespace SimpleRDBMSRestfulAPI.Controllers;

/// <summary>
/// Kubernetes probe surface for the <c>simple-rest-api</c> deployment.
///
/// <para><b>GET /health — LIVENESS.</b> Answers "is this process alive?" and nothing else.
/// It makes no dependency call. Liveness failure restarts the pod, so a liveness probe that
/// touches a database turns one blip into a simultaneous restart of every replica.</para>
///
/// <para><b>Readiness is deliberately not implemented yet</b>, and this service is the LEAST
/// questionable case in the estate for eventually having one: it exists to expose its own
/// database over REST, so without that database it can serve nothing, and that database is
/// its own rather than shared estate infrastructure. It was removed for consistency while the
/// per-service gating decision is made — not because the check was wrong.
///
/// Elsewhere readiness gated on the SHARED redis-cluster-leader and Redpanda that nine bhs01
/// containers all point at, where one blip would mark every replica of nine services NotReady
/// at once and empty their Service endpoint lists. That is the case that forced the deferral.
///
/// The removed probe — which opened the read-path connection and ran CanConnectAsync under a
/// wall-clock ceiling with an explicit driver connect timeout — is in this file's git history.
/// Note when re-adding it: the databases reached through /api/connection are per-request and
/// arbitrary in number, so they are not candidates for a readiness gate. Only the service's
/// own metadata database is.</para>
/// </summary>
[Route("health")]
[ApiController]
public class HealthController : ControllerBase
{
    /// <summary>Deployment name, so an operator can tell which service answered.</summary>
    private const string ServiceName = "simple-rest-api";

    /// <summary>
    /// Liveness. No dependency calls — deliberately. Returns 200 whenever the process can
    /// still serve a request.
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
}
