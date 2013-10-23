/*
 * Copyright (c) Dominick Baier, Brock Allen.  All rights reserved.
 * see LICENSE
 */

using System.IO;
using System.Reflection;
using System.Text;

namespace Thinktecture.IdentityModel.EmbeddedSts.Assets
{
    class AssetManager
    {
        static readonly string Prefix = typeof(AssetManager).Namespace + ".";
        
        public static string LoadString(string file)
        {
            return Encoding.UTF8.GetString(LoadBytes(file));
        }
        
        public static byte[] LoadBytes(string file)
        {
            using (var ms = new MemoryStream())
            {
                using (var s = Assembly.GetExecutingAssembly().GetManifestResourceStream(Prefix + file))
                {
                    s.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
    }
}
