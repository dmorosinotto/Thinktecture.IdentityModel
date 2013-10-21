/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using System.Collections.Generic;

namespace Thinktecture.IdentityModel.Tokens
{
    public static class ClaimMappings
    {
        public static Dictionary<string, string> None
        {
            get
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
