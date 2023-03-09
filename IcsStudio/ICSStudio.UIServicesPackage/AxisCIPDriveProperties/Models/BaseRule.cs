namespace ICSStudio.UIServicesPackage.AxisCIPDriveProperties.Models
{
    public class BaseRule
    {
        public string Name { get; set; }
        public bool Value { get; set; }

        public static void ApplyReadonlyRule(object obj, BaseRule[] rules)
        {
            if (obj == null || rules == null || rules.Length == 0)
                return;

            foreach (var rule in rules)
            {
                PropertySetting.SetPropertyReadOnly(obj, rule.Name, rule.Value);
            }

        }

        public static void ApplyVisibilityRule(object obj, BaseRule[] rules)
        {
            if (obj == null || rules == null || rules.Length == 0)
                return;

            foreach (var rule in rules)
            {
                PropertySetting.SetPropertyVisibility(obj, rule.Name, rule.Value);
            }
        }
    }
}
