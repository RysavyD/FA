using _3F.Model.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace _3F.Web.Extensions
{
    public static class EnumExtension
    {
        public static string Description(this RelationshipStatus status)
        {
            switch (status)
            {
                case RelationshipStatus.Divorced: return "Rozvedený(-á)";
                case RelationshipStatus.Engaged: return "Zadaný(-á)";
                case RelationshipStatus.Married: return "Ženatý / Vdaná";
                case RelationshipStatus.Single: return "Nezadaný(-á)";
                case RelationshipStatus.Undefined: return "Nechci uvést";
                case RelationshipStatus.Widower: return "Vdovec / vdova";
                default: return "Nechci uvést";
            }
        }
    }
}