namespace CampersHub.Server.Common.Helpers
{
    public static class PropertyHelper
    {
        public static object GetPropertyValue(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);

            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(obj);
            }

            return null;
        }
    }
}
