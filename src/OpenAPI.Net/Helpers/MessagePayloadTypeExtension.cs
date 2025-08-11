using Google.Protobuf;
using System;
using System.Reflection;

namespace OpenAPI.Net.Helpers
{
    public static class MessagePayloadTypeExtension
    {
        /// <summary>
        /// This method returns the payload type of a message
        /// </summary>
        /// <typeparam name="T">IMessage</typeparam>
        /// <param name="message">The message</param>
        /// <returns>uint (Payload Type)</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static uint GetPayloadType<T>(this T message) where T : IMessage
        {
            PropertyInfo property;

            try
            {
                property = message.GetType().GetProperty("PayloadType");
            }
            catch (Exception ex) when (ex is AmbiguousMatchException || ex is ArgumentNullException)
            {
                throw new InvalidOperationException($"Couldn't get the PayloadType of the message {message}", ex);
            }

            return Convert.ToUInt32(property.GetValue(message));
        }

        /// <summary>
        /// Sets a field value on a Protobuf message by field name if the field exists.
        /// </summary>
        /// <typeparam name="T">The message type</typeparam>
        /// <param name="message">The message instance</param>
        /// <param name="fieldName">The name of the field to set</param>
        /// <param name="value">The value to set</param>
        /// <returns>True if the field was set, false if the field doesn't exist</returns>
        public static bool SetField<T>(this IMessage<T> message, string fieldName, object value) where T : IMessage<T>
        {
            // Get the property info for the field
            var property = message.GetType().GetProperty(fieldName);
            if (property != null && property.CanWrite)
            {
                // Convert the value to the appropriate type if needed
                var convertedValue = ConvertValueToPropertyType(value, property.PropertyType);
                property.SetValue(message, convertedValue);
                return true;
            }

            return false;
        }

        private static object ConvertValueToPropertyType(object value, Type targetType)
        {
            if (value == null)
                return null;

            if (targetType.IsAssignableFrom(value.GetType()))
                return value;

            // Handle common type conversions
            if (targetType == typeof(long) && value is int intValue)
                return (long)intValue;

            if (targetType == typeof(int) && value is long longValue)
                return (int)longValue;

            // Add more conversions as needed

            // Try standard conversion
            return Convert.ChangeType(value, targetType);
        }
    }
}
