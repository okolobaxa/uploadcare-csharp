﻿using System;
using System.Net;
using Uploadcare.Data;
using Uploadcare.Url;

namespace Uploadcare.API
{
	/// <summary>
	/// Uploadcare API client.
	/// 
	/// Provides simple access to {@code UploadcareFile} and {@code Project} resources.
	/// </summary>
	public sealed class Client
	{
		private readonly string _publicKey;
		private readonly string _privateKey;
		private readonly bool _simpleAuth;
	    private readonly RequestHelper _requestHelper;

	    /// <summary>
		/// Initializes a client with custom access keys.
		/// Can use simple or secure authentication.
		/// </summary>
		/// <param name="publicKey"> Public key </param>
		/// <param name="privateKey"> Private key </param>
		/// <param name="simpleAuth"> If {@code false}, HMAC-based authentication is used </param>
		public Client(string publicKey, string privateKey, bool simpleAuth = true)
		{
			_publicKey = publicKey;
			_privateKey = privateKey;
			_simpleAuth = simpleAuth;
            _requestHelper = new RequestHelper(this);
		}

		/// <summary>
		/// Creates a client with demo credentials.
		/// Useful for tests and anonymous access.
		/// 
		/// <b>Warning!</b> Do not use in production.
		/// All demo account files are eventually purged.
		/// </summary>
		/// <returns> A demo client </returns>
		public static Client DemoClient()
		{
			return new Client("demopublickey", "demoprivatekey");
		}

		/// <summary>
		/// Returns the public key.
		/// </summary>
		/// <returns> Public key </returns>
		public string PublicKey
		{
			get
			{
				return _publicKey;
			}
		}

		/// <summary>
		/// Returns the private key.
		/// </summary>
		/// <returns> Private key </returns>
		public string PrivateKey
		{
			get
			{
				return _privateKey;
			}
		}

		/// <summary>
		/// Returns {@code true}, if simple authentication is used.
		/// </summary>
		/// <returns> {@code true}, if simple authentication is used, {@code false} otherwise </returns>
		public bool SimpleAuth
		{
			get
			{
				return _simpleAuth;
			}
		}

        public RequestHelper GetRequestHelper()
        {
            return _requestHelper;
        }

		/// <summary>
		/// Requests project info from the API.
		/// </summary>
		/// <returns> Project resource </returns>
		public Project Project
		{
			get
			{
				var url = Urls.ApiProject();

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                var result = _requestHelper.ExecuteQuery(request, true, new ProjectData());

			    return new Project(this, result);
			}
		}

		/// <summary>
		/// Requests file data.
		/// </summary>
		/// <param name="fileId"> Resource UUID </param>
		/// <returns> UploadcareFile resource </returns>
		public UploadcareFile GetFile(Guid fileId)
		{
			var url = Urls.ApiFile(fileId);
            
            var request = (HttpWebRequest)WebRequest.Create(url);
		    request.Method = "GET";

		    var result = _requestHelper.ExecuteQuery(request, true, new FileData());

		    return new UploadcareFile(this, result);
		}

	    /// <summary>
		/// Marks a file as deleted.
		/// </summary>
		/// <param name="fileId"> Resource UUID </param>
		public void DeleteFile(Guid fileId)
		{
            var url = Urls.ApiFile(fileId);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "DELETE";

            _requestHelper.ExecuteCommand(request, true);
		}

		/// <summary>
		/// Marks a file as saved.
		/// 
		/// This has to be done for all files you want to keep.
		/// Unsaved files are eventually purged.
		/// </summary>
		/// <param name="fileId"> Resource UUID </param>
		public void SaveFile(Guid fileId)
		{
            var url = Urls.ApiFileStorage(fileId);

            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";

            _requestHelper.ExecuteCommand(request, true);
		}
	}

}