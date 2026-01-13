using System;
using System.Globalization;

namespace Gizmo.Client.UI
{
    public class StringConverter<TValue> : Converter<TValue, string>
    {
        public TValue GetValue(string value)
        {
            ClearGetError();

            try
            {
                if (typeof(TValue) == typeof(string))
                    return (TValue)(object)value;

                if (string.IsNullOrEmpty(value))
                    return default(TValue);

                else if (typeof(TValue) == typeof(char) || typeof(TValue) == typeof(char?))
                {
                    //TODO: A FAIL IF MORE THAN 1.
                    return (TValue)(object)value[0];
                }

                // bool
                else if (typeof(TValue) == typeof(bool) || typeof(TValue) == typeof(bool?))
                {
                    var lowerValue = value.ToLowerInvariant();
                    if (lowerValue == "true")
                        return (TValue)(object)true;
                    if (lowerValue == "false")
                        return (TValue)(object)false;
                    UpdateGetError("The field should be boolean.");
                }

                // short
                else if (typeof(TValue) == typeof(short) || typeof(TValue) == typeof(short?))
                {
                    if (short.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return (TValue)(object)parsedValue;
                    UpdateGetError("The field should be an integer number.");
                }

                // ushort
                else if (typeof(TValue) == typeof(ushort) || typeof(TValue) == typeof(ushort?))
                {
                    if (ushort.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return (TValue)(object)parsedValue;
                    UpdateGetError("The field should be an integer number.");
                }

                // int
                else if (typeof(TValue) == typeof(int) || typeof(TValue) == typeof(int?))
                {
                    if (int.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return (TValue)(object)parsedValue;
                    UpdateGetError("The field should be an integer number.");
                }

                // uint
                else if (typeof(TValue) == typeof(uint) || typeof(TValue) == typeof(uint?))
                {
                    if (uint.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return (TValue)(object)parsedValue;
                    UpdateGetError("The field should be an integer number.");
                }

                // long
                else if (typeof(TValue) == typeof(long) || typeof(TValue) == typeof(long?))
                {
                    if (long.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return (TValue)(object)parsedValue;
                    UpdateGetError("The field should be an integer number.");
                }

                // ulong
                else if (typeof(TValue) == typeof(ulong) || typeof(TValue) == typeof(ulong?))
                {
                    if (ulong.TryParse(value, NumberStyles.Integer, Culture, out var parsedValue))
                        return (TValue)(object)parsedValue;
                    UpdateGetError("The field should be an integer number.");
                }

                // float
                else if (typeof(TValue) == typeof(float) || typeof(TValue) == typeof(float?))
                {
                    if (float.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                        return (TValue)(object)parsedValue;
                    UpdateGetError("The field should be a number.");
                }

                // double
                else if (typeof(TValue) == typeof(double) || typeof(TValue) == typeof(double?))
                {
                    if (double.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                        return (TValue)(object)parsedValue;
                    UpdateGetError("The field should be a number.");
                }

                // decimal
                else if (typeof(TValue) == typeof(decimal) || typeof(TValue) == typeof(decimal?))
                {
                    if (InvariantDecimalPoint)
                    {
                        var tmpValue = value.Replace(',', '.');
                        if (decimal.TryParse(tmpValue, NumberStyles.Any, CultureInfo.InvariantCulture, out var parsedValue))
                            return (TValue)(object)parsedValue;
                        UpdateGetError("The field should be a number.");
                    }
                    else
                    {
                        if (decimal.TryParse(value, NumberStyles.Any, Culture, out var parsedValue))
                            return (TValue)(object)parsedValue;
                        UpdateGetError("The field should be a number.");
                    }
                }

                // DateTime
                else if (typeof(TValue) == typeof(DateTime) || typeof(TValue) == typeof(DateTime?))
                {
                    if (DateTime.TryParseExact(value, Format, Culture, DateTimeStyles.None, out var parsedValue))
                        return (TValue)(object)parsedValue;
                    UpdateGetError("The field should be a date.");
                }

                // guid
                else if (typeof(TValue) == typeof(Guid) || typeof(TValue) == typeof(Guid?))
                {
                    if (Guid.TryParse(value, out var parsedValue))
                        return (TValue)(object)parsedValue;
                    UpdateGetError("The field should be a GUID.");
                }
                else
                {
                    UpdateGetError($"Conversion to type {typeof(TValue)} is not implemented.");
                }
            }
            catch (Exception ex)
            {
                UpdateGetError("Conversion error: " + ex.Message);
            }

            return default(TValue);
        }

        public string SetValue(TValue arg)
        {
            ClearSetError();

            if (arg == null)
                return null;

            try
            {
                // string
                if (typeof(TValue) == typeof(string))
                    return (string)(object)arg;

                // char
                if (typeof(TValue) == typeof(char))
                    return ((char)(object)arg).ToString(Culture);
                if (typeof(TValue) == typeof(char?))
                    return ((char?)(object)arg).Value.ToString(Culture);

                // bool
                if (typeof(TValue) == typeof(bool))
                    return ((bool)(object)arg).ToString(CultureInfo.InvariantCulture);
                if (typeof(TValue) == typeof(bool?))
                    return ((bool?)(object)arg).Value.ToString(CultureInfo.InvariantCulture);

                // short
                if (typeof(TValue) == typeof(short))
                    return ((short)(object)arg).ToString(Format, Culture);
                if (typeof(TValue) == typeof(short?))
                    return ((short?)(object)arg).Value.ToString(Format, Culture);

                // ushort
                if (typeof(TValue) == typeof(ushort))
                    return ((ushort)(object)arg).ToString(Format, Culture);
                if (typeof(TValue) == typeof(ushort?))
                    return ((ushort?)(object)arg).Value.ToString(Format, Culture);

                // int
                else if (typeof(TValue) == typeof(int))
                    return ((int)(object)arg).ToString(Format, Culture);
                else if (typeof(TValue) == typeof(int?))
                    return ((int?)(object)arg).Value.ToString(Format, Culture);

                // uint
                else if (typeof(TValue) == typeof(uint))
                    return ((uint)(object)arg).ToString(Format, Culture);
                else if (typeof(TValue) == typeof(uint?))
                    return ((uint?)(object)arg).Value.ToString(Format, Culture);

                // long
                else if (typeof(TValue) == typeof(long))
                    return ((long)(object)arg).ToString(Format, Culture);
                else if (typeof(TValue) == typeof(long?))
                    return ((long?)(object)arg).Value.ToString(Format, Culture);

                // ulong
                else if (typeof(TValue) == typeof(ulong))
                    return ((ulong)(object)arg).ToString(Format, Culture);
                else if (typeof(TValue) == typeof(ulong?))
                    return ((ulong?)(object)arg).Value.ToString(Format, Culture);

                // float
                else if (typeof(TValue) == typeof(float))
                    return ((float)(object)arg).ToString(Format, Culture);
                else if (typeof(TValue) == typeof(float?))
                    return ((float?)(object)arg).Value.ToString(Format, Culture);

                // double
                else if (typeof(TValue) == typeof(double))
                    return ((double)(object)arg).ToString(Format, Culture);
                else if (typeof(TValue) == typeof(double?))
                    return ((double?)(object)arg).Value.ToString(Format, Culture);

                // decimal
                else if (typeof(TValue) == typeof(decimal))
                    return ((decimal)(object)arg).ToString(Format, Culture);
                else if (typeof(TValue) == typeof(decimal?))
                    return ((decimal?)(object)arg).Value.ToString(Format, Culture);

                // DateTime
                else if (typeof(TValue) == typeof(DateTime))
                    return ((DateTime)(object)arg).ToString(Format, Culture);
                else if (typeof(TValue) == typeof(DateTime?))
                    return ((DateTime?)(object)arg).Value.ToString(Format, Culture);

                // guid
                else if (typeof(TValue) == typeof(Guid))
                {
                    var value = (Guid)(object)arg;
                    return value.ToString();
                }
                else if (typeof(TValue) == typeof(Guid?))
                {
                    var value = (Guid?)(object)arg;
                    return value.Value.ToString();
                }

                return arg.ToString();
            }
            catch (FormatException ex)
            {
                UpdateSetError("Conversion error: " + ex.Message);
                return null;
            }
        }
    }
}
