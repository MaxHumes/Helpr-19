using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelprAPI.Models;

namespace HelprAPI.Utility.PostComparers
{
    //class to make comparisons between posts based off of their distance from the user
    public class DistanceComparer : IComparer<PostModel>
    {
        public int Compare(PostModel x, PostModel y)
        {
            if(x.dist_from_user == null && y.dist_from_user == null)
            {
                return 0;
            }
            else if(x.dist_from_user == null)
            {
                return 1;
            }
            else if(y.dist_from_user == null)
            {
                return -1;
            }
            
            if(x.dist_from_user < y.dist_from_user)
            {
                return -1;
            }
            else if(x.dist_from_user == y.dist_from_user)
            {
                return 0;
            }

            return 1;
        }
    }
}
