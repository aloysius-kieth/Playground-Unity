using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace TRINAX.Utils {
    public class CredentialsValidator : MonoBehaviour {

        const int MAX_MOBILE_LENGTH = 8;

        public static bool validateName(string _name) {
            if (!string.IsNullOrEmpty(_name)) {
                return true;
            }
            return false;
        }
        public static bool validateMobile(string _mobile) {
            if (!string.IsNullOrEmpty(_mobile) && _mobile.Length == MAX_MOBILE_LENGTH) {
                return true;
            }
            return false;
        }
        //public static bool validateEmail(string _email) {
        //    if (!string.IsNullOrEmpty(_email)) {
        //        Debug.Log(_email.Replace("@", "").Length);
        //        int countSymbol = _email.Length - _email.Replace("@", "").Length;
        //        if (countSymbol == 1 && _email.Contains(".") && _email.IndexOf('@') != 0 && !_email.Contains(" ")) {
        //            return true;
        //        }
        //    }
        //    return false;
        //}
        public static bool validateEmail(string email) {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try {
                // Normalize the domain
                email = Regex.Replace(email, @"(@)(.+)$", DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));

                // Examines the domain part of the email and normalizes it.
                string DomainMapper(Match match) {
                    // Use IdnMapping class to convert Unicode domain names.
                    var idn = new IdnMapping();

                    // Pull out and process domain name (throws ArgumentException on invalid)
                    string domainName = idn.GetAscii(match.Groups[2].Value);

                    return match.Groups[1].Value + domainName;
                }
            }
            catch (RegexMatchTimeoutException e) {
                return false;
            }
            catch (ArgumentException e) {
                return false;
            }

            try {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException) {
                return false;
            }
        }
    }
}