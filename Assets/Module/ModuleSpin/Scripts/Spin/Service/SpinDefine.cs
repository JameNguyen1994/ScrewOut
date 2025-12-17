using PS.Analytic;

public class SpinDefine
{
    public const int RANDOM_OFFSET_RANGE = 22;
    public const int MIN_ROUNDS = 10;
    public const int MAX_ROUNDS = 20;

    public const int UNLOCK_SESSION = 2;

    public static int REQURIED_SCREW
    {
        get
        {
            if (GameAnalyticController.Instance != null)
            {
                var remote = GameAnalyticController.Instance.Remote();
                return remote != null ? remote.Requried_Screw : 75;
            }

            return 75;
        }
    }

    public static int DAILY_SPIN
    {
        get
        {
            if (GameAnalyticController.Instance != null)
            {
                var remote = GameAnalyticController.Instance.Remote();
                return remote != null ? remote.Daily_Spin : 2;
            }

            return 2;
        }
    }

    public const string SPIN_WITH_SCREW = "SPIN\n{0}/{1}";
    public const string SPIN_WITH_ADS = "DAILY\n{0}/{1}";
    public const string SPIN_WITH_ADS_COUNT_DOWN = "DAILY\n<size=45>{0}</size>";
}