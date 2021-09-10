using System;
using System.Numerics;

namespace INNO6.Core
{
    public class SphericalCoordinateCoverter
    {
        public static void ToPolar(Vector3 cart, out double radius, out double theta, out double phi)
        {
            double x = Convert.ToDouble(cart.X);
            double y = Convert.ToDouble(cart.Y);
            double z = Convert.ToDouble(cart.Z);
            radius = Math.Sqrt(x * x + y * y + z * z);
            if (radius == 0)
            {
                theta = 0;
                phi = 0;
            }
            else
            {
                theta = Math.Acos(cart.Z / radius);
                phi = Math.Atan2(cart.Y, cart.X);
            }
        }

        public static Vector3 ToCartesian(float radius, float longitude, float latitude)
        {
            float x = radius * (float)(Math.Sin(latitude) * Math.Cos(longitude));
            float y = radius * (float)(Math.Sin(latitude) * Math.Sin(longitude));
            float z = radius * (float)Math.Cos(latitude);

            return new Vector3(x, y, z);
        }

    }
}
