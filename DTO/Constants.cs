using System.ComponentModel;

namespace WebApplication1.DTO
{
    

    public static class ComplaintStatus
    {

        public enum ComplaintStatusDesc
        {
            [Description("Pending")]
            CS101,
            [Description("Resolved")]
            CS102
        }
        public static string GetStatus(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }
    }

    public static class ComplaintType
    {
        public enum ComplaintTypeDesc
        {
            [Description("Electricion")]
            Electricity,
            [Description("Carpenter")]
            Furniture,
            [Description("Plumber")]
            Plumbing
        }

        

        public static string GetWorkerType1(this Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return attribute == null ? value.ToString() : attribute.Description;
        }
        public static string[] GetComplaintType()
        {
            var enumKeys = Enum.GetNames<ComplaintTypeDesc>();
            return enumKeys;
        }

        public static ComplaintTypeDesc[] GetComplaintType1()
        {
            var enumValues = Enum.GetValues(typeof(ComplaintTypeDesc));
            return (ComplaintTypeDesc[])enumValues;
        }

        public static string GetWorkerType(string type)
        {
            switch (type.ToLower())
            {
                case "electricion":
                    return ComplaintTypeDesc.Electricity.ToString();
                case "carpenter":
                    return ComplaintTypeDesc.Furniture.ToString();
                case "plumber":
                    return ComplaintTypeDesc.Plumbing.ToString();
                default:
                    throw new ArgumentException($"Invalid complaint type: {type}");
            }
        }

    }
}
