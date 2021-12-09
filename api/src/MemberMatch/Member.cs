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
        ///        * capitalize everything
        ///        * trim spaces from ends
        ///        * remove "." and "'"
        ///        * split on hyphens, slashes, and spaces
        ///        * remove empty strings.
        ///        * ignore any one character names
        /// </remarks>
        public static List<string> ProcessName(string field)
        {
            var nameList =
                field.ToUpperInvariant().Trim().

                // TODO what about other single-quote like characters such as back quote
                Replace(".", string.Empty).Replace("'", string.Empty).

                // TODO and all whitespace?
                Split(new[] { '-', ' ', '/' }, System.StringSplitOptions.RemoveEmptyEntries).
                Where(name => name.Length > 1).ToList();
            return nameList;
        }

        public override string ToString()
        {
            return $"{string.Join("/", this.FirstList)} {string.Join("/", this.LastList)} @ {this.City}";
        }
    }
}
