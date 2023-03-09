using System;

namespace ICSStudio.Utils
{
    /// <summary>
    /// Speed,Accel(Decel),Jerk,PercentTime(%)
    /// PercentTime = 0.01-100
    /// </summary>
    public class JerkRateCalculation
    {
        public static float CalculateJerk(float speed, float accel, float percentTime)
        {
            if (percentTime < 0 || percentTime > 100)
                throw new ArithmeticException("percentTime is not in 0-100");

            if (speed < float.Epsilon)
                return 0;

            return accel * accel * (200 / percentTime - 1) / speed;
        }

        public static double CalculateJerk(double speed, double accel, double percentTime)
        {
            if (percentTime < 0 || percentTime > 100)
                throw new ArithmeticException("percentTime is not in 0-100");

            if (speed < double.Epsilon)
                return 0;

            return accel * accel * (200 / percentTime - 1) / speed;
        }

        public static double CalculatePercentTime(double speed, double accel, double jerk)
        {
            return 200 / (speed * jerk / (accel * accel) + 1);
        }
    }
}
