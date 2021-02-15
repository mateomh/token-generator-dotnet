using System;
// using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
// using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace token_generator_dotnet.Contollers {
  [ApiController]
  [Route("/")]
  public class TokenGeneratorController : ControllerBase
  {
    [HttpGet]
    public string GetToken()
    {
      string userName =  Request.Headers["user"];

      Console.WriteLine($"User: {userName}");

      string generatedToken = GenerateToken(userName, 10000);
      Token token = new() {
        token = generatedToken
      };

      var response = JsonSerializer.Serialize(token);
      return response;
    }

    private record Token
    {
      public string token { get; init; }
    }

    private string GenerateToken(string userName, int expiresInSecs)
    {
      if (userName == null){
        userName = "Guest";
      }

      long EPOCH_SECONDS = 62167219200;
      string expires = "";

      TimeSpan timeSinceEpoch = DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0));
      expires = (Math.Floor(timeSinceEpoch.TotalSeconds) + EPOCH_SECONDS + expiresInSecs).ToString();
 
      string appID = Environment.GetEnvironmentVariable("APP_ID");
      string key = Environment.GetEnvironmentVariable("APP_KEY");

      string jid = userName + "@" + appID;
      string body = "provision" + "\0" + jid + "\0" + expires + "\0" + "";

      var encoder = new UTF8Encoding();
      var hmacsha = new HMACSHA384(encoder.GetBytes(key));
      byte[] mac = hmacsha.ComputeHash(encoder.GetBytes(body));

      // macBase64 can be used for debugging
      //string macBase64 = Convert.ToBase64String(hashmessage);

      // Get the hex version of the mac
      string macHex = BytesToHex(mac);

      string serialized = body + '\0' + macHex;

      string token = Convert.ToBase64String(encoder.GetBytes(serialized));

      return token ;
    }

    private static string BytesToHex(byte[] bytes)
    {
        var hex = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            hex.AppendFormat("{0:x2}", b);
        }
        return hex.ToString();
    }
  }
}