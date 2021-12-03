using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceResults.MemberMatch
{
    public class Member
    {
        public List<string> FirstList;
        public List<string> LastList;
        public string City;

        public static List<string> ProcessName(string field)
        {
            /* Rules for names:
                Assume no accent marks !!!TODO
                trim spaces from ends !!!TODO
                capitalize everything
                make any middle name part of the first or last name via spaces
                Remove "." and "'"
                ignore any one character names
                Split on hyphens, slashes, and spaces and treat as nicknames
                treat the nickname column as a first name nickname
                remove empty strings
            */
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
