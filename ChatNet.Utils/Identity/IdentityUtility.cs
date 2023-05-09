using ChatNet.Data.Models;
using System.Security.Claims;
using System.Security.Principal;

namespace ChatNet.Utils.Identity
{
    public static class IdentityUtility
    {
        /// <summary>
        /// Gives back general authenticated user data
        /// </summary>
        /// <param name="identity">User identity</param>
        /// <returns></returns>
        public static IdentityUserData? GetIdentityUserData(IIdentity? identity)
        {
            if (identity is not ClaimsIdentity claimsIdentity)
                return null;

            var data = new IdentityUserData
            {
                IsAuthenticated = identity.IsAuthenticated
            };

            foreach (var claim in claimsIdentity.Claims)
            {
                switch (claim.Type)
                {
                    case ClaimTypes.NameIdentifier:
                        if (int.TryParse(claim.Value, out int id))
                            data.Id = id;
                        break;
                    case ClaimTypes.GivenName:
                        data.FirstName = claim.Value;
                        break;
                    case ClaimTypes.Surname:
                        data.LastName = claim.Value;
                        break;
                    case ClaimTypes.Name:
                        data.FullName = claim.Value;
                        break;
                    default:
                        if (claim.Type.EndsWith("username"))
                            data.Username = claim.Value;
                        break;
                }
            }

            return data;
        }
    }
}
