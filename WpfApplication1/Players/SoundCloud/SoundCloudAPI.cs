using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PlayServer.Players.SoundCloud
{
    class SoundCloudAPI
    {
        private WebClient client;
        private string json;
        private ClientClass _clientClass;

        public SoundCloudAPI()
        {

            client = new WebClient();
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                var name = "PlayServer.Players.SoundCloud.ClientInfo.json";

                using (Stream stream = assembly.GetManifestResourceStream(name))
                using (StreamReader read = new StreamReader(stream))
                {
                    json = read.ReadToEnd();
                }
            }


            catch (FileNotFoundException ex)
            {
                Console.WriteLine("Json file not found");
            }

            _clientClass = ServiceStack.Text.JsonSerializer.DeserializeFromString<ClientClass>(json);
        }

        public bool TryToLogin()
        {


            bool isLoggedIn = false;
            string token = Login();

            if (token != string.Empty)
            {
                isLoggedIn = true;
                Console.WriteLine(token);
            }

            return isLoggedIn;
        }

        private string Login()
        {
            string token = string.Empty;

            string soundCloudTokenRes = "https://api.soundcloud.com/oauth2/token";
           
            //Authentication data
            string postData = "client_id=" + _clientClass.ClientID
                + "&client_secret=" + _clientClass.ClientSecret
                + "&grant_type=password&username=" + _clientClass.Username
                + "&password=" + _clientClass.Password;

     



            //Authentication

            string tokenInfo = client.UploadString(soundCloudTokenRes, postData);

            return token;

        }
    }


    public class ClientClass
    {
        private string mClientID;
        private string mClientSecret;
        private string mUserName;
        private string mPassword;

        public string ClientID { get { return mClientID; } set { mClientID = value; } }
        public string ClientSecret { get { return mClientSecret; } set { mClientSecret = value; } }

        public string Username { get { return mUserName; } set { mUserName = value; } }
        public string Password { get { return mPassword; } set { mPassword = value; } }


    }
}
