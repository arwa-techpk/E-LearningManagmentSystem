using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Net;
using System.Text;

namespace ELMS.Infrastructure.Zoom
{
    public class ZoomMeetingRequest
    {
        public string Topic { get; set; }
        public string Duration { get; set; }
        public DateTime StartDateTime { get; set; }
        public string Type { get; set; } = "2";
        public string ApiSecret { get; set; }
        public string APIKey { get; set; }
        public string schedule_for { get; set; }
        public string TimeZone { get; set; } = "Asia/Riyadh";
        public string UserId { get; set; }
    }
    public class ZoomMeetingResponse
    {
        public string start_url { get; set; }
        public string join_url { get; set; }

        public string numericStatusCode { get; set; }
    }
    public class ZoomNet
    {
        public ZoomMeetingResponse CreateZoomMeeting(ZoomMeetingRequest zoomMeetingRequest)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var now = DateTime.UtcNow;
            var apiSecret = zoomMeetingRequest.ApiSecret;
            byte[] symmetricKey = Encoding.ASCII.GetBytes(apiSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = zoomMeetingRequest.APIKey,
                Expires = now.AddSeconds(3000),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(symmetricKey), SecurityAlgorithms.HmacSha256),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            var client = new RestClient(String.Format("https://api.zoom.us/v2/users/{0}/meetings", zoomMeetingRequest.UserId));
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(new
            {
                topic = zoomMeetingRequest.Topic,
                duration = zoomMeetingRequest.Duration,
                start_time = zoomMeetingRequest.StartDateTime,
                type = zoomMeetingRequest.Type,
                schedule_for = zoomMeetingRequest.schedule_for,
                timezone = zoomMeetingRequest.TimeZone
            });
            request.AddHeader("authorization", String.Format("Bearer {0}", tokenString));

            IRestResponse restResponse = client.Execute(request);
            HttpStatusCode statusCode = restResponse.StatusCode;
            int numericStatusCode = (int)statusCode;
            var jObject = JObject.Parse(restResponse.Content);
            return new ZoomMeetingResponse()
            {
                start_url = (string)jObject["start_url"],
                join_url = (string)jObject["join_url"],
                numericStatusCode = Convert.ToString(numericStatusCode)
            };
        }
    }
}
