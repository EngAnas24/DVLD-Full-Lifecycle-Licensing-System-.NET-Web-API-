namespace DataAccessLayer.Dtos
{
    public class UserDto
    {
        public int ID { get; set; }
        public int PersonalID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsActive { get; set; }

    }

}