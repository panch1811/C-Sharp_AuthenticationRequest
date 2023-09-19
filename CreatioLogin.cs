using System;
using System.IO;
using System.Net;


namespace AuthenticationRequest
{
  class CreatioLogin
  {
    private readonly string _appUrl;
    private CookieContainer _authCookie;
    private readonly string _authServiceUrl;
    private readonly string _userName;
    private readonly string _userPassword;
    public CreatioLogin(string appUrl, string userName, string userPassword)
    {
      _authCookie = new CookieContainer();
      _appUrl = appUrl;
      _authServiceUrl = _appUrl + @"/ServiceModel/AuthService.svc/Login";
      _userName = userName;
      _userPassword = userPassword;
    }
    public CookieContainer TryLogin()
    {
      var authData = @"{
                    ""UserName"":""" + _userName + @""",
                    ""UserPassword"":""" + _userPassword + @"""
                }";
      var request = CreateRequest(_authServiceUrl, authData, "POST");
      request.CookieContainer = _authCookie;
      using (var response = (HttpWebResponse)request.GetResponse())
      {
        if (response.StatusCode == HttpStatusCode.OK)
        {
          using (var reader = new StreamReader(response.GetResponseStream()))
          {
            var responseMessage = reader.ReadToEnd();
            Console.WriteLine(responseMessage);
            if (responseMessage.Contains("\"Code\":1"))
            {
              throw new UnauthorizedAccessException($"Unauthorized {_userName} for {_appUrl}");
            }
          }
          string authName = ".ASPXAUTH";
          string? authCookieValue = response.Cookies[authName]?.Value;

          if (authCookieValue == null)
          {
            Console.WriteLine("Auth Cookie Value is null.");
          }

          _authCookie.Add(new Uri(_appUrl), new Cookie(authName, authCookieValue));
        }
      }
      return _authCookie;
    }

    public HttpWebRequest CreateRequest(string url, string requestData, string method)
    {
      HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
      request.ContentType = "application/json";
      request.Method = method;
      request.KeepAlive = true;
      if (!string.IsNullOrEmpty(requestData))
      {
        using (var requestStream = request.GetRequestStream())
        {
          using (var writer = new StreamWriter(requestStream))
          {
            writer.Write(requestData);
          }
        }
      }
      return request;
    }
  }
}