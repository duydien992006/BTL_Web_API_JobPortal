using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Common.Json
{
    public class UtcDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return ChuanHoaUtc(reader.GetDateTime());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(ChuanHoaUtc(value));   // Kind = Utc nên luôn ghi kèm "Z"
        }

        private static DateTime ChuanHoaUtc(DateTime v)
        {
            switch (v.Kind)
            {
                case DateTimeKind.Utc: return v;
                case DateTimeKind.Local: return v.ToUniversalTime();      // chuỗi có offset +07:00...
                default: return DateTime.SpecifyKind(v, DateTimeKind.Utc); // không có múi giờ -> coi là UTC
            }
        }
    }
}
