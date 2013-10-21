using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
