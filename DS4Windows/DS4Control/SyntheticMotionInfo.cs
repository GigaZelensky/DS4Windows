/*
DS4Windows
Copyright (C) 2023  Travis Nickles

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

namespace DS4Windows
{
    public class SyntheticMotionInfo
    {
        public const bool DEFAULT_ENABLED = false;
        public const DS4Controls DEFAULT_UP_CONTROL = DS4Controls.L2;
        public const DS4Controls DEFAULT_DOWN_CONTROL = DS4Controls.R2;
        public const DS4Controls DEFAULT_LEFT_CONTROL = DS4Controls.None;
        public const DS4Controls DEFAULT_RIGHT_CONTROL = DS4Controls.None;
        public const int DEFAULT_TRIGGER_THRESHOLD = 128;
        public const int DEFAULT_PULSE_DURATION_MS = 260;
        public const int DEFAULT_GYRO_PEAK = 14000;
        public const double DEFAULT_TILT_ANGLE = 82.0;
        public const bool DEFAULT_INVERT = false;

        public const int MIN_TRIGGER_THRESHOLD = 0;
        public const int MAX_TRIGGER_THRESHOLD = 255;
        public const int MIN_PULSE_DURATION_MS = 60;
        public const int MAX_PULSE_DURATION_MS = 1000;
        public const int MIN_GYRO_PEAK = 1000;
        public const int MAX_GYRO_PEAK = 30000;
        public const double MIN_TILT_ANGLE = 10.0;
        public const double MAX_TILT_ANGLE = 120.0;

        public bool enabled = DEFAULT_ENABLED;
        public DS4Controls upControl = DEFAULT_UP_CONTROL;
        public DS4Controls downControl = DEFAULT_DOWN_CONTROL;
        public DS4Controls leftControl = DEFAULT_LEFT_CONTROL;
        public DS4Controls rightControl = DEFAULT_RIGHT_CONTROL;
        public int triggerThreshold = DEFAULT_TRIGGER_THRESHOLD;
        public int pulseDurationMs = DEFAULT_PULSE_DURATION_MS;
        public int gyroPeak = DEFAULT_GYRO_PEAK;
        public double tiltAngle = DEFAULT_TILT_ANGLE;
        public bool invert = DEFAULT_INVERT;

        public void Reset()
        {
            enabled = DEFAULT_ENABLED;
            upControl = DEFAULT_UP_CONTROL;
            downControl = DEFAULT_DOWN_CONTROL;
            leftControl = DEFAULT_LEFT_CONTROL;
            rightControl = DEFAULT_RIGHT_CONTROL;
            triggerThreshold = DEFAULT_TRIGGER_THRESHOLD;
            pulseDurationMs = DEFAULT_PULSE_DURATION_MS;
            gyroPeak = DEFAULT_GYRO_PEAK;
            tiltAngle = DEFAULT_TILT_ANGLE;
            invert = DEFAULT_INVERT;
        }
    }
}
