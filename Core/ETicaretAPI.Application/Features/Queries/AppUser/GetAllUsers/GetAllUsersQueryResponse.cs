namespace ETicaretAPI.Application.Features.Queries.AppUser.GetAllUsers
{
    public class GetAllUsersQueryResponse
    {
        public int TotalCount { get; set; }
        public object Users { get; set; }
    }
}