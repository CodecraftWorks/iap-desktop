// Copyright 2024 Google LLC
//
// Licensed to the Apache Software Foundation (ASF) under one
// or more contributor license agreements.  See the NOTICE file
// distributed with this work for additional information
// regarding copyright ownership.  The ASF licenses this file
// to you under the Apache License, Version 2.0 (the
// "License"); you may not use this file except in compliance
// with the License.  You may obtain a copy of the License at
// 
//   http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing,
// software distributed under the License is distributed on an
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
// KIND, either express or implied.  See the License for the
// specific language governing permissions and limitations
// under the License.
//

using System;
using System.Runtime.InteropServices;

namespace Google.Solutions.Platform
{
    /// <summary>
    /// Processor architectures supported by IAP Desktop on macOS.
    /// </summary>
    public enum Architecture
    {
        Unknown,
        X64,
        Arm64
    }

    /// <summary>
    /// macOS process and OS environment helpers.
    /// </summary>
    public static class ProcessEnvironment
    {
        /// <summary>
        /// Returns the native CPU architecture of the current process.
        /// </summary>
        public static Architecture NativeArchitecture
        {
            get
            {
                return RuntimeInformation.ProcessArchitecture switch
                {
                    System.Runtime.InteropServices.Architecture.X64 => Architecture.X64,
                    System.Runtime.InteropServices.Architecture.Arm64 => Architecture.Arm64,
                    _ => Architecture.Unknown
                };
            }
        }

        /// <summary>
        /// Returns true when the process is running under Rosetta 2
        /// (x64 binary on Apple Silicon).
        /// </summary>
        public static bool IsTranslated
        {
            get
            {
                // sysctl.proc_translated is set to 1 when running under Rosetta.
                int value = 0;
                int size = sizeof(int);
                if (NativeMethods.sysctlbyname(
                    "sysctl.proc_translated",
                    ref value,
                    ref size,
                    IntPtr.Zero,
                    0) == 0)
                {
                    return value == 1;
                }

                return false;
            }
        }

        private static class NativeMethods
        {
            [DllImport("libc")]
            internal static extern int sysctlbyname(
                string name,
                ref int oldp,
                ref int oldlenp,
                IntPtr newp,
                uint newlen);
        }
    }
}
