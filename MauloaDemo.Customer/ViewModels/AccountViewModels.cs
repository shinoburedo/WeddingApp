using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MauloaDemo.Utilities;

namespace MauloaDemo.Customer.ViewModels
{
    public class LoginViewModel
    {
        //[Required]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        public bool Registered { get; set; }

        public bool DisplayNavigation { get; set; }

        public void ValidateEmail(string language) {

            if (string.IsNullOrEmpty(this.Email)) {
                if (language == "J") {
                    throw new ArgumentNullException(String.Empty, "Ｅメールアドレスを入力してください。");
                } else {
                    throw new ArgumentNullException(String.Empty, "Please enter your Email address.");
                }
            } else {
                if (!System.Text.RegularExpressions.Regex.IsMatch(
                    this.Email,
                    @"\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase)) {
                    if (language == "J") {
                        throw new ArgumentNullException(String.Empty, "Eメールアドレスを正しく入力してください。");
                    } else {
                        throw new ArgumentNullException(String.Empty, "Please enter correct Email address.");
                    }
                }
                if (this.Email.Length > 200) {
                    if (language == "J") {
                        throw new ArgumentNullException(String.Empty, "Eメールアドレスは200文字以内で入力してください。");
                    } else {
                        throw new ArgumentNullException(String.Empty, "Please enter your Email address in 200 characters or less.");
                    }
                }
            }

        }

        public void ValidateLogin(string language) {

            ValidateEmail(language);

            if (string.IsNullOrEmpty(this.Password)) {
                if (language == "J") {
                    throw new ArgumentNullException(String.Empty, "パスワードを入力してください。");
                } else {
                    throw new ArgumentNullException(String.Empty, "Please enter your password.");
                }
            } else {
                if (this.Password.Length > 15) {
                    if (language == "J") {
                        throw new ArgumentNullException(String.Empty, "入力情報に誤りがあります。");
                    } else {
                        throw new ArgumentNullException(String.Empty, "Invalid login attempt.");
                    }
                }
            }

        }
    }

}
