using staj_r_backend.Helper;
using staj_r_backend.Helper.Token;
using staj_r_backend.Models;
using staj_r_backend.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace staj_r_backend.Controllers
{
    public class UserController
    {
        public TokenEntity parseToken(string token)
        {
            Token tk = new Token();
            return tk.decrypt(token);
        }
        public async static Task<string> tokenizeUserWToken(UserWToken uwt)
        {
            Token tk = new Token();
            return tk.encryptUserWToken(uwt);
        }
        public async static Task<UserWToken> detokenizeUserWToken(string uwtStr)
        {
            Token tk = new Token();
            return tk.decryptUserWToken(uwtStr);
        }
        //Departman bilgisi varsa kullanılır.
        public async Task<bool> registerOther(string token, string number, string name, string surname, string email, int roleID)
        {
            try
            {
                string uNumber = new Token().decrypt(token).number;
                return await registerCommon(number, name, surname, email, null, roleID, uNumber);
            }
            catch
            {
                return false;
            }
        }
        //Departman bilgisi yoksa kullanılır.
        public async Task<bool> registerDepManager(string number, string name, string surname, string department, string email)
        {
            try
            {
                return await registerCommon(number, name, surname, email, department, 11, null);
            }
            catch
            {
                return false;
            }
        }

        //uNumber: İşlemi gerçekleştiren kullanıcının numarasıdır. =>
        private async Task<bool> registerCommon(string number, string name, string surname, string email, string department, int roleID, string uNumber)
        {
            PasswordHelper ph = new PasswordHelper();
            string password = "";
            string encrypted = "";
            do
            {
                password = ph.generatePass();
                encrypted = ph.encrypt(password);
            } while (encrypted.Contains('\'') || encrypted.Contains('"'));
            string message = $"Merhaba {name}!<br>Staj-R kullanıcı kaydınız başarılı bir şekilde yapılmıştır. Sisteme okul numaranız ve bu e-postada yer alan " +
                "parolanız ile giriş yapabilirsiniz.<br>" +
                $"<br><br><b>PAROLANIZI KİMSEYLE PAYLAŞMAYINIZ!</b><br><br><br>Parolanız: {password}<br><br>" +
                $"Hemen sisteme giriş yapmak için <a href=\"www.stajr.azurewebsites.net/stajR\">buraya</a> tıklayınız.";
            await SendMail.sendMail(email, "Staj-R Kullanıcı Kaydınız", message);
            UserModel um = new UserModel();
            return await um.registerModel(number, name, surname, email, encrypted, department, roleID, uNumber);
        }

        public async Task<Result<List<role_auth>>> getRoles()
        {
            try
            {
                UserModel um = new UserModel();
                var res = await um.getRoles();
                if (res == null)
                {
                    return new Result<List<role_auth>>(false);
                }
                return new Result<List<role_auth>>()
                {
                    isSuccess = true,
                    value = res,
                };
            }
            catch
            {
                return new Result<List<role_auth>>();
            }
        }
        public async Task<bool> createRole(string name, List<string> authorities)
        {
            try
            {
                UserModel um = new UserModel();
                return await um.createRole(name, authorities);
            }
            catch
            {
                return false;
            }
        }
        public async Task<Result<List<userList>>> getUsers(string token)
        {
            try
            {
                string number = parseToken(token).number;
                UserModel um = new UserModel();
                var qres = await um.getUsers(number);
                if (qres == null)
                {
                    return new Result<List<userList>>(false);
                }
                else
                {
                    return new Result<List<userList>>()
                    {
                        isSuccess = true,
                        value = qres,
                    };
                }
            }
            catch
            {
                return new Result<List<userList>>(false);
            }
        }
        #region Kullanıcılar sayfası popup
        public async Task<Result<Dictionary<int, string>>> getRolesForDropdown(string token)
        {
            try
            {
                string number = parseToken(token).number;
                UserModel um = new UserModel();
                var res = await um.getRolesForDropDown(number);
                if (res == null)
                {
                    return new Result<Dictionary<int, string>>(false);
                }
                else
                {
                    return new Result<Dictionary<int, string>>
                    {
                        isSuccess = true,
                        value = res,
                    };

                }
            }
            catch
            {
                return new Result<Dictionary<int, string>>(false);
            }
        }
        public async Task<bool> updateRole(string number, int roleID)
        {
            try
            {
                UserModel um = new UserModel();
                return await um.updateRole(number, roleID);
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> updateEmail(string number, string email)
        {
            try
            {
                UserModel um = new UserModel();
                return await um.updateEmail(number, email);
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> updatePassword(string number)
        {
            try
            {
                PasswordHelper ph = new PasswordHelper();
                string password = ph.generatePass();
                string encrypted = ph.encrypt(password);
                UserModel um = new UserModel();
                List<string> list = await um.updatePassword(number, encrypted);
                string name = list[0];
                string email = list[1];
                string message = $"Merhaba {name}!<br>Staj-R kullanıcı parolanız sıfırlanmıştır. Sisteme okul numaranız ve bu e-postada yer alan yeni" +
                    "parolanız ile giriş yapabilirsiniz.<br>" +
                    $"<br><br><b>PAROLANIZI KİMSEYLE PAYLAŞMAYINIZ!</b><br><br><br>Parolanız: {password}<br><br>" +
                    $"Hemen sisteme giriş yapmak için <a href=\"www.stajr.azurewebsites.net/stajR\">buraya</a> tıklayınız.";
                await SendMail.sendMail(email, "Staj-R Şifre Değişikliği", message);
                return true;
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}
