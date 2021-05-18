using Microsoft.VisualStudio.TestTools.UnitTesting;
using StaffingService.DataAccess;

namespace StaffingService.UnitTest
{
    [TestClass]
    public class UserDalTest
    {
        [TestMethod, TestCategory("_Dal"), TestCategory("_Ado"), TestCategory("_User"), TestCategory("Login")]
        public void User_Dal_Ado_Login()
        {
            //var dal = new UserDal();
            //var result = dal.GetUser("rajsivaguru@gmail.com");
            var result = UserDal.Instance.GetUser("rajsivaguru@gmail.com");
        }
    }
}
