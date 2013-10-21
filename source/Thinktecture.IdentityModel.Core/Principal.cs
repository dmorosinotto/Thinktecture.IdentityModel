/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace Thinktecture.IdentityModel
{
    public static class Principal
    {
        public static ClaimsPrincipal Anonymous
        {
            get
            {
                var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, "")
                    };

                var anonId = new ClaimsIdentity(claims);
                var anonPrincipal = new ClaimsPrincipal(anonId);

                return anonPrincipal;
            }
        }

        public static ClaimsPrincipal Create(string authenticationType, params Claim[] claims)
        {
            return new ClaimsPrincipal(new ClaimsIdentity(claims, authenticationType));
        }

        public static IEnumerable<Claim> CreateRoles(string[] roleNames)
        {
            if (roleNames == null || roleNames.Count() == 0)
            {
                return new Claim[] { };
            }

            return new List<Claim>(from r in roleNames select new Claim(ClaimTypes.Role, r)).ToArray();
        }
    }
}
