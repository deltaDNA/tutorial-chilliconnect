//
//  This file was auto-generated using the ChilliConnect SDK Generator.
//
//  The MIT License (MIT)
//
//  Copyright (c) 2019 ChilliConnect Ltd
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using SdkCore;

namespace ChilliConnect
{
	/// <summary>
	/// <para>A container for all information that will be sent to the server during a
 	/// Add Events api call.</para>
	///
	/// <para>This is immutable after construction and is therefore thread safe.</para>
	/// </summary>
	public sealed class AddEventsRequest : IImmediateServerRequest
	{
		/// <summary>
		/// The url the request will be sent to.
		/// </summary>
		public string Url { get; private set; }
		
		/// <summary>
		/// The HTTP request method that should be used.
		/// </summary>
		public HttpRequestMethod HttpRequestMethod { get; private set; }
		
		/// <summary>
		/// MetricsAccessToken as returned from a Player login call.
		/// </summary>
        public string MetricsAccessToken { get; private set; }
	
		/// <summary>
		/// An array of events.
		/// </summary>
        public ReadOnlyCollection<MetricsEvent> Events { get; private set; }

		/// <summary>
		/// Initialises a new instance of the request with the given properties.
		/// </summary>
		///
		/// <param name="events">An array of events.</param>
		/// <param name="metricsAccessToken">MetricsAccessToken as returned from a Player login call.</param>
		/// <param name="serverUrl">The server url for this call.</param>
		public AddEventsRequest(IList<MetricsEvent> events, string metricsAccessToken, string serverUrl)
		{
			ReleaseAssert.IsNotNull(events, "Events cannot be null.");
	
			ReleaseAssert.IsNotNull(metricsAccessToken, "Metrics Access Token cannot be null.");
	
            Events = Mutability.ToImmutable(events);
            MetricsAccessToken = metricsAccessToken;
			
			Url = serverUrl + "/1.0/events/batch";
			HttpRequestMethod = HttpRequestMethod.Post;
		}

		/// <summary>
		/// Serialises all header properties. The output will be a dictionary containing 
		/// the extra header key-value pairs in addition the standard headers sent with 
		/// all server requests. Will return an empty dictionary if there are no headers.
		/// </summary>
		///
		/// <returns>The header key-value pairs.</returns>
		public IDictionary<string, string> SerialiseHeaders()
		{
			var dictionary = new Dictionary<string, string>();
			
			// Metrics Access Token
			dictionary.Add("Metrics-Access-Token", MetricsAccessToken.ToString());
		
			return dictionary;
		}
		
		/// <summary>
		/// Serialises all body properties. The output will be a dictionary containing the 
		/// body of the request in a form that can easily be converted to Json. Will return
		/// an empty dictionary if there is no body.
		/// </summary>
		///
		/// <returns>The body Json in dictionary form.</returns>
		public IDictionary<string, object> SerialiseBody()
		{
            var dictionary = new Dictionary<string, object>();
			
			// Events
            var serialisedEvents = JsonSerialisation.Serialise(Events, (MetricsEvent element) =>
            {
                return element.Serialise();
            });
            dictionary.Add("Events", serialisedEvents);
	
			return dictionary;
		}
	}
}
