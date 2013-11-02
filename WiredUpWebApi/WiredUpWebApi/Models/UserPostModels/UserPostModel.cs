using System;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;

namespace WiredUpWebApi.Models.UserPostModels
{
    [DataContract]
    public class UserPostModel
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "postedBy")]
        public string PostedBy { get; set; }

        [DataMember(Name = "postDate")]
        public DateTime PostDate { get; set; }

        public static Expression<Func<UserPost, UserPostModel>> FromUserPost
        {
            get
            {
                return post => new UserPostModel()
                {
                    Id = post.Id,
                    Content = post.Content,
                    PostedBy = post.User.FirstName + " " + post.User.LastName,
                    PostDate = post.PostDate
                };
            }
        }
    }
}