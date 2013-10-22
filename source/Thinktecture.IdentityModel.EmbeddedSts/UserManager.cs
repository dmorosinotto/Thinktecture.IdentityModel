using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Script.Serialization;

namespace Thinktecture.IdentityModel.EmbeddedSts
{
    class User
    {
        public string Name { get; set; }
        public UserClaim[] Claims { get; set; }

    }

    class UserClaim
    {
        public string Type { get; set; }
        public string Value { get; set; }
    }

    class UserManager
    {
        public static string[] GetAllUserNames()
        {
            var users = GetAllUsers();
            return users.Select(x => x.Name).Distinct().ToArray();
        }

        public static IEnumerable<Claim> GetClaimsForUser(string name)
        {
            var users = GetAllUsers();
            var user = users.FirstOrDefault(x => x.Name == name);
            var claims = user.Claims.Select(x => new Claim(x.Type, x.Value)).ToList();
            return claims;
        }

        static IEnumerable<User> GetAllUsers()
        {
            var ser = new JavaScriptSerializer();
            return (User[])ser.Deserialize(ReadClaimsFile(), typeof(User[]));
        }

        static string ClaimsFilePath
        {
            get
            {
                return Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "App_Data\\EmbeddedStsUsers.json");
            }
        }

        static string ReadClaimsFile()
        {
            using (var sw = File.OpenText(ClaimsFilePath))
            {
                return sw.ReadToEnd();
            }
        }

        internal static void WriteClaimsFile()
        {
            var path = ClaimsFilePath;
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            if (!File.Exists(path))
            {
                using (var sw = File.CreateText(path))
                {
                    var ser = new JavaScriptSerializer();
                    var json = ser.Serialize(GetDefaultUsers());
                    sw.Write(json);
                    sw.Flush();
                }
            }
        }

        static User[] GetDefaultUsers()
        {
            return new User[]
            {
                new User
                {
                    Name = "Alice", 
                    Claims = new UserClaim[]
                    {
                        new UserClaim
                        {
                            Type = ClaimTypes.NameIdentifier,
                            Value = "alice"
                        },
                        new UserClaim
                        {
                            Type = ClaimTypes.Name,
                            Value = "Alice"
                        },
                        new UserClaim
                        {
                            Type = ClaimTypes.Email,
                            Value = "alice@alice.com"
                        }
                    }
                },
                new User
                {
                    Name = "Bob", 
                    Claims = new UserClaim[]
                    {
                        new UserClaim
                        {
                            Type = ClaimTypes.NameIdentifier,
                            Value = "bob"
                        },
                        new UserClaim
                        {
                            Type = ClaimTypes.Name,
                            Value = "Bob"
                        },
                        new UserClaim
                        {
                            Type = ClaimTypes.Email,
                            Value = "bob@bob.com"
                        }
                    }
                }
            };
        }
    }
}
