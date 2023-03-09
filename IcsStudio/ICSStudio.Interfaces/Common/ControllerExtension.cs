namespace ICSStudio.Interfaces.Common
{
    public static class ControllerExtension
    {
        public static bool HasMotionGroup(this IController controller)
        {
            if (controller == null)
                return false;

            foreach (var tag in controller.Tags)
            {
                if (tag.DataTypeInfo.DataType.IsMotionGroupType)
                    return true;
            }

            return false;
        }
    }
}
