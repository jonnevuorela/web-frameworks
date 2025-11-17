using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.CustomExceptions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace API.Policies.Handlers
{
    namespace API.Policies.Handlers
    {
        public class XpAuthorizationHandler(IUserService _userService)
            : AuthorizationHandler<XpRequirement>
        {
            // HandleRequirementAsync tulee base-luokasta
            protected override async Task HandleRequirementAsync(
                AuthorizationHandlerContext context,
                XpRequirement requirement
            )
            {
                // context.User sisältää jwt:n claimit
                // jos jwt sisältää claimin XP,
                // mennään eteenpäin

                var idClaim = context.User.FindFirst(c => c.Type == "sub");

                if (idClaim == null)
                {
                    return;
                }

                try
                {
                    var loggedInUser = await _userService.GetAccount(int.Parse(idClaim.Value));

                    if (requirement.Xp < loggedInUser.Xp)
                    {
                        context.Succeed(requirement);
                    }
                }
                catch (NotFoundException)
                {
                    return;
                }
                catch (Exception)
                {
                    return;
                }
            }
        }
    }
}
