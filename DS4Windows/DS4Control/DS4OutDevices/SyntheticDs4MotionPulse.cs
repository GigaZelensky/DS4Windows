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

using System;
using System.Diagnostics;

namespace DS4Windows
{
    internal sealed class SyntheticDs4MotionPulse
    {
        private const int UpDirection = 1;
        private const int DownDirection = -1;
        private const int RightDirection = 2;
        private const int LeftDirection = -2;
        private const int StickCenter = 128;

        private readonly long[] pulseStartTicks = new long[Global.MAX_DS4_CONTROLLER_COUNT];
        private readonly int[] pulseDirections = new int[Global.MAX_DS4_CONTROLLER_COUNT];
        private readonly int[] previousRequestedDirections = new int[Global.MAX_DS4_CONTROLLER_COUNT];

        public SixAxis Apply(DS4State state, int device)
        {
            SixAxis fallbackMotion = state.Motion ?? new SixAxis(0, 0, 0, 0, 0, 0, state.elapsedTime);

            if (device < 0 || device >= Global.MAX_DS4_CONTROLLER_COUNT)
            {
                return fallbackMotion;
            }

            SyntheticMotionInfo info = Global.GetSyntheticMotionInfo(device);
            if (info == null || !info.enabled)
            {
                ResetDevice(device);
                return fallbackMotion;
            }

            int requestedDirection = GetRequestedDirection(state, info);
            if (requestedDirection != 0 &&
                (previousRequestedDirections[device] == 0 ||
                 previousRequestedDirections[device] != requestedDirection))
            {
                pulseStartTicks[device] = Stopwatch.GetTimestamp();
                pulseDirections[device] = requestedDirection;
            }

            previousRequestedDirections[device] = requestedDirection;

            if (pulseDirections[device] == 0)
            {
                return fallbackMotion;
            }

            long elapsedTicks = Stopwatch.GetTimestamp() - pulseStartTicks[device];
            long pulseDurationTicks = GetPulseDurationTicks(info);
            if (elapsedTicks >= pulseDurationTicks)
            {
                pulseDirections[device] = 0;
                return fallbackMotion;
            }

            double progress = elapsedTicks / (double)pulseDurationTicks;
            return CreateMotionSample(progress, pulseDirections[device], fallbackMotion,
                state.elapsedTime, info);
        }

        public void Reset()
        {
            Array.Clear(pulseStartTicks, 0, pulseStartTicks.Length);
            Array.Clear(pulseDirections, 0, pulseDirections.Length);
            Array.Clear(previousRequestedDirections, 0, previousRequestedDirections.Length);
        }

        private void ResetDevice(int device)
        {
            previousRequestedDirections[device] = 0;
            pulseDirections[device] = 0;
            pulseStartTicks[device] = 0;
        }

        private static int GetRequestedDirection(DS4State state, SyntheticMotionInfo info)
        {
            bool tiltUp = IsControlPressed(state, info.upControl, info.triggerThreshold);
            bool tiltDown = IsControlPressed(state, info.downControl, info.triggerThreshold);
            bool tiltLeft = IsControlPressed(state, info.leftControl, info.triggerThreshold);
            bool tiltRight = IsControlPressed(state, info.rightControl, info.triggerThreshold);

            int directionCount = (tiltUp ? 1 : 0) + (tiltDown ? 1 : 0) +
                                 (tiltLeft ? 1 : 0) + (tiltRight ? 1 : 0);
            if (directionCount != 1)
            {
                return 0;
            }

            int direction = tiltUp ? UpDirection :
                tiltDown ? DownDirection :
                tiltRight ? RightDirection :
                LeftDirection;

            return info.invert ? -direction : direction;
        }

        private static bool IsControlPressed(DS4State state, DS4Controls control, int threshold)
        {
            int triggerThreshold = Math.Clamp(threshold,
                SyntheticMotionInfo.MIN_TRIGGER_THRESHOLD,
                SyntheticMotionInfo.MAX_TRIGGER_THRESHOLD);
            int stickThreshold = GetStickThreshold(triggerThreshold);

            switch (control)
            {
                case DS4Controls.LXNeg:
                    return state.LX <= StickCenter - stickThreshold;
                case DS4Controls.LXPos:
                    return state.LX >= StickCenter + stickThreshold;
                case DS4Controls.LYNeg:
                    return state.LY <= StickCenter - stickThreshold;
                case DS4Controls.LYPos:
                    return state.LY >= StickCenter + stickThreshold;
                case DS4Controls.L1:
                    return state.L1;
                case DS4Controls.L2:
                    return state.L2Btn || state.L2 >= triggerThreshold;
                case DS4Controls.L2FullPull:
                    return state.L2 >= Math.Max(triggerThreshold, 250);
                case DS4Controls.L3:
                    return state.L3;
                case DS4Controls.R1:
                    return state.R1;
                case DS4Controls.R2:
                    return state.R2Btn || state.R2 >= triggerThreshold;
                case DS4Controls.R2FullPull:
                    return state.R2 >= Math.Max(triggerThreshold, 250);
                case DS4Controls.R3:
                    return state.R3;
                case DS4Controls.Square:
                    return state.Square;
                case DS4Controls.Triangle:
                    return state.Triangle;
                case DS4Controls.Circle:
                    return state.Circle;
                case DS4Controls.Cross:
                    return state.Cross;
                case DS4Controls.DpadUp:
                    return state.DpadUp;
                case DS4Controls.DpadRight:
                    return state.DpadRight;
                case DS4Controls.DpadDown:
                    return state.DpadDown;
                case DS4Controls.DpadLeft:
                    return state.DpadLeft;
                case DS4Controls.PS:
                    return state.PS;
                case DS4Controls.TouchLeft:
                    return state.TouchLeft;
                case DS4Controls.TouchUpper:
                    return state.Touch1 || state.Touch2;
                case DS4Controls.TouchMulti:
                    return state.Touch2Fingers;
                case DS4Controls.TouchRight:
                    return state.TouchRight;
                case DS4Controls.Share:
                    return state.Share;
                case DS4Controls.Options:
                    return state.Options;
                case DS4Controls.Mute:
                    return state.Mute;
                case DS4Controls.FnL:
                    return state.FnL;
                case DS4Controls.FnR:
                    return state.FnR;
                case DS4Controls.BLP:
                    return state.BLP;
                case DS4Controls.BRP:
                    return state.BRP;
                case DS4Controls.Capture:
                    return state.Capture;
                case DS4Controls.SideL:
                    return state.SideL;
                case DS4Controls.SideR:
                    return state.SideR;
                default:
                    return false;
            }
        }

        private static int GetStickThreshold(int threshold)
        {
            return Math.Clamp((int)Math.Round(threshold * 0.5),
                1,
                StickCenter - 1);
        }

        private static long GetPulseDurationTicks(SyntheticMotionInfo info)
        {
            int pulseDurationMs = Math.Clamp(info.pulseDurationMs,
                SyntheticMotionInfo.MIN_PULSE_DURATION_MS,
                SyntheticMotionInfo.MAX_PULSE_DURATION_MS);

            return Math.Max(1L, Stopwatch.Frequency * pulseDurationMs / 1000);
        }

        private static SixAxis CreateMotionSample(double progress, int direction,
            SixAxis previousMotion, double stateElapsedTime, SyntheticMotionInfo info)
        {
            double targetAngle;
            double gyroPitch;
            double maxTiltAngle = Math.Clamp(info.tiltAngle,
                SyntheticMotionInfo.MIN_TILT_ANGLE,
                SyntheticMotionInfo.MAX_TILT_ANGLE);
            int gyroPeak = Math.Clamp(info.gyroPeak,
                SyntheticMotionInfo.MIN_GYRO_PEAK,
                SyntheticMotionInfo.MAX_GYRO_PEAK);

            if (progress < 0.45)
            {
                double segmentProgress = progress / 0.45;
                targetAngle = maxTiltAngle * EaseOutCubic(segmentProgress);
                gyroPitch = gyroPeak * Math.Sin(segmentProgress * Math.PI);
            }
            else if (progress < 0.78)
            {
                targetAngle = maxTiltAngle;
                gyroPitch = 0.0;
            }
            else
            {
                double segmentProgress = (progress - 0.78) / 0.22;
                targetAngle = maxTiltAngle * (1.0 - EaseInCubic(segmentProgress));
                gyroPitch = -gyroPeak * 0.65 * Math.Sin(segmentProgress * Math.PI);
            }

            int directionSign = direction > 0 ? 1 : -1;
            double angleRad = directionSign * targetAngle * Math.PI / 180.0;
            int accelAxis = (int)(Math.Sin(angleRad) * SixAxis.ACC_RES_PER_G);
            int accelZ = (int)(Math.Cos(angleRad) * SixAxis.ACC_RES_PER_G);
            int gyroAxis = (int)(directionSign * gyroPitch);

            if (Math.Abs(direction) == Math.Abs(LeftDirection))
            {
                return new SixAxis(
                    0,
                    0,
                    gyroAxis,
                    accelAxis,
                    0,
                    accelZ,
                    previousMotion?.elapsed ?? stateElapsedTime,
                    previousMotion);
            }

            return new SixAxis(
                0,
                gyroAxis,
                0,
                0,
                accelAxis,
                accelZ,
                previousMotion?.elapsed ?? stateElapsedTime,
                previousMotion);
        }

        private static double EaseOutCubic(double value)
        {
            double inverse = 1.0 - value;
            return 1.0 - inverse * inverse * inverse;
        }

        private static double EaseInCubic(double value)
        {
            return value * value * value;
        }
    }
}
