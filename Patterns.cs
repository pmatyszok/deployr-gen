using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace deployRGen
{
    class Patterns
    {
        /// <summary>
        /// Should match:
        /// - deployrUtils::deployrInput(...)
        /// - deployrInput(...)
        /// and get ... as json named group.
        /// </summary>
        public static readonly Regex InputDefinition = new Regex(
            @"(deployrUtils::)?deployrInput\((?<json>[-'\s:{}""a-zA-Z0-9_,\.\[\]]+)\)", RegexOptions.Compiled);

        /// <summary>
        /// Should match "name" : "xxx" and get xxx as name named group
        /// </summary>
        public static readonly Regex NameDefinition = new Regex(@"\s*\""name\""\s*:\s*\""(?<name>\w+)\""",
            RegexOptions.Compiled);

        /// <summary>
        /// Should match "render" : "xxx" and get xxx as type named group (will work for "data.frame", too).
        /// </summary>
        public static readonly Regex TypeDefinition = new Regex(@"\s*\""render\""\s*:\s*\""(?<type>[\w\.]+)\""",
            RegexOptions.Compiled);

        public static readonly Regex DefaultDefinition = new Regex(@"\s*\""default\""\s*:\s*\""(?<value>[^\""]+)\""");
    }
}
