using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using ConsInterfeces.Rup2ConsImportContentSystemData;

public static class ImportContentSystemDataRequestDumper
{
    public static string DumpImportContentSystemDataRequest(ImportContentSystemDataRequest request)
    {
        var lines = new List<string>();
        var visited = new HashSet<object>(ReferenceEqualityComparer.Instance);

        WriteObject(
            request,
            string.Empty,
            lines,
            visited,
            null,
            false);

        return string.Join(Environment.NewLine, lines);
    }

    private static void WriteObject(
        object obj,
        string path,
        List<string> lines,
        HashSet<object> visited,
        PropertyInfo ownerProperty,
        bool maskSensitive)
    {
        if (obj == null)
        {
            if (!string.IsNullOrEmpty(path))
            {
                lines.Add(path + " = null");
            }
            return;
        }

        Type type = obj.GetType();

        if (IsSimpleType(type))
        {
            string valueText = maskSensitive ? "****" : FormatSimpleValue(obj);
            lines.Add(path + " = " + valueText);
            return;
        }

        if (!type.IsValueType)
        {
            if (visited.Contains(obj))
            {
                if (!string.IsNullOrEmpty(path))
                {
                    lines.Add(path + " = [CYCLIC_REFERENCE]");
                }
                return;
            }

            visited.Add(obj);
        }

        if (obj is IEnumerable enumerable && !(obj is string))
        {
            WriteEnumerable(enumerable, path, lines, visited, ownerProperty);
            return;
        }

        PropertyInfo[] properties = type
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(ShouldSerializeProperty)
            .OrderBy(GetOrder)
            .ThenBy(p => p.Name)
            .ToArray();

        foreach (PropertyInfo prop in properties)
        {
            string soapName = GetSoapPropertyName(prop);
            string childPath = Combine(path, soapName);
            bool shouldMask = IsSensitive(prop);

            object value;
            try
            {
                value = prop.GetValue(obj, null);
            }
            catch
            {
                lines.Add(childPath + " = [UNREADABLE]");
                continue;
            }

            if (value == null)
            {
                lines.Add(childPath + " = null");
                continue;
            }

            WriteObject(
                value,
                childPath,
                lines,
                visited,
                prop,
                shouldMask);
        }
    }

    private static void WriteEnumerable(
        IEnumerable enumerable,
        string path,
        List<string> lines,
        HashSet<object> visited,
        PropertyInfo ownerProperty)
    {
        int index = 0;
        bool hasAny = false;

        foreach (object item in enumerable)
        {
            hasAny = true;
            string indexedPath = path + "[" + index + "]";

            if (item == null)
            {
                lines.Add(indexedPath + " = null");
            }
            else
            {
                WriteObject(
                    item,
                    indexedPath,
                    lines,
                    visited,
                    null,
                    false);
            }

            index++;
        }

        if (!hasAny)
        {
            lines.Add(path + " = [EMPTY]");
        }
    }

    private static bool ShouldSerializeProperty(PropertyInfo prop)
    {
        if (prop == null || !prop.CanRead)
            return false;

        if (prop.GetIndexParameters().Length > 0)
            return false;

        if (string.Equals(prop.Name, "PropertyChanged", StringComparison.Ordinal))
            return false;

        if (typeof(Delegate).IsAssignableFrom(prop.PropertyType))
            return false;

        if (HasXmlIgnore(prop))
            return false;

        return true;
    }

    private static bool HasXmlIgnore(PropertyInfo prop)
    {
        return prop.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Length > 0;
    }

    private static string GetSoapPropertyName(PropertyInfo prop)
    {
        XmlElementAttribute xmlElement = prop
            .GetCustomAttributes(typeof(XmlElementAttribute), true)
            .Cast<XmlElementAttribute>()
            .FirstOrDefault();

        if (xmlElement != null && !string.IsNullOrEmpty(xmlElement.ElementName))
            return xmlElement.ElementName;

        XmlArrayAttribute xmlArray = prop
            .GetCustomAttributes(typeof(XmlArrayAttribute), true)
            .Cast<XmlArrayAttribute>()
            .FirstOrDefault();

        if (xmlArray != null && !string.IsNullOrEmpty(xmlArray.ElementName))
            return xmlArray.ElementName;

        return prop.Name;
    }

    private static int GetOrder(PropertyInfo prop)
    {
        XmlElementAttribute xmlElement = prop
            .GetCustomAttributes(typeof(XmlElementAttribute), true)
            .Cast<XmlElementAttribute>()
            .FirstOrDefault();

        if (xmlElement != null)
            return xmlElement.Order;

        XmlArrayAttribute xmlArray = prop
            .GetCustomAttributes(typeof(XmlArrayAttribute), true)
            .Cast<XmlArrayAttribute>()
            .FirstOrDefault();

        if (xmlArray != null)
            return xmlArray.Order;

        return int.MaxValue;
    }

    private static bool IsSensitive(PropertyInfo prop)
    {
        return string.Equals(prop.Name, "Haslo", StringComparison.OrdinalIgnoreCase)
            || string.Equals(GetSoapPropertyName(prop), "Haslo", StringComparison.OrdinalIgnoreCase);
    }

    private static string Combine(string parent, string child)
    {
        if (string.IsNullOrEmpty(parent))
            return child;

        return parent + "." + child;
    }

    private static bool IsSimpleType(Type type)
    {
        type = Nullable.GetUnderlyingType(type) ?? type;

        return type.IsPrimitive
            || type.IsEnum
            || type == typeof(string)
            || type == typeof(decimal)
            || type == typeof(DateTime)
            || type == typeof(DateTimeOffset)
            || type == typeof(TimeSpan)
            || type == typeof(Guid);
    }

    private static string FormatSimpleValue(object value)
    {
        if (value == null)
            return "null";

        bool boolValue;
        if (value is bool)
        {
            boolValue = (bool)value;
            return boolValue ? "true" : "false";
        }

        IFormattable formattable = value as IFormattable;
        if (formattable != null)
            return formattable.ToString(null, CultureInfo.InvariantCulture);

        return value.ToString();
    }

    private sealed class ReferenceEqualityComparer : IEqualityComparer<object>
    {
        public static readonly ReferenceEqualityComparer Instance = new ReferenceEqualityComparer();

        public new bool Equals(object x, object y)
        {
            return ReferenceEquals(x, y);
        }

        public int GetHashCode(object obj)
        {
            return RuntimeHelpers.GetHashCode(obj);
        }
    }
}