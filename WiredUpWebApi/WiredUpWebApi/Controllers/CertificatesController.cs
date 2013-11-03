using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WiredUpWebApi.Data;
using WiredUpWebApi.Models;
using WiredUpWebApi.Models.CertificateModels;
using WiredUpWebApi.Models.Constants;

namespace WiredUpWebApi.Controllers
{
    public class CertificatesController : BaseApiController
    {
        private const int CertificateNameMaxLength = CertificateConstants.NameMaxLength;
        private const int CertificateUrlMaxLength = CertificateConstants.UrlMaxLength;

        public CertificatesController()
            : base()
        {
        }

        public CertificatesController(IUnitOfWorkData db)
            : base(db)
        {
        }

        [HttpPost]
        [ActionName("add")]
        public HttpResponseMessage AddCertificate(
            [FromBody]CertificateCreateModel model, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                this.ValidateCertificateName(model.Name);
                this.ValidateCertificateUrl(model.Url);

                var user = this.GetUserBySessionKey(sessionKey);
                var certificate = new Certificate()
                {
                    Name = model.Name,
                    Url = model.Url
                };

                user.Certificates.Add(certificate);
                this.db.Users.Update(user);
                this.db.SaveChanges();

                var returnData = new CertificateModel()
                {
                    Id = certificate.Id,
                    Name = certificate.Name
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, returnData);
                return response;
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("all")]
        public IQueryable<CertificateModel> GetAllCertificates(
            [FromUri]int userId, [FromUri]string sessionKey)
        {
            if (!this.IsSessionKeyValid(sessionKey))
            {
                throw new ArgumentException("Invalid session key");
            }

            var certificates = this.db.Certificates
                .All()
                .Where(c => c.OwnerId == userId)
                .Select(CertificateModel.FromCertificate);

            return certificates;
        }

        [HttpGet]
        [ActionName("details")]
        public CertificateDetailedModel GetCertificateDetails(
            [FromUri]int id, [FromUri]string sessionKey)
        {
            if (!this.IsSessionKeyValid(sessionKey))
            {
                throw new ArgumentException("Invalid session key");
            }

            var certificates = this.db.Certificates
                .All()
                .Include(c => c.Owner)
                .Where(c => c.Id == id)
                .Select(CertificateDetailedModel.FromCertificate)
                .FirstOrDefault();

            return certificates;
        }

        [HttpPut]
        [ActionName("edit")]
        public HttpResponseMessage EditCertificate(
            [FromBody]CertificateEditModel model, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                this.ValidateCertificateName(model.Name);
                this.ValidateCertificateUrl(model.Url);

                var user = this.GetUserBySessionKey(sessionKey);
                var certificate = user.Certificates.FirstOrDefault(c => c.Id == model.Id);
                if (certificate == null)
                {
                    throw new ArgumentException("The user does not have certificate with id=" + model.Id);
                }

                certificate.Name = model.Name;
                certificate.Url = model.Url;

                this.db.Certificates.Update(certificate);
                this.db.SaveChanges();

                var returnData = new CertificateModel()
                {
                    Id = certificate.Id,
                    Name = certificate.Name
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, returnData);
                return response;
            });

            return responseMsg;
        }

        [HttpDelete]
        [ActionName("delete")]
        public HttpResponseMessage DeleteCertificate([FromUri]int id, [FromUri]string sessionKey)
        {
            var responseMsg = this.PerformOperationAndHandleExceptions(() =>
            {
                var user = this.GetUserBySessionKey(sessionKey);
                var certificate = user.Certificates.FirstOrDefault(c => c.Id == id);
                if (certificate == null)
                {
                    throw new ArgumentException("The user does not have such certificate");
                }

                this.db.Certificates.Delete(certificate);
                this.db.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                return response;
            });

            return responseMsg;
        }

        private void ValidateCertificateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The 'Name' is required");
            }

            if (name.Length > CertificateNameMaxLength)
            {
                throw new ArgumentException(
                    string.Format(
                        "The 'Name' must be less than {0} characters long",
                        CertificateNameMaxLength));
            }
        }

        private void ValidateCertificateUrl(string url)
        {
            if (!string.IsNullOrWhiteSpace(url) && url.Length > CertificateUrlMaxLength)
            {
                throw new ArgumentException(
                    string.Format(
                        "The 'Url' must be less than {0} characters long",
                        CertificateUrlMaxLength));
            }
        }
    }
}