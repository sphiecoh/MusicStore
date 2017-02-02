using Nancy;
using Nancy.Authentication.Forms;
using NancyMusicStore.Common;
using NancyMusicStore.Models;
using System;
using System.Data;
using System.Security.Claims;
using System.Security.Principal;

namespace NancyMusicStore
{
    internal class UserMapper : IUserMapper
    {
        private readonly IDbHelper _dbHelper;
        public UserMapper(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }

        public ClaimsPrincipal GetUserFromIdentifier(Guid identifier, NancyContext context)
        {
            const string cmd = "public.get_user_by_userid";
            var user = _dbHelper.QueryFirstOrDefault<SysUser>(cmd, new
            {
                uid = identifier.ToString()
            }, null, null, CommandType.StoredProcedure);

            return user == null
                       ? null
                       : new ClaimsPrincipal(new GenericIdentity(user.SysUserName));
        }
    }
}