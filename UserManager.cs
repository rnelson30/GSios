using System;
using System.Collections.Generic;
using CoreLocation;
using GSios.DataAccess;
using GSios.Models;


namespace GSios.Managers
{
    public static class UserManager
    {
        //public UserManager()
        //{
        //}
        static int MAX_DISTANCE = 10;

        public static int totalEnclosedMembers(CLLocationCoordinate2D coord, IEnumerable<Coordinate> userLocs){
            int count = 0;
            foreach (Coordinate memberCoord in userLocs){
                if(isEnclosed(coord,memberCoord)){
                    count++;
                }
            }
            return count;
        }
        public static IEnumerable<Coordinate> GetUserLocations(){
            return new UserDataAccess().getUserLocations();
        }

        public static void AddUser(long id, string FullName){
            Console.WriteLine(new UserDataAccess().AddUser(id, FullName));
        }

        //Math to find how many users are within an enclosed region
        private static bool isEnclosed(CLLocationCoordinate2D coord, Coordinate memberCoord){
            int radius = 6371000;
            var lat1 = coord.Latitude * Math.PI / 180.0;
            var lat2 = memberCoord.Latitude * Math.PI / 180.0;
            var deltaLat = (memberCoord.Latitude - coord.Latitude) * Math.PI / 180.0;
            var deltaLong = (memberCoord.Longitude - coord.Longitude) * Math.PI / 180.0;

            var a = Math.Sin(deltaLat / 2.0) * Math.Sin(deltaLat / 2.0) +
                         Math.Cos(lat1) * Math.Cos(lat2) *
                         Math.Sin(deltaLong / 2) * Math.Sin(deltaLong / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            var distance = radius * c;

            if (distance < MAX_DISTANCE) return true;

            return false;


        }
    }
}
