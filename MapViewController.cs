using Foundation;
using System;
using UIKit;
using CoreLocation;
using MapKit;
using CoreGraphics;
using System.Collections.Generic;
using System.IO;
using GSios.Models;
using GSios.Managers;

namespace GSios
{
    public partial class MapViewController : UIViewController
    {
        bool locsHide = true;
        MKMapView map;
        MyMapDelegate mapDel;
        UISearchController searchController;
        CLLocationManager locationManager = new CLLocationManager();
        LocationTableViewController locationTableViewController;
        SearchResultsViewController searchResultsController;

        partial void GroupsButton_TouchUpInside(UIButton sender)
        {
        }

        partial void LocationsButton_TouchUpInside(UIButton sender)
        {
            locsHide = !locsHide;
            LocationContainer.Hidden = locsHide;
        }

        public MapViewController(IntPtr handle) : base(handle)
        {
            map = new MKMapView(UIScreen.MainScreen.Bounds);
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            locationManager.RequestWhenInUseAuthorization();
            View.UserInteractionEnabled = true;
            LocationContainer.Hidden = locsHide;

            // Create Map 
			mapDel = new MyMapDelegate();
			map.Delegate = mapDel;

			map.MapType = MKMapType.Standard;
			map.ShowsUserLocation = true;
			
            View = map;
            View.AddSubview(SearchView);
            View.AddSubview(LocationContainer);


            // set map center and region
            const double lat = 43.161030;
            const double lon = -77.610924;
            var mapCenter = new CLLocationCoordinate2D(lat, lon);
            var mapRegion = MKCoordinateRegion.FromDistance(mapCenter, 20000, 20000);
            map.CenterCoordinate = mapCenter;
            map.Region = mapRegion;
           

            //Create Serach Bar
            searchResultsController = new SearchResultsViewController(map);
            var searchUpdater = new SearchResultsUpdator();
            searchUpdater.UpdateSearchResults += searchResultsController.Search;

            //add the search controller
            searchController = new UISearchController(searchResultsController)
            {
                SearchResultsUpdater = searchUpdater
            };


            SearchView.AddSubview(searchController.SearchBar);
            SearchView.BackgroundColor = new UIColor(255, 255, 255, (System.nfloat).20);

            searchController.SearchBar.SizeToFit();
            searchController.SearchBar.SearchBarStyle = UISearchBarStyle.Minimal;
            searchController.SearchBar.Placeholder = "Find Locations You Want To Save";

            searchController.HidesNavigationBarDuringPresentation = false;


            DefinesPresentationContext = true;

        }

        public override void TouchesBegan(NSSet touches,UIEvent evt){
            base.TouchesBegan(touches,evt);
            UITouch touch = touches.AnyObject as UITouch;
            if(touch != null){
                locsHide = true;
                LocationContainer.Hidden = locsHide;
            }
        }

        public override void ViewWillAppear(bool animated){
            base.ViewWillAppear(animated);
            PinSavedLocations(map);
        }

        public class SearchResultsUpdator : UISearchResultsUpdating
        {
            public event Action<string> UpdateSearchResults = delegate { };

            public override void UpdateSearchResultsForSearchController(UISearchController searchController)
            {
                this.UpdateSearchResults(searchController.SearchBar.Text);
            }
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            base.PrepareForSegue(segue, sender);

            if (segue.Identifier == "LocationsSegue")
            {
                locationTableViewController = (LocationTableViewController)segue.DestinationViewController;
                locationTableViewController.Map = map;
                locationTableViewController.ContainerView = LocationContainer;
            }
        }

		private void PinSavedLocations(MKMapView map)
		{

			var MapItems = new List<SavedLocation>();
			var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			string pathToDatabase = Path.Combine(documentsFolder, "locations_db.db");

			using (var connection = new SQLite.SQLiteConnection(pathToDatabase))
			{
				var query = connection.Table<SavedLocation>();

				foreach (SavedLocation mapItem in query)
				{
					MapItems.Add(mapItem);
				}
			}
			var userLocations = UserManager.GetUserLocations();

			foreach (SavedLocation item in MapItems)
			{
				CLLocationCoordinate2D coord = new CLLocationCoordinate2D()
				{
					Longitude = item.Longitude,
					Latitude = item.Latitude
				};
				var userCount = getUserCount(coord, userLocations);
				var customItem = new CustomAnnotation(item.Name, coord, userCount);
				map.AddAnnotation(customItem);
			}
		}

		private int getUserCount(CLLocationCoordinate2D coord, IEnumerable<Coordinate> userLocs)
		{
			return UserManager.totalEnclosedMembers(coord, userLocs);
		}

        class MyMapDelegate : MKMapViewDelegate
        {
            string pId = "PinAnnotation";
            string cId = "CustomAnnotation";
            string pathToDatabase;
            UIButton AddLocation; 

            public override MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
            {
                MKAnnotationView anView;

                if (annotation is MKUserLocation)
                    return null;

                if (annotation is CustomAnnotation)
                {
                    
                    // show monkey annotation
                    anView = mapView.DequeueReusableAnnotation(cId);

                    if (anView == null)
                        anView = new MKAnnotationView(annotation, cId);
                    
                    UILabel lbl = new UILabel(new CGRect(0, 0, 30, 30))
                    {
                        Text = ((CustomAnnotation)annotation).EnclosedMembers.ToString()
                    };

                    lbl.Layer.MasksToBounds = true;
                    lbl.Layer.CornerRadius = lbl.Frame.Width / 2;

                    lbl.TextAlignment = UITextAlignment.Center;
                    lbl.TextColor = UIColor.White;
                    lbl.AdjustsFontSizeToFitWidth = true;
                    lbl.BackgroundColor = UIColor.Blue;

                    anView.Frame = new CGRect()
                    {
                        Width = 30,
                        Height = 30
                    };
                    anView.Add(lbl);
                    anView.CanShowCallout = true;

                }
                else
                {
					var documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
					pathToDatabase = Path.Combine(documentsFolder, "locations_db.db");

					AddLocation = UIButton.FromType(UIButtonType.ContactAdd);
					AddLocation.TouchUpInside += (sender, ea) =>
					{
						using (var connection = new SQLite.SQLiteConnection(pathToDatabase))
						{
                            connection.Insert(new SavedLocation()
                            {
                                Name = annotation.GetTitle(),
								Latitude = annotation.Coordinate.Latitude,
								Longitude = annotation.Coordinate.Longitude
							});
                            connection.Close();
						}
                        mapView.RemoveAnnotation(annotation);
                    };
                    
                    // show pin annotation
                    anView = (MKPinAnnotationView)mapView.DequeueReusableAnnotation(pId);

                    if (anView == null)
                        anView = new MKPinAnnotationView(annotation, pId);

                    ((MKPinAnnotationView)anView).PinColor = MKPinAnnotationColor.Red;
                    anView.CanShowCallout = true;
                    anView.RightCalloutAccessoryView = AddLocation;
                }

                return anView;
            }
        }

	}

}