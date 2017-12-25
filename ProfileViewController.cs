using System.Data.Sql;
using System.Data.SqlClient;
using System;
using UIKit;
using System.IO;

using SQLite;

namespace GSios
{
    public partial class ProfileViewController : UIViewController
    {
		private string pathToDatabase;
		private string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        private string ProfileName;
		public ProfileViewController(IntPtr handle) : base(handle)
        {
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetUserName();

            ProfileLabel.Text = ProfileName;
            ProfileLabel.TextAlignment = UITextAlignment.Center;

        }
        private void SetUserName(){
            pathToDatabase = Path.Combine(documentsFolder, "users_db.db");
            using (var connection = new SQLiteConnection(pathToDatabase))
            {
				var user = connection.Table<LocalUser>().Where(us => us.LoggedIn == true);
                ProfileName = user.ElementAt(0).Name; 
			}
        }
        partial void SignOutButton_TouchUpInside(UIButton sender)
        {
            pathToDatabase = Path.Combine(documentsFolder, "users_db.db");
			using (var connection = new SQLiteConnection(pathToDatabase))
			{
                var user = connection.Table<LocalUser>().Where(us => us.LoggedIn == true);
                var i = connection.Execute("update LocalUser set LoggedIn = 'false' where Id = @id",user.ElementAt(0).Id);

			}
            DismissViewController(true,null);
        }
    }
	
	
}