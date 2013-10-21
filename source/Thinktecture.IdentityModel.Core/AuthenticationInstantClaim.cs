/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using System;
using System.Security.Claims;
using System.Xml;
using Thinktecture.IdentityModel.Constants;

namespace Thinktecture.IdentityModel
{
    /// <summary>
    /// Helper class to create an authentication instant claim
    /// </summary>
    public static class AuthenticationInstantClaim
    {
        /// <summary>
        /// Returns an authentication instant claim for the current date/time
        /// </summary>
        /// <value>authentication instant claim.</value>
        public static Claim Now
        {
            get
            {
                return new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(DateTime.UtcNow, DateTimeFormats.Generated), ClaimValueTypes.DateTime);
            }
        }
    }
}
