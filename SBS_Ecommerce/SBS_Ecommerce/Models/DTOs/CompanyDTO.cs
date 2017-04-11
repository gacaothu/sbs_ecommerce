namespace SBS_Ecommerce.Models.DTOs
{
    public class CompanyDTO
    {
        public Company Items { get; set; }
        public int Return_Code { get; set; }
        public int Rows_Count { get; set; }
        public string Msg { get; set; }
    }

    public class Company
    {
        public int Company_ID { get; set; }
        public string Name { get; set; }
        public string GST_Registration { get; set; }
        public string Address { get; set; }
        public int Country_ID { get; set; }
        public int State_ID { get; set; }
        public string Zip_Code { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public int Currency_ID { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string Language_Code { get; set; }
        public string Time_Zone_Id { get; set; }
    }
}