using System;
using System.Net.NetworkInformation;

namespace Shared.Utilties
{
    public static class Guard
    {
        public static Guid CorrelationTokenValidation(ref string tokenValue)
        {
            // tokenValue == "00000000-0000-0000-0000-000000000000"
            if (string.IsNullOrEmpty(tokenValue))
            {
                return Guid.NewGuid();
            }
            else if (tokenValue == "00000000-0000-0000-0000-000000000000")
            {
                return Guid.NewGuid();
            }

            if (Guid.TryParse(tokenValue, out Guid correlationToken))
            {
                return correlationToken;
            }
            {
                throw new ArgumentException("CorrelationToken is not a valid Guid", nameof(tokenValue));
            }
        }

        public static void ForInvalidGuid(string guidString)
        {
            // tokenValue == "00000000-0000-0000-0000-000000000000"
            if (string.IsNullOrEmpty(guidString) ||
                guidString == "00000000-0000-0000-0000-000000000000" ||
                !Guid.TryParse(guidString, out Guid dummyValue))
            {
                throw new ArgumentException("Guid Value is incorrect", nameof(guidString));
            }
        }

        public static void ForLessEqualZero(int value, string parameterName)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} cannot be equal to ZERO");
        }

        public static void ForLessEqualZero(decimal value, string parameterName)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} cannot be equal to ZERO");
        }

        public static void ForLessEqualZero(long value, string parameterName)
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} cannot be equal to ZERO");
        }

        public static void MustBeEqualToZero(int value, string parameterName)
        {
            if (value != 0)
                throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} must be assigned value of ZERO");
        }

        public static string DeafultToValue(string value, string defaultValue)
        {
            if (string.IsNullOrEmpty(value))
                return defaultValue;

            return value;
        }
        public static void ForNullOrEmpty(string value, string parameterName)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} cannot be Null or Empty");
        }

        public static void ForValidGuid(Guid value, string parameterName)
        {
            if (value == Guid.Empty)
                throw new ArgumentOutOfRangeException(parameterName, $"{parameterName} cannot be Empty");
        }

        public static void ForNullObject(object target, string parameterName)
        {
            if (target == null)
                throw new ArgumentNullException(parameterName, $"{parameterName} cannot be Null or Empty");
        }

        /// <summary>
        /// Test whether objects are Null or DBNull
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrDbNull(this object obj)
        {
            return obj == null
                   || obj.GetType() == typeof(DBNull);
        }


        /// <summary>
        /// Capitalizes first character of string and lower-cases the rest
        /// </summary>
        /// <param>TargetString to transform</param>
        /// <returns>Transformed string</returns>
        public static string TidyCase(string sourceStr)
        {
            sourceStr.Trim();
            if (!string.IsNullOrEmpty(sourceStr))
            {
                char[] allCharacters = sourceStr.ToCharArray();

                for (int i = 0; i < allCharacters.Length; i++)
                {
                    char character = allCharacters[i];
                    if (i == 0)
                    {
                        if (char.IsLower(character))
                        {
                            allCharacters[i] = char.ToUpper(character);
                        }
                    }
                    else
                    {
                        if (char.IsUpper(character))
                        {
                            allCharacters[i] = char.ToLower(character);
                        }
                    }
                }
                return new string(allCharacters);
            }
            return sourceStr;
        }
    }
}