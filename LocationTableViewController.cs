using Foundation;
using System;
using UIKit;
using System.Collections.Generic;
using System.IO;
using MapKit;
using CoreLocation;
using SQLite;
using System.Threading.Tasks;

namespace GSios
{
    public partial class LocationTableViewController : UITableViewController
    {
        private string pathToDatabase;
        static readonly string mapItemCellId = "mapItemCellId2";
        public MKMapView Map { get; set; }
	public List<SavedLocation> SavedLocations { get; set; }
        public UIView ContainerView { get; set; }

	public LocationTableViewController(IntPtr handle) :base(handle)
        {
		SavedLocations = new List<SavedLocation>();
	}

	public override void ViewDidLoad()
	{
		base.ViewDidLoad();

		var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
		pathToDatabase = Path.Combine(documentsFolder, "locations_db.db");

		using (var connection = new SQLite.SQLiteConnection(pathToDatabase))
		{
			connection.CreateTable<SavedLocation>();
		}
                RefreshControl = new UIRefreshControl();
		RefreshControl.ValueChanged += async (sender, e) =>
		{
			await RefreshAsync();
		};
	}

	public override void ViewDidAppear(bool animated)
	{
		base.ViewDidAppear(animated);
		SavedLocations = new List<SavedLocation>();

		using (var connection = new SQLite.SQLiteConnection(pathToDatabase))
		{
			var query = connection.Table<SavedLocation>();

			foreach (SavedLocation mapItem in query)
			{
				SavedLocations.Add(mapItem);
				TableView.ReloadData();
			}

		}
	}

        private async Task RefreshAsync(){
			
            	RefreshControl.BeginRefreshing();
		SavedLocations = new List<SavedLocation>();

		using (var connection = new SQLite.SQLiteConnection(pathToDatabase))
		{
			var query = connection.Table<SavedLocation>();

			foreach (SavedLocation mapItem in query)
			{	
				SavedLocations.Add(mapItem);
				TableView.ReloadData();
			}

		}
		RefreshControl.EndRefreshing();
		TableView.ReloadData();
        }
	public override nint RowsInSection(UITableView tableView, nint section)
	{
		return SavedLocations.Count;
	}

	public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
	{
		UITableViewCell cell = tableView.DequeueReusableCell(mapItemCellId);

		if (cell == null)
			cell = new UITableViewCell();

		var data = SavedLocations[indexPath.Row];

		cell.TextLabel.Text = data.Name;
         	return cell;
	}
	public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, Foundation.NSIndexPath indexPath)
	{
		switch (editingStyle)
		{
			case UITableViewCellEditingStyle.Delete:
			using (var connection = new SQLite.SQLiteConnection(pathToDatabase))
			{
				connection.Delete(SavedLocations[indexPath.Row]);
			}
                    	var annotations = Map.Annotations;
                    	foreach(IMKAnnotation pin in annotations){
                        if(pin.Coordinate.Latitude == SavedLocations[indexPath.Row].Latitude && 
                        	pin.Coordinate.Longitude == SavedLocations[indexPath.Row].Longitude){
                            	Map.RemoveAnnotation(pin);
                        }
       		}
		// remove the item from the underlying data source
		SavedLocations.RemoveAt(indexPath.Row);
                // delete the row from the table
                tableView.DeleteRows(new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
			break;
		case UITableViewCellEditingStyle.None:
		Console.WriteLine("CommitEditingStyle:None called");
			break;
		}
	}
	public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
	{
		return true; // return false if you wish to disable editing for a specific indexPath or for all rows
	}
		
	public override string TitleForDeleteConfirmation(UITableView tableView, NSIndexPath indexPath)
	{   // Optional - default text is 'Delete'
		return "Delete";
	}
	
	public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
	{
		// add item to map
        	var mapItem = SavedLocations[indexPath.Row];
            	CLLocationCoordinate2D coord = new CLLocationCoordinate2D(mapItem.Latitude, mapItem.Longitude);

		Map.SetCenterCoordinate(coord, true);
            	ContainerView.Hidden = true;
	}

        private void RefreshMap(){    
        }
    }
    
    public class SavedLocation
    {

	[PrimaryKey, AutoIncrement]
	public int Id { get; set; }

        public string Name { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }
    }
}
