using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace TerminPlanungsAssistent.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the TerminPlanungsAssistentUser class
    public class TerminPlanungsAssistentUser : IdentityUser
    {
        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string FirstName { get; set; }

        [PersonalData]
        [Column(TypeName = "nvarchar(100)")]
        public string LirstName { get; set; }
    }
}
