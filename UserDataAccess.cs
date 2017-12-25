using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using GSios.Models;
using Newtonsoft.Json;

namespace GSios.DataAccess
{
    public class UserDataAccess
    {
        public UserDataAccess()
        {
        }
	
	//Get coordinaes of all users
	public IEnumerable<Coordinate> getUserLocations()
	{

	    JsonValue json2 = GetCoords("http://gsios.azurewebsites.net/location/userLocations");
	    var coords = JsonConvert.DeserializeObject<IEnumerable<Coordinate>>(json2.ToString());
	    return coords;
	}
 
	//Add user to DB
        public async Task<bool> AddUser(long id, string FullName)
        {
            var client = new HttpClient();
            var model = new RegistrationModel()
            {
                UserName = "username",
                Password = "password",
                Latitude = 2343225,
                Longitude = 2323562,
                PhoneNumber = 23423452
            };
            var json = JsonConvert.SerializeObject(model);
            HttpContent content = new StringContent(json);
            content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

            var response = await client.PostAsync("http://gsios.azurewebsites.net/users/Register", content);

            return response.IsSuccessStatusCode;

        }
        
        //Get all coordinates 
	private JsonValue GetCoords(string url)
	{
	    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
	    request.ContentType = "application/json";
	    request.Method = "GET";

	    using (WebResponse response = request.GetResponseAsync().GetAwaiter().GetResult())
	    {
	        using (Stream stream = response.GetResponseStream())
		{
		    JsonValue jsonDoc = JsonObject.Load(stream);
		    Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());
		    return jsonDoc;
		}
	    }
	}
    }
}
