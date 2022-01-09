namespace RaceResults.Api.MemberProviders.WildApricot
{
    public struct WildApricotMember
    {
        public int Id { get; set; }

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
