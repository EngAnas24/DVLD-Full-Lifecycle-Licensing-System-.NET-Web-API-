using DVDL.Domain.Entities;
using DAL.DataAccessLayer.Common;
using DataAccessLayer.Dtos;
using Entites;
namespace BusinessLayer.Services
    {
        public class DriverService
    {
            public List<DataAccessLayer.Dtos.DriverDto> GetAllDriver()
            {
                return SqlHelper.ExecuteQueryAll<DataAccessLayer.Dtos.DriverDto>("SPGetAllDrivers");
            }
            public DataAccessLayer.Dtos.DriverDto GetDriverByDriverID(int id)
            {
                int returnedId;

                var Driver = SqlHelper.ExecuteeQuerySingl<DataAccessLayer.Dtos.DriverDto>(
                    "SPGetDriverByDriverID",
                    new { ID = id },
                    out returnedId
                );

                return Driver;
            }
        public DataAccessLayer.Dtos.DriverDto GetDriverByPersonID(int id)
        {
            int returnedId;

            var Driver = SqlHelper.ExecuteeQuerySingl<DataAccessLayer.Dtos.DriverDto>(
                "SPGetDriverByPersonID",
                new { ID = id },
                out returnedId
            );

            return Driver;
        }
        public int InsertDriver(Driver Driver)
            {
                return SqlHelper.ExecuteNonQuery("[SPInsertUpdateDriver]", Driver, true);
            }
            public int UpdateDriver(Driver Driver)
            {
                return SqlHelper.ExecuteNonQuery("[SPInsertUpdateDriver]", Driver, isInsert: false);
            }
            public int DeleteDriver(int id)
            {
                return SqlHelper.ExecuteNonQuery("SPDeleteDriver", new { DriveralID = id });
            }
         
        }

    }



