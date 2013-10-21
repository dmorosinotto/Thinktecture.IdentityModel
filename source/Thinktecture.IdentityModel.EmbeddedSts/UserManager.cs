using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Thinktecture.IdentityModel.EmbeddedSts
{
    class UserManager
    {
        public static string[] GetAllUserNames()
        {
            return new[] { "Alice", "Bob", "Eve", "Trudy" };
        }

        public static IEnumerable<Claim> GetClaimsForUser(string name)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, name));
            return claims;
        }
    }
}
