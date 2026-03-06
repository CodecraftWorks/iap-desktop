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
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace Google.Solutions.Platform.Security
{
    /// <summary>
    /// Provides access to the macOS system and user Keychain for
    /// storing and retrieving credentials and certificates.
    /// </summary>
    public static class MacOsKeychain
    {
        /// <summary>
        /// Returns all client certificates available in the current user's
        /// Keychain (login keychain and system keychain).
        /// </summary>
        public static X509Certificate2Collection GetClientCertificates()
        {
            var collection = new X509Certificate2Collection();

            // Open the "MY" store which maps to the macOS login Keychain
            // for the current user. The X509Store abstraction is supported
            // on macOS via the Security framework.
            using var store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadOnly);
            collection.AddRange(
                store.Certificates
                    .Cast<X509Certificate2>()
                    .Where(c => c.HasPrivateKey)
                    .ToArray());

            return collection;
        }

        /// <summary>
        /// Stores a password in the current user's login Keychain.
        /// </summary>
        /// <param name="service">Service name (e.g. "iap-desktop").</param>
        /// <param name="account">Account/username.</param>
        /// <param name="password">Password to store.</param>
        public static void StorePassword(string service, string account, string password)
        {
            var passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);

            var status = NativeMethods.SecKeychainAddGenericPassword(
                IntPtr.Zero,
                (uint)service.Length,
                service,
                (uint)account.Length,
                account,
                (uint)passwordBytes.Length,
                passwordBytes,
                out _);

            if (status != 0)
            {
                throw new InvalidOperationException(
                    $"SecKeychainAddGenericPassword failed with status {status}.");
            }
        }

        /// <summary>
        /// Retrieves a password from the current user's login Keychain.
        /// Returns <c>null</c> if no matching entry is found.
        /// </summary>
        public static string? RetrievePassword(string service, string account)
        {
            var status = NativeMethods.SecKeychainFindGenericPassword(
                IntPtr.Zero,
                (uint)service.Length,
                service,
                (uint)account.Length,
                account,
                out uint passwordLength,
                out IntPtr passwordData,
                out IntPtr itemRef);

            if (status == NativeMethods.ErrSecItemNotFound)
            {
                return null;
            }

            if (status != 0)
            {
                throw new InvalidOperationException(
                    $"SecKeychainFindGenericPassword failed with status {status}.");
            }

            try
            {
                var bytes = new byte[passwordLength];
                Marshal.Copy(passwordData, bytes, 0, (int)passwordLength);
                return System.Text.Encoding.UTF8.GetString(bytes);
            }
            finally
            {
                _ = NativeMethods.SecKeychainItemFreeContent(IntPtr.Zero, passwordData);
            }
        }

        private static class NativeMethods
        {
            internal const int ErrSecItemNotFound = -25300;

            [DllImport("/System/Library/Frameworks/Security.framework/Security")]
            internal static extern int SecKeychainAddGenericPassword(
                IntPtr keychain,
                uint serviceNameLength,
                string serviceName,
                uint accountNameLength,
                string accountName,
                uint passwordLength,
                byte[] passwordData,
                out IntPtr itemRef);

            [DllImport("/System/Library/Frameworks/Security.framework/Security")]
            internal static extern int SecKeychainFindGenericPassword(
                IntPtr keychainOrArray,
                uint serviceNameLength,
                string serviceName,
                uint accountNameLength,
                string accountName,
                out uint passwordLength,
                out IntPtr passwordData,
                out IntPtr itemRef);

            [DllImport("/System/Library/Frameworks/Security.framework/Security")]
            internal static extern int SecKeychainItemFreeContent(
                IntPtr attrList,
                IntPtr data);
        }
    }
}
