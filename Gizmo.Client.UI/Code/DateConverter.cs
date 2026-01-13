using System;

namespace Gizmo.Client.UI
{
    public class DateConverter<TValue> : Converter<TValue, string>
    {
        public TValue GetValue(DateTime? value)
        {
            ClearGetError();

            try
            {
                if (value == null)
                    return default(TValue);

                if (typeof(TValue) == typeof(string))
                    return (TValue)(object)value.Value.ToString(); //TODO: A FORMAT

                // DateTime
                else if (typeof(TValue) == typeof(DateTime) || typeof(TValue) == typeof(DateTime?))
                {
                    return (TValue)(object)value.Value;
                }

                // TimeSpan
                else if (typeof(TValue) == typeof(TimeSpan) || typeof(TValue) == typeof(TimeSpan?))
                {
                    return (TValue)(object)value.Value.Subtract(DateTime.MinValue);
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

        public DateTime? SetValue(TValue arg)
        {
            ClearSetError();

            if (arg == null)
                return null;

            try
            {
                // string
                if (typeof(TValue) == typeof(string))
                {
                    if (string.IsNullOrEmpty((string)(object)arg))
                    {
                        return null;
                    }
                    else
                    {
                        return DateTime.Parse((string)(object)arg); //TODO: A FORMAT
                    }
                }
                // DateTime
                else if (typeof(TValue) == typeof(DateTime))
                {
                    return (DateTime)(object)arg;
                }
                else if (typeof(TValue) == typeof(DateTime?))
                {
                    return (DateTime?)(object)arg;
                }
                // TimeSpan
                else if (typeof(TValue) == typeof(TimeSpan))
                {
                    TimeSpan ts = (TimeSpan)(object)arg;
                    return new DateTime(ts.Ticks);
                }
                else if (typeof(TValue) == typeof(TimeSpan?))
                {
                    TimeSpan? ts = (TimeSpan?)(object)arg;
                    return new DateTime(ts.Value.Ticks);
                }

                UpdateSetError($"Conversion to type {typeof(TValue)} is not implemented.");
                return null;
            }
            catch (FormatException ex)
            {
                UpdateSetError("Conversion error: " + ex.Message);
                return null;
            }
        }
    }
}
