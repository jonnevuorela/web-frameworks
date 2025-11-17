using System;
using System.Security.Claims;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Middlewares;

public class RequireLoggedInUserMiddleware(IUserService service) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // endpoint on se controllerin routehandler, jossa kutsu tapahtuu
        var endpoint = context.GetEndpoint();
        // endpoint voi olla null, jos kirjoitat urlin väärin
        if (endpoint != null)
        {
            // haetaan käyttäjä vain , jos authorize-attribute-on käytössä
            var authorizedAttr = endpoint.Metadata.GetMetadata<AuthorizeAttribute>();
            // tänne mennään vain, jos [Authorize] on käytössö
            if (authorizedAttr != null)
            {
                var unAuthorizedResponse = new { title = "Unauthorized" };
                var idClaim = context.User.Claims.FirstOrDefault(c => c.Type == "sub");
                if (idClaim == null)
                {
                    context.Response.StatusCode = 401; // Unauthorized
                    await new ObjectResult(unAuthorizedResponse).ExecuteResultAsync(
                        new ActionContext { HttpContext = context }
                    );
                    return;
                }

                var success = int.TryParse(idClaim.Value, out int parsedId);
                if (!success)
                {
                    context.Response.StatusCode = 401;
                    await new ObjectResult(unAuthorizedResponse).ExecuteResultAsync(
                        new ActionContext { HttpContext = context }
                    );
                    return;
                }

                try
                {
                    var user = await service.GetAccount(parsedId);
                    // laitetaan sisäänkirjautunut käyttäjä osaksi httpcontextia
                    context.Items["loggedInUser"] = user;
                }
                catch (Exception)
                {
                    context.Response.StatusCode = 401;
                    await new ObjectResult(unAuthorizedResponse).ExecuteResultAsync(
                        new ActionContext { HttpContext = context }
                    );
                    return;
                }
            }
        }
        // kaikki tarkistukset hypätään yli, jos Authorize-attribute ei ole käytössä

        // mennään eteenpäin
        await next(context);
    }
}
