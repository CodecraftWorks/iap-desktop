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
using System.IO;

namespace Google.Solutions.Platform
{
    /// <summary>
    /// macOS user-environment utilities.
    /// </summary>
    public static class UserEnvironment
    {
        /// <summary>
        /// Returns the path to the user's application-support directory,
        /// equivalent to <c>~/Library/Application Support</c>.
        /// </summary>
        public static string ApplicationSupportDirectory =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Library",
                "Application Support",
                "IapDesktop");

        /// <summary>
        /// Returns the path to the user's preferences directory,
        /// equivalent to <c>~/Library/Preferences</c>.
        /// </summary>
        public static string PreferencesDirectory =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                "Library",
                "Preferences");

        /// <summary>
        /// Ensures that the application-support directory exists.
        /// </summary>
        public static string EnsureApplicationSupportDirectoryExists()
        {
            var dir = ApplicationSupportDirectory;
            Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
