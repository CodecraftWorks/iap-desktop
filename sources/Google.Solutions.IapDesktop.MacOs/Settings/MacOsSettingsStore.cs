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

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Google.Solutions.Platform;

namespace Google.Solutions.IapDesktop.MacOs.Settings
{
    /// <summary>
    /// Persists application settings as a JSON file in
    /// <c>~/Library/Application Support/IapDesktop/settings.json</c>,
    /// providing a native macOS alternative to the Windows registry.
    /// </summary>
    public class MacOsSettingsStore
    {
        private readonly string filePath;
        private Dictionary<string, string?> values;

        public MacOsSettingsStore()
        {
            var dir = UserEnvironment.EnsureApplicationSupportDirectoryExists();
            this.filePath = Path.Combine(dir, "settings.json");
            this.values = Load();
        }

        private Dictionary<string, string?> Load()
        {
            if (!File.Exists(this.filePath))
            {
                return new Dictionary<string, string?>();
            }

            var json = File.ReadAllText(this.filePath);
            return JsonSerializer.Deserialize<Dictionary<string, string?>>(json)
                ?? new Dictionary<string, string?>();
        }

        public string? Read(string key)
        {
            return this.values.TryGetValue(key, out var value) ? value : null;
        }

        public void Write(string key, string? value)
        {
            this.values[key] = value;
            Save();
        }

        public void Delete(string key)
        {
            this.values.Remove(key);
            Save();
        }

        private void Save()
        {
            var json = JsonSerializer.Serialize(
                this.values,
                new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(this.filePath, json);
        }
    }
}
