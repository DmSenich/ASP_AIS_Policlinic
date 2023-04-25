using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ASP_AIS_Policlinic.Areas.Identity.Data;

// Add profile data for application users by adding properties to the IdentityUser class
public class PoliclinicUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? ModelId { get; set; }
}

