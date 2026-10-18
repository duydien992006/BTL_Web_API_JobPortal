using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Json;


namespace Common.Extensions
{
    public static class MvcExtensions
    {
        public static IMvcBuilder AddJobPortalMvc(this IMvcBuilder builder)
        {
            // Sửa lỗi gốc "The X field is required": không tự coi string không có dấu ? là bắt buộc.
            // Trường nào bắt buộc thì ghi [Required] rõ ràng trên Model
            builder.AddMvcOptions(o => o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true);

            // Mọi DateTime vào/ra API đều được chuẩn hóa UTC
            builder.AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter()));

            return builder;
        }
    }
}
