// NEW FILE: Assets/Scripts/MCOfferwallSDK/Domain/MCGenderEnum.cs
namespace Assets.Scripts.MCOfferwallSDK.Domain
{
    /// <summary>
    /// User gender enum, used across SDKs.
    /// </summary>
    public enum MCGenderEnum
    {
        Male,   // "m"
        Female, // "f"
        Other   // "o"
    }

    // ADD: Extension to map to wire value ("m"/"f"/"o")
    public static class MCGenderEnumExtensions
    {
        public static string ToWireValue(this MCGenderEnum gender)
        {
            switch (gender)
            {
                case MCGenderEnum.Male:
                    return "m";
                case MCGenderEnum.Female:
                    return "f";
                case MCGenderEnum.Other:
                default:
                    return "o";
            }
        }
    }
}
