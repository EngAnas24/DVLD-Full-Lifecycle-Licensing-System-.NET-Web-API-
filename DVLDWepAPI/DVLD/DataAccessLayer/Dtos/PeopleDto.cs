using Entites;
using System;

namespace DataAccessLayer.Dtos
{
    public class PeopleDto
    {
            public int PersonalID { get; set; }
            public int NationalNo { get; set; }
            public string FullName { get; set; }
            public string FirstName { get; set; }
            public string SecondName { get; set; }
            public string ThirdName { get; set; }
            public string LastName { get; set; }
            public DateTime DateOfBirth { get; set; }
            public string GenderName { get; set; }   // من الـ Proc (Male/Female)
            public string Gender { get; set; }   // من الـ Proc (Male/Female)
            public string Address { get; set; }
            public string Phone { get; set; }
            public string Email { get; set; }
            public int NationalityCountryID { get; set; }
            public string CountryName { get; set; }  // من الـ Join
            public string ImagePath { get; set; }

    }
    // ... أي حقول أخرى للعرض
}

