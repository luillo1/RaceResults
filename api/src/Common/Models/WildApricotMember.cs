using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RaceResults.Common.Models
{
    public struct WildApricotMember
    {
  public int id { get; set; }
  public string Url { get; set; }
  public string FirstName { get; set; }
  public string LastName { get; set; }
  public string Organization { get; set; }
  public string Email { get; set; }
  public string Phone { get; set; }
  public bool TermsOfUseAccepted { get; set; }
  public bool HasAvailableUserCard { get; set; }
  public string MembershipStateDescription { get; set; }
  public bool IsRecurringPaymentsActive { get; set; }
    }
}
