using System;
using CoreLocation;
using MapKit;

namespace GSios.Models
{
    public class CustomAnnotation : MKAnnotation
    {
	string title;
	CLLocationCoordinate2D coord;
        int enclosedMembers;

	public CustomAnnotation(string title, CLLocationCoordinate2D coord, int enclosedMembers)
	{
	    this.title = title;
	    this.coord = coord;
            this.enclosedMembers = enclosedMembers;

	}

	public override string Title
	{
	    get
	    {
		return title;
	    }
	}

	public override CLLocationCoordinate2D Coordinate
	{
	    get
	    {
		return coord;
	    }
	}

        public int EnclosedMembers
        {
            get 
            {
                return enclosedMembers;
            }   
        }
    }
}
