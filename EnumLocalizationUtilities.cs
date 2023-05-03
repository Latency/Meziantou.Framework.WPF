using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Meziantou.Framework.WPF;

internal static class EnumLocalizationUtilities
{
    private static readonly IDictionary<Type, LocalizedEnumValueCollection> EnumsCache      = new Dictionary<Type, LocalizedEnumValueCollection>();
    private static readonly IDictionary<Expression, object>                 PropertiesCache = new Dictionary<Expression, object>();

    public static LocalizedEnumValueCollection GetEnumLocalization<T>()
        where T : struct => GetEnumLocalization(typeof(T));

    public static LocalizedEnumValueCollection GetEnumLocalization(Type type)
    {
        if (EnumsCache.TryGetValue(type, out var value))
            return value;

        IList<LocalizedEnumValue> result = new List<LocalizedEnumValue>();
        var enumValues = type.GetEnumValues();

        foreach (Enum? enumValue in enumValues)
        {
            if (enumValue is null)
                throw new NullReferenceException();

            var enumName = enumValue.ToString()!;
            var fieldInfo = type.GetField(enumName)!;

            var displayAttribute = fieldInfo.GetCustomAttribute<DisplayAttribute>();
            if (displayAttribute != null)
            {
                result.Add(new LocalizedEnumValue(enumValue, displayAttribute));
            }
            else
            {
                result.Add(new LocalizedEnumValue(enumValue, enumName));
            }
        }

        var localizedValueCollection = new LocalizedEnumValueCollection(result);
        EnumsCache.Add(type, localizedValueCollection);
        return localizedValueCollection;
    }

    public static string? GetPropertyLocalization<T>(Expression<Func<T>> exp)
    {
        if (!PropertiesCache.TryGetValue(exp, out var value))
        {
            var memberExpression = (MemberExpression)exp.Body;
            var displayAttribute = memberExpression.Member.GetCustomAttribute<DisplayAttribute>();
            value = displayAttribute == null ? memberExpression.Member.Name : displayAttribute.GetName();

            PropertiesCache.Add(exp, value);
        }

        if (value is DisplayAttribute attribute)
            return attribute.GetName();

        return value.ToString();
    }

    public static string GetEnumMemberLocalization(Enum value)
    {
        var localizedValueCollection = GetEnumLocalization(value.GetType());
        return localizedValueCollection[value].Name;
    }
}
