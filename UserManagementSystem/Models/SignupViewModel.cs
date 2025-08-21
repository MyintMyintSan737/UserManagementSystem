namespace UserManagementSystem.Models
{
    public class SignupViewModel
    {
        public Users NewUser { get; set; } = new Users();
        public List<Users> UserList { get; set; } = new List<Users>();
    }
}
