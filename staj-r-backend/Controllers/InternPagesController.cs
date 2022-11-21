using staj_r_backend.Helper.Token;
using staj_r_backend.Models;
using staj_r_backend.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace staj_r_backend.Controllers
{
    public class InternPagesController
    {
        private InternPagesModel ipm = new InternPagesModel();
        private Token tokenClass = new Token();
        public InternPagesController()
        {
            ipm = new InternPagesModel();
        }

        private TokenEntity getToken(string token)
        {
            return tokenClass.decrypt(token);
        }
        public async Task<bool> manageApplyingDates(string token, internships en, internDateForDeps dates, bool isNewEra)
        {
            try
            {
                TokenEntity tken = tokenClass.decrypt(token);
                return await ipm.manageApplyingDates(tken.number, en, dates, isNewEra);
            }
            catch
            {
                return false;
            }
        }

        public async Task<Result<internDateForDeps>> getApplyingDates(string token, internships en)
        {
            try
            {
                var res = await ipm.getApplyingDates(getToken(token).number, en);
                if (res == null)
                {
                    return new Result<internDateForDeps>(false);
                }
                return new Result<internDateForDeps>()
                {
                    isSuccess = true,
                    value = res,
                };
            }
            catch
            {
                return new Result<internDateForDeps>(false);
            }
        }




        public async Task<Result<List<studentDetails>>> getStudents(string token, internships i)
        {
            try
            {
                var res = await ipm.getStudents(getToken(token).number, i);
                if (res == null)
                {
                    return new Result<List<studentDetails>>(false);
                }
                return new Result<List<studentDetails>>()
                {
                    isSuccess = true,
                    value = res,
                };
            }
            catch (Exception ex)
            {
                return new Result<List<studentDetails>>(false);
            }
        }
        #region studentDetails________POPUP

        public async Task<Result<Popup>> getStudentDetails(string stdnumber, internships type)
        {
            try
            {
                var res = await ipm.getStudentDetails(stdnumber, type);
                if (res == null)
                {
                    return new Result<Popup>(false);
                }
                return new Result<Popup>()
                {
                    isSuccess = true,
                    value = res,
                };
            }
            catch
            {
                return new Result<Popup>(false);
            }

        }

        #endregion

        public async Task<bool> updateMessage(int doesInternID, string message)
        {
            try
            {
                var res = await ipm.updateMessage(doesInternID, message);
                return res;
            }
            catch
            {
                return false;
            }
        }

        //Başvuru kablulü ya da reddi için
        public async Task<Result<Popup>> markApply(int doesInternID, bool isaccepting, string stdnumber, internships type)
        {
            try
            {
                var res = await ipm.markApply(doesInternID, isaccepting, stdnumber, type);
                if (res == null)
                {
                    return new Result<Popup>(false);
                }
                return new Result<Popup>()
                {
                    isSuccess = true,
                    value = res,
                };
            }
            catch
            {
                return new Result<Popup>(false);
            }
        }

        public async Task<bool> setInterviewDate(int doesInternID, DateTime interviewDate)
        {
            try
            {
                var res = await ipm.setInterviewDate(doesInternID, interviewDate);
                return res;
            }
            catch
            {
                return false;
            }
        }

        //Stajın başarılı veya başarısız işaretlenmesi için
        public async Task<Result<Popup>> markInternship(int doesInternID, bool isaccepting, string stdnumber, internships type)
        {
            try
            {
                var res = await ipm.markInternship(doesInternID, isaccepting, stdnumber, type);
                if (res == null)
                {
                    return new Result<Popup>(false);
                }
                return new Result<Popup>()
                {
                    isSuccess = true,
                    value = res,
                };
            }
            catch
            {
                return new Result<Popup>(false);
            }
        }
    }
}

