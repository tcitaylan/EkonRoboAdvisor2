// using FluentValidation;
// using RoboServices.Models;
// using RoboServices.Resources;
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading.Tasks;

// namespace RoboServices.Utils
// {
//     public class Validators
//     {
//         public class RegisterValidator : AbstractValidator<Tuple<UsersDto, string, bool>>
//         {
//             public RegisterValidator()
//             {
//                 RuleFor(x => x.Item1.Email).Must(NotNullWhiteSpace).WithMessage(Lang.CannotEmpty).Length(0, 500).WithMessage(Lang.Char).EmailAddress().WithMessage(Lang.ValidAc);
//                 RuleFor(x => x.Item1.Hash).Must(NotNullWhiteSpace).WithMessage(Lang.EmptyPass).Length(6, 30).WithMessage(Lang.PassError).Must(IsValidPassword).WithMessage(Lang.LetterNumber);
//                 RuleFor(x => x.Item1.NameSurname).Must(NotNullWhiteSpace).WithMessage(Lang.NameSurnameNotEmpty).Length(0, 500).WithMessage(Lang.NameSurnameChar);
//                 RuleFor(x => x.Item1.Type).NotNull().WithMessage(Lang.IncorrectCustomerType).Length(0, 500).WithMessage(Lang.IncorrectCustomerType);
//                 RuleFor(x => x.Item3).Equal(true).WithMessage(Lang.VerifyKVVK);
//                 RuleFor(x => x).Must(PassEqual).WithMessage(Lang.PassVerify);
//                 RuleFor(x => x.Item2).Must(NotNullWhiteSpace).WithMessage(Lang.EmptyPass).Length(6, 30).WithMessage(Lang.PassError);
//             }
//         }

//         public class LoginValidator : AbstractValidator<UsersDto>
//         {
//             public LoginValidator()
//             {
//                 RuleFor(x => x.Email).Must(NotNullWhiteSpace).WithMessage(Lang.CannotEmpty).Length(0, 500).WithMessage(Lang.Char).EmailAddress().WithMessage(Lang.ValidAc);
//                 RuleFor(x => x.Hash).Must(NotNullWhiteSpace).WithMessage(Lang.EmptyPass).Length(6, 30).WithMessage(Lang.PassError);

//                 //RuleFor(x => x.Email).Must(NotUniqueUser).WithMessage("Bu email sistemde kayıtlı değil.");

//             }
//         }

//         public class UserExistsValidator : AbstractValidator<string>
//         {
//             public UserExistsValidator()
//             {
//                 RuleFor(x => x).Must(NotNullWhiteSpace).WithMessage(Lang.CannotEmpty).Length(0, 500).WithMessage(Lang.Char).EmailAddress().WithMessage(Lang.ValidAc);

//                 //RuleFor(x => x).Must(NotUniqueUser).WithMessage("Bu email sistemde kayıtlı değil.");
//             }
//         }

//         public class ChangePasswordValidator : AbstractValidator<Tuple<string, string, string>>
//         {
//             public ChangePasswordValidator()
//             {
//                 RuleFor(x => x.Item1).Must(NotNullWhiteSpace).WithMessage(Lang.CannotEmpty).Length(0, 500).WithMessage(Lang.Char).EmailAddress().WithMessage(Lang.ValidAc);

//                 //RuleFor(x => x.Item1).Must(NotUniqueUser).WithMessage("Bu email sistemde kayıtlı değil.");

//                 RuleFor(x => x.Item2).Equal(x => x.Item3).WithMessage(Lang.PassVerify);

//                 RuleFor(x => x.Item2).Must(NotNullWhiteSpace).WithMessage(Lang.EmptyPass).Length(6, 30).WithMessage(Lang.PassError).Must(IsValidPassword).WithMessage(Lang.LetterNumber); ;

//                 RuleFor(x => x.Item3).Must(NotNullWhiteSpace).WithMessage(Lang.EmptyPass).Length(6, 30).WithMessage(Lang.PassError).Must(IsValidPassword).WithMessage(Lang.LetterNumber); ;

//             }
//         }

//         public class ProfileUpdateValidator : AbstractValidator<UsersDto>
//         {
//             public ProfileUpdateValidator()
//             {
//                 RuleFor(x => x.NameSurname).Must(NotNullWhiteSpace).WithMessage(Lang.NameSurnameNotEmpty).Length(0, 500).WithMessage(Lang.NameSurnameChar);
//                 RuleFor(x => x.Type).NotNull().WithMessage(Lang.IncorrectCustomerType).Length(0, 500).WithMessage(Lang.IncorrectCustomerType);
//                 RuleFor(x => x.GSM).NotNull().WithMessage(Lang.GsmNotEmpt).NotEmpty().WithMessage(Lang.ErrorGsm).Length(9, 15).Must(BeNumber).WithMessage(Lang.GsmValid);
//             }
//         }


//         #region Validation Methods
//         public static bool IsValidPassword(string str)
//         {
//             if (str != null)
//             {
//                 var rule1 = str.Any(char.IsLetter);
//                 var rule2 = str.Any(char.IsNumber);

//                 if (rule1 && rule2)
//                 {
//                     return true;
//                 }
//             }

//             return false;
//         }

//         public static bool IsZeroOrOne(int num)
//         {
//             if (num == 0 || num == 1)
//             {
//                 return true;
//             }

//             return false;
//         }

//         public static bool ControlHtml(string htmlString)
//         {
//             List<string> forbiddenCodes = new List<string>() { /*"for(", "while(", "foreach("*/ "<script>", "</script>", "<script" };

//             string tempString = htmlString.Replace(Environment.NewLine, "").Replace(" ", "");

//             foreach (var item in forbiddenCodes)
//             {
//                 if (tempString.Contains(item))
//                 {
//                     return false;
//                 }
//             }

//             return true;
//         }

//         public static bool NotNullWhiteSpace(string str)
//         {
//             return (!string.IsNullOrWhiteSpace(str));
//         }

//         public static bool BeNumber(string str)
//         {
//             if (!string.IsNullOrWhiteSpace(str))
//             {
//                 foreach (char c in str)
//                 {
//                     if (c < '0' || c > '9')
//                         return false;
//                 }
//             }

//             return true;
//         }

//         public static bool IsBinary(int? integer)
//         {
//             return integer != null && (integer == 0 || integer == 1);
//         }

//         public static bool PassEqual(Tuple<UsersDto, string, bool> tup)
//         {
//             return tup.Item1.Hash == tup.Item2;
//         }

//         public static bool IsASCII(string value)
//         {
//             return Encoding.UTF8.GetByteCount(value) == value.Length;
//         }

//         public static char[] startingChars = new char[] { '<', '&' };

//         public static bool IsDangerousString(string s)
//         {
//             int startIndex = 0;
//             while (true)
//             {
//                 int num2 = s.IndexOfAny(startingChars, startIndex);

//                 if (num2 < 0)
//                 {
//                     return false;
//                 }
//                 if (num2 == (s.Length - 1))
//                 {
//                     return false;
//                 }

//                 char ch = s[num2];
//                 if (ch != '&')
//                 {
//                     if ((ch == '<') && ((IsAtoZ(s[num2 + 1]) || (s[num2 + 1] == '!')) || ((s[num2 + 1] == '/') || (s[num2 + 1] == '?'))))
//                     {
//                         return true;
//                     }
//                 }
//                 else if (s[num2 + 1] == '#')
//                 {
//                     return true;
//                 }
//                 startIndex = num2 + 1;
//             }
//         }

//         public static bool IsAtoZ(char c)
//         {
//             return (((c >= 'a') && (c <= 'z')) || ((c >= 'A') && (c <= 'Z')));
//         }
//         #endregion
//     }
// }
