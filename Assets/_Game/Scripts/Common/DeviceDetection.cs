using PS.Utils;
using UnityEngine;

public class DeviceDetection : Singleton<DeviceDetection>
{
    [SerializeField] private DeviceType deviceType;

    public DeviceType DeviceType { get => deviceType; }

    void Start()
    {
        DetectDevice();
    }

    void DetectDevice()
    {
        if (SystemInfo.deviceType == UnityEngine.DeviceType.Handheld)
        {
            if (IsTablet())
            {
                deviceType = DeviceType.Tablet;
            }
            else
            {
                deviceType = DeviceType.Phone;
            }
        }

    }

    bool IsTablet()
    {
        string deviceModel = SystemInfo.deviceModel.ToLower();

        // Kiểm tra với các tên thiết bị phổ biến của tablet
        if (deviceModel.Contains("ipad"))
        {
            return true;
        }

        return false;
    }

}

public enum DeviceType
{
    Phone = 0,
    Tablet = 1
}