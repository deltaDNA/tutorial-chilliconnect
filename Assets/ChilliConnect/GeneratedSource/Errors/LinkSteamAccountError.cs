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
using System.Diagnostics;
using SdkCore;

namespace ChilliConnect
{
	/// <summary>
	/// <para>A container for information on any errors that occur during a 
	/// LinkSteamAccountRequest.</para>
	///
	/// <para>This is immutable after construction and is therefore thread safe.</para>
	/// </summary>
	public sealed class LinkSteamAccountError
	{
		/// <summary>
		/// An enum describing each of the possible error codes that can be returned from a
		/// CCLinkSteamAccountRequest.
		/// </summary>
		public enum Error
		{
			/// <summary> 
			/// A connection could not be established.
			/// </summary>
			CouldNotConnect = -2,
	
			/// <summary> 
			/// An unexpected, fatal error has occured on the server. 
			/// </summary>
			UnexpectedError = 1,
			
	
			/// <summary>
			/// Invalid Request. One of more of the provided fields were not correctly formatted.
			/// The data property of the response body will contain specific error messages for
			/// each field.
			/// </summary>
			InvalidRequest = 1007,
	
			/// <summary>
			/// Rate Limit Reached. Too many requests. Player has been rate limited. The data
			/// property of the response may contain more details.
			/// </summary>
			RateLimitReached = 10003,
	
			/// <summary>
			/// Temporary Service Error. A temporary error is preventing the request from being
			/// processed.
			/// </summary>
			TemporaryServiceError = 1008,
	
			/// <summary>
			/// Expired Connect Access Token. The Connect Access Token used to authenticate with
			/// the server has expired and should be renewed.
			/// </summary>
			ExpiredConnectAccessToken = 1003,
	
			/// <summary>
			/// Invalid Connect Access Token. The Connect Access Token was not valid and cannot
			/// be used to authenticate requests.
			/// </summary>
			InvalidConnectAccessToken = 1004,
	
			/// <summary>
			/// SteamId Is Already Linked With Another Player. Another Player already has already
			/// linked their account with this SteamID.
			/// </summary>
			SteamAccountLinked = 2028,
	
			/// <summary>
			/// This Player is already linked with another Steam Account. A SteamID has already
			/// been associated with this Player.
			/// </summary>
			PlayerAlreadyLinkedToSteamAccount = 2027,
	
			/// <summary>
			/// Method Disabled. Public access to this method has been disabled on the
			/// ChilliConnect Dashboard.
			/// </summary>
			MethodDisabled = 1011,
	
			/// <summary>
			/// No Steam Details Configured For Project. Steam access details have not been
			/// entered into the Dashboard.
			/// </summary>
			NoSteamAppDetailsHaveBeenConfigured = 2026,
	
			/// <summary>
			/// Incorrect Steam AppID For Provided Session Ticket. The Session Ticket supplied by
			/// the user does not authenticate with the Steam App ID specified in Dashboard.
			/// </summary>
			SteamAppIdIncompatibleWithSessionTicket = 2031,
	
			/// <summary>
			/// Steam Details Have Been Configured Incorrectly. Steam Authentication has returned
			/// an error code that signals an issue with the AppID and API Key configured in
			/// Dashboard.
			/// </summary>
			SteamAppDetailsConfiguredIncorrectly = 2030,
	
			/// <summary>
			/// Supplied Steam Ticket Could Not Be Authenticated. The Supplied Steam Session
			/// Ticket could not be authenticated with Steamworks Servers.
			/// </summary>
			SuppliedSteamTicketIsInvalid = 2025,
	
			/// <summary>
			/// Account Restriction. Account does not have access to this feature, or has
			/// exceeded the usage limit.
			/// </summary>
			AccountRestriction = 10002
		}
		
		private const int SuccessHttpResponseCode = 200;
		private const int UnexpectedErrorHttpResponseCode = 500;
		
		/// <summary> 
		/// A code describing the error that has occurred.
		/// </summary>
		public Error ErrorCode { get; private set; }
		
		/// <summary> 
		/// A description of the error that as occurred.
		/// </summary>
		public string ErrorDescription { get; private set; }
        
        /// <summary> 
		/// A dictionary of additional, error specific information.
		/// </summary>
		public MultiTypeValue ErrorData { get; private set; }

		/// <summary> 
		/// Initialises a new instance from the given server response. The server response
		/// must describe an error otherwise this will throw an error.
		/// </summary>
		///
		/// <param name="serverResponse">The server response from which to initialise this error.
		/// The response must describe an error state.</param>
		public LinkSteamAccountError(ServerResponse serverResponse)
		{
			ReleaseAssert.IsNotNull(serverResponse, "A server response must be supplied.");
			ReleaseAssert.IsTrue(serverResponse.Result != HttpResult.Success || serverResponse.HttpResponseCode != SuccessHttpResponseCode, "Input server response must describe an error.");
			
			switch (serverResponse.Result)
			{
				case HttpResult.Success:
					if (serverResponse.HttpResponseCode == UnexpectedErrorHttpResponseCode)
					{
						ErrorCode = Error.UnexpectedError;
                        ErrorData = MultiTypeValue.Null;
					}
					else
					{
						ErrorCode = GetErrorCode(serverResponse);
                        ErrorData = GetErrorData(serverResponse.Body);
					}
					break;
				case HttpResult.CouldNotConnect:
					ErrorCode = Error.CouldNotConnect;
                    ErrorData = MultiTypeValue.Null;
					break;
				default:
					throw new ArgumentException("Invalid value for server response result.");
			}
			
			ErrorDescription = GetErrorDescription(ErrorCode);
		}
		
		/// <summary> 
		/// Initialises a new instance from the given error code.
		/// </summary>
		///
		/// <param name="errorCode">The error code.</param>
		public LinkSteamAccountError(Error errorCode)
		{
			ErrorCode = errorCode;
            ErrorData = MultiTypeValue.Null;
			ErrorDescription = GetErrorDescription(ErrorCode);
		}
		
		/// <summary>
		/// Parses the response body to get the response code.
		/// </summary>
		///
		/// <returns>The error code in the given response body.</returns>
		///
		/// <param name="serverResponse">The server response from which to get the error code. This
		/// must describe an successful response from the server which contains an error in the
		/// response body.</param>
		private static Error GetErrorCode(ServerResponse serverResponse) 
		{
			const string JsonKeyErrorCode = "Code";
			
			ReleaseAssert.IsNotNull(serverResponse, "A server response must be supplied.");
			ReleaseAssert.IsTrue(serverResponse.Result == HttpResult.Success, "The result must describe a successful server response.");
			ReleaseAssert.IsTrue(serverResponse.HttpResponseCode != SuccessHttpResponseCode && serverResponse.HttpResponseCode != UnexpectedErrorHttpResponseCode, 
				"Must not be a successful or unexpected HTTP response code.");
				
			object errorCodeObject = serverResponse.Body[JsonKeyErrorCode];
			ReleaseAssert.IsTrue(errorCodeObject is long, "'Code' must be a long.");
			
			long errorCode = (long)errorCodeObject;
			
			switch (errorCode)
			{
				case 1007:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 422, @"Invalid HTTP response code for error code.");
					return Error.InvalidRequest;		
				case 10003:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 429, @"Invalid HTTP response code for error code.");
					return Error.RateLimitReached;		
				case 1008:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 503, @"Invalid HTTP response code for error code.");
					return Error.TemporaryServiceError;		
				case 1003:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 401, @"Invalid HTTP response code for error code.");
					return Error.ExpiredConnectAccessToken;		
				case 1004:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 401, @"Invalid HTTP response code for error code.");
					return Error.InvalidConnectAccessToken;		
				case 2028:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 409, @"Invalid HTTP response code for error code.");
					return Error.SteamAccountLinked;		
				case 2027:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 409, @"Invalid HTTP response code for error code.");
					return Error.PlayerAlreadyLinkedToSteamAccount;		
				case 1011:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 403, @"Invalid HTTP response code for error code.");
					return Error.MethodDisabled;		
				case 2026:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 409, @"Invalid HTTP response code for error code.");
					return Error.NoSteamAppDetailsHaveBeenConfigured;		
				case 2031:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 409, @"Invalid HTTP response code for error code.");
					return Error.SteamAppIdIncompatibleWithSessionTicket;		
				case 2030:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 422, @"Invalid HTTP response code for error code.");
					return Error.SteamAppDetailsConfiguredIncorrectly;		
				case 2025:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 409, @"Invalid HTTP response code for error code.");
					return Error.SuppliedSteamTicketIsInvalid;		
				case 10002:
					ReleaseAssert.IsTrue(serverResponse.HttpResponseCode == 403, @"Invalid HTTP response code for error code.");
					return Error.AccountRestriction;		
				default:
					return Error.UnexpectedError;
			}
		}
        
        /// <summary>
        /// Extracts the error data json from the given response body.
        /// </summary>
        ///
        /// <returns>The additional error data.<returns/>
        ///
        /// <param name="responseBody">The response body containing the error data.</param>        
        private static MultiTypeValue GetErrorData(IDictionary<string, object> responseBody)
        {
            const string JsonKeyErrorData = "Data";
			
			ReleaseAssert.IsNotNull(responseBody, "The response body cannot be null.");
            
            if (!responseBody.ContainsKey(JsonKeyErrorData))
            {
                return MultiTypeValue.Null;
            }
            
            return new MultiTypeValue(responseBody[JsonKeyErrorData]);
        }
		
		/// <summary>
		/// Gets the error message for the given error code.
		/// </summary>
		///
		/// <returns>The error message.</returns>
		///		
		/// <param name="errorCode">The error code.</param>
		private static string GetErrorDescription(Error errorCode)
		{
			switch (errorCode) 
			{
				case Error.CouldNotConnect:
					return "A connection could not be established.";
				case Error.InvalidRequest:
					return "Invalid Request. One of more of the provided fields were not correctly formatted."
						+ " The data property of the response body will contain specific error messages for"
						+ " each field.";
				case Error.RateLimitReached:
					return "Rate Limit Reached. Too many requests. Player has been rate limited. The data"
						+ " property of the response may contain more details.";
				case Error.TemporaryServiceError:
					return "Temporary Service Error. A temporary error is preventing the request from being"
						+ " processed.";
				case Error.ExpiredConnectAccessToken:
					return "Expired Connect Access Token. The Connect Access Token used to authenticate with"
						+ " the server has expired and should be renewed.";
				case Error.InvalidConnectAccessToken:
					return "Invalid Connect Access Token. The Connect Access Token was not valid and cannot"
						+ " be used to authenticate requests.";
				case Error.SteamAccountLinked:
					return "SteamId Is Already Linked With Another Player. Another Player already has already"
						+ " linked their account with this SteamID.";
				case Error.PlayerAlreadyLinkedToSteamAccount:
					return "This Player is already linked with another Steam Account. A SteamID has already"
						+ " been associated with this Player.";
				case Error.MethodDisabled:
					return "Method Disabled. Public access to this method has been disabled on the"
						+ " ChilliConnect Dashboard.";
				case Error.NoSteamAppDetailsHaveBeenConfigured:
					return "No Steam Details Configured For Project. Steam access details have not been"
						+ " entered into the Dashboard.";
				case Error.SteamAppIdIncompatibleWithSessionTicket:
					return "Incorrect Steam AppID For Provided Session Ticket. The Session Ticket supplied by"
						+ " the user does not authenticate with the Steam App ID specified in Dashboard.";
				case Error.SteamAppDetailsConfiguredIncorrectly:
					return "Steam Details Have Been Configured Incorrectly. Steam Authentication has returned"
						+ " an error code that signals an issue with the AppID and API Key configured in"
						+ " Dashboard.";
				case Error.SuppliedSteamTicketIsInvalid:
					return "Supplied Steam Ticket Could Not Be Authenticated. The Supplied Steam Session"
						+ " Ticket could not be authenticated with Steamworks Servers.";
				case Error.AccountRestriction:
					return "Account Restriction. Account does not have access to this feature, or has"
						+ " exceeded the usage limit.";
				case Error.UnexpectedError:
				default:
					return "An unexpected server error occurred.";
			}
		}
	}
}
