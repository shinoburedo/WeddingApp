using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using CBAF.Attributes;
using CBAF;

namespace MauloaDemo.Models {

    [Serializable]
    [Table("c_account")]

    public class CAccount : IValidatableObject {

        [Key]
        public int account_id { get; set; }

        //[MaxLength(15)]
        //[Required]
        [Hankaku]
        public string password { get; set; }

        [NotMapped]
        public string password_val { get; set; }

        //[MaxLength(15)]
        //[Hankaku]
        [NotMapped]
        public string password_current { get; set; }

        public bool is_japan { get; set; }

        public string screen_color { get; set; }

        //[MaxLength(30)]
        //[Required]
        [Hankaku, UpperCase]
        public string e_last_name { get; set; }

        //[MaxLength(30)]
        //[Required]
        [Hankaku, UpperCase]
        public string e_first_name { get; set; }

        public bool is_groom { get; set; }

        //[MaxLength(200)]
        //[Required]
        [Hankaku]
        public string email { get; set; }

        [NotMapped]
        public string email_val { get; set; }

        [NotMapped]
        public string email_old { get; set; }

        public int? primary_id { get; set; }

        public string pwd_hash { get; set; }
        public DateTime? eff_from_pass { get; set; }
        public DateTime? eff_to_pass { get; set; }
        public string culture_name { get; set; }
        public string date_format { get; set; }
        public string time_format { get; set; }
        public string print_date_format { get; set; }

        [StringLength(7), Hankaku]
        public string c_num { get; set; }
        public bool locked { get; set; }

        public DateTime? resigned_date { get; set; }

        public string pwd_reset_key { get; set; }

        public DateTime create_date { get; set; }

        [Required, StringLength(15)]
        public string last_person { get; set; }

        public DateTime update_date { get; set; }

        [NotMapped]
        public int temp_id { get; set; }



        //********************************************************* 
        //******* ASP.NET Indentity 2.0 対応
        //********************************************************* 
        [NotMapped]
        public string Id
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
            }
        }

        [NotMapped]
        public string UserName
        {
            get
            {
                //User.Identity.NameにセットされるのはUserNameの値なので氏名ではなく「ユーザーID（=Email）」を返す必要がある。
                //氏名（ローマ字）を取得したい場合は CustomerName を使う事。
                return email;
            }
            set
            {
                email = value;
            }
        }

        //********************************************************* 
        //******* ↑↑↑ ASP.NET Indentity 2.0 対応 ↑↑↑ ここまで
        //********************************************************* 
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }



        public string CustomerName
        {
            get
            {
                return string.Format("{0} {1}", e_first_name, e_last_name);
            }
        }

        public CAccount MaskPassword()
        {
            this.password = "********";
            this.password_current = "********";
            this.password_val = "********";
            this.pwd_hash = "";
            return this;
        }

        public void ValidateForResetPassword(string language)
        {
            //    if (string.IsNullOrEmpty(this.pwd_reset_key))
            //    {
            //        if (language == "J")
            //        {
            //            throw new ArgumentNullException(String.Empty, "入力値が不正です。");
            //        }
            //        else
            //        {
            //            throw new ArgumentNullException(String.Empty, "Invalid input value.");
            //        }
            //    }
            //    if (string.IsNullOrEmpty(this.e_last_name))
            //    {
            //        if (language == "J")
            //        {
            //            throw new ArgumentNullException(String.Empty, "アルファベット氏名を入力してください。");
            //        }
            //        else
            //        {
            //            throw new ArgumentNullException(String.Empty, "Please enter your first name.");
            //        }
            //    }
            //    else
            //    {
            //        if (!HankakuZenkaku.isHankaku(this.e_last_name))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "アルファベット氏名は半角文字で入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter your first name.");
            //            }
            //        }
            //        if (this.e_last_name.Length > 20)
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "アルファベット氏名（姓）は20文字以内で入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter your first name in 20 characters or less.");
            //            }
            //        }
            //    }
            //    if (string.IsNullOrEmpty(this.e_first_name))
            //    {
            //        if (language == "J")
            //        {
            //            throw new ArgumentNullException(String.Empty, "アルファベット氏名を入力してください。");
            //        }
            //        else
            //        {
            //            throw new ArgumentNullException(String.Empty, "Please enter your first name.");
            //        }
            //    }
            //    else
            //    {
            //        if (!HankakuZenkaku.isHankaku(this.e_first_name))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "アルファベット氏名は半角文字で入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter your first name.");
            //            }
            //        }
            //        if (this.e_first_name.Length > 20)
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "アルファベット氏名（名）は20文字以内で入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter your first name in 20 characters or less.");
            //            }
            //        }
            //    }
            //    if (string.IsNullOrEmpty(this.password))
            //    {
            //        if (language == "J")
            //        {
            //            throw new ArgumentNullException(String.Empty, "パスワードを入力してください。");
            //        }
            //        else
            //        {
            //            throw new ArgumentNullException(String.Empty, "Please enter your password.");
            //        }
            //    }
            //    else
            //    {
            //        if (!checkPasswordChar(this.password, false))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "パスワードに使用できない文字が含まれています。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "You entered invalid characters in your password.");
            //            }
            //        }
            //        if (this.password.Length < 8 || this.password.Length > 20)
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "パスワードは8文字以上、20文字以内で入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter your password in between 8 and 20 characters.");
            //            }
            //        }
            //        if (!CheckPasswordCharNumeric(this.password))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "パスワードは少なくとも1文字の数字を含むようにしてください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter one numeric character at least in your password.");
            //            }
            //        }
            //        if (!CheckPasswordCharAlphabet(this.password))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "パスワードは少なくとも1文字ずつの大文字と小文字を含むようにしてください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter both uppercase and lowercase characters in your password.");
            //            }
            //        }
            //        if (!CheckPasswordCharSigns(this.password))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "パスワードは'!@#$%&()_-'の中から少なくとも1文字を含むようにしてください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter one character among '!@#$%&()_-' at least in your password.");
            //            }
            //        }
            //        if (this.password != this.password_val)
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "パスワードが正しく入力されているか確認してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please check that your password match.");
            //            }
            //        }
            //        var exists = new WtAccountRepository().GetList()
            //            .Any(m => m.pwd_reset_key == this.pwd_reset_key && m.e_last_name == this.e_last_name && m.e_first_name == this.e_first_name);
            //        if (!exists)
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "氏名が一致しません。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "The name you entered doesn't match.");
            //            }
            //        }
            //    }
        }

        public void ValidateForReregist(string language)
        {
            //    if (string.IsNullOrEmpty(this.email))
            //    {
            //        if (language == "J")
            //        {
            //            throw new ArgumentNullException(String.Empty, "Eメールアドレスを入力してください。");
            //        }
            //        else
            //        {
            //            throw new ArgumentNullException(String.Empty, "Please enter Email address.");
            //        }
            //    }
            //    else
            //    {
            //        if (!System.Text.RegularExpressions.Regex.IsMatch(
            //                this.email,
            //                @"\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z",
            //                System.Text.RegularExpressions.RegexOptions.IgnoreCase))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "Eメールアドレスを正しく入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter correct Email address.");
            //            }
            //        }
            //        if (this.email.Length > 200)
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "Eメールアドレスは200文字以内で入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter your Email address in 200 characters or less.");
            //            }
            //        }
            //    }
            //    var exists = new WtAccountRepository().GetList().Any(m => m.email == this.email);
            //    if (!exists)
            //    {
            //        throw new ArgumentException("no_email", "no_email");
            //    }
        }


            //public void ValidateSave(bool isNew, string edit_prm, string language)
            //{

            //    UpperCaseHelper.Apply(this);
            //    HankakuHelper.Apply(this);
            //    var repository = new WtAccountRepository();

            //    var exists = repository.GetList().Any(m => m.account_id == this.account_id);
            //    if (isNew && exists)
            //    {
            //        throw new Exception("account_id already exists.");
            //    }
            //    else if (!isNew && !exists)
            //    {
            //        throw new Exception("account_id was not found.");
            //    }
            //    if ((edit_prm == "Email") && string.IsNullOrEmpty(this.email))
            //    {
            //        if (language == "J")
            //        {
            //            throw new ArgumentNullException(String.Empty, "Eメールアドレスを入力してください。");
            //        }
            //        else
            //        {
            //            throw new ArgumentNullException(String.Empty, "Please enter Email address.");
            //        }
            //    }
            //    else
            //    {
            //        if ((edit_prm == "Email") &&
            //            (!System.Text.RegularExpressions.Regex.IsMatch(
            //                this.email,
            //                @"\A[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\z",
            //                System.Text.RegularExpressions.RegexOptions.IgnoreCase)))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "Eメールアドレスを正しく入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter correct Email address.");
            //            }
            //        }
            //        if ((edit_prm == "Email") && (this.email.Length > 200))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "Eメールアドレスは200文字以内で入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter your Email address in 200 characters or less.");
            //            }
            //        }
            //        if (edit_prm == "Email" && (this.email != this.email_val))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "Eメールアドレスが正しく入力されているか確認してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please check that your e-mail addresses match.");
            //            }
            //        }
            //        if (!isNew && edit_prm == "Email")
            //        {
            //            var exists_email = repository.GetList().Any(m => m.email == this.email && m.account_id != this.account_id);
            //            if (exists_email)
            //            {
            //                if (language == "J")
            //                {
            //                    throw new ArgumentNullException(String.Empty, "すでに登録されているEメールアドレスは使用できません。");
            //                }
            //                else
            //                {
            //                    throw new ArgumentNullException(String.Empty, "This Email address've been already registed.");
            //                }
            //            }
            //        }
            //    }

            //    if ((isNew || edit_prm == "Name") && string.IsNullOrEmpty(this.e_last_name))
            //    {
            //        if (language == "J")
            //        {
            //            throw new ArgumentNullException(String.Empty, "アルファベット氏名を入力してください。");
            //        }
            //        else
            //        {
            //            throw new ArgumentNullException(String.Empty, "Please enter your first name.");
            //        }
            //    }
            //    else
            //    {
            //        if ((isNew || edit_prm == "Name") && !HankakuZenkaku.isHankaku(this.e_last_name))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "アルファベット氏名は半角文字で入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter your first name.");
            //            }
            //        }
            //        if ((isNew || edit_prm == "Name") && this.e_last_name.Length > 20)
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "アルファベット氏名（姓）は20文字以内で入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter your first name in 20 characters or less.");
            //            }
            //        }
            //    }
            //    if ((isNew || edit_prm == "Name") && string.IsNullOrEmpty(this.e_first_name))
            //    {
            //        if (language == "J")
            //        {
            //            throw new ArgumentNullException(String.Empty, "アルファベット氏名を入力してください。");
            //        }
            //        else
            //        {
            //            throw new ArgumentNullException(String.Empty, "Please enter your first name.");
            //        }
            //    }
            //    else
            //    {
            //        if ((isNew || edit_prm == "Name") && !HankakuZenkaku.isHankaku(this.e_first_name))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "アルファベット氏名は半角文字で入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter your first name.");
            //            }
            //        }
            //        if ((isNew || edit_prm == "Name") && this.e_first_name.Length > 20)
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "アルファベット氏名（名）は20文字以内で入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter your first name in 20 characters or less.");
            //            }
            //        }
            //    }

            //    if ((isNew || edit_prm == "Password") && string.IsNullOrEmpty(this.password))
            //    {
            //        if (language == "J")
            //        {
            //            throw new ArgumentNullException(String.Empty, "パスワードを入力してください。");
            //        }
            //        else
            //        {
            //            throw new ArgumentNullException(String.Empty, "Please enter your password.");
            //        }
            //    }
            //    else
            //    {
            //        if ((isNew || edit_prm == "Password") && !checkPasswordChar(this.password, false))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "パスワードに使用できない文字が含まれています。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "You entered invalid characters in your password.");
            //            }
            //        }
            //        if ((isNew || edit_prm == "Password") && (this.password.Length < 8 || this.password.Length > 20))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "パスワードは8文字以上、20文字以内で入力してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter your password in between 8 and 20 characters.");
            //            }
            //        }
            //        if ((isNew || edit_prm == "Password") && !CheckPasswordCharNumeric(this.password))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "パスワードは少なくとも1文字の数字を含むようにしてください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter one numeric character at least in your password.");
            //            }
            //        }
            //        if ((isNew || edit_prm == "Password") && !CheckPasswordCharAlphabet(this.password))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "パスワードは少なくとも1文字ずつの大文字と小文字を含むようにしてください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter both uppercase and lowercase characters in your password.");
            //            }
            //        }
            //        if ((isNew || edit_prm == "Password") && !CheckPasswordCharSigns(this.password))
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "パスワードは'!@#$%&()_-'の中から少なくとも1文字を含むようにしてください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please enter one character among '!@#$%&()_-' at least in your password.");
            //            }
            //        }
            //        if ((isNew || edit_prm == "Password") && this.password != this.password_val)
            //        {
            //            if (language == "J")
            //            {
            //                throw new ArgumentNullException(String.Empty, "パスワードが正しく入力されているか確認してください。");
            //            }
            //            else
            //            {
            //                throw new ArgumentNullException(String.Empty, "Please check that your password match.");
            //            }
            //        }
            //        if (!isNew && edit_prm == "Password")
            //        {
            //            //パスワード一致するか
            //            var account = repository.Find(this.account_id);
            //            //パスワードのチェック
            //            if (!Crypto.VerifyHashedPassword(account.pwd_hash, this.password_current))
            //            {
            //                if (language == "J")
            //                {
            //                    throw new ArgumentNullException(String.Empty, "現在のパスワードが正しく入力されているか確認してください。");
            //                }
            //                else
            //                {
            //                    throw new ArgumentNullException(String.Empty, "Please check that your current password correct.");
            //                }
            //            }
            //        }
            //    }
            //}

            //private static bool checkPasswordChar(string str, bool allowBlank = true)
            //{
            //    if (string.IsNullOrEmpty(str)) return false;
            //    if (!allowBlank && str.Contains(" ")) return false;

            //    const string AllowedChars = "!@#$%&()_-";

            //    string t = null;

            //    for (int i = 0; i <= str.Length - 1; i++)
            //    {
            //        t = str.Substring(i, 1);

            //        if (!HankakuZenkaku.isHankaku(t))
            //            return false;

            //        if ((!HankakuZenkaku.IsAlpha(t)) && (!TypeHelper.IsNumeric(t)) && (AllowedChars.IndexOf(t) < 0))
            //            return false;
            //    }

        //    return true;
        //}

        private bool CheckPasswordCharNumeric(string pwd)
        {

            //ひとつでも数字があればOK
            foreach (char c in pwd.ToCharArray())
            {
                if (c >= '0' && c <= '9')
                {
                    return true;
                }
            }

            //ひとつも無ければNG
            return false;
        }

        private bool CheckPasswordCharAlphabet(string pwd)
        {
            int big = 0;
            int small = 0;

            //アルファベット(大文字)の数を数える。
            foreach (char c in pwd.ToCharArray())
            {
                if (c >= 'A' && c <= 'Z') big++;
            }

            //アルファベット(小文字)の数を数える。
            foreach (char c in pwd.ToCharArray())
            {
                if (c >= 'a' && c <= 'z') small++;
            }

            //大文字と小文字がともに１文字以上ずつあればOK。
            return (big >= 1 && small >= 1);
        }

        private bool CheckPasswordCharSigns(string pwd)
        {
            if (string.IsNullOrEmpty(pwd)) return false;
            const string AllowedChars = "!@#$%&()_-";

            //いずれか１つの記号を含む
            foreach (char c in AllowedChars.ToCharArray())
            {
                if (pwd.Contains(c)) return true;
            }

            return false;
        }

    }
}