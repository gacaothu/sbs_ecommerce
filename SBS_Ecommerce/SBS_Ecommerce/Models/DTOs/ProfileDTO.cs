namespace SBS_Ecommerce.Models.DTOs
{
    public class Profile
    {
        public int Profile_ID { get; set; }
        public int Company_ID { get; set; }
        public object Name { get; set; }
        public string First_Name { get; set; }
        public object Middle_Name { get; set; }
        public string Last_Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Nationality { get; set; }
        public string Mobile_No { get; set; }
        public string DOB { get; set; }
        public object Gender { get; set; }
        public object NRIC { get; set; }
        public object Remark { get; set; }
        public object Address { get; set; }
        public object Postal_Code { get; set; }
        public string Employee_No { get; set; }
        public string Country { get; set; }
    }

    public class ProfileDTO
    {
        public Profile Items { get; set; }
        public int Return_Code { get; set; }
        public int Rows_Count { get; set; }
        public string Msg { get; set; }
    }
}