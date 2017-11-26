using System.Collections.Generic;
using Nancy;
using Microsoft.AspNetCore.Http.Authentication;
using Nancy.Owin;
using System;

namespace NancyMusicStore.Common
{
    public static class Extensions
    {
        public static string GetUserName(this Nancy.NancyContext context) => context.CurrentUser?.FindFirst(_ => _.Type == "name")?.Value;
        public static string GetFirstName(this Nancy.NancyContext context) => context.CurrentUser?.FindFirst(_ => _.Type == "given_name")?.Value;
        public static string GetLastName(this Nancy.NancyContext context) => context.CurrentUser?.FindFirst(_ => _.Type == "family_name")?.Value;

    }
}