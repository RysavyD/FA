using System.ComponentModel;

namespace _3F.BusinessEntities.Enum
{
    public enum MainCategory
    {
        Sport = 1,
        [Description("Turistika")]
        Tourism = 2,
        [Description("Zábava")]
        Entertainment = 3
    }
}