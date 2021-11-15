using System.Collections.Generic;

namespace ELMS.Application.Constants
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.Create",
                $"Permissions.{module}.View",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete",
            };
        }

        public static class Dashboard
        {
            public const string View = "Permissions.Dashboard.View";
            public const string Create = "Permissions.Dashboard.Create";
            public const string Edit = "Permissions.Dashboard.Edit";
            public const string Delete = "Permissions.Dashboard.Delete";
        }

        public static class Courses
        {
            public const string View = "Permissions.Courses.View";
            public const string Create = "Permissions.Courses.Create";
            public const string Edit = "Permissions.Courses.Edit";
            public const string Delete = "Permissions.Courses.Delete";
        }

        public static class Users
        {
            public const string View = "Permissions.Users.View";
            public const string Create = "Permissions.Users.Create";
            public const string Edit = "Permissions.Users.Edit";
            public const string Delete = "Permissions.Users.Delete";
        }

        public static class Lecture
        {
            public const string View = "Permissions.Lecture.View";
            public const string Create = "Permissions.Lecture.Create";
            public const string Edit = "Permissions.Lecture.Edit";
            public const string Delete = "Permissions.Lecture.Delete";
        }
    }
}