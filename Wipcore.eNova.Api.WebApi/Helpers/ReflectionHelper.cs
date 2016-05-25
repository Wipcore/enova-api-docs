using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Wipcore.Core.SessionObjects;
using Wipcore.Enova.Core;

namespace Wipcore.Enova.Api.WebApi.Helpers
{
    public static class ReflectionHelper
    {
        /// <summary>
        /// Returns the most derived type, if more then one type is found it will prioritize types found outside of enova.
        /// </summary>
        /// <param name="type">Dervied from this type.</param>
        /// <returns></returns>
        public static Type GetMostDerivedEnovaType(this Type type)
        {
            var derivedType = type.GetMostDerivedTypes(ReflectionHelper.GetAllAvailableTypes()).OrderBy<Type, int>(
                x =>
                {
                    if (x.Namespace == typeof(BaseObject).Namespace)
                        return 1000;
                    
                    if (x.Namespace == typeof(EnovaBaseProduct).Namespace)
                        return 100;
                    
                    if (x.Namespace == "Wipcore.WebFoundation.Core")
                        return 50;

                    return 0;
                }).FirstOrDefault();

            //special handling for derived base-enova-product
            return derivedType == typeof (EnovaBook) ? typeof (EnovaBaseProduct) : derivedType;
        }


        /// <summary>
        /// Get all types in appdomain
        /// </summary>
        public static IEnumerable<Type> GetAllAvailableTypes()
        {
            return AppDomain.CurrentDomain.GetAssembliesSafe().SelectMany(assembly => assembly.GetExportedTypes());
        }

        /// <summary>
        /// Get most derived type from all available types
        /// </summary>
        public static Type GetMostDerivedType(this Type type)
        {
            return type.GetMostDerivedType(ReflectionHelper.GetAllAvailableTypes());
        }

        /// <summary>
        /// Get most derived type in list types
        /// </summary>
        public static Type GetMostDerivedType(this Type type, IEnumerable<Type> types)
        {
            return type.GetMostDerivedTypes(types).FirstOrDefault();
        }

        /// <summary>
        /// Checks if a type is nullable
        /// WARNING will only work with typeof(int) not o.GetType() read more at http://msdn.microsoft.com/en-us/library/ms366789.aspx
        /// </summary>
        public static bool IsNullable(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type GetNullableType(this Type type)
        {
            if (type.IsNullable()) return type.GetGenericArguments()[0];
            return type;
        }

        /// <summary>
        /// Get most derived types in list types. i.e. if two classes inherits the same class.
        /// </summary>
        public static IEnumerable<Type> GetMostDerivedTypes(this Type type, IEnumerable<Type> types)
        {
            var mostDerivedTypes = types.OrderByDescending(t => t.DerivedSteps(type)).ToList();
            var mostDerivedType = mostDerivedTypes.FirstOrDefault();
            if (mostDerivedType == null) return new List<Type>();
            var derivedSteps = mostDerivedType.DerivedSteps(type);
            if (derivedSteps == -1) return new List<Type>();
            return mostDerivedTypes.TakeWhile(t => t.DerivedSteps(type) == derivedSteps).ToList();
        }

        /// <summary>
        /// Get number of inheritances to the basetype
        /// </summary>
        public static int DerivedSteps(this Type type, Type baseType)
        {
            if (!baseType.IsAssignableFrom(type))
                return -1;

            if (baseType.Equals(type))
                return 0;

            return type.BaseType.DerivedSteps(baseType) + 1;
        }

        private static List<string> ExceptionDlls = new List<string>();

        public static Assembly[] GetAssembliesSafe(this AppDomain domain, bool includeSystemDlls = false)
        {
            return domain.GetAssemblies().Where(a =>
            {
                try
                {
                    if (ExceptionDlls.Contains(a.FullName)) return false;
                    if (a.FullName.StartsWith("System.") && !includeSystemDlls) return false;

                    if (a.ManifestModule.GetType().Namespace != "System.Reflection.Emit")
                    {
                        var t = a.GetExportedTypes();
                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
                catch (Exception e)
                {
                    if (!ExceptionDlls.Contains(a.FullName))
                        ExceptionDlls.Add(a.FullName);
                    Trace.WriteLine("GetAssembliesSafe:" + e);
                }
                return false;

            }).ToArray();
        }
    }
    
}