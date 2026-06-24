using System;
using System.ComponentModel.DataAnnotations;
namespace DVDL.Domain.Entities;

public class People	
{
    public int PersonalID { get; set; }
    public int NationalNo { get; set; }
    public string FirstName { get; set; }
    public string SecondName { get; set; }
    public string ThirdName { get; set; }
    public string lastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string Address { get; set; }
    public string phone{ get; set; }
    public string Email { get; set; }
    public int NationalityCountryID { get; set; }
    public string ImagePath { get; set; }

}
