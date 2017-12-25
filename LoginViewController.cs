using Foundation;
using System;
using UIKit;
using Xamarin.Auth;
using System.Json;
using SQLite;
using System.IO;
using System.Collections.Generic;
using GSios.Managers;

namespace GSios
{
    public partial class LoginViewController : UIViewController
    {
        private string pathToDatabase;
        private string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        public List<LocalUser> localUsers { get; set; }
        
        public LoginViewController (IntPtr handle) : base (handle)
        {
            localUsers = new List<LocalUser>(); 
        }
        
	public override void ViewDidLoad()
	{
	    base.ViewDidLoad();
            View.BackgroundColor = UIColor.Black;

            //local User DB
	    pathToDatabase = Path.Combine(documentsFolder, "users_db.db");

	    using (var connection = new SQLite.SQLiteConnection(pathToDatabase))
	    {
	       connection.CreateTable<LocalUser>();
               //connection.DeleteAll<LocalUser>(); clear local user DB
	    }
	}
	public override void ViewDidAppear(bool animated)
	{
	    base.ViewDidAppear(animated);
            pathToDatabase = Path.Combine(documentsFolder, "users_db.db");
            int count = 0;
            using (var connection = new SQLiteConnection(pathToDatabase))
            {
               var query = connection.Table<LocalUser>().Where(us => us.LoggedIn == true);
               count = query.Count();
            }
            if (count != 0)
               LoginButton.SendActionForControlEvents(UIControlEvent.TouchUpInside);
            else
               LoginToFaceBook();
	}

        //Facebook Login
	private void LoginToFaceBook()
	{
	    var auth = new OAuth2Authenticator(
	       clientId: "************", //ID hidden 
	       scope: "",
	       authorizeUrl: new Uri("https://m.facebook.com/dialog/oauth/"),
	       redirectUrl: new Uri("http://www.facebook.com/connect/login_success.html"));
	       auth.Completed += Auth_Completed;
	       var ui = auth.GetUI();
	       PresentViewController(ui, true, null);
	}

  	private async void Auth_Completed(object sender, AuthenticatorCompletedEventArgs e)
	{
	    if (e.IsAuthenticated)
	    {
	       var request = new OAuth2Request(
	          "GET",
		  new Uri("https://graph.facebook.com/me?fields=name"),
		  null,
		  e.Account);

	       var fbResponse = await request.GetResponseAsync();
	       var fbUser = JsonValue.Parse(fbResponse.GetResponseText());

	       var name = fbUser["name"];
	       var id = fbUser["id"];

               if(!(IsExistingUser(id)))
	          SaveUserLocally(id,name);
               AddUserToDB(id,name);
	    }
	    
	    DismissViewController(true, null);
        }
        private bool IsExistingUser(long id)
        {
	    pathToDatabase = Path.Combine(documentsFolder, "users_db.db");
            int count = 0;
	    using (var connection = new SQLiteConnection(pathToDatabase))
	    {
               var user = connection.Table<LocalUser>().Where(u => u.UserId == id);
               count = user.Count();
               if(count !=0){
                  var i =  connection.Execute("update LocalUser set LoggedIn = @val where Id = @id", true, user.ElementAt(0).Id);
               }
            }
            if (count == 0)
                return false;
            return true;
        }
        private void AddUserToDB(long id, string FullName)
        {
            UserManager.AddUser(id, FullName);
        }

        private void SaveUserLocally(long id, string name){
	    pathToDatabase = Path.Combine(documentsFolder, "users_db.db");

            using (var connection = new SQLiteConnection(pathToDatabase))
            {
                connection.Insert(new LocalUser()
                {
                    UserId = id,
                    Name = name,
                    LoggedIn = true
                });
            }
        }
     }

     public class LocalUser
     {
        [PrimaryKey, AutoIncrement]
	public int Id { get; set; }

        public long UserId { get; set; }

	public string Name { get; set; }

        public bool LoggedIn { get; set; }

     }
}
