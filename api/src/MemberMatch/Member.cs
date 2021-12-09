using System;
using System.Collections.Generic;
using System.Linq;

namespace RaceResults.MemberMatch
{
    public class Member
    {
        public Member(List<string> firstList, List<string> lastList, string city)
        {
            FirstList = firstList ?? throw new ArgumentNullException(nameof(firstList));
            LastList = lastList ?? throw new ArgumentNullException(nameof(lastList));
            City = city ?? throw new ArgumentNullException(nameof(city));
        }

        // TODO Need code for when City is unknown
        public List<string> FirstList { get; }

        public List<string> LastList { get; }

        public string City { get; }

        /// <summary>
        /// This takes a string (for example, one field from a *.tsv file)
        /// and turns it into a list of canonical names. For example, a nickname
        /// field of "Billy/Bill/Will" would turn into the list
        /// "BILLY","BILL","WILL".
        /// </summary>
        /// <param name="field">
        /// a string (for example, one field from a *.tsv file).
        /// </param>
        /// <returns>List of canonical names.</returns>
        /// <remarks>
        ///    Rules for names:
        ///        * assume no accent marks
        ///         * make any middle name part of the first or last name via spaces
        ///    Processing:
        ///        * trim spaces from ends
        ///        * capitalize everything
        ///        * remove "." and "'"
        ///        * ignore any one character names
        ///        * split on hyphens, slashes, and spaces
        ///        * remove empty strings.
        /// </remarks>
        public static List<string> ProcessName(string field)
        {
            field = field.ToUpperInvariant().Trim();

            // TODO what about other single-quote like characters such as back quote
            field = field.Replace(".", string.Empty).Replace("'", string.Empty);

            // TODO and all whitespace?
            string[] names = field.Split(new[] { '-', ' ', '/' }, System.StringSplitOptions.RemoveEmptyEntries);
            var names2 =
                (from name in names
                 where name.Length > 1
                 select name).ToList();
            return names2;
        }

        public override string ToString()
        {
            return $"{string.Join("/", this.FirstList)} {string.Join("/", this.LastList)} @ {this.City}";
        }
    }
}
