﻿//
// SecureMailboxAddress.cs
//
// Author: Jeffrey Stedfast <jestedfa@microsoft.com>
//
// Copyright (c) 2013-2025 .NET Foundation and Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System;
using System.Text;
using System.Collections.Generic;

using MimeKit.Utils;

namespace MimeKit.Cryptography {
	/// <summary>
	/// A secure mailbox address which includes a fingerprint for a certificate.
	/// </summary>
	/// <remarks>
	/// <para>When signing or encrypting a message, it is necessary to look up the
	/// X.509 certificate in order to do the actual sign or encrypt operation. One
	/// way of accomplishing this is to use the email address of sender or recipient
	/// as a unique identifier. However, a better approach is to use the fingerprint
	/// (or 'thumbprint' in Microsoft parlance) of the user's certificate.</para>
	/// </remarks>
	public class SecureMailboxAddress : MailboxAddress
	{
		/// <summary>
		/// Initialize a new instance of the <see cref="SecureMailboxAddress"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="SecureMailboxAddress"/> with the specified fingerprint.
		/// </remarks>
		/// <param name="encoding">The character encoding to be used for encoding the name.</param>
		/// <param name="name">The name of the mailbox.</param>
		/// <param name="route">The route of the mailbox.</param>
		/// <param name="address">The address of the mailbox.</param>
		/// <param name="fingerprint">The fingerprint of the certificate belonging to the owner of the mailbox.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="encoding"/> is <see langword="null"/>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="route"/> is <see langword="null"/>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="address"/> is <see langword="null"/>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="fingerprint"/> is <see langword="null"/>.</para>
		/// </exception>
		public SecureMailboxAddress (Encoding encoding, string name, IEnumerable<string> route, string address, string fingerprint) : base (encoding, name, route, address)
		{
			ValidateFingerprint (fingerprint);

			Fingerprint = fingerprint;
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="SecureMailboxAddress"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="SecureMailboxAddress"/> with the specified fingerprint.
		/// </remarks>
		/// <param name="name">The name of the mailbox.</param>
		/// <param name="route">The route of the mailbox.</param>
		/// <param name="address">The address of the mailbox.</param>
		/// <param name="fingerprint">The fingerprint of the certificate belonging to the owner of the mailbox.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="route"/> is <see langword="null"/>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="address"/> is <see langword="null"/>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="fingerprint"/> is <see langword="null"/>.</para>
		/// </exception>
		public SecureMailboxAddress (string name, IEnumerable<string> route, string address, string fingerprint) : base (name, route, address)
		{
			ValidateFingerprint (fingerprint);

			Fingerprint = fingerprint;
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="SecureMailboxAddress"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="SecureMailboxAddress"/> with the specified fingerprint.
		/// </remarks>
		/// <param name="encoding">The character encoding to be used for encoding the name.</param>
		/// <param name="name">The name of the mailbox.</param>
		/// <param name="address">The address of the mailbox.</param>
		/// <param name="fingerprint">The fingerprint of the certificate belonging to the owner of the mailbox.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="encoding"/> is <see langword="null"/>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="address"/> is <see langword="null"/>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="fingerprint"/> is <see langword="null"/>.</para>
		/// </exception>
		public SecureMailboxAddress (Encoding encoding, string name, string address, string fingerprint) : base (encoding, name, address)
		{
			ValidateFingerprint (fingerprint);

			Fingerprint = fingerprint;
		}

		/// <summary>
		/// Initialize a new instance of the <see cref="SecureMailboxAddress"/> class.
		/// </summary>
		/// <remarks>
		/// Creates a new <see cref="SecureMailboxAddress"/> with the specified fingerprint.
		/// </remarks>
		/// <param name="name">The name of the mailbox.</param>
		/// <param name="address">The address of the mailbox.</param>
		/// <param name="fingerprint">The fingerprint of the certificate belonging to the owner of the mailbox.</param>
		/// <exception cref="System.ArgumentNullException">
		/// <para><paramref name="address"/> is <see langword="null"/>.</para>
		/// <para>-or-</para>
		/// <para><paramref name="fingerprint"/> is <see langword="null"/>.</para>
		/// </exception>
		public SecureMailboxAddress (string name, string address, string fingerprint) : base (name, address)
		{
			ValidateFingerprint (fingerprint);

			Fingerprint = fingerprint;
		}

		static void ValidateFingerprint (string fingerprint)
		{
			if (fingerprint == null)
				throw new ArgumentNullException (nameof (fingerprint));

			for (int i = 0; i < fingerprint.Length; i++) {
				if (fingerprint[i] > 128 || !((byte) fingerprint[i]).IsXDigit ())
					throw new ArgumentException ("The fingerprint should be a hex-encoded string.", nameof (fingerprint));
			}
		}

		/// <summary>
		/// Gets the fingerprint of the certificate and/or key to use for signing or encrypting.
		/// </summary>
		/// <remarks>
		/// A fingerprint is a SHA-1 hash of the raw certificate data and is often used
		/// as a unique identifier for a particular certificate in a certificate store.
		/// </remarks>
		/// <seealso cref="System.Security.Cryptography.X509Certificates.X509Certificate2.Thumbprint"/>
		/// <seealso cref="BouncyCastleCertificateExtensions"/>
		/// <value>The fingerprint of the certificate.</value>
		public string Fingerprint {
			get; private set;
		}
	}
}
