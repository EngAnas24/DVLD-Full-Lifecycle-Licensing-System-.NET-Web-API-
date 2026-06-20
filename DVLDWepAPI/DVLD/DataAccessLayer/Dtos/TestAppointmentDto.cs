namespace DataAccessLayer.Dtos
{
    public class TestAppointmentDto
    {
        public int TestAppointmentID { get; set; }
        public decimal PaidFees { get; set; }
        public int LocalDrivingLicenseApplicationID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool IsLocked { get; set; }
        public int RetakeTestApplicationID { get; set; }

    }

    public class TestResultResult
    {
        public int returnNum { get; set; }
        public bool TestResult { get; set; }

    }
    public class TestQueryParams
    {
        public int ID { get; set; }
        public int AppID { get; set; }
        public int TypeID { get; set; }
    }
}
