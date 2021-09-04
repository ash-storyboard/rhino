﻿/*
    Copyright 2020-2021 Picovoice Inc.

    You may not use this file except in compliance with the license. A copy of the license is located in the "LICENSE"
    file accompanying this source.

    Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on
    an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the
    specific language governing permissions and limitations under the License.
*/


using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Pv
{
    public static class Utils
    {
        private static Architecture _arch => RuntimeInformation.ProcessArchitecture;

        private static string _env => RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "mac" :
                                                 RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "windows" :
                                                 RuntimeInformation.IsOSPlatform(OSPlatform.Linux) && _arch == Architecture.X64 ? "linux" :
                                                 RuntimeInformation.IsOSPlatform(OSPlatform.Linux) &&
                                                    (_arch == Architecture.Arm || _arch == Architecture.Arm64) ? PvLinuxEnv() : "";

        public static string PvModelPath()
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lib/common/rhino_params.pv");
        }

        public static string PvLibraryPath(string libName)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && _arch == Architecture.X64)
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"lib/{_env}/amd64/{libName}.dll");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && _arch == Architecture.X64)
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"lib/{_env}/x86_64/{libName}.dylib");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"lib/{_env}/{PvLinuxMachine()}/{libName}.so");
            }
            else
            {
                throw new PlatformNotSupportedException($"{RuntimeInformation.OSDescription} ({RuntimeInformation.OSArchitecture}) is not currently supported.\n" +
                                                        "Visit https://picovoice.ai/docs/api/rhino-dotnet/ to see a list of supported platforms.");
            }
        }

        public static string PvLinuxMachine()
        {
            string archInfo = "";
            if (_arch == Architecture.X64)
                return "x86_64";
            else if (_arch == Architecture.Arm64)
                archInfo = "-aarch64";

            string cpuPart = GetCpuPart();
            switch (cpuPart)
            {
                case "0xc07": return "cortex-a7" + archInfo;
                case "0xd03": return "cortex-a53" + archInfo;
                case "0xd07": return "cortex-a57" + archInfo;
                case "0xd08": return "cortex-a72" + archInfo;
                case "0xc08": return "";
                default:
                    throw new PlatformNotSupportedException($"This device (CPU part = {cpuPart}) is not supported by Picovoice.");
            }
        }

        public static string PvLinuxEnv()
        {
            string cpuPart = GetCpuPart();
            switch (cpuPart)
            {
                case "0xc07":
                case "0xd03":
                case "0xd08": return "raspberry-pi";
                case "0xd07": return "jetson";
                case "0xc08": return "beaglebone";
                default:
                    throw new PlatformNotSupportedException($"This device (CPU part = {cpuPart}) is not supported by Picovoice.");
            }
        }

        private static string GetCpuPart()
        {
            string cpuInfo = File.ReadAllText("/proc/cpuinfo");
            string[] cpuPartList = cpuInfo.Split('\n').Where(x => x.Contains("CPU part")).ToArray();
            if (cpuPartList.Length == 0)
                throw new PlatformNotSupportedException($"Unsupported CPU.\n{cpuInfo}");

            string cpuPart = cpuPartList[0].Split(' ').Last().ToLower();
            return cpuPart;
        }
    }
}
