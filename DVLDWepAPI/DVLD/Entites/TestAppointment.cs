namespace Entites
{
    public class TestAppointment
    {
        public int TestAppointmentID { get; set; }
        public int TestTypeID { get; set; }
        public byte LocalDrivingLicenseApplicationID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public int CreatedByUserID { get; set; }
        public byte IsLocked { get; set; }
        public decimal PaidFees { get; set; }
        public int RetakeTestApplicationID { get; set; }

    }
}
