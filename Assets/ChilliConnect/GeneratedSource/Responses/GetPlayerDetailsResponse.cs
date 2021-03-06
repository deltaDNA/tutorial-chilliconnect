//
//  This file was auto-generated using the ChilliConnect SDK Generator.
//
//  The MIT License (MIT)
//
//  Copyright (c) 2015 Tag Games Ltd
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
	/// A container for information on the response from a GetPlayerDetailsRequest.
	/// </summary>
	public sealed class GetPlayerDetailsResponse
	{
		/// <summary>
		/// The ChilliConnectID of the account.
		/// </summary>
        public string ChilliConnectId { get; private set; }
	
		/// <summary>
		/// The UserName currently associated with the account.
		/// </summary>
		public string UserName { get; private set; }
	
		/// <summary>
		/// The DisplayName currently associated with the account.
		/// </summary>
		public string DisplayName { get; private set; }
	
		/// <summary>
		/// The Email currently associated with the account.
		/// </summary>
		public string Email { get; private set; }
	
		/// <summary>
		/// The Country currently associated with the account. Format: ISO 3166-1 alpha-2.
		/// </summary>
		public string Country { get; private set; }
	
		/// <summary>
		/// List of DeviceModels being used by the player.
		/// </summary>
		public ReadOnlyCollection<string> DeviceModel { get; private set; }
	
		/// <summary>
		/// DeviceType being used by the player.
		/// </summary>
		public string DeviceType { get; private set; }
	
		/// <summary>
		/// Platform being used by the player.
		/// </summary>
		public string Platform { get; private set; }
	
		/// <summary>
		/// Date that indicates when the player was created (UTC). Format: ISO8601 e.g.
		/// 2016-01-12T11:08:23.
		/// </summary>
        public DateTime DateCreated { get; private set; }

		/// <summary>
		/// Initialises the response with the given json dictionary.
		/// </summary>
		///
		/// <param name="jsonDictionary">The dictionary containing the JSON data.</param>
		public GetPlayerDetailsResponse(IDictionary<string, object> jsonDictionary)
		{
			ReleaseAssert.IsNotNull(jsonDictionary, "JSON dictionary cannot be null.");
			ReleaseAssert.IsTrue(jsonDictionary.ContainsKey("ChilliConnectID"), "Json is missing required field 'ChilliConnectID'");
			ReleaseAssert.IsTrue(jsonDictionary.ContainsKey("DateCreated"), "Json is missing required field 'DateCreated'");
	
			foreach (KeyValuePair<string, object> entry in jsonDictionary)
			{
				// Chilli Connect Id
				if (entry.Key == "ChilliConnectID")
				{
                    ReleaseAssert.IsTrue(entry.Value is string, "Invalid serialised type.");
                    ChilliConnectId = (string)entry.Value;
				}
		
				// User Name
				else if (entry.Key == "UserName")
				{
					if (entry.Value != null)
					{
                        ReleaseAssert.IsTrue(entry.Value is string, "Invalid serialised type.");
                        UserName = (string)entry.Value;
                    }
				}
		
				// Display Name
				else if (entry.Key == "DisplayName")
				{
					if (entry.Value != null)
					{
                        ReleaseAssert.IsTrue(entry.Value is string, "Invalid serialised type.");
                        DisplayName = (string)entry.Value;
                    }
				}
		
				// Email
				else if (entry.Key == "Email")
				{
					if (entry.Value != null)
					{
                        ReleaseAssert.IsTrue(entry.Value is string, "Invalid serialised type.");
                        Email = (string)entry.Value;
                    }
				}
		
				// Country
				else if (entry.Key == "Country")
				{
					if (entry.Value != null)
					{
                        ReleaseAssert.IsTrue(entry.Value is string, "Invalid serialised type.");
                        Country = (string)entry.Value;
                    }
				}
		
				// Device Model
				else if (entry.Key == "DeviceModel")
				{
					if (entry.Value != null)
					{
                        ReleaseAssert.IsTrue(entry.Value is IList<object>, "Invalid serialised type.");
                        DeviceModel = JsonSerialisation.DeserialiseList((IList<object>)entry.Value, (object element) =>
                        {
                            ReleaseAssert.IsTrue(element is string, "Invalid element type.");
                            return (string)element;
                        });
                    }
				}
		
				// Device Type
				else if (entry.Key == "DeviceType")
				{
					if (entry.Value != null)
					{
                        ReleaseAssert.IsTrue(entry.Value is string, "Invalid serialised type.");
                        DeviceType = (string)entry.Value;
                    }
				}
		
				// Platform
				else if (entry.Key == "Platform")
				{
					if (entry.Value != null)
					{
                        ReleaseAssert.IsTrue(entry.Value is string, "Invalid serialised type.");
                        Platform = (string)entry.Value;
                    }
				}
		
				// Date Created
				else if (entry.Key == "DateCreated")
				{
                    ReleaseAssert.IsTrue(entry.Value is string, "Invalid serialised type.");
                    DateCreated = JsonSerialisation.DeserialiseDate((string)entry.Value);
				}
			}
		}
	}
}
